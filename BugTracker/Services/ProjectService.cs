using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly RolesService _rolesService;

        public ProjectService(ApplicationDbContext context, RolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            User currentProjectManager = await GetProjectManagerAsync(projectId);

            if (currentProjectManager != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();

                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;

            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<User> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<User> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<User> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<User> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new();

            projects = await _context.Projects.Where(x => x.CompanyId == companyId && x.Archived == false)
                                            .Include(x => x.Members)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.Comments)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.Attachments)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.History)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.Notifications)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.DeveloperUser)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.OwnerUser)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketPriority)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketType)
                                            .Include(x => x.ProjectPriority)
                                            .ToListAsync();

            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(x => x.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(x => x.Archived == true).ToList();
        }

        public Task<List<User>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects.Include(x => x.Tickets)
                                                     .Include(x => x.Members)
                                                     .Include(x => x.ProjectPriority)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId && x.CompanyId == companyId);

            return project;
        }

        public async Task<User> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId);

            foreach (User member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task<List<User>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId);

            List<User> members = new();

            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;
        }

        public Task<List<User>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users.Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Company)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Members)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                        .ThenInclude(x => x.DeveloperUser)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                        .ThenInclude(x => x.OwnerUser)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                        .ThenInclude(x => x.TicketPriority)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                        .ThenInclude(x => x.TicketStatus)
                                                                  .Include(x => x.Projects)
                                                                    .ThenInclude(x => x.Tickets)
                                                                        .ThenInclude(x => x.TicketType)
                                                                  .FirstOrDefaultAsync(x => x.Id == userId)).Projects.ToList();
                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<List<User>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<User> users = await _context.Users.Where(x => x.Projects.All(x => x.Id != projectId)).ToListAsync();

            return users.Where(x => x.CompanyId == companyId).ToList();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == projectId);

            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(x => x.Id == userId);
            }

            return result;
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            return (await _context.ProjectPriorities.FirstOrDefaultAsync(x => x.Name == priorityName)).Id;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId);

            try
            {
                foreach (User member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<User> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);

                foreach (User user in members)
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
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}