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
    /// Interaction logic for KhoaInfo.xaml
    /// </summary>
    public partial class KhoaInfo : Window
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        int iCount;
        bool bNhap;
        string strOldMaKhoa;

        public KhoaInfo()
        {
            InitializeComponent();
            LoadComponent();
        }

        void LoadData(string str)
        {
            try
            {
                SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(str, strConnectionString);
                DataTable dtDataTable = new DataTable();
                dtDataTable.Clear();
                sdaDataAdapter.Fill(dtDataTable);
                dgvKhoa.ItemsSource = dtDataTable.DefaultView;

            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        void LoadComponent()
        {
            string strKhoa;
            txtTenKhoa.IsEnabled = false;
            cmbTruongKhoa.IsEnabled = false;
            btnHuy.IsEnabled = false;
            btnLuu.IsEnabled = false;

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT MAKHOA, KHOA.TRGKHOA,GIAOVIEN.HOTEN FROM KHOA,GIAOVIEN WHERE KHOA.TRGKHOA = GIAOVIEN.MAGV";

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    strKhoa = string.Concat(dr.GetString(1) + "-" + dr.GetString(2));
                    cmbTruongKhoa.Items.Add(strKhoa);
                }
                dr.Close();

                cmbTruongKhoa.Items.Add("Chưa xác định trưởng khoa");

                cmd.CommandText = "select MAKHOA as 'Mã khoa', TENKHOA as 'Tên khoa', TRGKHOA as 'Mã trưởng khoa' from KHOA";

                LoadData(cmd.CommandText);


            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            conn.Close();
        }

        private void btnNhap_Click(object sender, RoutedEventArgs e)
        {
            txtTenKhoa.IsEnabled = true;
            cmbTruongKhoa.IsEnabled = true;
            btnHuy.IsEnabled = true;
            btnLuu.IsEnabled = true;
            btnNhap.IsEnabled = false;
            btnXoa.IsEnabled = false;
            btnSua.IsEnabled = false;
            btnXoa.IsEnabled = false;

            bNhap = true;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {

            string strMaGV;
            string strMaKhoa;

            if (cmbTruongKhoa.SelectedIndex < 0)
            {
                MessageBox.Show("Mời bạn chọn mã trưởng khoa");
                return;
            }

            if (cmbTruongKhoa.SelectedItem.ToString() == "Chưa xác định trưởng khoa")
                strMaGV = "GV000";
            else
            {
                strMaGV = cmbTruongKhoa.SelectedItem.ToString().Substring(0, 5);
            }

            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select count(*) from KHOA";

            SqlDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                iCount = dr.GetInt32(0);
            }
            iCount++;
            dr.Close();

            if (iCount < 10)
            {
                strMaKhoa = string.Concat("KH0" + iCount.ToString());
            }
            else
            {
                strMaKhoa = string.Concat("KH" + iCount.ToString());
            }

            cmd.Dispose();

            if (bNhap)
            {
                cmd.CommandText = "Insert into KHOA values (@MAKHOA,@TENKHOA,@TRGKHOA)";
                cmd.Parameters.AddWithValue("@MAKHOA", strMaKhoa);
                cmd.Parameters.AddWithValue("@TENKHOA", txtTenKhoa.Text);
                cmd.Parameters.AddWithValue("@TRGKHOA", strMaGV);
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = "update KHOA set TENKHOA = @TENKHOA, TRGKHOA = @TRGKHOA where MAKHOA = @MAKHOA";
                cmd.Parameters.AddWithValue("@MAKHOA", strOldMaKhoa);
                cmd.Parameters.AddWithValue("@TENKHOA", txtTenKhoa.Text);
                cmd.Parameters.AddWithValue("@TRGKHOA", strMaGV);
                cmd.ExecuteNonQuery();
            }



            LoadData("select MAKHOA as 'Mã khoa', TENKHOA as 'Tên khoa', TRGKHOA as 'Mã trưởng khoa' from KHOA");

            conn.Close();

            txtTenKhoa.IsEnabled = false;
            cmbTruongKhoa.IsEnabled = false;
            btnHuy.IsEnabled = false;
            btnLuu.IsEnabled = false;
            btnNhap.IsEnabled = true;
            btnXoa.IsEnabled = true;
            btnSua.IsEnabled = true;
            btnXoa.IsEnabled = true;
            txtTenKhoa.Clear();
            cmbTruongKhoa.SelectedIndex = -1;
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            txtTenKhoa.IsEnabled = false;
            cmbTruongKhoa.IsEnabled = false;
            btnHuy.IsEnabled = false;
            btnLuu.IsEnabled = false;
            btnNhap.IsEnabled = true;
            btnXoa.IsEnabled = true;
            btnSua.IsEnabled = true;
            btnXoa.IsEnabled = true;

            txtTenKhoa.Clear();
            cmbTruongKhoa.SelectedIndex = -1;
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgvKhoa.SelectedIndex < 0)
                return;

            strOldMaKhoa = ((DataRowView)dgvKhoa.SelectedItem).Row[0].ToString();
            txtTenKhoa.Text = ((DataRowView)dgvKhoa.SelectedItem).Row[1].ToString();

            txtTenKhoa.IsEnabled = true;
            cmbTruongKhoa.IsEnabled = true;
            btnHuy.IsEnabled = true;
            btnLuu.IsEnabled = true;
            btnNhap.IsEnabled = false;
            btnXoa.IsEnabled = false;
            btnSua.IsEnabled = false;
            btnXoa.IsEnabled = false;

            bNhap = false;
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgvKhoa.SelectedIndex < 0)
                return;
            strOldMaKhoa = ((DataRowView)dgvKhoa.SelectedItem).Row[0].ToString();

            conn.Open();
            SqlCommand cmd = new SqlCommand("delete from KHOA where MAKHOA = @MAKHOA", conn);
            cmd.Parameters.AddWithValue("@MAKHOA", strOldMaKhoa);
            cmd.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("Bạn đã xóa thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            strOldMaKhoa = "";

            LoadData("select MAKHOA as 'Mã khoa', TENKHOA as 'Tên khoa', TRGKHOA as 'Mã trưởng khoa' from KHOA");

        }
    }
}
