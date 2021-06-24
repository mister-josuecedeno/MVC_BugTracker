using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.ActionFilters
{
    public class CustomTicketAccess : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;
        private readonly IBTTicketService _ticketService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<BTUser> _roleManager;

        public CustomTicketAccess(ApplicationDbContext context,
                                  IBTRolesService roleService,
                                  IBTTicketService ticketService,
                                  UserManager<BTUser> userManager,
                                  IHttpContextAccessor httpContext, 
                                  RoleManager<BTUser> roleManager)
        {
            _context = context;
            _roleService = roleService;
            _ticketService = ticketService;
            _userManager = userManager;
            _httpContext = httpContext;
            _roleManager = roleManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ticketId = Convert.ToInt32(filterContext.ActionArguments.SingleOrDefault(param => param.Value is "id"));
            var ticket = _context.Ticket.Find(ticketId);
            string userId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var user = _context.Users.Find(userId);
            var myRole = "";

            switch (myRole) 
            {
                case "Developer":

                    break;
            }
        }
    }
}
