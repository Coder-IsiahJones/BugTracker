using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class LookupService : ILookupService
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion Properties

        #region Constructor

        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Get All Ticket Statuses

        public async Task<List<TicketStatus>> GetAllTicketStatusesAsync()
        {
            try
            {
                return await _context.TicketStatuses.ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get All Ticket Statuses

        #region Get Project Priorities

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Project Priorities

        #region Get Ticket Priorities

        public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            try
            {
                return await _context.TicketPriorities.ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Priorities

        #region Get Ticket Types

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            try
            {
                return await _context.TicketTypes.ToListAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Types
    }
}