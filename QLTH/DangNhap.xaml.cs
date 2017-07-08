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
using System.Data.SqlClient;
using System.Windows.Media.Animation;

namespace QLTH
{
    /// <summary>
    /// Interaction logic for DangNhap.xaml
    /// </summary>
    public partial class DangNhap : Window
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);

        public DangNhap()
        {
            InitializeComponent();
        }

        public void CountUser()
        {

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TAIKHOAN", conn);
                int iCount = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                if (iCount == 0)
                {
                    MessageBox.Show("Mời bạn tạo 1 tài khoản trước khi đăng nhập", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    AddAccountWindow newWindow = new AddAccountWindow();
                    newWindow.ShowDialog();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(string.Concat("Error:" + ex.ToString()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            conn.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string a = tenDN.Text;
            string b = matKhau.Password;
            Password TaiKhoanTraVe = new Password();
            string str = TaiKhoanTraVe.DangNhap(a, b);
            if (str == null)
                return;
            else
            {
                MainWindow window = new MainWindow(str);
                window.Show();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CountUser(); 
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string a = tenDN.Text;
                string b = matKhau.Password;
                Password TaiKhoanTraVe = new Password();
                string str = TaiKhoanTraVe.DangNhap(a, b);
                if (str == null)
                    return;
                else
                {
                    MainWindow window = new MainWindow(str);
                    window.Show();
                    this.Close();
                }
                return;
            }
        }

        private void tenDN_GotFocus(object sender, RoutedEventArgs e)
        {
            tenDN.Text = "";
            tenDN.GotFocus -= tenDN_GotFocus;
            tenDN.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void matKhau_GotFocus(object sender, RoutedEventArgs e)
        {
            matKhau.Password = "";
            matKhau.GotFocus -= matKhau_GotFocus;
        }

    
    }
}
