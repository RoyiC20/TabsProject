using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using System.Collections.Generic;
using TabsClassLibrary;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public ArtistsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Artists
        [HttpGet]
        public IActionResult GetArtists()
        {
            var artists = new List<Artist>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT ArtistID, Name FROM artists";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var artist = new Artist
                    {
                        ArtistID = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    artists.Add(artist);
                }

                reader.Close();
            }

            return Ok(artists);
        }
    }
}
