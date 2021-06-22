using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;

namespace MVC_BugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public TicketCommentsController(ApplicationDbContext context, 
                                        UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TicketComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketComment.Include(t => t.Ticket).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketComment == null)
            {
                return NotFound();
            }

            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public IActionResult Create()
        {
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,Comment")] TicketComment ticketComment)
        {
            // UserId,Created,Id
            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);

                ticketComment.Created = DateTimeOffset.Now;
                ticketComment.UserId = userId;
                
                _context.Add(ticketComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("TicketDetails", "Tickets", new { id = ticketComment.TicketId });
            }
            //ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketComment.TicketId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComment.FindAsync(id);
            if (ticketComment == null)
            {
                return NotFound();
            }
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketComment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,UserId,Comment,Created")] TicketComment ticketComment)
        {
            if (id != ticketComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketCommentExists(ticketComment.Id))
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
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketComment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketComment = await _context.TicketComment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketComment == null)
            {
                return NotFound();
            }

            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketComment = await _context.TicketComment.FindAsync(id);
            _context.TicketComment.Remove(ticketComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketCommentExists(int id)
        {
            return _context.TicketComment.Any(e => e.Id == id);
        }
    }
}
