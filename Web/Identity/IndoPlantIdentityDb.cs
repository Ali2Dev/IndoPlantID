using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Identity
{
    public class IndoPlantIdentityDb : IdentityDbContext<AppUser, AppRole, string>
    {
        public IndoPlantIdentityDb(DbContextOptions<IndoPlantIdentityDb> options) : base(options)
        {

        }
    }
}
