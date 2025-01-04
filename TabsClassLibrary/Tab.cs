namespace TabsClassLibrary
{
    public class Tab
    {
        public int TabID { get; set; } // Primary Key
        public string Instrument { get; set; } = string.Empty; // Instrument (Guitar, Bass)
        public string Difficulty { get; set; } = string.Empty; // Difficulty (Easy, Medium, Hard)
        public string Text { get; set; } = string.Empty; // Tab content in text format
        public int SongID { get; set; } // Foreign Key linking to Song

        // Navigation property 
        public Song? Song { get; set; }
    }
}
