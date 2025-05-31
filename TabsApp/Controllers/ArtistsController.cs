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


        [HttpDelete("{id}")]
        public IActionResult DeleteArtist(int id)
        {
            try
            {
                using var connection = _databaseService.GetConnection();
                connection.Open();

                // בדיקה אם יש שירים שקשורים לאמן
                var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM songs WHERE ArtistID = @id", connection);
                checkCmd.Parameters.AddWithValue("@id", id);
                long count = (long)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    return BadRequest(new { Message = "לא ניתן למחוק אמן שיש לו שירים משויכים." });
                }

                // מחיקה
                var deleteCmd = new MySqlCommand("DELETE FROM artists WHERE ArtistID = @id", connection);
                deleteCmd.Parameters.AddWithValue("@id", id);

                int rows = deleteCmd.ExecuteNonQuery();
                if (rows > 0)
                    return Ok(new { Message = "האמן נמחק בהצלחה" });
                else
                    return NotFound(new { Message = "האמן לא נמצא" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה במחיקת אמן: " + ex.Message);
                return StatusCode(500, new { Message = "שגיאה בשרת" });
            }
        }





    }
}
