using LostAndFoundSystem.Data;
using LostAndFoundSystem.Interfaces;
using LostAndFoundSystem.Models;

namespace LostAndFoundSystem.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRepository<Role> Roles { get; private set; }
        public IRepository<User> Users { get; private set; }
        public IRepository<Item> Items { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Roles = new Repository<Role>(context);
            Users = new Repository<User>(context);
            Items = new Repository<Item>(context);
        }

        public async Task<int> SaveAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose() =>
            _context.Dispose();
    }
}