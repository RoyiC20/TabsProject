using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsApp.Models;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public FavoritesController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Favorites
        [HttpGet]
        public IActionResult GetFavorites()
        {
            var favorites = new List<Favorite>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT UserID, SongID, favorited_at AS FavoritedAt FROM favorites";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var favorite = new Favorite
                    {
                        UserID = reader.GetInt32(0),
                        SongID = reader.GetInt32(1),
                        FavoritedAt = reader.GetDateTime(2)
                    };
                    favorites.Add(favorite);
                }

                reader.Close();
            }

            return Ok(favorites);
        }
    }
}
