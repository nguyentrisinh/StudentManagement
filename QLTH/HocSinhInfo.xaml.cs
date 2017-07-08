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
    /// Interaction logic for HocSinhInfo.xaml
    /// </summary>
    public partial class HocSinhInfo : Window
    {
        private int iLuaChon;
        private string strMaHocSinh;
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        string strNamNhapHoc;
        string strSTT;
        string strNamHoc;
        string strOldClass;

        private bool CheckAge (int iAge)
        {
            int[] iAgeQuyDinh = new int [2];
            int i = 0;

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select SL from QUYDINH where MAQD = 'QD01' or MAQD = 'QD02'";

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    iAgeQuyDinh[i] = dr.GetByte(0);
                    i++;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình kiểm tra quy định", "THông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            conn.Close();
            if (iAge <= iAgeQuyDinh[1] && iAge >= iAgeQuyDinh[0])
                return true;
            MessageBox.Show(string.Concat("Tuổi của học sinh phải từ " + iAgeQuyDinh[0].ToString() + " đến " + iAgeQuyDinh[1].ToString()));
            return false;
        }

        private bool CheckSiSo (string MaLop)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select SL from QUYDINH where MAQD = @MAQD";
            cmd.Parameters.AddWithValue("@MAQD", "QD03");
            int iQuyDinh = int.Parse(cmd.ExecuteScalar().ToString());

            cmd.CommandText = "select SISO from LOP where MALOP = @MALOP";
            cmd.Parameters.AddWithValue("@MALOP", MaLop);
            int iSiSo = int.Parse(cmd.ExecuteScalar().ToString());

            conn.Close();
            if (iSiSo < iQuyDinh)
                return true;

            MessageBox.Show(string.Concat("Sĩ số mỗi lớp phải bé hơn " + iQuyDinh.ToString() + " học sinh"));
            return false;
        }

        private bool IsNumber(string strValue)
        {
            foreach (char c in strValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void UpdateSiSo(int i, string oldClass, string newClass)
        {
            if (i == 1)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from HOCSINH where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@MALOP", oldClass);
                    int iSiSo = int.Parse(cmd.ExecuteScalar().ToString());

                    cmd.CommandText = "update LOP set SISO = @SISO where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@SISO", iSiSo);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    
                }

                conn.Close();
            }
            else
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from HOCSINH where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@MALOP", newClass);
                    int iSiSo = int.Parse(cmd.ExecuteScalar().ToString());

                    cmd.CommandText = "update LOP set SISO = @SISO where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@SISO", iSiSo);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "select count(*) from HOCSINH where MALOP = @_MALOP";
                    cmd.Parameters.AddWithValue("@_MALOP", oldClass);
                    iSiSo = int.Parse(cmd.ExecuteScalar().ToString());
                    cmd.CommandText = "update LOP set SISO = @_SISO where MALOP = @_MALOP";
                    cmd.Parameters.AddWithValue("@_SISO", iSiSo);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {

                }

                conn.Close();
            }
        }



        private void LoadComponent ()
        {
            this.ResizeMode = ResizeMode.CanMinimize;

            cmbGioiTinh.Items.Add("Nam");
            cmbGioiTinh.Items.Add("Nữ");

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select * from LOP";
                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    string strMaLop = drDataReader.GetString(0);
                    cmbMaLop.Items.Add(strMaLop);
                }

                conn.Close();
            }
            catch(SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            txtNamNhapHoc.MaxLength = 4;
            txtSTT.MaxLength = 4;
            txtCMND.MaxLength = 15;

            txtSTT.ToolTip = "Bạn chỉ được nhập ký tự số vào khung này";
            txtNamNhapHoc.ToolTip = "Bạn chỉ được nhập ký tự số vào khung này";
            txtTenGVCN.ToolTip = "Bạn không được phép nhập vào khung này";
            txtCMND.ToolTip = "Bạn không được phép nhập vào khung này";

            txtTenGVCN.IsReadOnly = true;
        }

        void ShowData(string strMaHS, string strNH)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select left(MAHS,2), right(MAHS,4),HOCSINH.HOTEN,NGSINH,HOCSINH.GIOITINH,NOISINH,HOCSINH.DIACHI,HOCSINH.MALOP,GIAOVIEN.HOTEN,HOCSINH.CMND from HOCSINH,LOP,GIAOVIEN where LOP.MAGVCN = GIAOVIEN.MAGV and HOCSINH.MALOP = LOP.MALOP and MAHS = @MAHS and NAMHOC = @NAMHOC";
                cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                cmd.Parameters.AddWithValue("@NAMHOC", strNH);

                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    string strNamNH = string.Concat("20" + drDataReader.GetString(0));
                    txtNamNhapHoc.Text = strNamNH;
                    txtSTT.Text = drDataReader.GetString(1);
                    txtHoVaTen.Text = drDataReader.GetString(2);
                    dpNgaySinh.Text = drDataReader.GetDateTime(3).ToString();
                    cmbGioiTinh.SelectedValue = drDataReader.GetString(4);
                    txtNoiSinh.Text = drDataReader.GetString(5);
                    txtDiaChi.Text = drDataReader.GetString(6);
                    cmbMaLop.SelectedValue = drDataReader.GetString(7);
                    strOldClass = drDataReader.GetString(7);
                    txtTenGVCN.Text = drDataReader.GetString(8);
                    txtCMND.Text = drDataReader.GetString(9);
                }
            }
            catch(SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            conn.Close();
        }

        public HocSinhInfo()
        {
            InitializeComponent();
        }

        public HocSinhInfo (int i, string strNH)
        {
            iLuaChon = i;
            strNamHoc = strNH;
            InitializeComponent();
            LoadComponent();
        }

        public HocSinhInfo(int i, string str, string strNH)
        {
            iLuaChon = i;
            strMaHocSinh = str;
            strNamHoc = strNH;
            InitializeComponent();
            LoadComponent();
            ShowData(str,strNH);


            //bắt đầu phân trường hợp là Xem hay Sửa
            //Nếu là xem thì iLuaChon = 0, Sửa là -1
            if (iLuaChon == 0)
            {
                txtNamNhapHoc.IsReadOnly = true;
                txtSTT.IsReadOnly = true;
                txtHoVaTen.IsReadOnly = true;
                dpNgaySinh.IsEnabled = false;
                cmbGioiTinh.IsReadOnly = true;
                txtNoiSinh.IsReadOnly = true;
                txtDiaChi.IsReadOnly = true;
                cmbMaLop.IsReadOnly = true;
                txtTenGVCN.IsReadOnly = true;
                dpNgaySinh.IsEnabled = false;
                btnLuu.IsEnabled = false;
                txtCMND.IsReadOnly = true;
            }
            else
            {
                txtNamNhapHoc.IsEnabled = false;
                txtSTT.IsEnabled = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void btnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbMaLop_DropDownClosed(object sender, EventArgs e)
        {
            string strMaLop = this.cmbMaLop.Text;

            try
            {
                conn.Open();
                string strCmd = string.Concat("select HOTEN from GIAOVIEN,LOP where MALOP = '" + strMaLop + "' and MAGVCN = MAGV");

                SqlCommand cmd = new SqlCommand();
                cmd.Dispose();
                cmd.CommandText = strCmd;
                cmd.Connection = conn;

                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    string strName = drDataReader.GetString(0);
                    this.txtTenGVCN.Text = strName;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi xảy ra, mời nhập lại");
            }
            conn.Close();
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            bool bTest = true;


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



            if (!IsNumber(txtNamNhapHoc.Text))
            {
                bTest = false;
                txtNamNhapHoc.Background = Brushes.Aqua;
                txtNamNhapHoc.Background = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                txtNamNhapHoc.Background = Brushes.White;
                txtNamNhapHoc.Background = new SolidColorBrush(Colors.White);
            }

            if (!IsNumber(txtCMND.Text))
            {
                bTest = false;
                txtCMND.Background = Brushes.Aqua;
                txtCMND.Background = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                txtCMND.Background = Brushes.White;
                txtCMND.Background = new SolidColorBrush(Colors.White);
            }

            //kiểm tra nếu các điều kiện nhập sai thì tự động thoát ra kg chạy tiếp câu lệnh sql nữa 
            if (!bTest)
            {
                MessageBox.Show("Dữ liệu nhập vào có định dạng sai. Mời nhập lại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Check Age of student
            DateTime dtBirthday = DateTime.Parse(dpNgaySinh.Text);
            DateTime now = DateTime.Today;
            int age = now.Year - dtBirthday.Year;
            if (now < dtBirthday.AddYears(age))
                age--;

            if (!CheckAge(age))
            {
                return;
            }

            //kiem tra siso cua 1 lop < si so toi da trong quy dinh
            if (!CheckSiSo(cmbMaLop.Text))
                return;

            switch (txtNamNhapHoc.Text.Length)
            {
                case 1:
                    strNamNhapHoc = string.Concat("0" + txtNamNhapHoc.Text);
                    break;
                case 2:
                    strNamNhapHoc = txtNamNhapHoc.Text;
                    break;
                case 3:
                    strNamNhapHoc = txtNamNhapHoc.Text.Substring(1);
                    break;
                case 4:
                    strNamNhapHoc = txtNamNhapHoc.Text.Substring(2);
                    break;
            }

            switch (txtSTT.Text.Length)
            {
                case 1:
                    strSTT = string.Concat("000" + txtSTT.Text);
                    break;
                case 2:
                    strSTT = string.Concat("00" + txtSTT.Text);
                    break;
                case 3:
                    strSTT = string.Concat("0" + txtSTT.Text);
                    break;
                case 4:
                    strSTT = txtSTT.Text;
                    break;
            }

            string strMaHocSinh = string.Concat(strNamNhapHoc + strSTT);

            if (iLuaChon == 1)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into HOCSINH values(@MAHS,@NAMHOC,@HOTEN,@NGSINH,@GIOITINH,@CMND,@NOISINH,@DIACHI,@MALOP)";
                    cmd.Parameters.AddWithValue("@MAHS", strMaHocSinh);
                    cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                    cmd.Parameters.AddWithValue("@HOTEN", txtHoVaTen.Text);
                    cmd.Parameters.AddWithValue("@NGSINH", dpNgaySinh.Text);
                    cmd.Parameters.AddWithValue("@GIOITINH", cmbGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@CMND", txtCMND.Text);
                    cmd.Parameters.AddWithValue("@NOISINH", txtNoiSinh.Text);
                    cmd.Parameters.AddWithValue("@DIACHI", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@MALOP", cmbMaLop.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    UpdateSiSo(iLuaChon, cmbMaLop.Text, cmbMaLop.Text);


                    txtDiaChi.Clear();
                    txtHoVaTen.Clear();
                    txtCMND.Clear();
                    txtNamNhapHoc.Clear();
                    txtNoiSinh.Clear();
                    txtSTT.Clear();
                    txtTenGVCN.Clear();
                    cmbGioiTinh.SelectedValue = "";
                    cmbMaLop.SelectedValue = "";

                    
                    MessageBox.Show("Bạn đã nhập dữ liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Có lỗi trong quá trình nhập dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    cmd.CommandText = "update  HOCSINH set HOTEN = @HOTEN, NGSINH = @NGSINH, GIOITINH = @GIOITINH, CMND = @CMND, @NOISINH = @NOISINH, DIACHI = @DIACHI, MALOP = @MALOP where MAHS = @MAHS";
                    cmd.Parameters.AddWithValue("@MAHS", strMaHocSinh);
                    cmd.Parameters.AddWithValue("@HOTEN", txtHoVaTen.Text);
                    cmd.Parameters.AddWithValue("@NGSINH", dpNgaySinh.Text);
                    cmd.Parameters.AddWithValue("@GIOITINH", cmbGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@CMND", txtCMND.Text);
                    cmd.Parameters.AddWithValue("@NOISINH", txtNoiSinh.Text);
                    cmd.Parameters.AddWithValue("@DIACHI", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@MALOP", cmbMaLop.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    UpdateSiSo(iLuaChon, strOldClass, cmbMaLop.Text);

                    MessageBox.Show("Bạn đã sửa dữ liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Có lỗi trong quá trình nhập dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Close();
            }
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
                float iQuyDinh =float.Parse(cmd.ExecuteScalar().ToString());
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

    }
}
