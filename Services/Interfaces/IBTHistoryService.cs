using MVC_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Services.Interfaces
{
    public interface IBTHistoryService
    {
        Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
        Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int projectId);
        Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int companyId);
    }
}
