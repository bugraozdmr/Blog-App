using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete;

public class EFCommentRepository : ICommentRepository
{
    private readonly BlogContext _context;

    public EFCommentRepository(BlogContext context)
    {
        _context = context;
    }


    public IQueryable<Comment> Comments => _context.Comments;

    public void CreateComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
    }
}