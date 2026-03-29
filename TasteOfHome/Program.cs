using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using TasteOfHome.Data;
using TasteOfHome.Services;
using TasteOfHome.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient<IAiRestaurantEnrichmentService, AiRestaurantEnrichmentService>();

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<PasswordResetService>();

builder.Services.AddHttpClient<ISmsSender, SmsSender>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasteofhome.db"));

builder.Services.Configure<GooglePlacesOptions>(
    builder.Configuration.GetSection("GooglePlaces"));
builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(12);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.SaveTokens = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("/login-google", (string? returnUrl) =>
{
    return Results.Challenge(
        new AuthenticationProperties
        {
            RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl
        },
        new[] { "Google" }
    );
});

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();