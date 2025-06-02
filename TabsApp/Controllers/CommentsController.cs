using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CommentsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Comments
        [HttpGet]
        public IActionResult GetComments()
        {
            var comments = new List<Comment>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT CommentID, UserID, SongID, comment AS CommentText, commented_at AS CommentedAt FROM comments";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var comment = new Comment
                    {
                        CommentID = reader.GetInt32(0),
                        UserID = reader.GetInt32(1),
                        SongID = reader.GetInt32(2),
                        Content = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4)
                    };
                    comments.Add(comment);
                }

                reader.Close();
            }

            return Ok(comments);
        }


        [HttpPost]
        [HttpPost]
        public IActionResult AddComment([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content) || comment.Content.Length > 300)
            {
                return BadRequest("תגובה לא תקינה – ארוכה מדי או ריקה.");
            }

            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = @"
        INSERT INTO comments (UserID, SongID, comment, commented_at) 
        VALUES (@userID, @songID, @comment, NOW())";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userID", comment.UserID);
            command.Parameters.AddWithValue("@songID", comment.SongID);
            command.Parameters.AddWithValue("@comment", comment.Content);

            command.ExecuteNonQuery();
            return Ok();
        }



        [HttpGet("by-song")]
        public IActionResult GetCommentsBySong(int songID)
        {
            var comments = new List<Comment>();

            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = @"
        SELECT c.CommentID, c.UserID, c.SongID, c.comment AS CommentText, c.commented_at AS CommentedAt, u.Name AS Username
        FROM comments c
        JOIN users u ON c.UserID = u.UserID
        WHERE c.SongID = @songID
        ORDER BY c.commented_at DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@songID", songID);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var comment = new Comment
                {
                    CommentID = reader.GetInt32(0),
                    UserID = reader.GetInt32(1),
                    SongID = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    Username = reader.GetString(5)
                };

                comments.Add(comment);
            }

            return Ok(comments);
        }



        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            using var connection = _databaseService.GetConnection();
            connection.Open();

            string query = "DELETE FROM comments WHERE CommentID = @id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
                return NoContent();
            else
                return NotFound();
        }



    }
}
