using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogApp.Components;

public class NewPostsViewComponent : ViewComponent
{
    private readonly IPostRepository _postRepository;

    public NewPostsViewComponent(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // 5 ten fazla varsa sorun olmasın diye
        var posts = await _postRepository.Posts.OrderBy(p => p.CreatedAt).Take(5).ToListAsync();
        return View(posts);
    }
}