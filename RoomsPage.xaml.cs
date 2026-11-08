using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class RoomsPage : Page
    {
        private List<Номера> AllRooms;

        public RoomsPage()
        {
            InitializeComponent();

            LoadRooms();
        }

        private void LoadRooms()
        {
            AllRooms = HotelDBEntities.GetContext().Номера.ToList();
            ApplyFilters();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (AllRooms == null)
                return;

            var rooms = AllRooms.AsEnumerable();

            string search = SearchBox.Text.Trim();

            if (search != "")
            {
                rooms = rooms.Where(r => r.номер.ToString().Contains(search));
            }

            string type = (TypeFilterComboBox.SelectedItem as ComboBoxItem) ?.Content.ToString();

            if (!string.IsNullOrEmpty(type) && type != "Все типы")
            {
                rooms = rooms.Where(r => r.тип_номера == type);
            }

            string status = (StatusFilterComboBox.SelectedItem as ComboBoxItem) ?.Content.ToString();

            if (!string.IsNullOrEmpty(status) && status != "Все статусы")
            {
                rooms = rooms.Where(r => r.статус == status);
            }

            string sort = (SortComboBox.SelectedItem as ComboBoxItem) ?.Content.ToString();

            switch (sort)
            {
                case "Сначала дешёвые":
                    rooms = rooms.OrderBy(r => r.цена_за_сутки);
                    break;

                case "Сначала дорогие":
                    rooms = rooms.OrderByDescending(r => r.цена_за_сутки);
                    break;

                case "По возрастанию вместимости":
                    rooms = rooms.OrderBy(r => r.вместимость);
                    break;

                case "По убыванию вместимости":
                    rooms = rooms.OrderByDescending(r => r.вместимость);
                    break;

                case "По номеру комнаты":
                    rooms = rooms.OrderBy(r => r.номер);
                    break;
            }

            var result = rooms.ToList();

            RoomsListView.ItemsSource = result;

            CountText.Text = "Количество номеров: " + result.Count;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = (sender as Button).DataContext as Номера;

            Manager.MainFrame.Navigate(new AddEditPage(selectedRoom));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = (sender as Button).DataContext as Номера;

            if (selectedRoom.статус != "Свободен")
            {
                MessageBox.Show("Можно удалять только свободные номера!");
                return;
            }

            bool hasBookings = HotelDBEntities.GetContext().Бронирования.Any(b => b.id_номера == selectedRoom.id_номера);

            if (hasBookings)
            {
                MessageBox.Show("Нельзя удалить номер, у которого есть бронирования!");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить этот номер?","Удаление номера",MessageBoxButton.YesNo,MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var context = HotelDBEntities.GetContext();

                context.Номера.Remove(selectedRoom);
                context.SaveChanges();

                MessageBox.Show("Номер удалён!");

                LoadRooms();
            }
        }
    }
}