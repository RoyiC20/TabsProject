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

        // GET: api/Likes/isliked?songID=1&userID=2
        [HttpGet("isliked")]
        public IActionResult IsLiked(int songID, int userID)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "SELECT COUNT(*) FROM likes WHERE SongID = @songID AND UserID = @userID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", songID);
            command.Parameters.AddWithValue("@userID", userID);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return Ok(count > 0);
        }

        // POST: api/Likes
        [HttpPost]
        public IActionResult AddLike([FromBody] Like like)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "INSERT IGNORE INTO likes (SongID, UserID) VALUES (@songID, @userID)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", like.SongID);
            command.Parameters.AddWithValue("@userID", like.UserID);

            command.ExecuteNonQuery();
            return Ok();
        }

        // DELETE: api/Likes?songID=1&userID=2
        [HttpDelete]
        public IActionResult RemoveLike(int songID, int userID)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "DELETE FROM likes WHERE SongID = @songID AND UserID = @userID";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", songID);
            command.Parameters.AddWithValue("@userID", userID);

            command.ExecuteNonQuery();
            return Ok();
        }
    }
}

