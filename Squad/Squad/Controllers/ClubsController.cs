using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Squad.Models;
using Squad.ViewModels;
using X.PagedList;

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

        public  IActionResult Index(int? page)
        {
            var clubs =  _context.Clubs.OrderByDescending(m => m.Rate).ToList().ToPagedList(page ?? 1, 5);
            // clubs.ToPagedList(page ?? 1, 9);
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Clubs.FindAsync(id);

            if (movie == null)
                return NotFound();

            var viewModel = new ClubFormViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                ShortName=movie.ShortName,
                Stadium=movie.Stadium,
                WebSite=movie.WebSite,
                LeagueId = movie.LeagueId,
                Rate = movie.Rate,
                Year = movie.Year,
                Titles = movie.Titles,
                Logo = movie.Logo,
                Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync()
            };

            return View("ClubForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClubFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                return View("ClubForm", model);
            }

            var club = await _context.Clubs.FindAsync(model.Id);

            if (club == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Logo = dataStream.ToArray();

                if (!_allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG, .JPG images are allowed!");
                    return View("ClubForm", model);
                }

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Leagues = await _context.Leagues.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("ClubForm", model);
                }

                club.Logo = model.Logo;
            }

            club.Name = model.Name;
            club.ShortName = model.ShortName;
            club.LeagueId = model.LeagueId;
            club.Stadium = model.Stadium;
            club.WebSite = model.WebSite;
            club.Year = model.Year;
            club.Rate = model.Rate;
            club.Titles = model.Titles;
            

            _context.SaveChanges();

            //_toastNotification.AddSuccessToastMessage("Movie updated successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var club = await _context.Clubs.Include(m => m.League).SingleOrDefaultAsync(m => m.Id == id);

            if (club == null)
                return NotFound();

            return View(club);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var club = await _context.Clubs.FindAsync(id);

            if (club == null)
                return NotFound();

            _context.Clubs.Remove(club);
            _context.SaveChanges();

            return Ok();

        }


    }

}
