using System;

namespace OrderProcessingSystem
{
    class Program
    {
        // Named handlers so Part 6 can unsubscribe Handler1 specifically
        static void Handler1(Order order)
        {
            Console.WriteLine($"  [Handler1 - Email]      Confirmation sent to {order.CustomerName}.");
        }

        static void Handler2(Order order)
        {
            Console.WriteLine($"  [Handler2 - SMS]        Notification sent for order #{order.Id}.");
        }

        static void Handler3(Order order)
        {
            Console.WriteLine($"  [Handler3 - Audit]      Order #{order.Id} logged at {DateTime.Now:HH:mm:ss}.");
        }

        static void Main(string[] args)
        {
            var order = new Order { Id = 1, CustomerName = "Ahmed Samir", Price = 250m, Quantity = 3 };
            var orderService = new OrderService();

            // ===================== Part 1 - user-defined delegate =====================
            Console.WriteLine("=== Part 1 - PriceCalculator (user-defined delegate) ===");
            PriceCalculator calc = PriceCalculators.CalculateTotal;
            Console.WriteLine($"CalculateTotal:             {orderService.CalculateOrderPrice(order, calc):C}");

            calc = PriceCalculators.CalculateTotalWithDiscount;
            Console.WriteLine($"CalculateTotalWithDiscount: {orderService.CalculateOrderPrice(order, calc):C}");

            // ===================== Part 2 - Func<> =====================
            Console.WriteLine("\n=== Part 2 - Func<Order, decimal> ===");
            Func<Order, decimal> fullPrice = o => o.Price * o.Quantity;
            Func<Order, decimal> discountedPrice = o => (o.Price * o.Quantity) - 15m; // flat discount
            Console.WriteLine($"Full price:       {orderService.CalculateOrderPrice(order, fullPrice):C}");
            Console.WriteLine($"Discounted price: {orderService.CalculateOrderPrice(order, discountedPrice):C}");

            // ===================== Part 3 - Predicate<> =====================
            Console.WriteLine("\n=== Part 3 - Predicate<Order> validation ===");
            Predicate<Order> hasPositiveQuantity = o => o.Quantity > 0;
            Predicate<Order> hasPositivePrice = o => o.Price > 0;
            Predicate<Order> hasCustomerName = o => !string.IsNullOrWhiteSpace(o.CustomerName);

            Console.WriteLine($"Quantity > 0?        {orderService.ValidateOrder(order, hasPositiveQuantity)}");
            Console.WriteLine($"Price > 0?           {orderService.ValidateOrder(order, hasPositivePrice)}");
            Console.WriteLine($"Has customer name?   {orderService.ValidateOrder(order, hasCustomerName)}");

            // ===================== Part 4 - Action<> =====================
            Console.WriteLine("\n=== Part 4 - Action<Order> ===");
            Action<Order> printOrder = o => Console.WriteLine($"  [Print] Order {o.Id} -> {o.CustomerName}, qty {o.Quantity}.");
            Action<Order> sendConfirmation = o => Console.WriteLine($"  [Confirmation] Email queued for {o.CustomerName}.");
            Action<Order> writeAudit = o => Console.WriteLine($"  [Audit] Order {o.Id} written to log.");

            orderService.ProcessOrder(order, printOrder);
            orderService.ProcessOrder(order, sendConfirmation);
            orderService.ProcessOrder(order, writeAudit);

            // ===================== Part 5 & 6 - Events, subscription, multicast =====================
            Console.WriteLine("\n=== Part 5 & 6 - Events, subscribe/unsubscribe, multicast ===");
            orderService.OrderProcessed += Handler1;
            orderService.OrderProcessed += Handler2;
            orderService.OrderProcessed += Handler3;

            Console.WriteLine("-- All three handlers subscribed --");
            orderService.ProcessOrder(order); // Handler1, Handler2, Handler3 all fire

            orderService.OrderProcessed -= Handler1;
            Console.WriteLine("\n-- Handler1 unsubscribed --");
            orderService.ProcessOrder(order); // Only Handler2 and Handler3 fire

            // ===================== Bonus Challenge =====================
            Console.WriteLine("\n=== Bonus - runtime-selectable pricing strategies ===");
            foreach (var entry in PricingStrategies.All)
            {
                decimal price = orderService.CalculateOrderPrice(order, entry.Value);
                Console.WriteLine($"{entry.Key,-14} => {price:C}");
            }

            // ===================== Part 7 - Final Application flow =====================
            Console.WriteLine("\n=== Part 7 - Final Application (full pipeline) ===");
            RunOrderPipeline(order, orderService);

            Console.WriteLine("\nDone.");
        }

        // Order -> Validate Order -> Calculate Price -> Process Order -> OrderProcessed -> Handler1/2/3
        static void RunOrderPipeline(Order order, OrderService service)
        {
            Console.WriteLine($"Order            --> #{order.Id} received from {order.CustomerName}.");

            bool valid = service.ValidateOrder(order, o => o.Quantity > 0);
            Console.WriteLine($"Validate Order   --> {(valid ? "valid" : "invalid")}");

            decimal price = service.CalculateOrderPrice(order, PricingStrategies.All["10% Discount"]);
            Console.WriteLine($"Calculate Price  --> {price:C}");

            service.ProcessOrder(order, o => Console.WriteLine($"Process Order    --> #{o.Id} handed off for fulfillment."));

            Console.WriteLine("OrderProcessed   --> raised, notifying subscribers:");
            service.ProcessOrder(order); // Handler2 and Handler3 are still subscribed at this point
        }
    }
}
