using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; // Required for Expression<Func<T, bool>>

// ----------------------------------------------------------------------
// 1. Models (Entities)
//    These represent your domain objects.
// ----------------------------------------------------------------------

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

// ----------------------------------------------------------------------
// 2. Data Source (In-Memory Setup)
//    Simulates a database or external data source.
// ----------------------------------------------------------------------

public static class InMemoryDataSource
{
    public static List<Customer> Customers { get; private set; }
    public static List<Order> Orders { get; private set; }
    public static List<Product> Products { get; private set; }
    public static List<OrderDetail> OrderDetails { get; private set; }

    static InMemoryDataSource() // Static constructor to initialize data once
    {
        InitializeData();
    }

    private static void InitializeData()
    {
        Customers = new List<Customer>
        {
            new Customer { CustomerId = 1, Name = "Alice Smith", City = "New York" },
            new Customer { CustomerId = 2, Name = "Bob Johnson", City = "London" },
            new Customer { CustomerId = 3, Name = "Charlie Brown", City = "New York" },
            new Customer { CustomerId = 4, Name = "Diana Miller", City = "Paris" },
            new Customer { CustomerId = 5, Name = "Eve Davis", City = "London" }
        };

        Orders = new List<Order>
        {
            new Order { OrderId = 101, CustomerId = 1, OrderDate = new DateTime(2024, 1, 15), TotalAmount = 150.00m },
            new Order { OrderId = 102, CustomerId = 2, OrderDate = new DateTime(2024, 1, 20), TotalAmount = 200.50m },
            new Order { OrderId = 103, CustomerId = 1, OrderDate = new DateTime(2024, 2, 10), TotalAmount = 75.25m },
            new Order { OrderId = 104, CustomerId = 3, OrderDate = new DateTime(2024, 2, 15), TotalAmount = 300.00m },
            new Order { OrderId = 105, CustomerId = 4, OrderDate = new DateTime(2024, 3, 5), TotalAmount = 120.00m },
            new Order { OrderId = 106, CustomerId = 2, OrderDate = new DateTime(2024, 3, 10), TotalAmount = 50.00m }
        };

        Products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Laptop", Price = 1200.00m },
            new Product { ProductId = 2, ProductName = "Mouse", Price = 25.00m },
            new Product { ProductId = 3, ProductName = "Keyboard", Price = 75.00m },
            new Product { ProductId = 4, ProductName = "Monitor", Price = 300.00m },
            new Product { ProductId = 5, ProductName = "Webcam", Price = 50.00m }
        };

        OrderDetails = new List<OrderDetail>
        {
            new OrderDetail { OrderDetailId = 1, OrderId = 101, ProductId = 1, Quantity = 1 },
            new OrderDetail { OrderDetailId = 2, OrderId = 101, ProductId = 2, Quantity = 2 },
            new OrderDetail { OrderDetailId = 3, OrderId = 102, ProductId = 3, Quantity = 1 },
            new OrderDetail { OrderDetailId = 4, OrderId = 102, ProductId = 4, Quantity = 1 },
            new OrderDetail { OrderDetailId = 5, OrderId = 103, ProductId = 2, Quantity = 3 },
            new OrderDetail { OrderDetailId = 6, OrderId = 104, ProductId = 1, Quantity = 1 },
            new OrderDetail { OrderDetailId = 7, OrderId = 104, ProductId = 5, Quantity = 2 },
            new OrderDetail { OrderDetailId = 8, OrderId = 105, ProductId = 3, Quantity = 1 },
            new OrderDetail { OrderDetailId = 9, OrderId = 106, ProductId = 5, Quantity = 1 }
        };
    }
}

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
    public CustomerRepository() : base(InMemoryDataSource.Customers, c => c.CustomerId) { }
}

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository() : base(InMemoryDataSource.Orders, o => o.OrderId) { }

    public IEnumerable<Order> GetOrdersByCustomerId(int customerId)
    {
        return _entities.Where(o => o.CustomerId == customerId).ToList();
    }
}

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository() : base(InMemoryDataSource.Products, p => p.ProductId) { }
}

public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository() : base(InMemoryDataSource.OrderDetails, od => od.OrderDetailId) { }

    public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId)
    {
        return _entities.Where(od => od.OrderId == orderId).ToList();
    }
}

// ----------------------------------------------------------------------
// 5. Program.cs (Application Entry Point)
//    Demonstrates usage of repositories, LINQ, and lambda expressions.
// ----------------------------------------------------------------------

