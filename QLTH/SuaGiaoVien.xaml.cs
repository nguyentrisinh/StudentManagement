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
    /// Interaction logic for SuaGiaoVien.xaml
    /// </summary>
    public partial class SuaGiaoVien : Window
    {
        SqlConnection con;
        SqlDataAdapter da = new SqlDataAdapter();
        DataTable dtDataTable = new DataTable("GIAOVIEN");
        DataSet GiaoVienDataSet = new DataSet();
        int iLuaChon;
        public SuaGiaoVien(string MaGV, string Hoten, string Gioitinh, string Khoa, string CMND, string Ngay, string Heso, string Luong, string Diachi, int i)
        {
            InitializeComponent();
            LoadComponent();
            Magv_sua.Text = MaGV;
            Hoten_sua.Text = Hoten;
            Gioitinh_sua.Text = Gioitinh;
            //Khoa_sua.Text = Khoa;
            Heso_sua.Text = Heso;
            Luong_sua.Text= Luong;
            Ngay_sua.Text = Ngay;
            Diachi_sua.Text = Diachi;
            txtCMND.Text = CMND;
            iLuaChon = i;

            //Lấy dữ liệu từ khoa của GV được chọn đưa vào comboBox
            try
            {
                connect();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "Select * from KHOA where TENKHOA = @TENKHOA";
                cmd.Parameters.AddWithValue("@TENKHOA", Khoa);
                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    cmbKhoa.SelectedValue = string.Concat(drDataReader.GetString(0) + "-" + drDataReader.GetString(1));
                }

                con.Close();
            }
            catch (SqlException)
            {
                con.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            if ( iLuaChon == 0 )
            {
                Magv_sua.IsEnabled = false;
                Hoten_sua.IsEnabled = false;
                Gioitinh_sua.IsEnabled = false;
                //Khoa_sua.IsEnabled = false;
                Heso_sua.IsEnabled = false;
                Luong_sua.IsEnabled = false;
                Ngay_sua.IsEnabled = false;
                Diachi_sua.IsEnabled = false;
            }



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

            int temp = 0;
            string heso = Heso_sua.Text;
            string luong = Luong_sua.Text;

            for (int i = 0; i < heso.Length; i++) // kiem tra he so luong nhap vao
            {
                if (!char.IsNumber(heso[i]) && !(heso[i].ToString() == ".")) // neu co ki tu nao khong la so hay dau cham =="
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


            if (temp == 0)
            {
                return true;
            }
            return false;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (iLuaChon == 1)
            {
                connect();
                //getData();

                if (this.KiemTraInput())
                {
                    SqlCommand commandUpdate = new SqlCommand();
                    commandUpdate.Connection = con;
                    commandUpdate.CommandType = CommandType.Text;
                    commandUpdate.CommandText = @"UPDATE GIAOVIEN Set HOTEN = @HOTEN, GIOITINH = @GIOITINH, KHOA = @KHOA, CMND = @CMND, NGVL = @NGVL, DIACHI = @DIACHI, HESO = @HESO, MUCLUONG = @MUCLUONG WHERE MAGV = @MAGV";
                    commandUpdate.Parameters.AddWithValue("@MAGV", Magv_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@HOTEN", Hoten_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@GIOITINH", Gioitinh_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@KHOA", cmbKhoa.SelectedItem.ToString().Substring(0, 4));
                    commandUpdate.Parameters.AddWithValue("@CMND", txtCMND.Text);
                    commandUpdate.Parameters.AddWithValue("@HESO", Heso_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@MUCLUONG", Luong_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@NGVL", Ngay_sua.Text);
                    commandUpdate.Parameters.AddWithValue("@DIACHI", Diachi_sua.Text);
                    da.UpdateCommand = commandUpdate;
                    int x = commandUpdate.ExecuteNonQuery();
                    da.Update(dtDataTable);
                    if (x == 1)
                    {
                        MessageBox.Show("Bạn đã sửa một giáo viên", "THÔNG BÁO", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Sửa không thành công", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    //SqlCommandBuilder builder = new SqlCommandBuilder(da);
                    //da.DeleteCommand = builder.GetDeleteCommand(true);
                    //da.UpdateCommand = builder.GetUpdateCommand(true);
                    //da.InsertCommand = builder.GetInsertCommand(true);
                    //da.Update(dtDataTable);
                    con.Close();
                    Close();
                }
                else
                    MessageBox.Show("Dữ liệu không đúng qui cách, nhập lại", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                this.Close();
        }
    }
}
