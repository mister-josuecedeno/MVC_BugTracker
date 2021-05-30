using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Models.Enums;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;

        public BTProjectService(ApplicationDbContext context,
                                IBTRolesService roleService)
        {
            _context = context;
            _roleService = roleService;
        }


        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            // Do we have a currentPM
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            // If yes, remove the PM/s
            if (currentPM != null) 
            {
                try 
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"*** ERROR *** - Error removing PM - {ex.Message}");
                    return false;
                }
            }

            // ADD New PM (after removing existing PM/s)
            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"*** ERROR *** - Error adding PM - {ex.Message}");
                return false;
            }

        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

                    try 
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch
                    {
                        throw; // ?? why is a false not thrown here
                    }
                } 
                else 
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error adding user to project - {ex.Message}");
                return false;
            }
            
        }

        public async Task<List<BTUser>> DevelopersOnProjectAsync(int projectId)
        {
            // [Refactor opp]

            // Get the Members
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);
            
            List<BTUser> developers = new List<BTUser>();

            // Compare BTUser list to roles and only keep Developer
            foreach (BTUser user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, Roles.Developer.ToString()))
                {
                    developers.Add(user);
                }
            }

            return developers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            // [REFACTOR] Move the code from Company (X)
            
            List<Project> projects = new();

            projects = await _context.Project
                                    .Include(p => p.Members)
                                    .Include(p => p.ProjectPriority)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.OwnerUser)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.DeveloperUser)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Comments)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Attachments)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.History)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Priority)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Status)
                                    .Include(p => p.Tickets)
                                        .ThenInclude(t => t.Type)
                                    .Where(p => p.CompanyId == companyId).ToListAsync();

            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            // Get the priority id to avoid spelling errors
            int priorityId = await LookupProjectPriorityId(priorityName);
            
            // Get all projects for this company
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            // Filter only for the provided priority
            List<Project> priority = projects.Where(p => p.ProjectPriorityId == priorityId).ToList();

            return priority;
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            // Get all projects by this company
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            // Filter only where archived = true
            List<Project> archived = projects.Where(p => p.Archived == true).ToList();

            return archived;
        }

        public async Task<List<BTUser>> GetMembersWithoutPMAsync(int projectId)
        {
            // Get Project Details
            Project project = await _context.Project
                                            .Include(p => p.Company)
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            // Get the company id (should only be 1)
            var companyId = project.CompanyId.Value;

            // Who does not have the role PM in the company (?? Is it bad practice to use role service??)
            string pm = Roles.ProjectManager.ToString();
            List<BTUser> members = await _roleService.UsersNotInRoleAsync(pm, companyId);

            // Who of these members is on the project
            List<BTUser> notPM = new();
            List<BTUser> projMembers = project.Members.ToList();

            foreach (BTUser member in projMembers) 
            {
                if (members.Contains(member)) 
                {
                    notPM.Add(member);
                }
            }

            return notPM;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            // [REFACTOR OPP]
            
            // Get Project Details
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = project.Members.ToList();

            BTUser pm = new();

            foreach(BTUser member in members) 
            { 
                if(await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString())) 
                {
                    return member;    // Returns one PM (even if there are more)
                }
            }

            return pm;
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = new();

            foreach (BTUser user in project.Members) 
            {
                if (await _roleService.IsUserInRoleAsync(user, role)) 
                {
                    members.Add(user);
                }
            }

            return members;
        }

        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            Project project = await _context.Project
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = project.Members.Any(u => u.Id == userId);

            return result;
        }

        public async Task<List<Project>> ListUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Priority)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Status)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.Type)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** Error retrieving list of user projects - {ex.Message}");
                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            // This method is void and will not return an object/value
            
            // get Project including the members
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            // Remove Existing PMs
            try
            {
                // loop members of the project
                foreach (BTUser member in project.Members)
                {
                    // Remove if role == PM
                    if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        // Use remove method
                        await RemoveUserFromProjectAsync(member.Id, project.Id);
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

                if (await IsUserOnProject(userId, projectId))
                {
                    try
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error removing user from project - {ex.Message}");
                throw;
            }

        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            // no return - void
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Project.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser bTUser in members)
                {
                    try
                    {
                        project.Members.Remove(bTUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error removing users from project by role - {ex.Message}");
                throw;
            }
        }

        public async Task<List<BTUser>> SubmittersOnProjectAsync(int projectId)
        {
            // [REFACTOR to a single method]
            
            // Get the Members
            Project project = await _context.Project
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> submitters = new List<BTUser>();

            // Compare BTUser list to roles and only keep Submitter
            foreach (BTUser user in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(user, Roles.Submitter.ToString()))
                {
                    submitters.Add(user);
                }
            }

            return submitters;
        }

        public async Task<List<BTUser>> UsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users
                                               .Where(u => u.CompanyId == companyId && u.Projects.All(p => p.Id != projectId))
                                               .ToListAsync();

            return users;
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriority.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }

    }
}
