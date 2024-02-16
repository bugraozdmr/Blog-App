using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Entity;
using BlogApp.Models;
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
}