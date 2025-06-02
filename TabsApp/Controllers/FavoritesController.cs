using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
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

        // בדיקה אם שיר במועדפים: GET api/favorites/isfavorited?songID=1&userID=2
        [HttpGet("isfavorited")]
        public IActionResult IsFavorited(int songID, int userID)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "SELECT COUNT(*) FROM favorites WHERE SongID = @songID AND UserID = @userID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", songID);
            command.Parameters.AddWithValue("@userID", userID);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return Ok(count > 0);
        }

        // הוספת מועדף: POST api/favorites
        [HttpPost]
        public IActionResult AddFavorite([FromBody] Favorite favorite)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "INSERT IGNORE INTO favorites (SongID, UserID) VALUES (@songID, @userID)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", favorite.SongID);
            command.Parameters.AddWithValue("@userID", favorite.UserID);

            command.ExecuteNonQuery();
            return Ok();
        }

        // הסרת מועדף: DELETE api/favorites?songID=1&userID=2
        [HttpDelete]
        public IActionResult RemoveFavorite(int songID, int userID)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "DELETE FROM favorites WHERE SongID = @songID AND UserID = @userID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", songID);
            command.Parameters.AddWithValue("@userID", userID);

            command.ExecuteNonQuery();
            return Ok();
        }



        // GET: api/favorites/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetFavoritesByUser(int userId)
        {
            var favorites = new List<Song>();

            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = @"
    SELECT s.SongID, s.Name, s.ArtistID, a.Name AS ArtistName, t.TabID
    FROM favorites f
    JOIN songs s ON f.SongID = s.SongID
    JOIN artists a ON s.ArtistID = a.ArtistID
    LEFT JOIN tabs t ON t.SongID = s.SongID
    WHERE f.UserID = @userID";


            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userID", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                favorites.Add(new Song
                {
                    SongID = reader.GetInt32(0),               // s.SongID
                    Name = reader.GetString(1),                // s.Name
                    ArtistID = reader.GetInt32(2),             // s.ArtistID
                    Artist = new Artist                        // הוספת אמן
                    {
                        ArtistID = reader.GetInt32(2),
                        Name = reader.GetString(3)             // a.Name AS ArtistName
                    },
                    Tab = new Tab
                    {
                        TabID = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)  // t.TabID
                    }
                });
            }


            return Ok(favorites);
        }



    }
}



