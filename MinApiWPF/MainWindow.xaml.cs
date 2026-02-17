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
      MessageBox.Show($"Fehler beim Starten: { ex.Message } ");
    }

  }

  private void FillCbosWithData()
  {
    cboCustomers.ItemsSource= _api.CustomersGet();

    //var employees = _client.Get<List<EmployeeDto>>("employees");
    cboEmployees.ItemsSource = _api.EmployeesGet();

    cboProducts.ItemsSource = _api.ProductsGet();

  }

  private void CboSelectionChanged(object sender, SelectionChangedEventArgs e) => LoadOrders();

  private void LoadOrders()
  {
    if(cboEmployees.SelectedIndex < 0 || cboCustomers.SelectedIndex < 0) { return; }

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

  }
  private void AddOrderDetail_Clicked(object sender, RoutedEventArgs e)
  {

  }

  private void BtnNewOrder_Clicked(object sender, RoutedEventArgs e)
  {

  }

  private void BtnDeleteOrder_Clicked(object sender, RoutedEventArgs e)
  {

  }
}
