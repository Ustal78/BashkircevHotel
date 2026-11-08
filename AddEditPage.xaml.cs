using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hotel
{
    public partial class AddEditPage : Page
    {
        private Номера currentRoom;

        public AddEditPage(Номера selectedRoom = null)
        {
            InitializeComponent();

            if (selectedRoom != null)
            {
                currentRoom = selectedRoom;
                TitleText.Text = "Редактирование номера";

                NumberBox.Text = currentRoom.номер.ToString();
                CapacityBox.Text = currentRoom.вместимость.ToString();
                PriceBox.Text = currentRoom.цена_за_сутки.ToString();

                TypeBox.SelectedItem = TypeBox.Items.Cast<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == currentRoom.тип_номера);

                StatusBox.SelectedItem = StatusBox.Items.Cast<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == currentRoom.статус);
            }
            else
            {
                currentRoom = new Номера();

                TypeBox.SelectedIndex = 0;
                StatusBox.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int number;
            int capacity;
            decimal price;

            if (!int.TryParse(NumberBox.Text, out number) || !int.TryParse(CapacityBox.Text, out capacity) || !decimal.TryParse(PriceBox.Text, out price))
            {
                MessageBox.Show("Проверьте правильность введённых чисел!");
                return;
            }

            if (number <= 0 || capacity <= 0 || price <= 0)
            {
                MessageBox.Show("Значения должны быть больше нуля!");
                return;
            }

            if (TypeBox.SelectedItem == null || StatusBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип и статус номера!");
                return;
            }

            bool numberExists = HotelDBEntities.GetContext().Номера
                .Any(r => r.номер == number &&
                          r.id_номера != currentRoom.id_номера);

            if (numberExists)
            {
                MessageBox.Show("Номер комнаты уже существует!");
                return;
            }

            currentRoom.номер = number;
            currentRoom.тип_номера = (TypeBox.SelectedItem as ComboBoxItem).Content.ToString();

            currentRoom.вместимость = capacity;
            currentRoom.цена_за_сутки = price;

            currentRoom.статус = (StatusBox.SelectedItem as ComboBoxItem).Content.ToString();

            var context = HotelDBEntities.GetContext();

            if (currentRoom.id_номера == 0)
            {
                context.Номера.Add(currentRoom);
            }

            context.SaveChanges();

            MessageBox.Show("Данные сохранены!");

            Manager.MainFrame.Navigate(new RoomsPage());
        }
    }
}