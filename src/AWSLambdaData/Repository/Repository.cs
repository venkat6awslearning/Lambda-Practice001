using System.Linq.Expressions;

namespace AWSLambdaData.Repository
{
    // ----------------------------------------------------------------------
    // 4. Repositories (Implementations of the Contracts)
    //    Handle the actual data access logic.
    // ----------------------------------------------------------------------

    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected List<TEntity> _entities;
        private Func<TEntity, int> _idSelector; // A function to get the ID from an entity

        // Constructor requires the in-memory list and a way to get the ID
        public BaseRepository(List<TEntity> entities, Func<TEntity, int> idSelector)
        {
            _entities = entities;
            _idSelector = idSelector;
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            // Use AsQueryable() to ensure the predicate can be applied correctly
            // and then convert back to List for in-memory operations.
            return _entities.AsQueryable().Where(predicate).ToList();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _entities.ToList(); // Return a copy to prevent external modification of the internal list
        }

        public TEntity GetById(int id)
        {
            // Use the provided idSelector to find the entity by its ID
            return _entities.FirstOrDefault(e => _idSelector(e) == id);
        }

        public void Remove(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities.ToList()) // .ToList() to avoid modifying collection during iteration
            {
                _entities.Remove(entity);
            }
        }
    }

    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository() : base(Datasource.InMemoryDataSource.Customers, c => c.CustomerId) { }
    }

    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository() : base(Datasource.InMemoryDataSource.Orders, o => o.OrderId) { }

        public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
        {
            return _entities.Where(o => o.CustomerId == customerId).ToList();
        }
    }

    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository() : base(Datasource.InMemoryDataSource.Products, p => p.ProductId) { }
    }

    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository() : base(Datasource.InMemoryDataSource.OrderDetails, od => od.OrderDetailId) { }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            return _entities.Where(od => od.OrderId == orderId).ToList();
        }
    }

}
