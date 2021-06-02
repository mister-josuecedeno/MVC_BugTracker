using MVC_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        public Task AssignTicketAsync(int ticketId, string userId);

        public Task<BTUser> GetTicketDeveloperAsync(int ticketId);

        public Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);

        public Task<List<Ticket>> GetArchivedTicketsByCompanyAsync(int companyId);

        public Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);

        public Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);

        public Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);

        public Task<List<Ticket>> GetAllPMTicketsAsync(string userId);

        public Task<List<Ticket>> GetAllTicketsByRoleAsync(string role, string userId);

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId);

        public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);

        public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);

        public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);

        public Task<int?> LookupTicketPriorityIdAsync(string priorityName);

        public Task<int?> LookupTicketStatusIdAsync(string statusName);

        public Task<int?> LookupTicketTypeIdAsync(string typeName);
    }
}
