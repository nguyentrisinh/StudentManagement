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

namespace QLTH
{
    /// <summary>
    /// Interaction logic for ChangePass.xaml
    /// </summary>
    public partial class ChangePass : Window
    {
        public ChangePass()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (String.Compare(matKhaumoi.Password, matKhaumoi1.Password) == 0)
            {
                Password TaiKhoan = new Password();
                TaiKhoan.ThayDoiPass(tenDN.Text, matKhau.Password, matKhaumoi.Password);
            }
            else
            {
                MessageBox.Show("Cần xác nhận lại mật khẩu mới", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
    }
}
