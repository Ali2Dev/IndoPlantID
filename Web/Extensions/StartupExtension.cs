using Microsoft.AspNetCore.Identity;
using System;
using Web.Identity;
using Web.Identity.CustomValidations;
using Web.Localizations;

namespace Web.Extensions
{
    public static class StartupExtension
    {
        public static void ConfigureIdentityExtension(this IServiceCollection services)
        {

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                //User
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz123456789_";

                //Password
                //options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = true; //!*
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;

            }).AddPasswordValidator<PasswordValidator>().
                AddUserValidator<UserValidator>().
                AddErrorDescriber<LocalizationIdentityErrorDescriber>().
                AddEntityFrameworkStores<IndoPlantIdentityDb>();
        }
    }
}
