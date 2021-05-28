using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _roleService;
        private readonly IBTCompanyInfoService _infoService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context,
                               RoleManager<IdentityRole> roleManager,
                               UserManager<BTUser> userManager,
                               IBTRolesService roleService,
                               IBTCompanyInfoService infoService, 
                               IBTProjectService projectService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _roleService = roleService;
            _infoService = infoService;
            _projectService = projectService;
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            try
            {
                Ticket ticket = await _context.Ticket
                                              .FirstOrDefaultAsync(t => t.Id == ticketId);

                ticket.DeveloperUserId = userId;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error assigning ticket - {ex.Message}");
                throw;
            }

        }

        public async Task<List<Ticket>> GetAllPMTicketsAsync(string userId)
        {
            // [Refactor what additional information should I include?]
            List<Ticket> tickets = await _context.Ticket
                                                 .Where(t => t.OwnerUserid == userId)
                                                 .ToListAsync();
            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            // [Ticket may need more content]

            List<Ticket> tickets = (await _context.Project
                                        .Include(p => p.Tickets)
                                        .FirstOrDefaultAsync(p => p.CompanyId == companyId)).Tickets.ToList();

            return tickets;
        }

        public Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)

            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)
            // What about Company?

            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)

            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)

            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)

            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId)
        {
            // Call GetAllTicketsByCompanyAsync (be sure to add all ticket details)
            // What about company?

            throw new NotImplementedException();
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            Ticket ticket = await _context.Ticket
                                          .Include(t => t.DeveloperUser)
                                          .FirstOrDefaultAsync(t => t.Id == ticketId);

            BTUser developer = ticket.DeveloperUser;

            return developer;
        }

        public Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            // Priority Id by Priority Name
            
            throw new NotImplementedException();
        }

        public Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            // Status Id by Status Name

            throw new NotImplementedException();
        }

        public Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            // Type Id by Type Name

            throw new NotImplementedException();
        }
    }
}
