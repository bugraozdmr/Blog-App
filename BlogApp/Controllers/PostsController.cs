using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

public class PostsController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository; // artık kullanmıyoruz bunu
    private readonly ICommentRepository _commentRepository;

    public PostsController(IPostRepository postRepository,
        ITagRepository tagRepository,
        ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _commentRepository = commentRepository;
    }

    [HttpGet("posts/index")]
    public IActionResult RedirectToPosts()
    {
        return RedirectToAction("Index");
    }

    
    [HttpGet("posts")]
    public async Task<IActionResult> Index(string? tag)
    {
        // cookiedeki bilgiler User.Claims
        var claims = User.Claims;
        
        // anlık Iqueryable
        var posts = _postRepository.Posts;

        if (!string.IsNullOrEmpty(tag))
        {
            posts = posts.Where(p => p.Tags.Any(t => t.Url.Equals(tag)));
        }
        
        return View(new PostViewModel()
        {
            // gelen post kullanılır
            Posts = await posts
                .Include(x => x.Tags)
                .ToListAsync()
            //Tags = _tagRepository.Tags.ToList()
        });
    }

    // böyle yapınca daha iyi oldu commentde çalışıyor böyle
    [HttpGet("posts/{url?}")]
    public async Task<IActionResult> Details(string? url)
    {
        if (_postRepository.Posts.FirstOrDefault(x=>x.Url.Equals(url)) is null)
        {
            return RedirectToAction("Index");
        }
        
        if (string.IsNullOrEmpty(url))
        {
            return RedirectToAction("Index","Posts");
        }
        
        var post = await _postRepository
            .Posts
            .Include(x => x.Tags)   // tag bilgileride gelsin -- bu yoksa tagler gelmez
            .Include(x => x.Comments)
            .ThenInclude(x => x.User)   // gitmiş olduğu commenttede user bilgisini alsın... şeklinde
            .FirstOrDefaultAsync(p => p.Url.Equals(url));
        return View(post);
    }

    
    [HttpPost]
    //[ValidateAntiForgeryToken]  // name değerleri iletilir
    public IActionResult AddComment(int PostId,string Text,string UrlD)
    {
        if (Text is null)
        {
            //ModelState.AddModelError("","boş değer gönderemezsin");
            //TempData["message"] = "Girilmesi gereken alanları boş gönderemezsin.";
            //return RedirectToAction("Details",new {url = UrlD});
            return BadRequest();
        }

        // !! Cookiede tutmak sunucuyu yormaz.artık
        // ona atamıştık değeri
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var Username = User.FindFirstValue(ClaimTypes.Name);
        var avatar = User.FindFirstValue(ClaimTypes.UserData);
        
        
        var entity = new Comment()
        {
            Text = Text,
            CreatedAt = DateTime.Now,
            PostId = PostId,
            UserId = int.Parse(userId ?? "")
        };
        
        _commentRepository.CreateComment(entity);
        
        //return Redirect($"/posts/{Url}");                                 sildik
        // redirectoroute ile programda tanımlı olanlar çağırılıp sonrasında new {url = Url} denebilirdi

        // şuan çalışıyor sorunsuz -- json dönmesi lazım dönen değeri jquery kullanacak
        return Json(new
        {
            Username,
            Text,
            entity.CreatedAt,
            avatar    // resim bu 
        });

        /*return Ok(new
        {
            Username,
            Text,
            entity.CreatedAt,
            entity.User.Image
        });*/
    }

    [Authorize]
    [HttpGet("posts/create")]
    public IActionResult Create()
    {
        return View();
    }
    
    
    [HttpPost]
    public IActionResult Create([FromForm]CreatePostViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (ModelState.IsValid)
        {   // başlangıç olarak isactive false
            _postRepository.CreatePost(new Post()
            {
                IsActive = false,
                UserId = int.Parse(userId),
                Title = model.Title,
                Content = model.Content,
                Description = model.Description,
                Image = model.Image,
                CreatedAt = DateTime.Now,
                Tags = _tagRepository.Tags.Take(2).ToList(),
                Url = $"{RemoveNonAlphanumericAndSpecialChars(ReplaceTurkishCharacters(model.Title.Replace(' ', '-').ToLower()))}.{GenerateUniqueHash()}"
            });
            return RedirectToAction("Index", "Posts");
        }
        return View(model);
    }


    [Authorize]
    [HttpGet("posts/list")]
    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
        var role = User.FindFirstValue(ClaimTypes.Role);

        var posts = _postRepository.Posts;

        // role yoksa sadece kendininkileri değişebilir
        if (string.IsNullOrEmpty(role))
        {
            posts = posts.Where(i => i.UserId == userId);
        }
        
        return View(await posts.ToListAsync());
    }
    
    
    private static string GenerateUniqueHash()
    {
        string guid = Guid.NewGuid().ToString("N");

        
        int length = 6;
        if (guid.Length < length)
        {
            length = guid.Length;
        }

        string uniqueHash = guid.Substring(0, length);

        return uniqueHash;
    }
    
    private string ReplaceTurkishCharacters(string input)
    {
        input = input.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");
        input = input.Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u").Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");
        return input;
    }
    
    private string RemoveNonAlphanumericAndSpecialChars(string input)
    {
        // LINQ kullanarak boşluk, tire ve nokta karakterlerini filtrele
        var filteredCharacters = input
            .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')
            .ToArray();

        // Filtrelenmiş karakterleri yeni bir string olarak oluştur
        string result = new string(filteredCharacters);

        return result;
    }
}