namespace BlogApp.Entity;

public class Comment
{
    public int CommentId { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }

    // burda id yi ve post u almak zorunda -- ayrıca comment hem post hemde kullanıcı ile ilişkilendirilir
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}