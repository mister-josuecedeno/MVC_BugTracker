using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public BTNotificationService(ApplicationDbContext context, 
                                     IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public Task AdminsNotificationAsync(Notification notification, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task EmailNotificationAsync(Notification notification, string emailSubject)
        {
            BTUser btUser = await _context.Users.FindAsync(notification.RecipientId);

            // Send email
            string btUserEmail = btUser.Email;
            string message = notification.Message;

            try
            {
                await _emailSender.SendEmailAsync(btUserEmail, emailSubject, message);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            List<Notification> notifications = await _context.Notification
                                                             .Include(n => n.Ticket)
                                                                .ThenInclude(t => t.Project)
                                                             .Include(n => n.Recipient)
                                                             .Include(n => n.Sender)
                                                             .Where(n => n.RecipientId == userId)
                                                             .ToListAsync();
            
            return notifications;
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            List<Notification> notifications = await _context.Notification
                                                             .Include(n => n.Ticket)
                                                                .ThenInclude(t => t.Project)
                                                             .Include(n => n.Recipient)
                                                             .Include(n => n.Sender)
                                                             .Where(n => n.SenderId == userId)
                                                             .ToListAsync();

            return notifications;
        }

        public async Task MembersNotificationAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach(BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;

                    // await SaveNotificationAsync(notification);

                    // Send Email (but could also send out through other methods - e.g., SMS)
                    await EmailNotificationAsync(notification, notification.Title);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public Task SMSNotificationAsync(string phone, Notification notification)
        {
            throw new NotImplementedException();
        }
    }
}
