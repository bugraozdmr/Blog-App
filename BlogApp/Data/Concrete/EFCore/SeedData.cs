using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EFCore;

public class SeedData
{
    public static async void TestDatas(IApplicationBuilder app)
    {
        var context = app.ApplicationServices
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<BlogContext>();

        // bekleyen migration varsa direkt al
        if (context.Database.GetAppliedMigrations().Any())
        {
            context.Database.Migrate();
        }

        if (!context.Tags.Any())
        {
            context.Tags.AddRange(
                new Entity.Tag() {Text = "web programlama"},
                new Entity.Tag() {Text = "php"},
                new Entity.Tag() {Text = "backend"},
                new Entity.Tag() {Text = "frontend"},
                new Entity.Tag() {Text = "sistem programlama"}
                );

            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User(){Username = "polat"},
                new User(){Username = "grant"}
            );
            context.SaveChanges();
        }
        
        if (!context.Posts.Any())
        {
            context.Posts.AddRange(
                new Post(){Title = "asp.net core"
                    ,Content = "asp net core eğitimi"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-10)
                    ,Image = "/images/postimages/asp.jpg"
                    ,Tags = context.Tags.Take(3).ToList()
                    ,UserId = 1},
                new Post(){Title = "next.js"
                    ,Content = "javascript camp"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-2)
                    ,Image = "/images/postimages/next.jpg"
                    ,Tags = context.Tags.Take(2).ToList()
                    ,UserId = 2}
            );
            context.SaveChanges();
        }
        
        
    }
}