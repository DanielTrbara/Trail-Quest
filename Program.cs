using Microsoft.EntityFrameworkCore;
using StepUp.Data;

var builder = WebApplication.CreateBuilder(args);

// === DB einrichten (SQLite) ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC aktivieren
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- DB automatisch anlegen/migrieren ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
// ----------------------------------------

// Error Handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// WICHTIG: statische Dateien (wwwroot)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Standard-Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();