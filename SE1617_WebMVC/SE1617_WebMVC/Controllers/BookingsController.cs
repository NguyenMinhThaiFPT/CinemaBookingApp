using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1617_WebMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SE1617_WebMVC.Controllers
{
    public class BookingsController : Controller
    {
        private readonly CinemaContext _context;

        public BookingsController(CinemaContext context)
        {
            _context = context;
        }


        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Show)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create(int id)
        {
            Show show = _context.Shows.Where(s => s.ShowId == id).ToList().SingleOrDefault();
            List<Booking> list = _context.Bookings.Where(b => b.ShowId == id).ToList();

            string[] booked = BookedMatrix(id);
            string[] raw_seatstatus = new string[100];
            for (int i = 0; i < 100; i++)
            {
                raw_seatstatus[i] = "0";
            }
            string seat = string.Join("", raw_seatstatus);

            ViewBag.listBook = list;
            ViewBag.Show = show;
            ViewBag.seat = seat;

            return View(booked);
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,ShowId,Name,SeatStatus,Amount")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return Redirect($"/Bookings/Index/{booking.ShowId}");
            }
            ViewData["ShowId"] = new SelectList(_context.Shows, "ShowId", "ShowId", booking.ShowId);
            return View();
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Bookings
                .Include(b => b.Show)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            var show = booking.Show;
            ViewBag.Show = show;
            string[] booked = BookedMatrix((int)show.ShowId);
            ViewBag.booked = booked;
            ViewData["ShowId"] = new SelectList(_context.Shows, "ShowId", "ShowId", booking.ShowId);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,ShowId,Name,SeatStatus,Amount")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = booking.ShowId });
            }
            ViewData["ShowId"] = new SelectList(_context.Shows, "ShowId", "ShowId", booking.ShowId);
            return RedirectToAction(nameof(Index), new { id = booking.ShowId });
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Show)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Show)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return Redirect($"/Bookings/Index/{booking.ShowId}");
        }
        public IActionResult Index(int id)
        {
            List<Booking> list = _context.Bookings.Where(b => b.ShowId == id).ToList();
            ViewBag.listBook = list;
            ViewBag.showId = id;
            string[] booked = BookedMatrix(id);
            return View(booked);
        }
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }

        public string[] BookedMatrix(int ShowId)
        {
            List<string> ListSeats = _context
                                                  .Bookings
                                                  .Where(u => u.ShowId == ShowId)
                                                  .Select(u => u.SeatStatus)
                                                  .ToList();
            int count = ListSeats.Count;
            ViewBag.Count = count;
            //return ListSeats[0].Length.ToString();

            string[] BookedMatrix = new string[100];
            for (int i = 0; i < 100; i++)
            {
                BookedMatrix[i] = "0";
            }

            foreach (string seat in ListSeats)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (seat[i] == '1') BookedMatrix[i] = "1";
                }
            }
            return BookedMatrix;
        }

        public string IndividualBookedMatrix(int bid)
        {
            string seat = _context
                                                  .Bookings
                                                  .Where(u => u.BookingId == bid)
                                                  .Select(u => u.SeatStatus)
                                                  .ToList().SingleOrDefault();
            return seat;
        }
    }
}
