namespace CarRentalAPI.DTO
{
    public class RoleDTO
    {
        public int Role_ID {  get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public List<UserDTO>? Users { get; set; }

    }
}
