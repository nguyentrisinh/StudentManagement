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
    /// Interaction logic for NhapDiem.xaml
    /// </summary>
    public partial class NhapDiem : Window
    {
        public NhapDiem()
        {
            InitializeComponent();
            fill_combobox_MON();
            fill_combobox_Check_lop();
            check_lophoc.SelectedIndex = 0;
            Load_DataGrid(check_lophoc.SelectedItem.ToString());
        }

        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        

        void fill_combobox_MON()
        {
            try
            {
                conn.Open();
                string Query = "SELECT * FROM MONHOC";
                SqlCommand cm = new SqlCommand(Query, conn);
                cm.Connection = conn;
                SqlDataReader dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    string MAMONHOC = dr.GetString(0);
                    box_mon.Items.Add(MAMONHOC);
                }
                conn.Close();

            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

        }

        void fill_combobox_Check_lop()
        {
            try
            {
                conn.Open();
                string Query = "SELECT * FROM LOP";
                SqlCommand cm = new SqlCommand(Query, conn);
                cm.Connection = conn;
                SqlDataReader dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    string MALOP = dr.GetString(0);
                    check_lophoc.Items.Add(MALOP);
                }
                conn.Close();

            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        void Load_DataGrid(string malop)
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;
                cm.Parameters.AddWithValue("@_malop", malop);
                cm.CommandText = @"SELECT HOCSINH.MAHS as 'Mã học sinh', HOTEN as 'Họ tên', MALOP as 'Mã lớp', MAMH as 'Mã môn học', HOCKY as 'Học kì', HOCSINH.NAMHOC as 'Năm học', H1D1, H1D2, H1D3, H1D4, H1D5, H2D1, H2D2, H2D3, H2D4, H2D5, THI as 'Điểm thi', DTB as 'Điểm TB', DANHGIA as 'Đánh giá' FROM HOCSINH LEFT JOIN KETQUAMON ON HOCSINH.MAHS = KETQUAMON.MAHS WHERE MALOP = @_malop";

                SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(cm);

                DataTable dtDataTable = new DataTable();
                dtDataTable.Clear();
                sdaDataAdapter.Fill(dtDataTable);
                data_grid.ItemsSource = dtDataTable.DefaultView;
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void data_grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (data_grid.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)data_grid.SelectedItem;
                mahocsinh.Text = row["Mã học sinh"].ToString(); 
                tenhocsinh.Text = row["Họ tên"].ToString();
                malop.Text = row["Mã lớp"].ToString();
                namhoc.Text = row["Năm học"].ToString();
            }

        }

        private int GetHeso1(string maMonhoc)
        {
            int heso;
            
            try
            {
                conn.Open();
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = "SELECT HESO1 FROM MONHOC WHERE MAMH = @_MAMH";
                cm.Parameters.AddWithValue("@_MAMH", maMonhoc);
                heso = Convert.ToInt32(cm.ExecuteScalar().ToString());
                conn.Close();
                return heso;
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }

        }

        private int GetHeso2(string maMonhoc)
        {
            int heso;

            try
            {
                conn.Open();
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = "SELECT HESO2 FROM MONHOC WHERE MAMH = @_MAMH";
                cm.Parameters.AddWithValue("@_MAMH", maMonhoc);
                heso = Convert.ToInt32(cm.ExecuteScalar().ToString());
                conn.Close();
                return heso;
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }

        }

        private void Dis()
        {
            h1d5.IsEnabled = true;
            h1d4.IsEnabled = true;
            h1d3.IsEnabled = true;
            h1d2.IsEnabled = true;
            h1d1.IsEnabled = true;
            h2d5.IsEnabled = true;
            h2d4.IsEnabled = true;
            h2d3.IsEnabled = true;
            h2d2.IsEnabled = true;
            h2d1.IsEnabled = true;

            switch (GetHeso1(box_mon.SelectedItem.ToString()))
            {
                case 5:
                    break;
                case 4: h1d5.IsEnabled = false;
                    break;
                case 3: h1d5.IsEnabled = false;
                    h1d4.IsEnabled = false;
                    break;
                case 2: h1d5.IsEnabled = false;
                    h1d4.IsEnabled = false;
                    h1d3.IsEnabled = false;
                    break;
                case 1: h1d5.IsEnabled = false;
                    h1d4.IsEnabled = false;
                    h1d3.IsEnabled = false;
                    h1d2.IsEnabled = false;
                    break;
                case 0: h1d5.IsEnabled = false;
                    h1d4.IsEnabled = false;
                    h1d3.IsEnabled = false;
                    h1d2.IsEnabled = false;
                    h1d1.IsEnabled = false;
                    break;
            }

            switch (GetHeso2(box_mon.SelectedItem.ToString()))
            {
                case 5:
                    break;
                case 4: h2d5.IsEnabled = false;
                    break;
                case 3: h2d5.IsEnabled = false;
                    h2d4.IsEnabled = false;
                    break;
                case 2: h2d5.IsEnabled = false;
                    h2d4.IsEnabled = false;
                    h2d3.IsEnabled = false;
                    break;
                case 1: h2d5.IsEnabled = false;
                    h2d4.IsEnabled = false;
                    h2d3.IsEnabled = false;
                    h2d2.IsEnabled = false;
                    break;
                case 0: h2d5.IsEnabled = false;
                    h2d4.IsEnabled = false;
                    h2d3.IsEnabled = false;
                    h2d2.IsEnabled = false;
                    h2d1.IsEnabled = false;
                    break;
            }
        }

        private void box_mon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dis();
        }

        private string CheckDat(float fDiem)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select SL from QUYDINH where MAQD = @MAQD";
                cmd.Parameters.AddWithValue("@MAQD", "QD04");
                float iQuyDinh = float.Parse(cmd.ExecuteScalar().ToString());
                if (fDiem < iQuyDinh)
                {
                    conn.Close();
                    return "Không đạt";
                }
                else
                {
                    conn.Close();
                    return "Đạt";
                }
            }
            catch (SqlException)
            {
                conn.Close();
                return null;
            }
        } 
        private void Button_Click(object sender, RoutedEventArgs e)
        {  
            double hs1 = (Convert.ToDouble(h1d1.Text) + Convert.ToDouble(h1d2.Text) + Convert.ToDouble(h1d3.Text) + Convert.ToDouble(h1d4.Text) + Convert.ToDouble(h1d5.Text)) / GetHeso1(box_mon.SelectedItem.ToString());
            double hs2 = (Convert.ToDouble(h2d1.Text) + Convert.ToDouble(h2d2.Text) + Convert.ToDouble(h2d3.Text) + Convert.ToDouble(h2d4.Text) + Convert.ToDouble(h2d5.Text)) / GetHeso2(box_mon.SelectedItem.ToString());
            double thi = Math.Round(Convert.ToDouble(diemthi.Text),2);
            float dtb = (float)Math.Round((hs1 + hs2 * 2 + thi * 3) / 6,2);
            
            string danhgia = CheckDat(dtb);
            conn.Open();
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = "insert into KETQUAMON (MAHS, MAMH, NAMHOC, HOCKY, H1D1, H1D2, H1D3, H1D4, H1D5, H2D1, H2D2, H2D3, H2D4, H2D5,THI,DTB, DANHGIA) values (@MAHS, @MAMH, @NAMHOC, @HOCKY,@H1D1,@H1D2,@H1D3,@H1D4,@H1D5,@H2D1,@H2D2,@H2D3,@H2D4,@H2D5,@THI,@DTB,@DANHGIA)";
            cmd2.Parameters.AddWithValue("@MAMH", box_mon.SelectedItem.ToString());
            cmd2.Parameters.AddWithValue("@DTB", dtb.ToString());
            cmd2.Parameters.AddWithValue("@MAHS", mahocsinh.Text.ToString());
            cmd2.Parameters.AddWithValue("@NAMHOC", namhoc.Text);
            cmd2.Parameters.AddWithValue("@HOCKY", box_hocky.Text);
            cmd2.Parameters.AddWithValue("@DANHGIA", danhgia);
            cmd2.Parameters.AddWithValue("@H1D1", h1d1.Text);
            cmd2.Parameters.AddWithValue("@H1D2", h1d2.Text);
            cmd2.Parameters.AddWithValue("@H1D3", h1d3.Text);
            cmd2.Parameters.AddWithValue("@H1D4", h1d4.Text); 
            cmd2.Parameters.AddWithValue("@H1D5", h1d5.Text);
            cmd2.Parameters.AddWithValue("@H2D1", h2d1.Text);
            cmd2.Parameters.AddWithValue("@H2D2", h2d2.Text);
            cmd2.Parameters.AddWithValue("@H2D3", h2d3.Text);
            cmd2.Parameters.AddWithValue("@H2D4", h2d4.Text);
            cmd2.Parameters.AddWithValue("@H2D5", h2d5.Text);
            cmd2.Parameters.AddWithValue("@THI", diemthi.Text);
            cmd2.ExecuteNonQuery();
            AverageGrade avgGrade = new AverageGrade(mahocsinh.Text.ToString(), namhoc.Text, box_hocky.Text);
            avgGrade.UpdateDTB();
            conn.Close();
            Load_DataGrid(check_lophoc.Text);
        }

        private void check_lophoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Load_DataGrid(check_lophoc.Text);
        }

        private void check_monhoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Load_DataGrid(check_lophoc.Text);
        }

        private void check_lophoc_DropDownClosed(object sender, EventArgs e)
        {
            Load_DataGrid(check_lophoc.Text);
        }
    }
}
