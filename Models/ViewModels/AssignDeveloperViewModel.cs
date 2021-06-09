using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public SelectList Developers { get; set; }
        public string DeveloperId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
