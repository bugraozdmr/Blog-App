using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models;

public class RegisterViewModel
{
    
    [DataType(DataType.EmailAddress)]
    [Display(Name="Eposta")]
    [Required]
    public string Email { get; set; }
    [Required]
    [Display(Name="Parola")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [Display(Name="Parola Tekrar")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; }
    [Required]
    [Display(Name="Kullanıcı Adı")]
    [DataType(DataType.Text)]
    public string Username { get; set; }
    
    [Required]
    [Display(Name="Full isim")]
    [DataType(DataType.Text)]
    public string Name { get; set; }
}