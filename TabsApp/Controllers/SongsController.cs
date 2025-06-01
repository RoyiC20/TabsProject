using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public SongsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Songs
        [HttpGet]
        public IActionResult GetSongs()
        {
            var songs = new List<Song>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT s.SongID, s.Name, s.ArtistID, a.Name AS ArtistName, t.difficulty, t.instrument, t.tabID
                    FROM songs s
                    LEFT JOIN artists a ON s.ArtistID = a.ArtistID
                    LEFT JOIN tabs t ON s.SongID = t.songID";
 //                   where t.instrument = "x";

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var song = new Song
                    {
                        SongID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ArtistID = reader.GetInt32(2),
                        Artist = new Artist
                        {
                            ArtistID = reader.GetInt32(2),
                            Name = reader.IsDBNull(3) ? null : reader.GetString(3)
                        },  
                        Tab = new Tab
                        {
                            Difficulty = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Instrument = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            TabID = reader.GetInt32(6)
                        }
                    };

                    songs.Add(song);
                }

                reader.Close();
            }

            return Ok(songs);
        }


        // TabsController.cs
        [HttpGet("exists")]
        public IActionResult SongExists(string title, string artist)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = @"
        SELECT COUNT(*) FROM songs s
        JOIN artists a ON s.ArtistID = a.ArtistID
        WHERE LOWER(s.Name) = LOWER(@title) AND LOWER(a.Name) = LOWER(@artist)";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@artist", artist);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return Ok(count > 0);
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
        {
            try
            {
                bool success = _databaseService.DeleteSongWithTabs(id);
                if (success)
                    return Ok();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה במחיקה: " + ex.Message);
                return StatusCode(500, "שגיאה בשרת");
            }
        }

        [HttpGet("user/{userID}")]
        public IActionResult GetSongsByUser(int userID)
        {
            var songs = new List<Song>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = @"
            SELECT s.SongID, s.Name, s.ArtistID, a.Name AS ArtistName, s.UserID,
                   t.Difficulty, t.Instrument, t.TabID
            FROM songs s
            LEFT JOIN artists a ON s.ArtistID = a.ArtistID
            LEFT JOIN tabs t ON s.SongID = t.SongID
            WHERE s.UserID = @userID";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userID", userID);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var song = new Song
                    {
                        SongID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ArtistID = reader.GetInt32(2),
                        UserID = reader.GetInt32(4),
                        Artist = new Artist
                        {
                            ArtistID = reader.GetInt32(2),
                            Name = reader.IsDBNull(3) ? null : reader.GetString(3)
                        },
                        Tab = new Tab
                        {
                            Difficulty = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            Instrument = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            TabID = reader.GetInt32(7)
                        }
                    };

                    songs.Add(song);
                }

                reader.Close();
            }

            return Ok(songs);
        }




    }
}
