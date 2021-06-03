using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models.ViewModels
{
    public class MyTicketsViewModel
    {
        public List<Ticket> Developer { get; set; } = new();
        public List<Ticket> Submitter { get; set; } = new();
    }
}
