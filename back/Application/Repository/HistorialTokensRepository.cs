using Domine.Data;
using Domine.Entities;
using Domine.Interfaces;


namespace Application.Repository;
    public class HistorialTokensRepository: GenericRepository<HistorialRefreshToken>, IHistorialTokens
    {
        
        protected readonly TestFullStackDbContext _context;
        public HistorialTokensRepository(TestFullStackDbContext context) : base(context)
        {
            _context = context;
        }
    }