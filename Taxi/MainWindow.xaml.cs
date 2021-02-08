using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaxiDriverServiceWPF.DataTypes;
using TaxiDriverServiceWPF.UnitOfWorkNS;
using System.ComponentModel;

namespace TaxiDriverServiceWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaxiDriver currentDriver;
        public MainWindow()
        {
            InitializeComponent();
            Closing += endOfWork_Close;
            AddClientsInfo();
            AddDriversInfo();
            AddOrdersInfo();

        }
        private void AddClientsInfo()
        {
            using (var cont = new DriverContext())
            {
                var modest = new TaxiClient("Модест", "+380964569852");
                var vlad = new TaxiClient("Влад", "+380935263145");
                var yura = new TaxiClient("Юра", "+380965214563");
                var solomiua = new TaxiClient("Соломія", "+380964256312");
                var bohdan = new TaxiClient("Богдан", "+380968145263");

                cont.Clients.Add(modest);
                cont.Clients.Add(vlad);
                cont.Clients.Add(yura);
                cont.Clients.Add(solomiua);
                cont.Clients.Add(bohdan);
                cont.SaveChanges();
            }
        }
        private void AddDriversInfo()
        {
            using (var cont = new DriverContext())
            {
                var roman = new TaxiDriver("Паробій", "Роман", 19, "BC1567AC", 5, 50, 29);
                var colia = new TaxiDriver("Баранов", "Микола", 19, "BC7898BM", 3, 75, 0);
                var modest = new TaxiDriver("Радомський", "Модест", 20, "BC8765", 3, 23, 0);
                var rostik = new TaxiDriver("Радиш", "Ростислав", 19, "BC3456AM1", 3, 23, 50);

                cont.Drivers.Add(roman);
                cont.Drivers.Add(colia);
                cont.Drivers.Add(modest);
                cont.Drivers.Add(rostik);

                cont.SaveChanges();
            }
        }
        private void AddOrdersInfo()
        {
            using (var cont = new DriverContext())
            {
                var modest = (from elem in cont.Clients where elem.ClientId == 1 select elem).First();
                var vlad = (from elem in cont.Clients where elem.ClientId == 2 select elem).First();
                var bohdan = (from elem in cont.Clients where elem.ClientId == 5 select elem).First();

                var roman = (from elem in cont.Drivers where elem.DriverId == 1 select elem).First();
                var colia = (from elem in cont.Drivers where elem.DriverId == 2 select elem).First();

                cont.Orders.Add(new TaxiOrder(vlad, roman, Convert.ToDateTime("2017-12-07 18:00"), "Університетська,1", "Галицька,142", 19, 15, true));
                cont.Orders.Add(new TaxiOrder(modest, roman, Convert.ToDateTime("2017-12-07 19:00"), "Наукова,178", "Пасічна,89", 0, 0, false));
                cont.Orders.Add(new TaxiOrder(bohdan, roman, Convert.ToDateTime("2017-12-07 15:00"), "Шевченка,70", "Зелена,34", 0, 0, false));
                cont.Orders.Add(new TaxiOrder(modest, colia, Convert.ToDateTime("2017-12-07 14:00"), "Стрийська,17", "Зелена,3", 0, 0, false));
                cont.Orders.Add(new TaxiOrder(bohdan, colia, Convert.ToDateTime("2017-12-07 18:00"), "Стрийська,142", "Костюшка,2", 0, 0, false));
                cont.SaveChanges();
            }
        }

        public void updateOrders(TaxiOrder orderToUpdate)
        {
            using (UnitOfWork content = new UnitOfWork())
            {
                //UpdateOrderInfoINDB
                content.Orders.Update(orderToUpdate);
                content.Save();
                currentDriver.PayCheck += orderToUpdate.Cost;
                driverInfoCostDetails.Content = currentDriver.PayCheck + " грн";

                //ShowOrdersInListView //Eager Loading
                var currentOrders = content.Orders.Get(s => s.Driver.DriverId == currentDriver.DriverId, includeProperties: "Client");
                orders.Items.Clear();
                foreach (var order in currentOrders)
                {
                    orders.Items.Add(order);
                }
            }
        }

        private void orders_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                OrderWindow wind = new OrderWindow(item as TaxiOrder);
                wind.Show();
            }
        }

        private void startWork_Click(object sender, RoutedEventArgs e)
        {
            using (UnitOfWork content = new UnitOfWork())
            {
                currentDriver = content.Drivers.Get(s => s.Name == driverUserName.Text).FirstOrDefault();
                driverInfoSurnameNameDetails.Content = currentDriver.Surname + " " + currentDriver.Name;
                driverInfoAgeDetails.Content = currentDriver.Age;
                driverInfoCarDetails.Content = currentDriver.CarNumber;
                driverInfoExpDetails.Content = currentDriver.Experience;
                driverInfoCostDetails.Content = currentDriver.PayCheck + " грн";
                driverInfoCostPerMinDetails.Content = currentDriver.CostPerMinute;

                var currentOrders = content.Orders.Get(s => s.Driver.DriverId == currentDriver.DriverId, includeProperties: "Client");

                orders.Items.Clear();
                foreach (var order in currentOrders)
                {
                    orders.Items.Add(order);
                }
            }
        }

        private void updateDriverInfo()
        {
            using (UnitOfWork content = new UnitOfWork())
            {
                if (currentDriver != null)
                {
                    content.Drivers.Update(currentDriver);
                    content.Save();
                }
            }
        }

        private void endOfWork_Click(object sender, RoutedEventArgs e)
        {
            updateDriverInfo();
            Close();
        }

        private void endOfWork_Close(object sender, CancelEventArgs e)
        {
            updateDriverInfo();
            MessageBox.Show(String.Format("Дякуюємо за роботу {0}!", currentDriver.Name), "Допобачення");
        }
    }
}
