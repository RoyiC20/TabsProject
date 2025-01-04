namespace TabsClassLibrary
{
    public class Favorite
    {
        public int UserID { get; set; } // Foreign Key linking to User
        public int SongID { get; set; } // Foreign Key linking to Song
        public DateTime FavoritedAt { get; set; } // Timestamp when favorited

        // Navigation properties (optional)
        public User? User { get; set; }
        public Song? Song { get; set; }
    }
}
