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

namespace QLTH
{
    /// <summary>
    /// Interaction logic for ThemGiaoVien.xaml
    /// </summary>
    public partial class ThemGiaoVien : Window
    {
        SqlConnection con;
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dtDataTable = new DataTable("GIAOVIEN");
        DataSet GiaoVienDataSet = new DataSet();
        

        public ThemGiaoVien()
        {
            InitializeComponent();
            LoadComponent();
            
        }

        private void LoadComponent()
        {
            try
            {
                connect();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "Select * from KHOA";
                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    cmbKhoa.Items.Add(drDataReader.GetString(0) + "-" + drDataReader.GetString(1));
                }

                con.Close();
            }
            catch (SqlException)
            {
                con.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected void connect()
        {
            string strConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\QLHS.mdf;Integrated Security=True";

            try
            {
                con = new SqlConnection(strConnectionString);
                con.Open();
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected void getData()
        {
            
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT MAGV AS 'Mã giáo viên', HOTEN AS 'Họ tên', GIOITINH AS 'Giới tính', KHOA AS 'Khoa', NGVL AS 'Ngày vào làm', DIACHI AS 'Địa chỉ',HESO as 'Hệ số', MUCLUONG as 'Mức lương' FROM GIAOVIEN";
            da.SelectCommand = command;
            da.Fill(dtDataTable);
        }

        private bool KiemTraInput()
        {
            heso_tick.Text = "";
            luong_tick.Text = "";
            magv_tick.Text = "";

            int temp = 0;
            string heso = hesoluong_them.Text;
            string luong = mucluong_them.Text;

            connect(); // xem co trung MAGV
            //getData();
            SqlCommand command = new SqlCommand();
            command.Connection = con;
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT COUNT (MAGV) FROM GIAOVIEN WHERE MAGV = @MAGV";
            command.Parameters.AddWithValue("@MAGV", magv_them.Text);
            Int32 count = (Int32)command.ExecuteScalar();

            if (count != 0)
            {
                temp++;
                magv_tick.Text = "*";
            }

            for (int i = 0; i < heso.Length; i++) // kiem tra he so luong nhap vao
            {
                if (!char.IsNumber(heso[i]) && !(heso[i].ToString() == ".")) // neu co ki tu nao khong la so hay dau cham :))
                {
                    temp++;
                    heso_tick.Text = "*";
                    break;
                }
            }

            for (int i = 0; i < luong.Length; i++) // kiem tra luong nhap vao
            {
                if (!char.IsNumber(luong[i]) && !(luong[i].ToString() == "."))
                {
                    temp++;
                    luong_tick.Text = "*";
                    break;
                }
            }

            if (magv_them.Text.Length != 5)
            {
                magv_tick.Text = "*";
                temp++;
                MessageBox.Show("Qua 5 ki tu", "THÔNG BÁO", MessageBoxButton.OK);


            }

            if (cmbKhoa.SelectedIndex < 0)
            {
                temp++;
                MessageBox.Show("Mời chọn khoa cho giáo viên", "THÔNG BÁO", MessageBoxButton.OK);
            }
            


            if ( temp == 0)
            {
                return true;
            }
            return false;
            
        }
            

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            magv_them.Text.Replace(" ", " ");
            connect();
            //getData();
            if (this.KiemTraInput())
            {
                SqlCommand comInsert = new SqlCommand();
                comInsert.Connection = con;
                comInsert.CommandType = CommandType.Text;
                comInsert.CommandText = @"INSERT into GIAOVIEN (MAGV, HOTEN, GIOITINH, KHOA, CMND, NGVL, HESO, MUCLUONG, DIACHI) Values (@MAGV, @HOTEN, @GIOITINH, @KHOA, @CMND, @NGVL, @HESO, @MUCLUONG, @DIACHI) ";
                da.InsertCommand = comInsert;
                comInsert.Parameters.AddWithValue("@MAGV", magv_them.Text); // nhung dong nay con update them
                comInsert.Parameters.AddWithValue("@HOTEN", hoten_them.Text);
                comInsert.Parameters.AddWithValue("@GIOITINH", gioitinh_them.Text);
                comInsert.Parameters.AddWithValue("@KHOA", cmbKhoa.SelectedItem.ToString().Substring(0, 4));
                comInsert.Parameters.AddWithValue("@CMND", txtCMND.Text);
                comInsert.Parameters.AddWithValue("@HESO", hesoluong_them.Text);
                comInsert.Parameters.AddWithValue("@MUCLUONG", mucluong_them.Text);
                comInsert.Parameters.AddWithValue("@NGVL", ngayvaolam_them.Text);
                comInsert.Parameters.AddWithValue("@DIACHI", diachi_them.Text);

                int x = comInsert.ExecuteNonQuery();
                da.Update(dtDataTable);
                if (x == 1)
                {
                    MessageBox.Show("Bạn đã thêm một giáo viên", "THÔNG BÁO", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Thêm không thành công", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //SqlCommandBuilder builder = new SqlCommandBuilder(da);
                //da.DeleteCommand = builder.GetDeleteCommand(true);
                //da.UpdateCommand = builder.GetUpdateCommand(true);
                //da.InsertCommand = builder.GetInsertCommand(true);
                //da.Update(dtDataTable);
                //Close();
                con.Close();
            }
            else
                MessageBox.Show("Dữ liệu không đúng qui cách, nhập lại", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Warning); 
            
        }

        private void gioitinh_them_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void magv_them_TextChanged(object sender, TextChangedEventArgs e)
        {
            magv_them.MaxLength = 5;
        }


    }
}
