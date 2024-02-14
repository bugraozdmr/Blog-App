using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EFCore;

public class EFPostRepository : IPostRepository
{
    private readonly BlogContext _context;

    public EFPostRepository(BlogContext context)
    {
        _context = context;
    }

    // Tolist demedik burda sorgu için sonrasında biz yazarız
    public IQueryable<Post> Posts => _context.Posts;
    public void CreatePost(Post post)
    {
        _context.Posts.Add(post);
        _context.SaveChanges();
    }
}