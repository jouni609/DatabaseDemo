using Microsoft.EntityFrameworkCore;
using WebStore.Entities;

namespace WebStore.Assignments
{

    public class LinqQueriesAssignment
    {



        private readonly WebStoreContext _dbContext;

        public LinqQueriesAssignment(WebStoreContext context)
        {
            _dbContext = context;
        }



        public async Task Task01_ListAllCustomers()
        {
            var customers = await _dbContext.Customers
                .Select(c => new { FullName = c.FirstName + " " + c.LastName, c.Email })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FullName} - {c.Email}");
            }


        }


        public async Task Task02_ListOrdersWithItemCount()
        {

            var query = await _dbContext.Orders
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    o.OrderStatus,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .AsNoTracking()
                .ToListAsync();


            Console.WriteLine(" ");
            Console.WriteLine("=== TASK 02: List Orders With Item Count ===");
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CustomerName}");
                Console.WriteLine($"{item.OrderStatus}");
                Console.WriteLine($"{item.TotalQuantity}");
                Console.WriteLine("----------------------");
            }
        }

        public async Task Task03_ListProductsByDescendingPrice()
        {
            var query = await _dbContext.Products
                .OrderByDescending(p => p.Price)
                .Select(p => new { p.ProductName, p.Price })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 03: List Products By Descending Price ===");
            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. {item.ProductName} - {item.Price:C}");
                count++;
            }
        }


        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            var query = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending")
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    o.OrderDate,
                    TotalPrice = o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount)
                })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 04: List Pending Orders With Total Price ===");
            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. {item.OrderId} - {item.TotalPrice:C}");
                count++;
            }

        }


        public async Task Task05_OrderCountPerCustomer()
        {
            var query = await _dbContext.Customers
                .Select(c => new
                {
                    CustomerName = c.FirstName + " " + c.LastName,
                    OrderCount = c.Orders.Count()
                })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 05: Order Count Per Customer ===");

            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. {item.CustomerName} - {item.OrderCount} order total.");
                count++;
            }
        }


        public async Task Task06_Top3CustomersByOrderValue()
        {

            var query = await _dbContext.Customers
                .Select(c => new
                {
                    CustomerName = c.FirstName + " " + c.LastName,
                    TotalOrderValue = c.Orders
                        .SelectMany(o => o.OrderItems)
                        .Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount)
                })
                .OrderByDescending(c => c.TotalOrderValue)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();



            Console.WriteLine(" ");
            Console.WriteLine("=== Task 06: Top 3 Customers By Order Value ===");
            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. {item.CustomerName} - {item.TotalOrderValue:C} orders total.");
                count++;
            }
        }
        /// 7. Show all orders placed in the last 30 days (relative to now).
        ///    - Display order ID, date, and customer name.
        /// </summary>
        public async Task Task07_RecentOrders()
        {
            // TODO: Filter orders to only those with OrderDate >= (DateTime.Now - 30 days).
            //       Output ID, date, and the customer's name.
            var monthBefore = DateTime.Now.AddDays(-30);
            var query = await _dbContext.Orders
                .Where(o => o.OrderDate <= monthBefore)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                })
                .AsNoTracking()
                .ToListAsync();
            
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 07: Recent Orders ===");
            foreach ( var item in query) 
            {
                Console.WriteLine($"Id: {item.OrderId}, Order date: {item.OrderDate}, {item.CustomerName}");
            }
        }

        public async Task Task08_TotalSoldPerProduct()
        {

            var query = await _dbContext.Products
                .Select(p => new
                {
                    p.ProductName,
                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 08: Total Sold Per Product ===");
            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. {item.ProductName} - {item.TotalSold}");
                count++;
            }

        }


        public async Task Task09_DiscountedOrders()
        {

            var query = await _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    DiscountedProducts = o.OrderItems
                    .Where(oi => oi.Discount > 0)
                    .Select(oi => new
                    {
                        oi.Product.ProductName,
                        DiscountPercentage = oi.Discount,
                        DiscountedPrice = oi.UnitPrice * (1 - (oi.Discount / 100m)),
                    })
                })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 09: Discounted Orders ===");
            int count = 1;
            foreach (var item in query)
            {
                Console.WriteLine($"{count}. OrderId: {item.OrderId} Customer name: {item.CustomerName}");
                foreach (var product in item.DiscountedProducts)
                {
                    Console.WriteLine($" {product.ProductName} ({product.DiscountPercentage})% discount");
                    Console.WriteLine($"Final Price: {product.DiscountedPrice:C}");
                }
            }
        }


        public async Task Task10_AdvancedQueryExample()
        {
            var query = await _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Product.Categories.Any(c => c.CategoryName == "Electronics")))
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    OrderDate = o.OrderDate,
                    ElectronicProducts = o.OrderItems
                        .Where(oi => oi.Product.Categories.Any(c => c.CategoryName == "Electronics"))
                        .Select(oi => new
                        {
                            ProductName = oi.Product.ProductName,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            Categories = oi.Product.Categories.Select(c => c.CategoryName),
                            BestStock = oi.Product.Stocks
                                .OrderByDescending(s => s.QuantityInStock)
                                .Select(s => new
                                {
                                    StoreName = s.Store.StoreName,
                                    Quantity = s.QuantityInStock
                                })
                                .FirstOrDefault()
                        })
                })
                .AsNoTracking()
                .ToListAsync();

            Console.WriteLine(" ");
            Console.WriteLine("=== Task 10: Advanced Query Example ===");
            Console.WriteLine("Orders containing electronics products and stores with highest stock:");

            int orderCount = 1;
            foreach (var order in query)
            {
                Console.WriteLine($"{orderCount}. Order ID: {order.OrderId}");
                Console.WriteLine($"   Customer: {order.CustomerName}");
                Console.WriteLine($"   Date: {order.OrderDate:d}");

                int productCount = 1;
                foreach (var product in order.ElectronicProducts)
                {
                    Console.WriteLine($"   {productCount}. Product: {product.ProductName}");
                    Console.WriteLine($"      Categories: {string.Join(", ", product.Categories)}");
                    Console.WriteLine($"      Quantity Ordered: {product.Quantity}");
                    Console.WriteLine($"      Unit Price: {product.UnitPrice:C}");

                    if (product.BestStock != null)
                    {
                        Console.WriteLine($"      Best Availability: {product.BestStock.StoreName}");
                        Console.WriteLine($"      Stock Quantity: {product.BestStock.Quantity}");
                    }
                    else
                    {
                        Console.WriteLine("      No stock information available");
                    }

                    productCount++;
                }
                orderCount++;
            }
        }

    }
}
