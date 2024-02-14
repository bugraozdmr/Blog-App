using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EFCore;

public class EFTagRepository : ITagRepository
{
    private readonly BlogContext _context;

    public EFTagRepository(BlogContext context)
    {
        _context = context;
    }
    
    // Tolist demedik burda sorgu için sonrasında biz yazarız
    public IQueryable<Tag> Tags => _context.Tags;
    public void CreateTag(Tag tag)
    {
        _context.Tags.Add(tag);
        _context.SaveChanges();    
    }
}