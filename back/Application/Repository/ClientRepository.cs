using Domine.Data;
using Domine.Entities;
using Domine.Interfaces;

namespace Application.Repository;
    public class ClientRepository: GenericRepository<Client>, IClient
    {
        
        protected readonly TestFullStackDbContext _context;
        public ClientRepository(TestFullStackDbContext context) : base(context)
        {
            _context = context;
        }
    }