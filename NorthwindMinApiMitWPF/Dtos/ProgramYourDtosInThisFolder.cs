namespace NorthwindMinApiMitWPF.Dtos;
//public record EmployeeDto(int EmployeeId, string FirstName, string LastName);
public record EmployeeDto(
  int EmployeeId,
  string Name);

public record CustomerDto(
  string CustomerId,
  string ContactName);

public record ProductDto(
  string ProductName);
public record OrderDto(
  int  OrderId, 
  String OrderDate, 
  Boolean IsShipped, 
  int NrItems);
public record OrderDetailDto(
  int OrderId, 
  String Product, 
  String Category, 
  int Quantity, 
  decimal UnitPrice);

public record OrderPostDto(
  int EmployeeId, 
  String CustomerId);

public record OrderdetailPostDto(
  int OrderId,
  int ProductId,
  int Amount);
