using System;
using Microsoft.AspNetCore.Http;
using Week5.Services;
using Week5.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Register the UserService
builder.Services.AddScoped<UserService>();

// Build the app after registering all services
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable sessions
app.UseSession();

// Add authentication middleware
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();

app.MapRazorPages();

app.Run();