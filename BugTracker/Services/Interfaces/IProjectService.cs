using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface IProjectService
    {
        public Task AddNewProjectAsync(Project project);

        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        public Task<bool> AddUserToProjectAsync(string userId, int projectId);

        public Task ArchiveProjectAsync(Project project);

        public Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId);

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

        public Task<List<User>> GetAllProjectMembersExceptPMAsync(int projectId);

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

        public Task<List<User>> GetDevelopersOnProjectAsync(int projectId);

        public Task<User> GetProjectManagerAsync(int projectId);

        public Task<List<User>> GetProjectMembersByRoleAsync(int projectId, string role);

        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        public Task<List<User>> GetSubmittersOnProjectAsync(int projectId);

        public Task<List<User>> GetUsersNotOnProjectAsync(int projectId, int companyId);

        public Task<List<Project>> GetUserProjectsAsync(string userId);

        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        public Task<int> LookupProjectPriorityId(string priorityName);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        public Task RestoreProjectAsync(Project project);

        public Task UpdateProjectAsync(Project project);
    }
}