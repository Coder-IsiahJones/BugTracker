using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IRolesService _rolesService;

        public NotificationService(ApplicationDbContext context, IEmailSender emailSender, IRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Models.Notification> notifications = await _context.Notifications.Include(x => x.Recipient)
                                                                                      .Include(x => x.Sender)
                                                                                      .Include(x => x.Ticket)
                                                                                       .ThenInclude(x => x.Project)
                                                                                      .Where(x => x.RecipientId == userId)
                                                                                      .ToListAsync();

                return notifications;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Models.Notification> notifications = await _context.Notifications.Include(x => x.Recipient)
                                                                                      .Include(x => x.Sender)
                                                                                      .Include(x => x.Ticket)
                                                                                       .ThenInclude(x => x.Project)
                                                                                      .Where(x => x.SenderId == userId)
                                                                                      .ToListAsync();

                return notifications;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == notification.RecipientId);

            if (user != null)
            {
                string userEmail = user.Email;
                string message = notification.Message;

                try
                {
                    await _emailSender.SendEmailAsync(userEmail, emailSubject, message);

                    return true;
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<User> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (User user in members)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<User> members)
        {
            try
            {
                foreach (User user in members)
                {
                    notification.RecipientId = user.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}