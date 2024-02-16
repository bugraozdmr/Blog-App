
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete;
using BlogApp.Data.Concrete.EFCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("sqlConnection");

builder.Services.AddDbContextPool<BlogContext>(opt =>
{
    opt.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 4, 28)));
    // geliştirmede bu olabilir
    opt.EnableSensitiveDataLogging(true);
});

builder.Services.AddScoped<IPostRepository, EFPostRepository>();
builder.Services.AddScoped<ITagRepository, EFTagRepository>();
builder.Services.AddScoped<ICommentRepository, EFCommentRepository>();
builder.Services.AddScoped<IUserRepository, EFUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

// authorization ve authentication için gerekli routing
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// localhost://posts/react-dersleri

app.MapControllerRoute(
    name: "post_details",
    pattern: "posts/tag/{tag}",
    defaults:new {controller="Posts",action="Index"});

/*app.MapControllerRoute(
    name: "post_by_tag",
    pattern: "posts/details/{url}",
    defaults:new {controller="Posts",action="Details"});*/



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

SeedData.TestDatas(app);

app.Run();