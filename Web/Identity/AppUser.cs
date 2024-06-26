﻿using Microsoft.AspNetCore.Identity;

namespace Web.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string? Picture { get; set; }
        public SByte? TwoFactorType { get; set; }
    }
}
