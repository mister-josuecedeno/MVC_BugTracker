using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }
        
        [Display(Name = "Ticket Priority")]
        public string Name { get; set; }
    }
}
