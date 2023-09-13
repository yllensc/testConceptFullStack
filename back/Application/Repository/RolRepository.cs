using Domine.Data;
using Domine.Entities;
using Domine.Interfaces;

namespace Application.Repository;
    public class RolRepository: GenericRepository<Rol>, IRol
    {
        
        protected readonly TestFullStackDbContext _context;
        public RolRepository(TestFullStackDbContext context) : base(context)
        {
            _context = context;
        }

        
    }