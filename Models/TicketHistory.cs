using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [Display(Name = "Ticket")]
        public int TicketId { get; set; }

        [Display(Name = "Team Member")]
        public string UserId { get; set; }


        [Display(Name = "Updated Item")]
        public string Property { get; set; }

        [Display(Name = "Previous")]
        public string OldValue { get; set; }

        [Display(Name = "Current")]
        public string NewValue { get; set; }
        
        [Display(Name = "Description of Change")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }


        // Navigation
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
