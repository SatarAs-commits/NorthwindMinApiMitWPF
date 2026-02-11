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

using Org.OpenAPITools.Api;
using Org.OpenAPITools.Model;

namespace MinApiWPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private const string BaseUrl = "http://localhost:5000";
  private readonly MinApiBackendApi _api = new MinApiBackendApi(BaseUrl);
  public MainWindow()
  {
    InitializeComponent();
  }

  private void Window_Loaded(object sender, RoutedEventArgs e)
  {
   FillCbosWithData();
    
  }

  private void FillCbosWithData()
  {
    var employees = _api.EmployeesGet();
    cboEmployees.ItemsSource = employees;
    cboEmployees.SelectedIndex = 0;

    var customers = _api.CustomersGet();
    cboCustomers.ItemsSource = customers;
    cboCustomers.DisplayMemberPath = Name;
    cboCustomers.SelectedIndex = 0;

  }

  private void CboSelectionChanged(object sender, SelectionChangedEventArgs e)
  {

  }

  private void GrdOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
