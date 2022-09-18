using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Squad.Models;
using Squad.ViewModels;

namespace Squad.Controllers
{
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public ClubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clubs = await _context.Clubs.OrderByDescending(m => m.Rate).ToListAsync();
            return View(clubs);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ClubFormViewModel
            {
                Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync()
            };


            return View("ClubForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClubFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                return View("ClubForm", model);
            }

            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Logo", "Please select movie poster!");
                return View("ClubForm", model);
            }

            var logo = files.FirstOrDefault();

            if (!_allowedExtenstions.Contains(Path.GetExtension(logo.FileName).ToLower()))
            {
                model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Logo", "Only .PNG, .JPG images are allowed!");
                return View("ClubForm", model);
            }

            if (logo.Length > _maxAllowedPosterSize)
            {
                model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Logo", "Poster cannot be more than 1 MB!");
                return View("ClubForm", model);
            }

            using var dataStream = new MemoryStream();

            await logo.CopyToAsync(dataStream);

            var c = new Club
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Year = model.Year,
                Rate = model.Rate,
                WebSite = model.WebSite,
                Stadium=model.Stadium,
                Titles=model.Titles,
                LeagueId=model.LeagueId,
                Logo = dataStream.ToArray()
            };

            _context.Clubs.Add(c);
            _context.SaveChanges();

            //_toastNotification.AddSuccessToastMessage("Movie created successfully");
            return RedirectToAction(nameof(Index));
        }

    }

}
