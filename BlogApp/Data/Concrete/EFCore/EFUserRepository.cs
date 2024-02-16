using BlogApp.Data.Abstract;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EFCore;

public class EFUserRepository : IUserRepository
{
    private readonly BlogContext _context;

    public EFUserRepository(BlogContext context)
    {
        _context = context;
    }

    public IQueryable<User> Users => _context.Users;
    public void CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }
}