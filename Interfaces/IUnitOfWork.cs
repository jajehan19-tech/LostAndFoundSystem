using LostAndFoundSystem.Models;

namespace LostAndFoundSystem.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Role> Roles { get; }
        IRepository<User> Users { get; }
        IRepository<Item> Items { get; }
        Task<int> SaveAsync();
    }
}