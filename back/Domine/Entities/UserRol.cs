namespace Domine.Entities;
    public class UserRol : BaseEntity
    {
        public int IdUserFK { get; set; }
        public User User { get; set; }
        public int IdRolFK { get; set; }
        public Rol Rol { get; set; }
}