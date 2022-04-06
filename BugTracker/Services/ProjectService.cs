using BugTracker.Data;
using BugTracker.Enums;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public class ProjectService : IProjectService
    {
        #region Properties

        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;

        #endregion Properties

        #region Constructor

        public ProjectService(ApplicationDbContext context, IRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        #endregion Constructor

        #region Add New Project

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        #endregion Add New Project

        #region Add Project Manager

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

        #endregion Add Project Manager

        #region Add User To Project

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

        #endregion Add User To Project

        #region Archive Project

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;

                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;

                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Archive Project

        #region Get All Project Members Except Project Manager

        public async Task<List<User>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<User> developers = await GetProjectMembersByRoleAsync(projectId, RolesEnum.Developer.ToString());
            List<User> submitters = await GetProjectMembersByRoleAsync(projectId, RolesEnum.Submitter.ToString());
            List<User> admins = await GetProjectMembersByRoleAsync(projectId, RolesEnum.Admin.ToString());

            List<User> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        #endregion Get All Project Members Except Project Manager

        #region Get All Projects By Company

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
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
                                                .ThenInclude(x => x.TicketStatus)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketType)
                                            .Include(x => x.ProjectPriority)
                                            .ToListAsync();

            return projects;
        }

        #endregion Get All Projects By Company

        #region Get All Projects By Priority

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(x => x.ProjectPriorityId == priorityId).ToList();
        }

        #endregion Get All Projects By Priority

        #region Get Archived Projects By Company

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await _context.Projects.Where(x => x.CompanyId == companyId && x.Archived == true)
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

        #endregion Get Archived Projects By Company

        #region Get Developers On Project

        public Task<List<User>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }

        #endregion Get Developers On Project

        #region Get Project By Id

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketPriority)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketStatus)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.TicketType)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.DeveloperUser)
                                            .Include(x => x.Tickets)
                                                .ThenInclude(x => x.OwnerUser)
                                            .Include(x => x.Members)
                                            .Include(x => x.ProjectPriority)
                                            .FirstOrDefaultAsync(x => x.Id == projectId && x.CompanyId == companyId);

            return project;
        }

        #endregion Get Project By Id

        #region Get Project Manager

        public async Task<User> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId);

            if (project != null)
            {
                foreach (User member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, RolesEnum.ProjectManager.ToString()))
                    {
                        return member;
                    }
                }
            }

            return null;
        }

        #endregion Get Project Manager

        #region Get Project Members By Role

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

        #endregion Get Project Members By Role

        #region Get Submitters On Project

        public Task<List<User>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }

        #endregion Get Submitters On Project

        #region Get User Projects

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

        #endregion Get User Projects

        #region Is Assigned Project Manager

        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;

                if (projectManagerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Is Assigned Project Manager

        #region Get Users Not On Project

        public async Task<List<User>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<User> users = await _context.Users.Where(x => x.Projects.All(x => x.Id != projectId)).ToListAsync();

            return users.Where(x => x.CompanyId == companyId).ToList();
        }

        #endregion Get Users Not On Project

        #region Is User On Project

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

        #endregion Is User On Project

        #region Lookup Project Priority Id

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            return (await _context.ProjectPriorities.FirstOrDefaultAsync(x => x.Name == priorityName)).Id;
        }

        #endregion Lookup Project Priority Id

        #region Remove Project Manager

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(x => x.Members)
                                                     .FirstOrDefaultAsync(x => x.Id == projectId);

            try
            {
                foreach (User member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, RolesEnum.ProjectManager.ToString()))
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

        #endregion Remove Project Manager

        #region Remove User From Project

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

        #endregion Remove User From Project

        #region Remove Users From Project By Role

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

        #endregion Remove Users From Project By Role

        #region Restore Project

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;

                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;

                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Restore Project

        #region Update Project

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        #endregion Update Project

        #region Get Unassigned Projects

        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> result = new();
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects.Include(x => x.ProjectPriority)
                                                  .Where(x => x.CompanyId == companyId)
                                                  .ToListAsync();

                foreach (Project project in projects)
                {
                    if ((await GetProjectMembersByRoleAsync(project.Id, nameof(RolesEnum.ProjectManager))).Count == 0)
                    {
                        result.Add(project);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        #endregion Get Unassigned Projects
    }
}