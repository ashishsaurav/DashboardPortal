namespace DashboardPortal.DTOs
{
    public class EmailLoginRequest
    {
        public string Email { get; set; }
    }

    public class LoginResponse
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
