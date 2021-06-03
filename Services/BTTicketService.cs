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
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, IBTProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = new();

            try
            {
                ticket = await _context.Ticket
                                          .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting ticket - {ex.Message}");
                throw;
            }
            
            if(ticket != null)
            {
                try
                {
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                    ticket.DeveloperUserId = userId;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"*** ERROR *** - Error assigning ticket - {ex.Message}");
                    throw;
                }
            }

        }

        [Obsolete]
        public async Task<List<Ticket>> GetAllPMTicketsAsync(string userId)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Project> projects = await _projectService.ListUserProjectsAsync(userId);
                tickets = projects.SelectMany(t => t.Tickets).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting PM tickets - {ex.Message}");
                throw;
            }

            return tickets;

        }

        public async Task<List<Ticket>> GetAllUserTicketsAsync(string userId)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Project> projects = await _projectService.ListUserProjectsAsync(userId);
                tickets = projects.SelectMany(t => t.Tickets).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting PM tickets - {ex.Message}");
                throw;
            }

            return tickets;

        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            
            List<Ticket> tickets = new();

            try
            {
                tickets = await _context.Project
                                        .Include(p => p.Company)
                                        .Where(p => p.CompanyId == companyId)
                                        .SelectMany(p => p.Tickets)
                                        .Include(t => t.Attachments)
                                        .Include(t => t.Comments)
                                        .Include(t => t.History)
                                        .Include(t => t.DeveloperUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType)
                                        .Include(t => t.Project)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by company - {ex.Message}");
                throw;
            }

            return tickets;

        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
                tickets = companyTickets.Where(t => t.TicketPriority.Name.Equals(priorityName)).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by priority - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId)
        {
            List<Ticket> tickets = new();

            try
            {
                if (role.Equals(Roles.Developer.ToString())) 
                {
                    try
                    {
                        tickets = await _context.Ticket
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.OwnerUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.TicketType)
                                            .Include(t => t.Project)
                                                .ThenInclude(p => p.Members)
                                            .Include(t => t.Project)
                                                .ThenInclude(p => p.ProjectPriority)
                                            .Where(t => t.DeveloperUserId.Equals(userId))
                                            .ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"*** ERROR *** - Error getting developer tickets - {ex.Message}");
                        throw;
                    }
                    
                    
                }
                else
                {
                    try
                    {
                        tickets = await _context.Ticket
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.DeveloperUser)
                                            .Include(t => t.OwnerUser)
                                            .Include(t => t.TicketPriority)
                                            .Include(t => t.TicketStatus)
                                            .Include(t => t.TicketType)
                                            .Include(t => t.Project)
                                                .ThenInclude(p => p.Members)
                                            .Include(t => t.Project)
                                                .ThenInclude(p => p.ProjectPriority)
                                            .Where(t => t.OwnerUserId.Equals(userId))
                                            .ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"*** ERROR *** - Error getting submitter tickets - {ex.Message}");
                        throw;
                    }
                    
                }

                return tickets;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting role tickets - {ex.Message}");
                throw;
            }
            
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
                tickets = companyTickets.Where(t => t.TicketStatus.Name.Equals(statusName)).ToList();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by status - {ex.Message}");
                throw;
            }
            
            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
                tickets = companyTickets.Where(t => t.TicketType.Name.Equals(typeName)).ToList();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by type - {ex.Message}");
                throw;
            }
            
            return tickets;
        }

        public async Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
                tickets = companyTickets.Where(t => t.Archived == true).ToList();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting archived tickets by company - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetAllTicketsByProject(int projectId)
        {
            List<Ticket> tickets = new();
            
            try
            {
                // [REFACTOR] Add more details
                tickets = await _context.Ticket.Where(t => t.ProjectId == projectId).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting Project Tickets - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = await GetAllTicketsByProject(projectId);

                if (role.Equals(Roles.Developer))
                {
                    tickets = tickets.Where(t => t.DeveloperUserId.Equals(userId)).ToList();
                }
                else
                {
                    tickets = tickets.Where(t => t.OwnerUserId.Equals(userId)).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting Project tickets by role - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId) 
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName))
                                .Where(t => t.ProjectId == projectId).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by status - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId) 
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName))
                                .Where(t => t.ProjectId == projectId).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by type - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId) 
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName))
                                .Where(t => t.ProjectId == projectId).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting tickets by priority - {ex.Message}");
                throw;
            }

            return tickets;
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            BTUser developer = new();
            
            try
            {
                Ticket ticket = await _context.Ticket
                                              .Include(t => t.DeveloperUser)
                                              .FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket?.DeveloperUserId != null)
                {
                    developer = ticket.DeveloperUser;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting developer by ticket id - {ex.Message}");
                throw;
            }

            return developer;
        }


        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            int priorityId = new();

            try
            {
                priorityId = (await _context.TicketPriority.FirstOrDefaultAsync(t => t.Name.Equals(priorityName))).Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting priority id - {ex.Message}");
                throw;
            }

            return priorityId;
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            int statusId = new();

            try
            {
                statusId = (await _context.TicketStatus
                                              .FirstOrDefaultAsync(t => t.Name.Equals(statusName))).Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting status id - {ex.Message}");
                throw;
            }

            return statusId;
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            int typeId = new();

            try
            {
                typeId = (await _context.TicketType.FirstOrDefaultAsync(t => t.Name.Equals(typeName))).Id;

                

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error getting type id - {ex.Message}");
                throw;
            }

            return typeId;
        }
    }
}
