using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MVC_BugTracker.Services
{
    public class BTHistoryService : IBTHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            // New Ticket
            if(oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created"
                };
                await _context.TicketHistory.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Existing Tickets

                // Check Ticket Title
                if(oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Title: {newTicket.Title}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Check Ticket Description
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Description: {newTicket.Description}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Check Ticket Type
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Ticket Type",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Type: {newTicket.TicketType.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Check Ticket Priority
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Ticket Priority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Priority: {newTicket.TicketPriority.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Check Ticket Status
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Ticket Status",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Status: {newTicket.TicketStatus.Name}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Check Ticket Developer
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Ticket Developer: {newTicket.DeveloperUser?.FullName}"
                    };
                    await _context.TicketHistory.AddAsync(history);
                }

                // Save the TicketHistory DBSet to the database
                await _context.SaveChangesAsync();
            }
            
            return;
        }

        public async Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int companyId)
        {
            List<Project> projects = (await _context.Company
                                            .Include(c => c.Projects)
                                                .ThenInclude(p => p.Tickets)
                                                    .ThenInclude(t => t.History)
                                            .FirstOrDefaultAsync(c => c.Id == companyId)).Projects.ToList();

            List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();

            List<TicketHistory> ticketHistory = tickets.SelectMany(t => t.History).ToList();

            return ticketHistory;
        }

        public async Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int projectId)
        {
            Project project = await _context.Project.Include(project => project.Tickets)
                                                    .ThenInclude(t => t.History)
                                                    .FirstOrDefaultAsync(project => project.Id == projectId);

            List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t => t.History).ToList();


            return ticketHistory;
        }
    }
}
