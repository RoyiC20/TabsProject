using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public LikesController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Likes
        [HttpGet]
        public IActionResult GetLikes()
        {
            var likes = new List<Like>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT SongID, Count FROM likes";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var like = new Like
                    {
                        SongID = reader.GetInt32(0),
                        Count = reader.GetInt32(1)
                    };
                    likes.Add(like);
                }

                reader.Close();
            }

            return Ok(likes);
        }
    }
}
