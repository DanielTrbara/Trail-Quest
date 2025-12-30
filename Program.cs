using Microsoft.EntityFrameworkCore;
using StepUp.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string holen (appsettings oder ENV ConnectionStrings__DefaultConnection)
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");

// Render liefert oft postgres:// oder postgresql:// -> in Npgsql Format umwandeln
cs = NormalizePostgresConnectionString(cs);

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Migration beim Start anwenden
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

// Auf Render ist HTTPS-Termination vorne dran -> optional auskommentieren, wenn Warnung nervt
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static string NormalizePostgresConnectionString(string input)
{
    input = input.Trim();

    // Schon Key-Value? Dann passt es.
    if (input.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        return input;

    // URL-Form: postgres://user:pass@host:port/db
    if (input.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        input.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(input);

        var userInfo = uri.UserInfo.Split(':', 2);
        var user = Uri.UnescapeDataString(userInfo[0]);
        var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

        var db = uri.AbsolutePath.TrimStart('/');

        // Render Postgres -> SSL meist required
        return
            $"Host={uri.Host};" +
            (uri.Port > 0 ? $"Port={uri.Port};" : "") +
            $"Database={db};Username={user};Password={pass};" +
            "Ssl Mode=Require;Trust Server Certificate=true";
    }

    return input;
}
