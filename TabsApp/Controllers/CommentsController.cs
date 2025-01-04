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
                        CommentText = reader.GetString(3),
                        CommentedAt = reader.GetDateTime(4)
                    };
                    comments.Add(comment);
                }

                reader.Close();
            }

            return Ok(comments);
        }
    }
}
