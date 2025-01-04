namespace TabsApp.Models
{
    public class Like
    {
        public int SongID { get; set; } // Foreign Key linking to Song
        public int Count { get; set; } // Number of likes for the song

        // Navigation property (optional)
        public Song? Song { get; set; }
    }
}