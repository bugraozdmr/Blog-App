using System.Net;
using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Posts");
        }

        if (ModelState.IsValid)
        {
            var user = await _userRepository
                .Users
                .FirstOrDefaultAsync(x => x.Username.Equals(model.Username) || 
                                          x.Email.Equals(model.Email));

            if (user == null)
            {
                _userRepository.CreateUser(new User()
                {
                    Username = model.Username,
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Image = "/images/userimages/user1.png"
                });
                
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("","Username ya da email kullanımda");
            }
        }
        
        return View(model);
    }

}