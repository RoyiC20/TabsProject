using MySql.Data.MySqlClient;

namespace TabsApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }



        public bool DeleteSongWithTabs(int songId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // מחיקת טאבים קודם
                var deleteTabsCmd = new MySqlCommand("DELETE FROM tabs WHERE songID = @id", connection, transaction);
                deleteTabsCmd.Parameters.AddWithValue("@id", songId);
                deleteTabsCmd.ExecuteNonQuery();

                // מחיקת שיר
                var deleteSongCmd = new MySqlCommand("DELETE FROM songs WHERE songID = @id", connection, transaction);
                deleteSongCmd.Parameters.AddWithValue("@id", songId);
                int rowsAffected = deleteSongCmd.ExecuteNonQuery();

                transaction.Commit();

                return rowsAffected > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }


    }
}

    