using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text.Json;
using TabsApp.Services;
using TabsClassLibrary;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public TabsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Tabs
        [HttpGet]
        public IActionResult GetTabs()
        {
            var tabs = new List<Tab>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT TabID, Instrument, Difficulty, Text, SongID FROM tabs";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tab = new Tab
                    {
                        TabID = reader.GetInt32(0),
                        Instrument = reader.GetString(1),
                        Difficulty = reader.GetString(2),
                        Text = reader.GetString(3),
                        SongID = reader.GetInt32(4)
                    };
                    tabs.Add(tab);
                }

                reader.Close();
            }

            return Ok(tabs);
        }




        [HttpPost]
        public IActionResult SubmitTab([FromBody] TabSubmission submission)
        {
            int artistID;
            int songID;

            using var connection = _databaseService.GetConnection();
            connection.Open();

            // 1. בדיקה אם האמן קיים
            using (var checkArtistCmd = new MySqlCommand("SELECT ArtistID FROM artists WHERE Name = @name", connection))
            {
                checkArtistCmd.Parameters.AddWithValue("@name", submission.Artist);
                var result = checkArtistCmd.ExecuteScalar();

                if (result != null)
                {
                    artistID = Convert.ToInt32(result);
                }
                else
                {
                    // 2. הוספת אמן חדש
                    using var insertArtistCmd = new MySqlCommand(
                        "INSERT INTO artists (Name) VALUES (@name); SELECT LAST_INSERT_ID();",
                        connection
                    );
                    insertArtistCmd.Parameters.AddWithValue("@name", submission.Artist);
                    artistID = Convert.ToInt32(insertArtistCmd.ExecuteScalar());
                }
            }

            // 3. הוספת שיר חדש
            using (var insertSongCmd = new MySqlCommand(
                "INSERT INTO songs (Name, ArtistID) VALUES (@name, @artistID); SELECT LAST_INSERT_ID();",
                connection
            ))
            {
                insertSongCmd.Parameters.AddWithValue("@name", submission.Title);
                insertSongCmd.Parameters.AddWithValue("@artistID", artistID);
                songID = Convert.ToInt32(insertSongCmd.ExecuteScalar());
            }

            // 4. המרת תוכן הטאב ל־JSON
            string jsonText = JsonSerializer.Serialize(submission.TabLines);

            // 5. הוספת הטאב
            int insertedTabID;
            using (var insertTabCmd = new MySqlCommand("INSERT INTO tabs (Instrument, Difficulty, Text, SongID) VALUES (@instrument, @difficulty, @text, @songID); SELECT LAST_INSERT_ID();", connection))
            {
                insertTabCmd.Parameters.AddWithValue("@instrument", submission.Instrument);
                insertTabCmd.Parameters.AddWithValue("@difficulty", submission.Difficulty);
                insertTabCmd.Parameters.AddWithValue("@text", jsonText);
                insertTabCmd.Parameters.AddWithValue("@songID", songID);

                insertedTabID = Convert.ToInt32(insertTabCmd.ExecuteScalar());
            }


            return Ok(new { status = "saved", tabID = insertedTabID });
        }



        [HttpGet("{id}")]
        public IActionResult GetTabById(int id)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = @"
        SELECT t.TabID, t.Instrument, t.Difficulty, t.Text, t.SongID,
               s.Name AS SongName, s.ArtistID,
               a.Name AS ArtistName
        FROM tabs t
        LEFT JOIN songs s ON t.SongID = s.SongID
        LEFT JOIN artists a ON s.ArtistID = a.ArtistID
        WHERE t.TabID = @id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var tab = new Tab
                {
                    TabID = reader.GetInt32(0),
                    Instrument = reader.GetString(1),
                    Difficulty = reader.GetString(2),
                    Text = reader.GetString(3),
                    SongID = reader.GetInt32(4),
                    Song = new Song
                    {
                        SongID = reader.GetInt32(4),
                        Name = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        ArtistID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                        Artist = new Artist
                        {
                            ArtistID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            Name = reader.IsDBNull(7) ? "" : reader.GetString(7)
                        }
                    }
                };

                return Ok(tab);
            }

            return NotFound();
        }






    }
}
