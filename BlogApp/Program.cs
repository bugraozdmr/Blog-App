
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EFCore;
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

SeedData.TestDatas(app);

app.Run();