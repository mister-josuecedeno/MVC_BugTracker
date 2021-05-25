using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class Notification
    {
        // PK / FK
        public int Id { get; set; }

        [Display(Name = "Ticket")]
        public int TicketId { get; set; }

        [Display(Name = "Recipient")]
        [Required]
        public string RecipientId { get; set; }

        [Display(Name = "Sender")]
        [Required]
        public string SenderId { get; set; }

        // Data Fields
        [Display(Name = "Subject")]
        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }

        public bool Viewed { get; set; }

        // Navigation
        // 1-to-1
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser Recipient { get; set; }
        public virtual BTUser Sender { get; set; }

    }
}
