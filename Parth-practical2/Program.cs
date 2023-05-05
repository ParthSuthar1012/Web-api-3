using Microsoft.EntityFrameworkCore;
using pracical1.dataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var con = builder.Configuration.GetConnectionString("con_string");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(con, ServerVersion.AutoDetect(con));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
