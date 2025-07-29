using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambdaData.Datasource
{
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

}
