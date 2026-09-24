namespace OrderProcessingSystem
{
    // Part 1 - User-defined delegate
    public delegate decimal PriceCalculator(Order order);

    public static class PriceCalculators
    {
        // Matches the PriceCalculator signature: Order -> decimal
        public static decimal CalculateTotal(Order order)
        {
            return order.Price * order.Quantity;
        }

        public static decimal CalculateTotalWithDiscount(Order order)
        {
            decimal discount = order.Price * order.Quantity * 0.10m; // flat 10% discount
            return (order.Price * order.Quantity) - discount;
        }
    }
}
