
using GrueneisR.RestClientGenerator;

using Microsoft.OpenApi;

string corsKey = "_myCorsKey";
string swaggerVersion = "v1";
string swaggerTitle = "NorthwindMinApiMitWPF";
string restClientFolder = Environment.CurrentDirectory;
string restClientFilename = "_requests.http";

var builder = WebApplication.CreateBuilder(args);

#region -------------------------------------------- ConfigureServices

builder.Services
  .AddEndpointsApiExplorer()
  .AddAuthorization()
  .AddSwaggerGen(x => x.SwaggerDoc(
    swaggerVersion,
    new OpenApiInfo { Title = swaggerTitle, Version = swaggerVersion }
  ))
  .AddCors(options => options.AddPolicy(
    corsKey,
    x => x.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
  ))
  .AddRestClientGenerator(options => options
    .SetFolder(restClientFolder)
    .SetFilename(restClientFilename)
    .SetAction($"swagger/{swaggerVersion}/swagger.json")
  //.EnableLogging()
  );
builder.Services.AddLogging(x => x.AddCustomFormatter());

string? connectionString = builder.Configuration.GetConnectionString("NORTHWND");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"++++ ConnectionString: {connectionString}");
Console.ResetColor();
builder.Services.AddDbContext<NORTHWNDContext>(options => options.UseSqlServer(connectionString));
#endregion

var app = builder.Build();

#region -------------------------------------------- Middleware pipeline
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  Console.ForegroundColor = ConsoleColor.Green;
  Console.WriteLine("++++ Swagger enabled: http://localhost:5000");
  app.UseSwagger();
  Console.WriteLine($@"++++ RestClient generating (after first request) to {restClientFolder}\{restClientFilename}");
  app.UseRestClientGenerator();
  app.UseSwaggerUI(x => x.SwaggerEndpoint($"/swagger/{swaggerVersion}/swagger.json", swaggerTitle));
  Console.ResetColor();
}

app.UseCors(corsKey);
//app.UseHttpsRedirection();
app.UseAuthorization();
#endregion

app.Map("/", () => Results.Redirect("/swagger"));
app.MapDbTests();

app.MapGet("/employees", (NORTHWNDContext _db) =>
{
  return _db.Employees
    //.Select(x => x.TransformTo<EmployeeDto>());
    .Select(x => new EmployeeDto(
      x.EmployeeId, $"{x.LastName} {x.FirstName}"))
    .ToList();
});

app.MapGet("/customers", (NORTHWNDContext _db) =>
{
  return _db.Customers
    .Select(x => x.TransformTo<CustomerDto>());
});

app.MapGet("/products", (NORTHWNDContext _db) =>
{
  return _db.Products
    .Select(x => x.TransformTo<ProductDto>());
});

app.MapGet("/orders", (int EmployeeId, String CustomerId, NORTHWNDContext _db) =>
{
  return _db.Orders
    .Where(x => x.EmployeeId == EmployeeId && x.CustomerId == CustomerId)
    .Select(x => new OrderDto(
      x.OrderId,
      x.OrderDate.ToString(),
      x.ShippedDate != null,
      x.OrderDetails.Count))
    .ToList();
});

app.MapGet("/orderdetails", (int OrderId, NORTHWNDContext _db) =>
{
  return _db.OrderDetails
    .Where(x => x.OrderId == OrderId)
    .Select(x => new OrderDetailDto(
      x.OrderId,
      x.Product.ProductName,
      x.Product.Category.CategoryName,
      x.Quantity,
      (int)x.UnitPrice))
    .ToList();
});

app.MapPost("/orders", (OrderPostDto dto, NORTHWNDContext _db) =>
{

  var newOrder = new Order
  {
    EmployeeId = dto.EmployeeId,
    CustomerId = dto.CustomerId,
  };

  _db.Orders.Add(newOrder);
  _db.SaveChanges();

  return Results.Created($"/orders/{newOrder.OrderId}", newOrder.TransformTo<OrderDto>());
});

app.MapPost("/orderdetails", (OrderdetailPostDto dto, NORTHWNDContext _db) =>
{
  var product = _db.Products
    .FirstOrDefault(x => x.ProductId == dto.ProductId);

  if(product == null) { return Results.NotFound("Produkt nicht gefunden."); }

  var orderdetail = new OrderDetail
  {
    OrderId = dto.OrderId,
    ProductId = dto.ProductId,
    Quantity = (short)dto.Amount,
    UnitPrice = (decimal)product.UnitPrice,
  };

  _db.OrderDetails.Add(orderdetail);
  _db.SaveChanges();

  return Results.Ok();
});

app.MapDelete("/orders/{id}", (int id, NORTHWNDContext _db) =>
{
  var order = _db.Orders
    .Include(x => x.OrderDetails)
    .FirstOrDefault(x => x.OrderId == id);

  if (order == null) { return Results.NotFound($"Bestellung {id} wurde nicht gefunden."); }

  _db.OrderDetails.RemoveRange(order.OrderDetails);
  _db.Orders.Remove(order);
  _db.SaveChanges();

  return Results.NoContent();
});

Console.WriteLine($"Ready for clients at {DateTime.Now:HH:mm:ss} ...");
app.Run();
