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
                new Entity.Tag() {Text = "web programlama",Url= "web-programlama",Color = TagColors.danger},
                new Entity.Tag() {Text = "php",Url = "php",Color = TagColors.danger},
                new Entity.Tag() {Text = "backend",Url = "backend",Color = TagColors.primary},
                new Entity.Tag() {Text = "frontend",Url = "frontend",Color = TagColors.warning},
                new Entity.Tag() {Text = "sistem programlama",Url = "system-programming",Color = TagColors.success}
                );

            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User(){Username = "polat",Image = "/images/userimages/user1.png"},
                new User(){Username = "grant",Image = "/images/userimages/user2.png"}
            );
            context.SaveChanges();
        }
        
        if (!context.Posts.Any())
        {
            context.Posts.AddRange(
                new Post(){Title = "asp.net core"
                    ,Url = "asp-net-core" 
                    ,Content = "asp net core eğitimi"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-10)
                    ,Image = "/images/postimages/asp.jpg"
                    ,Tags = context.Tags.Take(3).ToList()
                    ,UserId = 1
                    ,Comments = new List<Comment>()
                    {   // user ve post ile ilgili değerleri vermeye gerek yok zaten anlar
                        new Comment() {CreatedAt = DateTime.Now,Text = "iyi bir kurs",UserId = 1},
                        new Comment() {CreatedAt = DateTime.Now.AddHours(-10),Text = "secdiğim kurs",UserId = 2},
                        
                    }
                },
                new Post(){Title = "next.js"
                    ,Url = "next-js"
                    ,Content = "javascript camp"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-2)
                    ,Image = "/images/postimages/next.jpg"
                    ,Tags = context.Tags.Take(2).ToList()
                    ,UserId = 2},
                new Post(){Title = "React"
                    ,Url = "react"
                    ,Content = "react dersleri"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-60)
                    ,Image = "/images/postimages/next.jpg"
                    ,Tags = context.Tags.Take(2).ToList()
                    ,UserId = 2},
                new Post(){Title = "Django dersleri"
                    ,Url = "Django-dersleri"
                    ,Content = "Django dersleri bizimle öğren"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-1)
                    ,Image = "/images/postimages/next.jpg"
                    ,Tags = context.Tags.Take(3).ToList()
                    ,UserId = 1},
                new Post(){Title = "Python"
                    ,Url = "python-dersleri"
                    ,Content = "Python dersleri bizimle öğren"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-5)
                    ,Image = "/images/postimages/asp.jpg"
                    ,Tags = context.Tags.Take(3).ToList()
                    ,UserId = 1},
                new Post(){Title = "Java dersleri"
                    ,Url = "Java-dersleri"
                    ,Content = "Java dersleri bizimle öğren"
                    ,IsActive = true
                    ,CreatedAt = DateTime.Now.AddDays(-20)
                    ,Image = "/images/postimages/asp.jpg"
                    ,Tags = context.Tags.Take(4).ToList()
                    ,UserId = 2}
            );
            context.SaveChanges();
        }
        
        
    }
}