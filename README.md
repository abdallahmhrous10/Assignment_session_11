# Assignment 08 — Solution

## Structure

- `Section01_LibrarySystem/` — Book / BookFunctions / LibraryEngine, with `ProcessBooks`
  parameterized four different ways (user-defined delegate, `Func<>`, anonymous method, lambda).
- `Section02_OrderProcessingSystem/` — the full Order Processing System (Parts 1–7 + Bonus Challenge).

Each project has its own `.csproj`, so each runs independently:

```
cd Section01_LibrarySystem && dotnet run
cd Section02_OrderProcessingSystem && dotnet run
```

## Note on Section 01

`ProcessBooks` is overloaded once for `BookFunctionDelegate` and once for `Func<Book, string>`.
Both delegate types have the identical shape (`Book -> string`), so passing a raw method group or
lambda straight into the call is ambiguous to the compiler (it can't tell which overload you mean).
The fix used here — assign the method group / anonymous method / lambda to an explicitly-typed local
variable first (`BookFunctionDelegate getTitle = ...` vs `Func<Book, string> getAuthors = ...`) — pins
down the type before the call, so overload resolution is unambiguous. That's also why the assignment's
four cases read naturally as four *named* variables rather than four inline expressions.

## Answers

### Part 2 — What problem does `Func<>` solve compared with creating a custom delegate?

A custom delegate has to be declared once for every distinct signature you need, and that
declaration only means something inside your own codebase. `Func<>` (and `Action<>`, `Predicate<>`)
are generic delegate types already built into .NET, so instead of writing a new `delegate` type for
every shape of method, you just plug the parameter/return types into `Func<TIn, TOut>`. That removes
boilerplate and, because every .NET API (LINQ included) already speaks `Func<>`, it makes your code
interoperate with the rest of the framework without any extra glue. The trade-off is that a custom
delegate like `PriceCalculator` documents *intent* through its name, while `Func<Order, decimal>`
only tells you the shape, not the purpose.

### Part 3 — Why is `Predicate<Order>` more expressive here than `Func<Order, bool>`?

Structurally they're identical — `Predicate<T>` is literally defined as `delegate bool Predicate<T>(T obj)`,
the same shape as `Func<T, bool>`. The difference is semantic, not functional: `Predicate<T>` exists
specifically to represent a *matching/validation test* ("does this item satisfy a condition?"), and
it's the type the BCL's own collection methods (`List<T>.Find`, `FindAll`, `RemoveAll`, `Exists`,
`TrueForAll`) already expect. Using `Predicate<Order>` for `ValidateOrder` tells a reader exactly what
the delegate is for at a glance; `Func<Order, bool>` is more generic and could just as easily be some
other boolean computation with no "validation rule" connotation.

### Q1 — Difference between `PriceCalculator` and `Func<Order, decimal>`?

Same signature (`Order -> decimal`), different origin. `PriceCalculator` is a delegate type *you*
declare, so its name documents exactly what it's for and it only exists in this codebase. `Func<Order,
decimal>` is a generic delegate already provided by .NET — no declaration needed, and it's instantly
recognizable to any C# developer or any API that already works with `Func<>` (LINQ, etc.).

### Q2 — Difference between `Action<Order>` and `Func<Order, decimal>`?

`Action<T>` wraps a method that returns nothing (`void`) — it performs a side effect (print, log,
send a message). `Func<T, TResult>` wraps a method that returns a value — it computes and hands back
a result. `Action<Order>` answers "do something with this order"; `Func<Order, decimal>` answers
"give me a number back from this order."

### Q3 — Why does `Predicate<T>` return `bool`? What problem is it designed to represent?

Because its whole purpose is a yes/no membership test: "does this item satisfy some condition?" That
boolean answer is exactly what filtering, searching, and validation logic needs in order to decide
whether to keep, reject, or flag an item — e.g. `list.FindAll(predicate)` or `list.RemoveAll(predicate)`
rely on that single true/false verdict per item.

### Q4 — Difference between a delegate and an event?

A delegate is just a type-safe reference to one or more methods — you can invoke it, reassign it
(`=`), or pass it around freely, subject only to normal access modifiers. An event is a *controlled*
wrapper around a multicast delegate field: code outside the declaring class can only subscribe
(`+=`) or unsubscribe (`-=`) — it can't invoke the event or replace its entire invocation list with
`=`. In short: an event is a delegate with encapsulation baked in, built for the publish/subscribe
pattern.

### Q5 — Why can't external code normally invoke an event declared in another class?

Because the `event` keyword compiles down to restricted `add`/`remove` accessors instead of exposing
the backing delegate field directly. The compiler only allows direct invocation (`OrderProcessed(...)`
or `.Invoke(...)`) and plain assignment (`=`) from inside the declaring class. This enforces the core
publisher/subscriber rule: only the class that owns the state should decide when a notification
fires; everyone else may only opt in or out.

### Q6 — What happens when multiple handlers subscribe to the same event?

The event's backing field becomes a multicast delegate — an ordered invocation list. When the event
is raised, every subscribed handler runs in the order it was added, synchronously, one after another,
all with the same arguments. (If one handler throws, the ones after it in the list won't run unless
you iterate the invocation list yourself and catch exceptions per-handler.)

### Q7 — Explain `orderService.OrderProcessed += HandleOrderProcessed;`

- **`OrderProcessed`** — the event field on `orderService` (a multicast delegate under the hood).
- **`+=`** — the subscribe operator: it doesn't replace the delegate, it *adds* the right-hand method
  to the existing invocation list (`Delegate.Combine`).
- **`HandleOrderProcessed`** — the method being registered, whose signature must match the event's
  delegate type (`Action<Order>`).

Put together: "register `HandleOrderProcessed` as one more method that runs whenever `orderService`
raises `OrderProcessed`."

### Q8 (Challenge) — `Action<Order>` field vs `event Action<Order>`?

If `OrderProcessed` were a plain public field (`public Action<Order> OrderProcessed;`), any outside
code could not only subscribe with `+=` but also **invoke it directly**
(`orderService.OrderProcessed(order)`) or **wipe out every other subscriber** with a careless
`orderService.OrderProcessed = myHandler;` — that silently detaches everyone else who had subscribed.
Declaring it as `event Action<Order> OrderProcessed;` locks external code down to `+=`/`-=` only;
invocation and reassignment are only legal from inside `OrderService`. That's the whole reason to use
`event` instead of exposing the delegate directly: it protects the publisher/subscriber contract so
one subscriber can never break another's subscription or trigger the notification itself.
