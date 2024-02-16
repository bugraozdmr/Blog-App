using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

public class UsersController : Controller
{
    
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login([FromForm]LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            
        }
        return View(model);
    }
}