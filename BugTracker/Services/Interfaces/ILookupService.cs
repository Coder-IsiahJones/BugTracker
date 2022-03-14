using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface ILookupService
    {
        public Task<List<TicketPriority>> GetTicketPrioritiesAsync();

        public Task<List<TicketStatus>> GetAllTicketStatusesAsync();

        public Task<List<TicketType>> GetTicketTypesAsync();

        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
    }
}
