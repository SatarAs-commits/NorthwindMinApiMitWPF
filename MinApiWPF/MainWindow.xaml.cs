using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RestSharp;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;

namespace MinApiWPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private const string BaseUrl = "http://localhost:5000";
  private readonly MinApiBackendApi _api = new(BaseUrl);

  //var options = new RestClientOptions("http://localhost:5000");
  //var _client = new RestClient(options);

  //private readonly RestClient _client = new RestClient(new RestClientOptions("http://localhost:5000"));

  public MainWindow()
  {
    InitializeComponent();



  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
    try
    {
      FillCbosWithData();
      cboCustomers.SelectedIndex = 0;
      cboEmployees.SelectedIndex = 3;
      cboProducts.SelectedIndex = 0;
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Fehler beim Starten: {ex.Message} ");
    }

  }

  private void FillCbosWithData()
  {
    cboCustomers.ItemsSource = _api.CustomersGet();

    //var employees = _client.Get<List<EmployeeDto>>("employees");
    cboEmployees.ItemsSource = _api.EmployeesGet();

    cboProducts.ItemsSource = _api.ProductsGet();

  }

  private void CboSelectionChanged(object sender, SelectionChangedEventArgs e) => LoadOrders();

  private void LoadOrders()
  {
    if (cboEmployees.SelectedIndex < 0 || cboCustomers.SelectedIndex < 0) { return; }

    var selectedEmployee = (EmployeeDto)cboEmployees.SelectedItem;
    var selectedCustomer = (CustomerDto)cboCustomers.SelectedItem;

    var employeeId = selectedEmployee.EmployeeId;
    var customerId = selectedCustomer.CustomerId;

    var ordersList = _api.OrdersGet(employeeId, customerId);

    grdOrders.ItemsSource = ordersList;

    if (ordersList != null) { grdOrders.SelectedIndex = 0; }
  }

  private void GrdOrders_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadOrderDetails();
  private void LoadOrderDetails()
  {
    if (grdOrders.SelectedIndex < 0) { return; }
    var selectedOrder = (OrderDto)grdOrders.SelectedItem;

    var orderDetailsList = _api.OrderdetailsGet(selectedOrder.OrderId);

    grdOrderDetails.ItemsSource = orderDetailsList;
    grdOrderDetails.SelectedIndex = 0;
  }
  private void AddOrderDetail_Clicked(object sender, RoutedEventArgs e)
  {
    try
    {
      if (grdOrders.SelectedIndex < 0
        || cboProducts.SelectedIndex < 0
        || !int.TryParse(txtQuantity.Text.Trim(), out int quantity)) { return; }

      var selectedOrder = (OrderDto)grdOrders.SelectedItem;
      int currentOrderId = selectedOrder.OrderId;

      var selectedProduct = (ProductDto)cboProducts.SelectedItem;

      var orderDetailsDto = new OrderdetailPostDto()
      {
        OrderId = selectedOrder.OrderId,
        ProductId = selectedProduct.ProductId,
        Amount = quantity
      };
        
      
      _api.OrderdetailsPost(orderDetailsDto);
      LoadOrderDetails();
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message);
    }
  }

  private void BtnNewOrder_Clicked(object sender, RoutedEventArgs e)
  {
    if(cboCustomers.SelectedIndex < 0 || cboEmployees.SelectedIndex < 0) { return; }

    try
    {
      var selectedEmployee = (EmployeeDto)cboEmployees.SelectedItem;
      var selectedCustomer = (CustomerDto)cboCustomers.SelectedItem;

      var newOrder = new OrderPostDto()
      {
        EmployeeId = selectedEmployee.EmployeeId,
        CustomerId = selectedCustomer.CustomerId,
      };

      _api.OrdersPost(newOrder);
      LoadOrders();
    }catch(Exception ex)
    {
      MessageBox.Show(ex.Message); 
    }
  }

  private void BtnDeleteOrder_Clicked(object sender, RoutedEventArgs e)
  {
    if(cboCustomers.SelectedIndex < 0 || cboEmployees.SelectedIndex < 0 ) { return; }

    try
    {
      var order = (OrderDto)grdOrders.SelectedItem;
      if(grdOrders.SelectedIndex > 0)
      {
        _api.OrdersIdDelete(order.OrderId);
        LoadOrders();
      }
    }catch(Exception ex)
    {
      MessageBox.Show(ex.Message); 
    }
  }
}
