using System;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InvitteId { get; set; }

        public bool IsValid { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; }
        public virtual User Invitee { get; set; }
        public virtual User Invitor { get; set; }
        public virtual Project Project { get; set; }
    }
}