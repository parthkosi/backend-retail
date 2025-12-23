namespace RetailAPI.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
    }

    public class ChangeRoleDto
    {
        public int UserId { get; set; }
        public string NewRole { get; set; }
    }
}
