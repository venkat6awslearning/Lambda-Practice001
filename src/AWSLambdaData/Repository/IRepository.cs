using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambdaData.Repository
{
    // ----------------------------------------------------------------------
    // 3. Interfaces (Repository Contracts)
    //    Define the contracts for data access operations.
    // ----------------------------------------------------------------------
    public interface IRepository<TEntity> where TEntity : class
    {
        // Retrieves an entity by its ID.
        TEntity GetById(int id);
        // Retrieves all entities of a given type.
        IEnumerable<TEntity> GetAll();
        // Finds entities based on a specified predicate (filter condition).
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        // Adds a single entity.
        void Add(TEntity entity);
        // Adds a range of entities.
        void AddRange(IEnumerable<TEntity> entities);
        // Removes a single entity.
        void Remove(TEntity entity);
        // Removes a range of entities.
        void RemoveRange(IEnumerable<TEntity> entities);
    }

    public interface ICustomerRepository : IRepository<Customer>
    {
        // Specific methods for customers, if any, beyond generic CRUD
        // Example: IEnumerable<Customer> GetCustomersByCity(string city);
    }

    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetOrdersByCustomerId(int customerId);
    }

    public interface IProductRepository : IRepository<Product>
    {
        // No specific methods beyond generic for now
    }

    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId);
    }

}
