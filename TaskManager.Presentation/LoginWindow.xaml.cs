using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskManager.Business;
using TaskManager.Data;
using System.Windows;

namespace TaskManager.Presentation
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var phoneNumber = phoneTextBox.Text;
            var password = passwordBox.Password;
            if (phoneNumber.Length > 10)
            {
                MessageBox.Show("Kullanıcı kodu 10 karakterden fazla olamaz.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = await _userService.AuthenticateUserAsync(phoneNumber, password);

            if (user != null)
            {
                GlobalVariables.LOGIN_ID = user.USER_ID;
                GlobalVariables.IS_ADMIN = user.IS_ADMIN;
                var mainWindow = new MainWindow(new TaskService(new TaskRepository()), _userService)
                {
                    UserDisplayName = $"{user.USER_NAME} {user.USER_SURNAME}"
                };
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Telefon numarası veya şifre yanlış.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}