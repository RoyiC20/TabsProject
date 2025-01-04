namespace TabsApp.Models
{
    public class Comment
    {
        public int CommentID { get; set; } // Primary Key
        public int UserID { get; set; } // Foreign Key linking to User
        public int SongID { get; set; } // Foreign Key linking to Song
        public string CommentText { get; set; } = string.Empty; // The content of the comment
        public DateTime CommentedAt { get; set; } // Timestamp of the comment

        // Navigation properties (optional)
        public User? User { get; set; }
        public Song? Song { get; set; }
    }
}
