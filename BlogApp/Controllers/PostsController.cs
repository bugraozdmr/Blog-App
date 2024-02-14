using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

public class PostsController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;

    public PostsController(IPostRepository postRepository,
        ITagRepository tagRepository)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
    }

    public IActionResult Index()
    {
        return View(new PostViewModel()
        {
            Posts = _postRepository.Posts.Take(5).ToList(),
            Tags = _tagRepository.Tags.ToList()
        });
    }
}