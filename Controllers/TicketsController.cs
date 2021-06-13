using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Extensions;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
using MVC_BugTracker.Models.ViewModels;
using MVC_BugTracker.Services.Interfaces;

namespace MVC_BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTCompanyInfoService _infoService;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTHistoryService _historyService;
        private readonly IBTCompanyInfoService _companyService;
        private readonly IBTNotificationService _notificationService;


        public TicketsController(ApplicationDbContext context,
                                 IBTCompanyInfoService infoService,
                                 IBTTicketService ticketService,
                                 UserManager<BTUser> userManager,
                                 IBTProjectService projectService,
                                 IBTHistoryService historyService, 
                                 IBTCompanyInfoService companyService, 
                                 IBTNotificationService notificationService)
        {
            _context = context;
            _infoService = infoService;
            _ticketService = ticketService;
            _userManager = userManager;
            _projectService = projectService;
            _historyService = historyService;
            _companyService = companyService;
            _notificationService = notificationService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ticket
                                        .Include(t => t.DeveloperUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.Project)
                                        .Include(t => t.TicketStatus)
                                        .ToListAsync();

            return View(await applicationDbContext);
        }

        // GET: ALL Tickets
        public async Task<IActionResult> AllTickets()
        {
            // GET company id
            int companyId = User.Identity.GetCompanyId().Value;

            List<Ticket> tickets = new();

            try
            {
                tickets = await _infoService.GetAllTicketsAsync(companyId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting all tickets - {ex.Message}");
                throw;
            }

            return View(tickets);
        }

        // GET: MY Tickets
        public async Task<IActionResult> MyTickets()
        {
            // GET my user id
            string userId = _userManager.GetUserId(User);

            // SET ViewModel
            MyTicketsViewModel tickets = new();
            List<Ticket> developerTickets = new();
            List<Ticket> submitterTickets = new();

            try
            {
                developerTickets = await _ticketService.GetAllTicketsByRoleAsync(Roles.Developer.ToString(), userId);
                tickets.Developer = developerTickets;

                submitterTickets = await _ticketService.GetAllTicketsByRoleAsync(Roles.Submitter.ToString(), userId);
                tickets.Submitter = submitterTickets;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting all tickets - {ex.Message}");
                throw;
            }

            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.Project)
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/TicketDetails/5
        public async Task<IActionResult> TicketDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.Project)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            // REMOVE
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Id");
            //ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name");

            // TODO Filter List

            // Get Current User
            BTUser btUser = await _userManager.GetUserAsync(User);

            // Get Current User Company Id
            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.ListUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,TicketPriorityId,TicketTypeId,Title,Description")] Ticket ticket)
        {
            // Created,Updated,Archived,ArchivedDate,OwnerUserId,DeveloperUserId,TicketStatusId,

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                
                ticket.Created = DateTimeOffset.Now;

                string userId = _userManager.GetUserId(User);
                ticket.OwnerUserId = userId;

                // All new tickets are 'New'
                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync("New")).Value;

                await _context.AddAsync(ticket);
                await _context.SaveChangesAsync();

                #region Add History
                // Add History
                Ticket newTicket = await _context.Ticket
                                                 .Include(t => t.TicketPriority)
                                                 .Include(t => t.TicketStatus)
                                                 .Include(t => t.TicketType)
                                                 .Include(t => t.Project)
                                                 .Include(t => t.DeveloperUser)
                                                 .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                await _historyService.AddHistoryAsync(null, newTicket, btUser.Id);
                #endregion

                #region Add Notification
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity.GetCompanyId().Value;

                Notification notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "New Ticket",
                    Message = $"New Ticket: {ticket.Title}, was created by {btUser?.FullName}",
                    Created = DateTimeOffset.Now,
                    SenderId = btUser?.Id,
                    RecipientId = projectManager?.Id
                };

                if(projectManager != null)
                {
                    // Notify the PM
                    await _notificationService.SaveNotificationAsync(notification);
                    await _notificationService.EmailNotificationAsync(notification, notification.Title);
                } else
                {
                    // Notify the Admin
                    await _notificationService.AdminsNotificationAsync(notification, companyId);
                    await _notificationService.EmailNotificationAsync(notification, notification.Title);
                }

                #endregion


                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }

            //ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", ticket.ProjectId);
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            //ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Id", ticket.TicketPriorityId);
            //ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Id", ticket.TicketStatusId);

            //return View(ticket);
            return RedirectToAction("Create");
        }

        // Assign Ticket
        [HttpGet]
        public async Task<IActionResult> AssignTicket(int? ticketId){

            if (!ticketId.HasValue)
            {
                return NotFound();
            }
            
            
            // Company Id
            int companyId = User.Identity.GetCompanyId().Value;
            
            AssignDeveloperViewModel model = new();

            model.Ticket = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);
            model.Developers = new SelectList(await _projectService.DevelopersOnProjectAsync(model.Ticket.ProjectId), "Id", "FullName");


            return View(model);
        }

        // Assign Ticket: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(AssignDeveloperViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.DeveloperId))
            {
                int companyId = User.Identity.GetCompanyId().Value;

                BTUser btUser = await _userManager.GetUserAsync(User);
                BTUser developer = (await _companyService.GetAllMembersAsync(companyId)).FirstOrDefault(m => m.Id == viewModel.DeveloperId);
                BTUser projectManager = await _projectService.GetProjectManagerAsync(viewModel.Ticket.ProjectId);

                Ticket oldTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                await _ticketService.AssignTicketAsync(viewModel.Ticket.Id, viewModel.DeveloperId);


                Ticket newTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);
            }
            return RedirectToAction("Details", new { id = viewModel.Ticket.Id });
        }



        // GET: Tickets/Edit/5
        // HIDE the Developer
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId,OwnerUserId,DeveloperUserId,Title,Description,Created,Updated,Archived,ArchivedDate")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                Notification notification;
                
                // Get Current User
                BTUser btUser = await _userManager.GetUserAsync(User);

                // Get Current User Company Id
                int companyId = User.Identity.GetCompanyId().Value;

                // project manager
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

                // Old Ticket (AsNoTracking gives a snapshot)
                Ticket oldTicket = await _context.Ticket
                                             .Include(t => t.TicketPriority)
                                             .Include(t => t.TicketStatus)
                                             .Include(t => t.TicketType)
                                             .Include(t => t.Project)
                                             .Include(t => t.DeveloperUser)
                                             .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();

                    #region Notification
                    // Create and save notification
                    notification = new()
                    {
                        TicketId = ticket.Id,
                        Title = $"Ticket modified on project - {oldTicket.Project.Name}",
                        Message = $"Ticket: {ticket.Id}: {ticket.Title} updated by {btUser.FullName}",
                        Created = DateTimeOffset.Now,
                        SenderId = btUser.Id,
                        RecipientId = projectManager?.Id
                    };

                    if (projectManager != null)
                    {
                        // Notify the PM 
                        await _notificationService.SaveNotificationAsync(notification);
                        await _notificationService.EmailNotificationAsync(notification, notification.Title);
                    }
                    else
                    {
                        // Notify the Admin
                        await _notificationService.AdminsNotificationAsync(notification, companyId);
                        await _notificationService.EmailNotificationAsync(notification, notification.Title);
                    }

                    if(ticket.DeveloperUserId != null)
                    {
                        // Alert developer if the ticket is modified
                        notification = new()
                        {
                            TicketId = ticket.Id,
                            Title = "A ticket assigned to you has been modified",
                            Message = $"Ticket: {ticket.Id}: {ticket.Title} updated by {btUser.FullName}",
                            Created = DateTimeOffset.Now,
                            SenderId = btUser.Id,
                            RecipientId = ticket.DeveloperUserId
                        };

                        await _notificationService.SaveNotificationAsync(notification);
                        await _notificationService.EmailNotificationAsync(notification, notification.Title);
                    }
                    #endregion

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // New Ticket (AsNoTracking gives a snapshot)
                Ticket newTicket = await _context.Ticket
                                             .Include(t => t.TicketPriority)
                                             .Include(t => t.TicketStatus)
                                             .Include(t => t.TicketType)
                                             .Include(t => t.Project)
                                             .Include(t => t.DeveloperUser)
                                             .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);


                return RedirectToAction("AllTickets");
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Id", ticket.TicketTypeId);
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.Project)
                .Include(t => t.TicketStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
    }
}
