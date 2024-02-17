using BlogApp.Data.Abstract;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

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

    public void EditPost(Post post)
    {
        var entity = _context.Posts.FirstOrDefault(i => i.PostId.Equals(post.PostId));

        if (entity is not null)
        {
            // burda olan bir elemanın elemanları değişiliyor ondan dolayı kullanmadığın değişmediğin elemanları göndermene gerek yok
            // ancak olurda dto ile direkt gönderme yaparsan o zaman tüm değerlerin gitmesi gerek. Ya da eşlenmeli ?? ""
            entity.Title = post.Title;
            entity.Description = post.Description;
            entity.Content = post.Content;
            entity.Image = post.Image;
            entity.Url = post.Url;
            //entity.CreatedAt = post.CreatedAt;
            //entity.Url = post.Url;
            entity.IsActive = post.IsActive;
            entity.Tags = post.Tags;

            _context.SaveChanges();
        }
        
    }

    public void EditPost(Post post, int[] tagIds)
    {
        var entity = _context.Posts.Include(i => i.Tags)
            .FirstOrDefault(i => i.PostId.Equals(post.PostId));

        if (entity is not null)
        {
            // burda olan bir elemanın elemanları değişiliyor ondan dolayı kullanmadığın değişmediğin elemanları göndermene gerek yok
            // ancak olurda dto ile direkt gönderme yaparsan o zaman tüm değerlerin gitmesi gerek. Ya da eşlenmeli ?? ""
            entity.Title = post.Title;
            entity.Description = post.Description;
            entity.Content = post.Content;
            entity.Image = post.Image;
            entity.Url = post.Url;
            //entity.CreatedAt = post.CreatedAt;
            //entity.Url = post.Url;
            entity.IsActive = post.IsActive;
            // içerenleri al ve listeye at
            entity.Tags = _context.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToList();

            _context.SaveChanges();
        }
    }
}