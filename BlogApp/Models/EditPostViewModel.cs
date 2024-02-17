using System.ComponentModel.DataAnnotations;
using BlogApp.Entity;

namespace BlogApp.Models;

public class EditPostViewModel
{
    public int PostId { get; set; }
    
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

    //public DateTime CreatedAt { get; set; } direkt dto'yu map edip yollamayacaksan gerek yok buna tek tek eşliyor

    // default değeri false
    [Display(Name = "Aktifleştir")]
    public bool isActive { get; set; }

    // initialize edildi
    public List<Tag> Tags { get; set; } = new();
}