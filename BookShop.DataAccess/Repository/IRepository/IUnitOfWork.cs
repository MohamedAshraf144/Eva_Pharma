namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        Task SaveAsync();
        void Dispose();
    }
}