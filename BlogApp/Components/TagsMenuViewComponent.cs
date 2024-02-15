using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Components;

public class TagsMenuViewComponent : ViewComponent
{
    private readonly ITagRepository _tagRepository;
    public TagsMenuViewComponent(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View(await _tagRepository.Tags.ToListAsync());
    }
}