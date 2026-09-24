using System;
using System.Collections.Generic;

namespace OrderProcessingSystem
{
    // Bonus Challenge - runtime-selectable pricing strategies.
    // OrderService never sees this class or knows how any price is computed -
    // it only ever receives a Func<Order, decimal> through its parameter.
    // The caller picks a strategy via dictionary lookup, so no if/else is
    // needed anywhere to decide which calculation runs.
    public static class PricingStrategies
    {
        public static readonly Dictionary<string, Func<Order, decimal>> All =
            new Dictionary<string, Func<Order, decimal>>
            {
                ["Normal"] = order => order.Price * order.Quantity,
                ["10% Discount"] = order => (order.Price * order.Quantity) * 0.90m,
                ["20% Discount"] = order => (order.Price * order.Quantity) * 0.80m,
                ["VIP"] = order => (order.Price * order.Quantity) * 0.70m
            };
    }
}
