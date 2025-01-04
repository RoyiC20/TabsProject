using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using TabsApp.Services;
using TabsClassLibrary;
using System.Collections.Generic;

namespace TabsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public TabsController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // GET: api/Tabs
        [HttpGet]
        public IActionResult GetTabs()
        {
            var tabs = new List<Tab>();

            using (MySqlConnection connection = _databaseService.GetConnection())
            {
                connection.Open();
                string query = "SELECT TabID, Instrument, Difficulty, Text, SongID FROM tabs";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tab = new Tab
                    {
                        TabID = reader.GetInt32(0),
                        Instrument = reader.GetString(1),
                        Difficulty = reader.GetString(2),
                        Text = reader.GetString(3),
                        SongID = reader.GetInt32(4)
                    };
                    tabs.Add(tab);
                }

                reader.Close();
            }

            return Ok(tabs);
        }
    }
}
