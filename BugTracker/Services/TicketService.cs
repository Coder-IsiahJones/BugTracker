using BugTracker.Data;
using BugTracker.Enums;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketPriority = BugTracker.Models.TicketPriority;
using TicketStatus = BugTracker.Models.TicketStatus;
using TicketType = BugTracker.Models.TicketType;

namespace BugTracker.Services
{
    public class TicketService : ITicketService
    {
        #region Properties

        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;
        private readonly IProjectService _projectService;

        #endregion Properties

        #region Constructor

        public TicketService(ApplicationDbContext context, IRolesService rolesService, IProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        #endregion Constructor

        #region Add New Ticket

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Add New Ticket

        #region Archive Ticket

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Archive Ticket

        #region Assign Ticket

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId);

            try
            {
                if (ticket != null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Assign Ticket

        #region Get All Tickets By Company

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects.Where(x => x.CompanyId == companyId)
                                                              .SelectMany(x => x.Tickets)
                                                                 .Include(x => x.Attachments)
                                                                 .Include(x => x.Comments)
                                                                 .Include(x => x.History)
                                                                 .Include(x => x.DeveloperUser)
                                                                 .Include(x => x.OwnerUser)
                                                                 .Include(x => x.TicketPriority)
                                                                 .Include(x => x.TicketStatus)
                                                                 .Include(x => x.TicketType)
                                                                 .Include(x => x.Project)
                                                              .ToListAsync();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get All Tickets By Company

        #region Get All Tickets By Priority

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(x => x.CompanyId == companyId)
                                                              .SelectMany(x => x.Tickets)
                                                                 .Include(x => x.Attachments)
                                                                 .Include(x => x.Comments)
                                                                 .Include(x => x.DeveloperUser)
                                                                 .Include(x => x.OwnerUser)
                                                                 .Include(x => x.TicketPriority)
                                                                 .Include(x => x.TicketStatus)
                                                                 .Include(x => x.TicketType)
                                                                 .Include(x => x.Project)
                                                             .Where(x => x.TicketPriorityId == priorityId)
                                                             .ToListAsync();
                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get All Tickets By Priority

        #region Get All Tickets By Status

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(x => x.CompanyId == companyId)
                                                              .SelectMany(x => x.Tickets)
                                                                 .Include(x => x.Attachments)
                                                                 .Include(x => x.Comments)
                                                                 .Include(x => x.DeveloperUser)
                                                                 .Include(x => x.OwnerUser)
                                                                 .Include(x => x.TicketPriority)
                                                                 .Include(x => x.TicketStatus)
                                                                 .Include(x => x.TicketType)
                                                                 .Include(x => x.Project)
                                                             .Where(x => x.TicketStatusId == statusId)
                                                             .ToListAsync();
                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get All Tickets By Status

        #region Get All Tickets By Type

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(x => x.CompanyId == companyId)
                                                              .SelectMany(x => x.Tickets)
                                                                 .Include(x => x.Attachments)
                                                                 .Include(x => x.Comments)
                                                                 .Include(x => x.DeveloperUser)
                                                                 .Include(x => x.OwnerUser)
                                                                 .Include(x => x.TicketPriority)
                                                                 .Include(x => x.TicketStatus)
                                                                 .Include(x => x.TicketType)
                                                                 .Include(x => x.Project)
                                                             .Where(x => x.TicketTypeId == typeId)
                                                             .ToListAsync();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get All Tickets By Type

        #region Get Archived Tickets

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(x => x.Archived == true).ToList();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Archived Tickets

        #region Get Project Tickets By Priority

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(x => x.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Project Tickets By Priority

        #region Get Project Tickets By Role

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(x => x.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Project Tickets By Role

        #region Get Project Tickets By Status

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(x => x.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Project Tickets By Status

        #region Get Project Tickets By Type

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(x => x.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Project Tickets By Type

        #region Get Ticket By Id

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets.Include(x => x.DeveloperUser)
                                             .Include(x => x.OwnerUser)
                                             .Include(x => x.Project)
                                             .Include(x => x.TicketPriority)
                                             .Include(x => x.TicketStatus)
                                             .Include(x => x.TicketType)
                                             .Include(x => x.Comments)
                                             .Include(x => x.Attachments)
                                             .Include(x => x.History)
                                             .FirstOrDefaultAsync(x => x.Id == ticketId);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket By Id

        #region Get Ticket Developer

        public async Task<User> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            User developer = new();

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(x => x.Id == ticketId);

                if (ticket?.DeveloperUser != null)
                {
                    developer = ticket.DeveloperUser;
                }

                return developer;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Developer

        #region Get Tickets By RoleAsync

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                if (role == RolesEnum.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }
                else if (role == RolesEnum.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(x => x.DeveloperUserId == userId).ToList();
                }
                else if (role == RolesEnum.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(x => x.OwnerUserId == userId).ToList();
                }
                else if (role == RolesEnum.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Tickets By RoleAsync

        #region Get Tickets By User Id

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            List<Ticket> tickets = new();

            try
            {
                if (await _rolesService.IsUserInRoleAsync(user, RolesEnum.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(x => x.Tickets)
                                                    .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, RolesEnum.Developer.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(x => x.Tickets)
                                                    .Where(x => x.DeveloperUserId == userId)
                                                    .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, RolesEnum.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(x => x.Tickets)
                                                    .Where(x => x.OwnerUserId == userId)
                                                    .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, RolesEnum.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(x => x.Tickets).ToList();
                }

                return tickets;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Get Tickets By User Id

        #region Lookup Ticket Priority Id

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(x => x.Name == priorityName);

                return priority?.Id;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Lookup Ticket Priority Id

        #region Lookup Ticket tStatus Id

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(x => x.Name == statusName);

                return status?.Id;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Lookup Ticket tStatus Id

        #region Lookup Ticket Type Id

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType ticketType = await _context.TicketTypes.FirstOrDefaultAsync(x => x.Name == typeName);

                return ticketType?.Id;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Lookup Ticket Type Id

        #region Add Ticket Comment

        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.AddAsync(ticketComment);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Add Ticket Comment

        #region Add Ticket Attachment
        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Ticket Attachment By Id
        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment ticketAttachment = await _context.TicketAttachments
                                                                  .Include(t => t.User)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Update Ticket

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion Update Ticket
    }
}