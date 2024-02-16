using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Eposta")]
    public string? Email { get; set; }
    
    [Required]
    //[StringLength(10,ErrorMessage = "Max 10 karakter",MinimumLength=6)]
    [StringLength(10,ErrorMessage = "Max 10 karakter")]
    [DataType(DataType.Password)]
    [Display(Name = "Parola")]
    public string? Password { get; set; }
}