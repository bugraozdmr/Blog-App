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

    // routelar çok karışık adam akıllı olması için düzenlenmeli
    
    [HttpGet("posts/index")]
    public IActionResult RedirectToPosts()
    {
        return RedirectToAction("Index");
    }

    
    [HttpGet("posts/tag/{tag}")]
    [HttpGet("posts/")]     // bunu ekleyerek çözdüm
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
                .Where(i => i.IsActive.Equals(true))
                .Include(x => x.Tags)
                .ToListAsync()
            //Tags = _tagRepository.Tags.ToList()
        });
    }

    // böyle yapınca daha iyi oldu commentde çalışıyor böyle
    [HttpGet("posts/{url?}")]
    public async Task<IActionResult> Details(string? url)
    {
        if (_postRepository.Posts.FirstOrDefault(x=>x.Url.Equals(url)) is null ||
            _postRepository.Posts.FirstOrDefault(x=>x.Url.Equals(url)).IsActive.Equals(false))
        {
            return RedirectToAction("Index");
        }
        
        if (string.IsNullOrEmpty(url))
        {
            return RedirectToAction("Index","Posts");
        }
        
        var post = await _postRepository
            .Posts
            .Include(x => x.User)
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
    [Authorize]
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
    [HttpGet("posts/edit/{id}")]
    public IActionResult Update([FromRoute]int id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var model = _postRepository
            .Posts
            .Include(a => a.Tags)
            .FirstOrDefault(x => x.PostId.Equals(id));

        ViewBag.Tags = _tagRepository.Tags.ToList();
        
        
        if (model is not null)
        {
            return View(new EditPostViewModel()
            {
                Content = model.Content,
                Title = model.Title,
                Description = model.Description,
                Image = model.Image,
                PostId = model.PostId,
                isActive = model.IsActive,
                Tags = model.Tags
            });
        }
        
        return NotFound();
    }

    [Authorize]
    [HttpPost("posts/edit/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Update([FromForm] EditPostViewModel model,int[] TagIds)
    {
        string? urlNew = null;
        
        if (ModelState.IsValid)
        {
            var prevModel = _postRepository.Posts.FirstOrDefault(x => x.PostId.Equals(model.PostId));
            
            if (!prevModel.Title.Equals(model.Title))
            {
                // title değiştiyse urlde değiscek
                urlNew =
                    $"{RemoveNonAlphanumericAndSpecialChars(ReplaceTurkishCharacters(model.Title.Replace(' ', '-').ToLower()))}.{GenerateUniqueHash()}";
            }
            
            
            // eğer olaki isactive true idi ama mod düzenlerken true olmasına rağmen false olacak -- ben bunu direkt moddada gösterirdim btw
            if (!User.IsInRole("Admin"))
            {
                // neyse o gitsin
                model.isActive = prevModel.IsActive;
            }
            
            var updateModel = new Post()
            {
                Content = model.Content,
                PostId = model.PostId,
                Description = model.Description,
                Title = model.Title,
                Image = model.Image,
                Url = urlNew ?? prevModel.Url,
                IsActive = model.isActive
            };

            
            //_postRepository.EditPost(updateModel);
            
            // değiştirdik
            _postRepository.EditPost(updateModel,TagIds);
            
            return RedirectToAction("List");
        }

        // hata varsada bunlar geçsin
        ViewBag.Tags = _tagRepository.Tags.ToList();
        
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