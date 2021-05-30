using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
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
        

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
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
            List<Ticket> tickets = await _context.Ticket
                                                 .Where(t => t.OwnerUserid == userId)
                                                 .ToListAsync();
            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            // [REFACTOR - Ticket may need more content]

            List<Ticket> tickets = (await _context.Project
                                        .Include(p => p.Tickets)
                                            .ThenInclude(t => t.Priority)
                                        .Include(p => p.Tickets)
                                            .ThenInclude(t => t.Status)
                                        .Include(p => p.Tickets)
                                            .ThenInclude(t => t.Type)
                                        .FirstOrDefaultAsync(p => p.CompanyId == companyId)).Tickets.ToList();

            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> tickets = companyTickets.Where(t => t.Priority.Name.Equals(priorityName)).ToList();

            return tickets;

        }

        public async Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId)
        {
            List<Ticket> tickets = new();

            if (role.Equals(Roles.Developer)) 
            { 
                tickets = await _context.Ticket.Where(t => t.DeveloperUserId.Equals(userId)).ToListAsync();
            }
            else
            {
                tickets = await _context.Ticket.Where(t => t.OwnerUserid.Equals(userId)).ToListAsync();
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> tickets = companyTickets.Where(t => t.Status.Name.Equals(statusName)).ToList();

            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> tickets = companyTickets.Where(t => t.Type.Name.Equals(typeName)).ToList();

            return tickets;
        }

        public async Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
            List<Ticket> tickets = companyTickets.Where(t => t.Archived == true).ToList();

            return tickets;

        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId)
        {
            List<Ticket> tickets = await _context.Ticket.Where(t => t.ProjectId == projectId).ToListAsync();

            if (role.Equals(Roles.Developer))
            {
                tickets = tickets.Where(t => t.DeveloperUserId.Equals(userId)).ToList();
            }
            else
            {
                tickets = tickets.Where(t => t.OwnerUserid.Equals(userId)).ToList();
            }

            return tickets;
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            Ticket ticket = await _context.Ticket
                                          .Include(t => t.DeveloperUser)
                                          .FirstOrDefaultAsync(t => t.Id == ticketId);

            BTUser developer = ticket.DeveloperUser;

            return developer;
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            int priorityId = (await _context.TicketPriority.FirstOrDefaultAsync(t => t.Name.Equals(priorityName))).Id;

            return priorityId;
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            int statusId = (await _context.TicketStatus.FirstOrDefaultAsync(t => t.Name.Equals(statusName))).Id;

            return statusId;
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            int typeId = (await _context.TicketType.FirstOrDefaultAsync(t => t.Name.Equals(typeName))).Id;

            return typeId;
        }
    }
}
