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
                string query = "SELECT SongID, Name, ArtistID FROM songs";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var song = new Song
                    {
                        SongID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        ArtistID = reader.GetInt32(2)
                    };
                    songs.Add(song);
                }

                reader.Close();
            }

            return Ok(songs);
        }
    }
}
