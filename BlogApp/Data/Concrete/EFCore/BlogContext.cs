using System.Reflection;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore;

public class BlogContext : DbContext
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public BlogContext(DbContextOptions options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // sorunu çözdü -- keyless entity identity için
        base.OnModelCreating(modelBuilder);
        // modelBuilder.ApplyConfiguration(new ProductConfig());
        // modelBuilder.ApplyConfiguration(new CategoryConfig());
        
        // kendi bulur
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}