using System;

namespace TestTwinCoreProject.Models
{
    public class InviteUser
    {
        public Guid Id { get; set; }
        public string InviteCode { get; set; }
        public Guid UserId { get; set; }
    }
}
