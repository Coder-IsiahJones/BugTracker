using BugTracker.Data;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public class CompanyInfoService : ICompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllMembersAsync(int companyId)
        {
            List<User> result = new();

            result = await _context.Users.Where(x => x.CompanyId == companyId).ToListAsync();

            return result;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> result = new();

            result = await _context.Projects.Where(x => x.CompanyId == companyId)
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

            return result;
        }

        public async Task<List<Ticket>> GetAllTicketAsync(int companyId)
        {
            List<Ticket> result = new();
            List<Project> projects = new();

            projects = await GetAllProjectsAsync(companyId);

            result = projects.SelectMany(x => x.Tickets).ToList();

            return result;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();

            if (companyId != null)
            {
                result = await _context.Companies.Include(x => x.Members)
                                                 .Include(x => x.Projects)
                                                 .Include(x => x.Invites)
                                                 .FirstOrDefaultAsync(x => x.Id == companyId);
            }

            return result;
        }
    }
}