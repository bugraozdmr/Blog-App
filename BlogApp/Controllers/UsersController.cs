using System.Net;
using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IActionResult Login()
    {
        // olduki canı sıkıldı böyle bir şey denedi böyle yap
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Posts");
        }
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm]LoginViewModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Posts");
        }
        
        if (ModelState.IsValid)
        {
            var user = _userRepository
                .Users
                .FirstOrDefault(x => x.Email.Equals(model.Email) 
                                     && x.Password.Equals(model.Password));

            if (user is not null)
            {
                var userClaims = new List<Claim>();
                // user id'si kalsın
                userClaims.Add(new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()));
                userClaims.Add(new Claim(ClaimTypes.Name,user.Username ?? ""));
                userClaims.Add(new Claim(ClaimTypes.GivenName,user.Name ?? ""));
                userClaims.Add(new Claim(ClaimTypes.UserData,user.Image ?? ""));

                if (user.Email == "bugra123@gmail.com")
                {
                    userClaims.Add(new Claim(ClaimTypes.Role,"Admin"));
                }

                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties()
                {
                    // beni hatırla demek
                    IsPersistent = true
                };

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Posts");
            }
            
            ModelState.AddModelError("","Kullanıcı bulunamadı eposta ya da şifre hatalı");
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        // identity kullanmış olsaydık bunlara gerek olmazdı ..
        // cookie ekliceksen tarayıcıya sonra sunucuya göndericeksen izle önce
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Posts");
    }

    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Posts");
        }
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register([FromForm] RegisterViewModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Posts");
        }

        if (ModelState.IsValid)
        {
            
            
            
            return RedirectToAction("Login");
        }
        
        return View(model);
    }

}