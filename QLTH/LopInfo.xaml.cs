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
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading;

namespace QLTH
{
    /// <summary>
    /// Interaction logic for LopInfo.xaml
    /// </summary>
    public partial class LopInfo : Window
    {

        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        void LoadComponent()
        {
            txtSiSo.IsReadOnly = true;
            this.ResizeMode = System.Windows.ResizeMode.CanMinimize;

            cmbKhoi.Items.Add("10");
            cmbKhoi.Items.Add("11");
            cmbKhoi.Items.Add("12");

            //lay du lieu tu database vao combo box MaGVCN
            SqlConnection conn = new SqlConnection(strConnectionString);
            conn.Open();
            try
            {
                string strCommand = "SELECT * FROM GIAOVIEN";
                SqlCommand cmd = new SqlCommand(strCommand,conn);

                SqlDataReader dr = cmd.ExecuteReader();

                cmd.Dispose();

                while(dr.Read())
                {
                    string name = dr.GetString(0);
                    cmbMaGVCN.Items.Add(name);
                }
            }
            catch(SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL","Cảnh báo",MessageBoxButton.OK,MessageBoxImage.Warning);  
            }

            txtSTT.MaxLength = 2;
            txtTenLop.MaxLength = 8;
            txtTenGVCN.IsReadOnly = true;

            //ghi chú khi rê chuột vào ComboBox hay TextBox
            txtSTT.ToolTip = "Bạn chỉ được nhập số vào khung này";
            txtSiSo.ToolTip = "Bạn chỉ được nhập số vào khung này";
            txtTenLop.ToolTip = "Bạn chỉ được nhập tên lớp có 8 ký tự";

            conn.Close();
        }
        public LopInfo()
        {
            InitializeComponent();
            LoadComponent();
        }


        private int iLuaChon;
        private string strMaLop;
        public LopInfo(int i)
        {
            iLuaChon = i;
            InitializeComponent();
            LoadComponent();
        }

