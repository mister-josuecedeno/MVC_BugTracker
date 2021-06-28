using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
using MVC_BugTracker.Services.Interfaces;

namespace MVC_BugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _roleService;
        private readonly IBTFileService _fileService;

        public TicketAttachmentsController(ApplicationDbContext context,
                                           UserManager<BTUser> userManager,
                                           IBTProjectService projectService,
                                           IBTRolesService roleService, 
                                           IBTFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _roleService = roleService;
            _fileService = fileService;
        }

        // GET: TicketAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketAttachment.Include(t => t.Ticket).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketAttachments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        public IActionResult Create()
        {
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ProjectId, string DeveloperUserId, string OwnerUserId, [Bind("Id,TicketId,Description,FileName")] TicketAttachment ticketAttachment, IFormFile FileData)
        {
            // Created,UserId,,FileData,FileContentType

            // Return to Referring Page
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            #region IsThisMyTicket
            // Is this my ticket???
            bool isThisMyTicket = false;

            // BTUser
            BTUser btUser = await _userManager.GetUserAsync(User);

            // UserId
            string userId = _userManager.GetUserId(User);

            // Developer
            bool isDeveloper = false;
            if (DeveloperUserId != "" && DeveloperUserId != null)
            {
                isDeveloper = (bool)(DeveloperUserId.Equals(userId));
            }

            // Submitter
            bool isSubmitter = false;

            if (OwnerUserId != "" && OwnerUserId != null)
            {
                isSubmitter = (bool)(OwnerUserId.Equals(userId));
            }

            // Project Manager
            var ticketPMId = (await _projectService.GetProjectManagerAsync(ProjectId)).Id;
            bool isPM = false;

            if (ticketPMId != null)
            {
                isPM = ticketPMId.Equals(userId);
            }

            // Admin
            var isAdmin = await _roleService.IsUserInRoleAsync(btUser, Roles.Admin.ToString());

            // !!! Demo User should not be allowed to edit
            var isDemo = await _roleService.IsUserInRoleAsync(btUser, Roles.DemoUser.ToString());

            isThisMyTicket = (isDeveloper || isSubmitter || isPM || isAdmin) && (!isDemo);


            if (!isThisMyTicket)
            {
                TempData["StatusMessage"] = "Error - You do not have access to complete this action.";
                return Redirect(ViewBag.returnUrl);
            }
            #endregion

            if (ModelState.IsValid)
            {
                ticketAttachment.Created = DateTimeOffset.Now;
                ticketAttachment.UserId = userId;

                ticketAttachment.FileData = await _fileService.EncodeFileAsync(FileData);
                ticketAttachment.FileContentType = _fileService.ContentType(FileData);

                _context.Add(ticketAttachment);
                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return RedirectToAction("TicketDetails", "Tickets", new { id = ticketAttachment.TicketId });
            }
            //ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketAttachment.TicketId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment.FindAsync(id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,UserId,Description,Created,FileName,FileData,FileContentType")] TicketAttachment ticketAttachment)
        {
            if (id != ticketAttachment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketAttachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketAttachmentExists(ticketAttachment.Id))
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
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Id", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProjectId, string DeveloperUserId, string OwnerUserId, int id)
        {

            // Return to Referring Page
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            #region IsThisMyTicket
            // Is this my ticket???
            bool isThisMyTicket = false;

            // BTUser
            BTUser btUser = await _userManager.GetUserAsync(User);

            // UserId
            string userId = _userManager.GetUserId(User);

            // Developer
            bool isDeveloper = false;
            if (DeveloperUserId != "" && DeveloperUserId != null)
            {
                isDeveloper = (bool)(DeveloperUserId.Equals(userId));
            }

            // Submitter
            bool isSubmitter = false;

            if (OwnerUserId != "" && OwnerUserId != null)
            {
                isSubmitter = (bool)(OwnerUserId.Equals(userId));
            }

            // Project Manager
            var ticketPMId = (await _projectService.GetProjectManagerAsync(ProjectId)).Id;
            bool isPM = false;

            if (ticketPMId != null)
            {
                isPM = ticketPMId.Equals(userId);
            }

            // Admin
            var isAdmin = await _roleService.IsUserInRoleAsync(btUser, Roles.Admin.ToString());

            // !!! Demo User should not be allowed to edit
            var isDemo = await _roleService.IsUserInRoleAsync(btUser, Roles.DemoUser.ToString());

            isThisMyTicket = (isDeveloper || isSubmitter || isPM || isAdmin) && (!isDemo);


            if (!isThisMyTicket)
            {
                TempData["StatusMessage"] = "Error - You do not have access to complete this action.";
                return Redirect(ViewBag.returnUrl);
            }
            #endregion

            var ticketAttachment = await _context.TicketAttachment.FindAsync(id);
            _context.TicketAttachment.Remove(ticketAttachment);
            await _context.SaveChangesAsync();

            //return RedirectToAction(nameof(Index));
            //return RedirectToAction("TicketDetails", "Tickets", new { id = ticketAttachment.TicketId });
            //return Redirect(returnUrl);
            return Redirect(ViewBag.returnUrl);
        }

        private bool TicketAttachmentExists(int id)
        {
            return _context.TicketAttachment.Any(e => e.Id == id);
        }
    }
}
