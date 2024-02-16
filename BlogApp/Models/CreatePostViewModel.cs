using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models;

public class CreatePostViewModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Başlık")]
    public string? Title { get; set; }
    
    // url üretilecek slug
    
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Resim Url")]
    public string? Image { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Sayfa içi açıklama")]
    public string? Content { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}