using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalAPI.Entities
{
    public class Role
    {
        [Key, Column("RoleID")]
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        
        //Role Tablosunda bir Foreign Key atamasına gerek yok.
    }
}
