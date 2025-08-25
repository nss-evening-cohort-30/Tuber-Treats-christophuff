using TuberTreats.Models;

List<TuberDriver> tuberDrivers = new List<TuberDriver>()
{
    new TuberDriver(){
        Id = 1,
        Name = "Aaron 'Jaws' Homoki",
        TuberDeliveries = new List<TuberOrder>(),
    },
    new TuberDriver(){
        Id = 2,
        Name = "Bam Margera",
        TuberDeliveries = new List<TuberOrder>(),
    },
    new TuberDriver(){
        Id = 3,
        Name = "Andy Anderson",
        TuberDeliveries = new List<TuberOrder>(),
    },
};

List<Customer> customers = new List<Customer>()
{
    new Customer(){
       Id = 1,
       Name = "Tony Hawk",
       Address = "900 Indy Lane",
       TuberOrders = new List<TuberOrder>()
    },
    new Customer(){
       Id = 2,
       Name = "Rodney Mullen",
       Address = "360 Flip Road",
       TuberOrders = new List<TuberOrder>()
    },
    new Customer(){
       Id = 3,
       Name = "Nyjah Houston",
       Address = "17 SLS Champion Drive",
       TuberOrders = new List<TuberOrder>()
    },
    new Customer(){
       Id = 4,
       Name = "Jamie Foy",
       Address = "306 K-Grind Avenue",
       TuberOrders = new List<TuberOrder>()
    },
    new Customer(){
       Id = 5,
       Name = "Yuto Horigome",
       Address = "270 Nollie Back-Lip Circle",
       TuberOrders = new List<TuberOrder>()
    },
};

List<Topping> toppings = new List<Topping>()
{
    new Topping(){
        Id = 1,
        Name = "Cheese",
    },
    new Topping(){
        Id = 2,
        Name = "Bacon",
    },
    new Topping(){
        Id = 3,
        Name = "Sour Cream",
    },
    new Topping(){
        Id = 4,
        Name = "Queso",
    },
    new Topping(){
        Id = 5,
        Name = "Roast Beef",
    },
};

List<TuberOrder> tuberOrders = new List<TuberOrder>()
{
    new TuberOrder(){
        Id = 1,
        OrderPlacedOnDate = new DateTime(2025, 8, 18),
        CustomerId = 1,
        TuberDriverId = 1,
        DeliveredOnDate = new DateTime(2025, 8, 18),
        Toppings = new List<Topping>(),
    },
    new TuberOrder(){
        Id = 2,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 5,
        TuberDriverId = 3,
        DeliveredOnDate = null,
        Toppings = new List<Topping>(),
    },
    new TuberOrder(){
        Id = 3,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = 3,
        TuberDriverId = null,
        DeliveredOnDate = null,
        Toppings = new List<Topping>(),
    },
};

