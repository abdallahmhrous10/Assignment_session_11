using System;

namespace OrderProcessingSystem
{
    public class OrderService
    {
        // Part 5 - Event raised whenever an order finishes processing.
        // Declared as an event (not a plain field) so outside code can only
        // subscribe/unsubscribe (+=/-=) and can never invoke or overwrite it.
        public event Action<Order> OrderProcessed;

        // Part 1 - Calculate price using a user-defined delegate
        public decimal CalculateOrderPrice(Order order, PriceCalculator calculator)
        {
            return calculator(order);
        }

        // Part 2 - Calculate price using the built-in Func<> delegate.
        // This overload is also what the Bonus Challenge strategies plug into -
        // OrderService only knows "give me a function that turns an Order into a decimal".
        public decimal CalculateOrderPrice(Order order, Func<Order, decimal> calculator)
        {
            return calculator(order);
        }

        // Part 3 - Validate an order against any caller-supplied rule
        public bool ValidateOrder(Order order, Predicate<Order> validationRule)
        {
            return validationRule(order);
        }

        // Part 4 - Run a caller-supplied action after "processing" the order,
        // without ProcessOrder needing to know what that action actually does
        public void ProcessOrder(Order order, Action<Order> action)
        {
            action(order);
        }

        // Part 5 - Process the order end-to-end and notify every subscriber
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"[OrderService] Order #{order.Id} processed.");
            OrderProcessed?.Invoke(order);
        }
    }
}
