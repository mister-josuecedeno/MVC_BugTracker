using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        
        [Display(Name = "Ticket")]
        public int TicketId { get; set; }

        [Display(Name = "Team Member")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Member Comment")]
        public string Comment { get; set; }

        [Display(Name = "Create Date")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Created { get; set; }
        
        
        // Navigation properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
