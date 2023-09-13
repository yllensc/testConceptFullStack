namespace Domine.Entities;
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set;}
        public string Password { get; set;}
        public ICollection<Rol> Roles { get; set; } = new HashSet<Rol>();
        public ICollection<UserRol> UsersRoles { get; set; } 
        public ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; set;} = new List<HistorialRefreshToken>();
    
    }