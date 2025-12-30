using Microsoft.EntityFrameworkCore;
using StepUp.Data;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// Wenn Render/Postgres vorhanden -> Postgres, sonst lokal SQLite
if (!string.IsNullOrWhiteSpace(cs) && (cs.Contains("Host=", StringComparison.OrdinalIgnoreCase) || cs.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)))
{
    builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=StepUp.db"));
}

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();