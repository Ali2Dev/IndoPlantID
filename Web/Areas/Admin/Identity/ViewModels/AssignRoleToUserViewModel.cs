namespace Web.Areas.Admin.Identity.ViewModels
{
    public class AssignRoleToUserViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsExist { get; set; }

    }
}
