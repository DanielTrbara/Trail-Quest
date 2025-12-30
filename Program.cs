using Microsoft.EntityFrameworkCore;
using StepUp.Data;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// DB: NUR Postgres, kein Fallback
// ===============================
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
{
    throw new Exception(
        "ConnectionStrings:DefaultConnection is missing. " +
        "Postgres connection string must be provided via environment variables."
    );
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(cs);
});

// ===============================
// MVC
// ===============================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ===============================
// DB Migration beim Start
// ===============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ===============================
// Middleware
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ===============================
// Routing
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();