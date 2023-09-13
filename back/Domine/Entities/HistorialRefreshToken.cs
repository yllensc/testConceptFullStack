namespace Domine.Entities;
    public class HistorialRefreshToken : BaseEntity
    {

        public int? IdUser { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateExpired { get; set; }

        public bool? IsActive { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
