using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1617_WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SE1617_WebMVC.Controllers
{
    public class ShowsController : Controller
    {
        private readonly CinemaContext _context;

        public ShowsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            var cinemaContext = _context.Shows.Include(s => s.Film).Include(s => s.Room).OrderByDescending(s => s.ShowDate);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title", cinemaContext.First().FilmId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", cinemaContext.First().RoomId);
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["date"] = date;
            return View(await cinemaContext.ToListAsync());
        }

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // GET: Shows/Create
        public IActionResult Create(DateTime date, int roomid)
        {

            string searchDate = date.ToString("yyyy-MM-dd");
            ViewData["date"] = searchDate;
            int?[] slots = getSlotsByDate(date, roomid);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", roomid);
            ViewData["slot"] = new SelectList(slots);

            return View();
        }

        private int?[] getSlotsByDate(DateTime date, int roomId)
        {
            int?[] a = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int?[] slots = _context.Shows.Where(s => s.RoomId == roomId && s.ShowDate == date).Select(s => s.Slot).ToArray();
            foreach (int number in slots)
            {
                int numToRemove = number;
                a = a.Where(val => val != numToRemove).ToArray();
            }
            return a;
        }

        private int?[] getSlotsByDateEdit(DateTime date, int roomId, int currentSlot)
        {
            int?[] a = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int?[] slots = _context.Shows.Where(s => s.RoomId == roomId && s.ShowDate == date).Select(s => s.Slot).ToArray();
            foreach (int number in slots)
            {
                int numToRemove = number;
                if (numToRemove != currentSlot)
                    a = a.Where(val => val != numToRemove).ToArray();
            }
            return a;
        }

        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowId,RoomId,FilmId,ShowDate,Price,Status,Slot")] Show show)
        {
            if (ModelState.IsValid)
            {
                _context.Add(show);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title", show.FilmId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", show.RoomId);
            return View(show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            int?[] slots = getSlotsByDateEdit((DateTime)show.ShowDate, show.RoomId, (int)show.Slot);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title", show.FilmId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", show.RoomId);
            ViewData["slot"] = new SelectList(slots);
            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShowId,RoomId,FilmId,ShowDate,Price,Status,Slot")] Show show)
        {
            if (id != show.ShowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.ShowId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "CountryCode", show.FilmId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", show.RoomId);
            return View(show);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Shows.FindAsync(id);
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.ShowId == id);
        }


        [HttpGet, ActionName("Search")]
        public IActionResult Search(DateTime date, int roomid, int filmid)
        {

            /*string date = form["date"];
            string roomid = form["roomid"];
            int filmid = Convert.ToInt32(form["filmid"]);*/
            List<Show> list = _context.Shows
                .Where(s => s.FilmId == filmid
                && s.ShowDate == date
                && s.RoomId == roomid)
                .OrderByDescending(s => s.ShowDate)
                .ToList<Show>();
            string searchDate = date.ToString("yyyy-MM-dd");
            ViewData["date"] = searchDate;
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title", filmid);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", roomid);


            return View("/Views/Shows/Index.cshtml", list);
        }

        private void Good()
        {
            int a = 1;
        }


    }
}
