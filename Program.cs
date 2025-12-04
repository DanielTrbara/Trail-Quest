using Microsoft.EntityFrameworkCore;
using StepUp.Data;

var builder = WebApplication.CreateBuilder(args);

// === DB einrichten (SQLite) ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC aktivieren
builder.Services.AddControllersWithViews();

var app = builder.Build();


// ----------------------------------------------------
//  DB automatisch anlegen/migrieren (WICHTIG für Render)
// ----------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // erstellt StepUp.db, falls sie nicht existiert
}
// ----------------------------------------------------


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();