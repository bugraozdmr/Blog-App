namespace BlogApp.Entity;

public class Post
{
    public int PostId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Image { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    
    public int UserId { get; set; }
    // sadece 1 kişi alabilir
    public User User { get; set; } = null!;
    // üretildiği yerde tanımlanmalı yoksa patlar
    public List<Tag> Tags { get; set; } = new List<Tag>();

    public List<Comment> Comments { get; set; } = new List<Comment>();
}