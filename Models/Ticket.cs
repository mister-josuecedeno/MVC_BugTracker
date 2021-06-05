using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class Ticket
    {
        // PK/FK
        public int Id { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }


        // Data Fields
        [Display(Name = "Submitter")]
        public string OwnerUserId { get; set; }

        [Display(Name = "Developer")]
        public string DeveloperUserId { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }

        [Display(Name = "Date Updated")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? Updated { get; set; }

        public bool Archived { get; set; }

        [Display(Name = "Date Archived")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? ArchivedDate { get; set; }


        // Navigation

        // 1-to-1
        public virtual Project Project { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual BTUser OwnerUser { get; set; }
        public virtual BTUser DeveloperUser { get; set; }

        // 1-to-Many (Eager Loading)
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();


    }
}