List<TuberTopping> tuberToppings = new List<TuberTopping>()
{
    new TuberTopping(){
        Id = 1,
        TuberOrderId = 1,
        ToppingId = 1,
    },
    new TuberTopping(){
        Id = 2,
        TuberOrderId = 1,
        ToppingId = 2,
    },
    new TuberTopping(){
        Id = 3,
        TuberOrderId = 1,
        ToppingId = 3,
    },
    new TuberTopping(){
        Id = 4,
        TuberOrderId = 2,
        ToppingId = 1,
    },
    new TuberTopping(){
        Id = 5,
        TuberOrderId = 2,
        ToppingId = 2,
    },
    new TuberTopping(){
        Id = 6,
        TuberOrderId = 2,
        ToppingId = 3,
    },
    new TuberTopping(){
        Id = 7,
        TuberOrderId = 2,
        ToppingId = 4,
    },
    new TuberTopping(){
        Id = 8,
        TuberOrderId = 3,
        ToppingId = 5,
    },
    new TuberTopping(){
        Id = 9,
        TuberOrderId = 3,
        ToppingId = 1,
    },
    new TuberTopping(){
        Id = 10,
        TuberOrderId = 3,
        ToppingId = 2,
    },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

/*          ORDERS              */
app.MapGet("/tuberorders", () =>
{
    return tuberOrders;
});

app.MapGet("/tuberorders/{id}", (int id) =>
{
    TuberOrder tuberOrder = tuberOrders.FirstOrDefault(to => to.Id == id);
    if (tuberOrder == null) return Results.NotFound();

    Customer customer = customers.FirstOrDefault(c => c.Id == tuberOrder.CustomerId);
    TuberDriver tuberDriver = tuberDrivers.FirstOrDefault(td => td.Id == tuberOrder.TuberDriverId);
    var orderToppings = tuberToppings.Where(tt => tt.TuberOrderId == tuberOrder.Id);
    List<Topping> customToppings = orderToppings
        .Select(ot => toppings.FirstOrDefault(t => t.Id == ot.ToppingId))
        .Where(t => t != null)
        .ToList();

    tuberOrder.Customer = customer;
    tuberOrder.TuberDriver = tuberDriver;
    tuberOrder.Toppings = customToppings;

    return Results.Ok(tuberOrder);
});

app.MapPost("/tuberorders", (TuberOrder newOrder) =>
{
    newOrder.Id = tuberOrders.Count + 1;
    newOrder.OrderPlacedOnDate = DateTime.Now;
    newOrder.Toppings = new List<Topping>();
    tuberOrders.Add(newOrder);

    return Results.Created($"/tuberorders/{newOrder.Id}", newOrder);
});

app.MapPut("/tuberorders/{id}", (int id, TuberOrder updatedOrder) =>
{
    var order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();
    
    order.TuberDriverId = updatedOrder.TuberDriverId;
    return Results.NoContent();
});

app.MapPost("/tuberorders/{id}/complete", (int id) =>
{
    var order = tuberOrders.FirstOrDefault(o => o.Id == id);
    if (order == null) return Results.NotFound();

    order.DeliveredOnDate = DateTime.Now;
    return Results.Ok(order);
});

/*          TOPPINGS              */
app.MapGet("/toppings", () =>
{
    return toppings;
});

app.MapGet("/toppings/{id}", (int id) =>
{
    var topping = toppings.FirstOrDefault(t => t.Id == id);
    if (topping == null) return Results.NotFound();
    return Results.Ok(topping);
});

/*          CUSTOMERS              */
app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();
    
    // Include orders for this customer
    customer.TuberOrders = tuberOrders.Where(o => o.CustomerId == customer.Id).ToList();
    return Results.Ok(customer);
});

app.MapPost("/customers", (Customer newCustomer) =>
{
    newCustomer.Id = customers.Count + 1;
    newCustomer.TuberOrders = new List<TuberOrder>();
    customers.Add(newCustomer);
    return Results.Created($"/customers/{newCustomer.Id}", newCustomer);
});

app.MapDelete("/customers/{id}", (int id) =>
{
    var customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null) return Results.NotFound();
    customers.Remove(customer);
    return Results.NoContent();
});

/*          TUBERDRIVERS              */
app.MapGet("/tuberdrivers", () =>
{
    return tuberDrivers;
});

app.MapGet("/tuberdrivers/{id}", (int id) =>
{
    var driver = tuberDrivers.FirstOrDefault(d => d.Id == id);
    if (driver == null) return Results.NotFound();

    // Include deliveries for this driver
    driver.TuberDeliveries = tuberOrders.Where(o => o.TuberDriverId == driver.Id).ToList();
    return Results.Ok(driver);
});

/*          TUBERTOPPINGS              */
app.MapGet("/tubertoppings", () =>
{
    return tuberToppings;
});

app.MapPost("/tubertoppings", (TuberTopping newTuberTopping) =>
{
    newTuberTopping.Id = tuberToppings.Count + 1;
    tuberToppings.Add(newTuberTopping);
    return Results.Created($"/tubertoppings/{newTuberTopping.Id}", newTuberTopping);
});

app.MapDelete("/tubertoppings/{id}", (int id) =>
{
    var tuberTopping = tuberToppings.FirstOrDefault(tt => tt.Id == id);
    if (tuberTopping == null) return Results.NotFound();
    tuberToppings.Remove(tuberTopping);
    return Results.NoContent();
});

app.Run();
//don't touch or move this!
public partial class Program { }