public class Program
{
    public static void Main(string[] args)
    {
        // Initialize Repositories
        // In a real application, these would typically be injected via Dependency Injection.
        ICustomerRepository customerRepository = new CustomerRepository();
        IOrderRepository orderRepository = new OrderRepository();
        IProductRepository productRepository = new ProductRepository();
        IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();

        Console.WriteLine("--- Initial Data (retrieved via Repositories) ---");
        Console.WriteLine("Customers:");
        customerRepository.GetAll().ToList().ForEach(c => Console.WriteLine($"  Id: {c.CustomerId}, Name: {c.Name}, City: {c.City}"));
        Console.WriteLine("\nOrders:");
        orderRepository.GetAll().ToList().ForEach(o => Console.WriteLine($"  Id: {o.OrderId}, CustomerId: {o.CustomerId}, Total: {o.TotalAmount:C}"));
        Console.WriteLine("\nProducts:");
        productRepository.GetAll().ToList().ForEach(p => Console.WriteLine($"  Id: {p.ProductId}, Name: {p.ProductName}, Price: {p.Price:C}"));
        Console.WriteLine("\nOrder Details:");
        orderDetailRepository.GetAll().ToList().ForEach(od => Console.WriteLine($"  DetailId: {od.OrderDetailId}, OrderId: {od.OrderId}, ProductId: {od.ProductId}, Quantity: {od.Quantity}"));

        Console.WriteLine("\n--- Filtering Examples (LINQ Where with Lambda via Repository.Find) ---");

        // Filter 1: Customers from New York
        // The predicate (c => c.City == "New York") is passed to the repository.
        var newYorkCustomers = customerRepository.Find(c => c.City == "New York").ToList();
        Console.WriteLine("\nCustomers from New York:");
        newYorkCustomers.ForEach(c => Console.WriteLine($"  {c.Name}"));

        // Filter 2: Orders with Total Amount greater than 100
        var largeOrders = orderRepository.Find(o => o.TotalAmount > 100m).ToList();
        Console.WriteLine("\nOrders with Total Amount > $100:");
        largeOrders.ForEach(o => Console.WriteLine($"  Order ID: {o.OrderId}, Total: {o.TotalAmount:C}"));

        // Filter 3: Products with price less than $100 and containing "o" in name
        var affordableGadgets = productRepository.Find(p => p.Price < 100m && p.ProductName.Contains("o")).ToList();
        Console.WriteLine("\nAffordable Gadgets (Price < $100 and 'o' in name):");
        affordableGadgets.ForEach(p => Console.WriteLine($"  {p.ProductName} ({p.Price:C})"));


        Console.WriteLine("\n--- LINQ Join Examples (using data retrieved from Repositories) ---");

        // Retrieve all necessary data from repositories for joins
        // In a real application, you might have service methods that orchestrate these calls.
        var allCustomers = customerRepository.GetAll();
        var allOrders = orderRepository.GetAll();
        var allProducts = productRepository.GetAll();
        var allOrderDetails = orderDetailRepository.GetAll();

        // Join 1: Get orders with customer names (Inner Join)
        var ordersWithCustomerNames = allOrders.Join(
            allCustomers,
            order => order.CustomerId,         // Key selector for the outer sequence (orders)
            customer => customer.CustomerId,   // Key selector for the inner sequence (customers)
            (order, customer) => new          // Result selector: defines the shape of the joined result
            {
                OrderId = order.OrderId,
                CustomerName = customer.Name,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            }).ToList();

        Console.WriteLine("\nOrders with Customer Names:");
        ordersWithCustomerNames.ForEach(oc => Console.WriteLine($"  Order ID: {oc.OrderId}, Customer: {oc.CustomerName}, Date: {oc.OrderDate.ToShortDateString()}, Total: {oc.TotalAmount:C}"));

        // Join 2: Get products purchased in each order (Join OrderDetails and Products)
        var productsInOrders = allOrderDetails.Join(
            allProducts,
            od => od.ProductId,
            p => p.ProductId,
            (od, p) => new
            {
                od.OrderId,
                p.ProductName,
                p.Price,
                od.Quantity,
                LineTotal = od.Quantity * p.Price
            }).ToList();

        Console.WriteLine("\nProducts in Orders:");
        productsInOrders.ForEach(pio => Console.WriteLine($"  Order ID: {pio.OrderId}, Product: {pio.ProductName}, Quantity: {pio.Quantity}, Line Total: {pio.LineTotal:C}"));

        // Join 3: Get all customers and their orders (Left Join Simulation using GroupJoin and SelectMany)
        // GroupJoin groups inner elements for each outer element. DefaultIfEmpty(null) ensures
        // outer elements without a match in the inner sequence are still included.
        var customersWithOrders = allCustomers.GroupJoin(
            allOrders,
            customer => customer.CustomerId,
            order => order.CustomerId,
            (customer, customerOrders) => new
            {
                Customer = customer,
                Orders = customerOrders.DefaultIfEmpty(null) // Include customer even if no orders
            })
            .SelectMany( // Flatten the grouped results
                customerAndOrders => customerAndOrders.Orders.Select(
                    order => new
                    {
                        customerAndOrders.Customer.Name,
                        customerAndOrders.Customer.City,
                        OrderId = order?.OrderId, // Null if no order (from DefaultIfEmpty)
                        OrderTotal = order?.TotalAmount // Null if no order
                    }
                )
            ).ToList();

        Console.WriteLine("\nCustomers with their Orders (Left Join Simulation):");
        customersWithOrders.ForEach(cwo =>
        {
            if (cwo.OrderId.HasValue)
            {
                Console.WriteLine($"  Customer: {cwo.Name} ({cwo.City}), Order ID: {cwo.OrderId}, Total: {cwo.OrderTotal:C}");
            }
            else
            {
                Console.WriteLine($"  Customer: {cwo.Name} ({cwo.City}), No Orders");
            }
        });


        Console.WriteLine("\n--- More Advanced LINQ with Grouping and Aggregation (Repository Data) ---");

        // Grouping: Total sales by customer
        var totalSalesByCustomer = allOrders.GroupBy(
            order => order.CustomerId, // Group by CustomerId
            (customerId, customerOrders) => new // Result selector for each group
            {
                CustomerId = customerId,
                TotalSales = customerOrders.Sum(o => o.TotalAmount), // Aggregate sum
                NumberOfOrders = customerOrders.Count() // Aggregate count
            })
            .Join(allCustomers, // Join back to customers to get customer name
                  sales => sales.CustomerId,
                  customer => customer.CustomerId,
                  (sales, customer) => new
                  {
                      CustomerName = customer.Name,
                      sales.TotalSales,
                      sales.NumberOfOrders
                  })
            .OrderByDescending(x => x.TotalSales) // Order by total sales
            .ToList();

        Console.WriteLine("\nTotal Sales by Customer:");
        totalSalesByCustomer.ForEach(tsc => Console.WriteLine($"  Customer: {tsc.CustomerName}, Total Sales: {tsc.TotalSales:C}, Orders: {tsc.NumberOfOrders}"));

        // Grouping: Products sold per order, with product names
        var productsSoldPerOrder = allOrderDetails.GroupBy(
            od => od.OrderId, // Group by OrderId
            (orderId, details) => new // Result selector for each group
            {
                OrderId = orderId,
                Products = details.Join(allProducts, // Join details with products to get product names
                                       d => d.ProductId,
                                       p => p.ProductId,
                                       (d, p) => new { p.ProductName, d.Quantity })
                                 .ToList()
            })
            .ToList();

        Console.WriteLine("\nProducts Sold Per Order:");
        foreach (var orderGroup in productsSoldPerOrder)
        {
            Console.WriteLine($"  Order ID: {orderGroup.OrderId}");
            orderGroup.Products.ForEach(p => Console.WriteLine($"    - {p.ProductName} (Qty: {p.Quantity})"));
        }

        Console.WriteLine("\n--- Repository CRUD Example ---");
        // Add a new customer
        var newCustomer = new Customer { CustomerId = 6, Name = "Frank Green", City = "Berlin" };
        customerRepository.Add(newCustomer);
        Console.WriteLine($"\nAdded new customer: {newCustomer.Name}");
        Console.WriteLine("Customers after add:");
        customerRepository.GetAll().ToList().ForEach(c => Console.WriteLine($"  {c.Name}"));

        // Find and remove a customer
        var customerToRemove = customerRepository.GetById(4); // Diana Miller
        if (customerToRemove != null)
        {
            customerRepository.Remove(customerToRemove);
            Console.WriteLine($"\nRemoved customer: {customerToRemove.Name}");
            Console.WriteLine("Customers after remove:");
            customerRepository.GetAll().ToList().ForEach(c => Console.WriteLine($"  {c.Name}"));
        }
        else
        {
            Console.WriteLine("\nCustomer with ID 4 not found for removal.");
        }
    }
}
