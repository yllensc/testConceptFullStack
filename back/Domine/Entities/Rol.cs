namespace Domine.Entities;
    public class Rol : BaseEntity
    {
        public string RolName { get; set; }
        public ICollection<User> Users { get; set; } = new HashSet<User>();

        public ICollection<UserRol> UsersRoles { get; set; } 

    }
