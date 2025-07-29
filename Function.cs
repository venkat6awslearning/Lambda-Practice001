using Amazon.Lambda.Core;
using AWSLambdaData;
using AWSLambdaData.Repository;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AWSLambdaPractice01;

public class Function
{
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public List<string> FunctionHandler(ILambdaContext context)
    {
        // Initialize Repositories
        // In a real application, these would typically be injected via Dependency Injection.
        ICustomerRepository customerRepository = new CustomerRepository();
        IOrderRepository orderRepository = new OrderRepository();
        IProductRepository productRepository = new ProductRepository();
        IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();

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
        // Create list and add multiple collections
        var fruits = new List<string> { "apple", "banana" };
        fruits.AddRange(new[] { "cherry", "grape", "apple", "pear" });

        // Filter: starts with 'a'
        var filtered = fruits.Where(f => f.StartsWith("a")).ToList();
        return filtered;
    }
}
