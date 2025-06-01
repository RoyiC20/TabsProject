namespace TabsClassLibrary
{
    public class Song
    {
        public int SongID { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty; // Song's name
        public int ArtistID { get; set; } // Foreign Key linking to Artist
        public int? UserID { get; set; } 

        // Navigation property
        public Artist? Artist { get; set; }

        public Tab? Tab { get; set; }
    }
}