﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
using MVC_BugTracker.Models.ViewModels;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Controllers
{
    [Authorize(Roles="Admin")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;

        public UserRolesController(ApplicationDbContext context, 
                                   UserManager<BTUser> userManager, 
                                   IBTRolesService rolesService)
        {
            _context = context;
            _userManager = userManager;
            _rolesService = rolesService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            // TODO: Company Users
            List<BTUser> users = _context.Users.ToList();

            foreach (var user in users)
            {
                ManageUserRolesViewModel vm = new();
                vm.BTUser = user;
                var selected = await _rolesService.ListUserRolesAsync(user);
                vm.Roles = new MultiSelectList(_context.Roles, "Name", "Name", selected);
                model.Add(vm);
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            try
            {
                BTUser user = _context.Users.Find(member.BTUser.Id);

                IEnumerable<string> roles = await _rolesService.ListUserRolesAsync(user);
            
                // Homework
                // await _userManager.RemoveFromRolesAsync(user, roles); // Homework - Add to service
                await _rolesService.RemoveUserFromRolesAsync(user, roles);
            
                string userRole = member.SelectedRoles.FirstOrDefault();

                if (Enum.TryParse(userRole, out Roles roleValue ))
                {
                    await _rolesService.AddUserToRoleAsync(user, userRole);
                    return RedirectToAction("ManageUserRoles");
                }
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                throw;
            }

            return RedirectToAction("ManageUserRoles");
        }
    }
}
