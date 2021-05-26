using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVC_BugTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MVC_BugTracker.Models.Company> Company { get; set; }
        public DbSet<MVC_BugTracker.Models.Invite> Invite { get; set; }
        public DbSet<MVC_BugTracker.Models.Notification> Notification { get; set; }
        public DbSet<MVC_BugTracker.Models.Project> Project { get; set; }
        public DbSet<MVC_BugTracker.Models.ProjectPriority> ProjectPriority { get; set; }
        public DbSet<MVC_BugTracker.Models.Ticket> Ticket { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketAttachment> TicketAttachment { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketComment> TicketComment { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketHistory> TicketHistory { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketPriority> TicketPriority { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketStatus> TicketStatus { get; set; }
        public DbSet<MVC_BugTracker.Models.TicketType> TicketType { get; set; }
    }
}
