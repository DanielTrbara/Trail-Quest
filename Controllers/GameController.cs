using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepUp.Data;
using StepUp.Models;

namespace StepUp.Controllers
{
    public class GameController(AppDbContext db) : Controller
    {
        // GET: /Game/Scan?poi=1
        [HttpGet]
        public IActionResult Scan(int poi)
        {
            if (poi <= 0)
            {
                // Fallback: einfach 1
                poi = 1;
            }

            var model = new ScanInputViewModel
            {
                PoiId = poi
            };

            return View(model);
        }

        // POST: /Game/Scan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scan(ScanInputViewModel model)
        {
            if (!model.Validate(ModelState))
            {
                return View(model);
            }

            var cleanedName = model.TeamName.Trim();
            var normalizedName = cleanedName.ToLowerInvariant();

            // Team holen oder anlegen (case-insensitive)
            var team = await db.Teams
                .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedName);

            if (team == null)
            {
                team = new Team
                {
                    Name = cleanedName
                };
                db.Teams.Add(team);
                await db.SaveChangesAsync();
            }

            // Prüfen, ob dieses Team diesen POI schon einmal gescannt hat
            var alreadyScanned = await db.Scans
                .AnyAsync(s => s.TeamId == team.Id && s.PoiId == model.PoiId);

            bool pointAdded = false;

            if (!alreadyScanned)
            {
                // Scan speichern (1 Punkt für diesen POI)
                var scan = new Scan
                {
                    TeamId = team.Id,
                    PoiId = model.PoiId,
                    ScannedAt = DateTime.UtcNow
                };

                db.Scans.Add(scan);
                await db.SaveChangesAsync();
                pointAdded = true;
            }

            // Ranking laden
            var ranking = await db.Teams
                .Select(t => new RankingEntryViewModel
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    Points = t.Scans.Count,
                    IsCurrentTeam = t.Id == team.Id
                })
                .OrderByDescending(r => r.Points)
                .ThenBy(r => r.TeamName)
                .ToListAsync();

            // Info für die View (optional)
            ViewBag.ScanMessage = pointAdded
                ? "Euch wurde 1 Punkt gutgeschrieben."
                : "Ihr habt diesen Standort bereits gescannt. Kein weiterer Punkt.";

            return View("Ranking", ranking);
        }

        // GET: /Game/Ranking
        [HttpGet]
        public async Task<IActionResult> Ranking()
        {
            var ranking = await db.Teams
                .Select(t => new RankingEntryViewModel
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    Points = t.Scans.Count,
                    IsCurrentTeam = false
                })
                .OrderByDescending(r => r.Points)
                .ThenBy(r => r.TeamName)
                .ToListAsync();

            return View(ranking);
        }
    }
}