        //hàm đưa thông tin lên các component của form
        void ShowData(string strMaLop)
        {
            conn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select left(MALOP,2), right(MALOP,2),TENLOP,SISO,MAGVCN,HOTEN from LOP,GIAOVIEN where LOP.MAGVCN = GIAOVIEN.MAGV and MALOP = @MALOP";
                cmd.Parameters.AddWithValue("@MALOP", strMaLop);

                SqlDataReader drDataReader = cmd.ExecuteReader();


                while (drDataReader.Read())
                {
                    cmbKhoi.SelectedValue = drDataReader.GetString(0);
                    cmbMaGVCN.SelectedValue = drDataReader.GetString(4);
                    txtSTT.Text = drDataReader.GetString(1);
                    txtTenLop.Text = drDataReader.GetString(2);
                    txtSiSo.Text = drDataReader.GetInt32(3).ToString();
                    txtTenGVCN.Text = drDataReader.GetString(5);
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            conn.Close();
        }
        
        public LopInfo(int i, string str)
        {
            iLuaChon = i;
            strMaLop = str;
            InitializeComponent();
            LoadComponent();
            ShowData(strMaLop);



            //bắt đầu phân trường hợp là Xem hay Sửa
            //Nếu là xem thì iLuaChon = 0, Sửa là -1
            if ( iLuaChon == 0 )
            {
                //disabled các textbox, combobox và button Luu
                txtSiSo.IsReadOnly = true;
                txtSTT.IsReadOnly = true;
                txtTenLop.IsReadOnly = true;
                cmbKhoi.IsReadOnly = true;
                cmbMaGVCN.IsReadOnly = true;
                btnLuu.IsEnabled = false;
            }
            else
            {
                //disabled cmbKhoi và txtSTT để người dùng không thể thay đổi mã Lớp vì vấn đề dễ đụng độ dữ liệu trong database
                cmbKhoi.IsEnabled = false;
                txtSTT.IsEnabled = false;
            }
        }


        private void cmbMaGVCN_DropDownClosed(object sender, EventArgs e)
        {
            string strMAGV = this.cmbMaGVCN.Text;
            try
            {
                conn.Open();
                string strCmd = string.Concat("select * from GIAOVIEN where MAGV = '" + strMAGV + "'");

                SqlCommand cmd = new SqlCommand();
                cmd.Dispose();
                cmd.CommandText = strCmd;
                cmd.Connection = conn;

                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    string strName = drDataReader.GetString(1);
                    this.txtTenGVCN.Text = strName;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi xảy ra, mời nhập lại");
            }
            conn.Close();
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Ham kiểm tra các ký tự trong chuỗi có phải là số kg
        private bool IsNumber (string strValue)
        {
            foreach(char c in strValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            bool bTest = true;

            if (cmbKhoi.SelectedIndex < 0)
            {
                MessageBox.Show("Mởi bạn chọn khối cho lớp học", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtSTT.Text.Length == 0)
            {
                txtSTT.Background = Brushes.Aqua;
                txtSTT.Background = new SolidColorBrush(Colors.Aqua);
                MessageBox.Show("Mởi bạn nhập số thứ tự của lớp học", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                txtSTT.Background = Brushes.White;
                txtSTT.Background = new SolidColorBrush(Colors.White);
            }


            if (!IsNumber(txtSiSo.Text))
            {
                bTest = false;
                txtSiSo.Background = Brushes.Aqua;
                txtSiSo.Background = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                txtSiSo.Background = Brushes.White;
                txtSiSo.Background = new SolidColorBrush(Colors.White);
            }

            if (!IsNumber(txtSTT.Text))
            {
                bTest = false;
                txtSTT.Background = Brushes.Aqua;
                txtSTT.Background = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                txtSTT.Background = Brushes.White;
                txtSTT.Background = new SolidColorBrush(Colors.White);
            }

            //kiểm tra nếu các điều kiện nhập sai thì tự động thoát ra kg chạy tiếp câu lệnh sql nữa 
            if (!bTest)
            {
                MessageBox.Show("Dữ liệu nhập vào có định dạng sai. Mời nhập lại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (txtSTT.Text.Length == 1)
            {
                txtSTT.Text = string.Concat("0" + txtSTT.Text);
            }
                  

            string strMaLopMoi = String.Concat(cmbKhoi.Text + "A" + txtSTT.Text);

            //Lựa chọn trường hợp là Nhập mới hay Sửa thông tin cá nhân để có câu lệnh SQL phù hợp
            if (iLuaChon == 1)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into LOP(MALOP,TENLOP,SISO,MAGVCN) values(@MALOP,@TENLOP,@SISO,@MAGVCN)";
                    cmd.Parameters.AddWithValue("@MALOP", strMaLopMoi);
                    cmd.Parameters.AddWithValue("@TENLOP", txtTenLop.Text);
                    cmd.Parameters.AddWithValue("@SISO", txtSiSo.Text);
                    cmd.Parameters.AddWithValue("MAGVCN", cmbMaGVCN.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    txtSiSo.Clear();
                    txtSTT.Clear();
                    txtTenGVCN.Clear();
                    txtTenLop.Clear();
                    cmbMaGVCN.Text = "";

                    MessageBox.Show("Bạn đã nhập dữ liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Co lỗi trong quá trình nhập dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "update LOP set TENLOP = @TENLOP, SISO = @SISO, MAGVCN = @MAGVCN where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@TENLOP", txtTenLop.Text);
                    cmd.Parameters.AddWithValue("@SISO", txtSiSo.Text);
                    cmd.Parameters.AddWithValue("@MAGVCN", cmbMaGVCN.SelectedValue);
                    cmd.Parameters.AddWithValue("@MALOP", strMaLopMoi);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("Bạn đã sửa dữ liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();

                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Có lỗi trong quá trình sửa thông tin. Mời bạn thực hiện lại thao tác", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        //để thay đổi result để khi showdialog có thể so sánh dialog có tắt chưa để thực hiện việc refesh
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
