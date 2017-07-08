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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;




namespace QLTH
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        private int iBang;
        SqlConnection conn = new SqlConnection(strConnectionString);

        //these params use for user mode
        string strTenDN = "";
        string strQuyen = "";

        void LoadData(string str)
        {
            try
            {
                SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(str, strConnectionString);
                DataTable dtDataTable = new DataTable();
                dtDataTable.Clear();
                sdaDataAdapter.Fill(dtDataTable);
                MainDataGrid.ItemsSource = dtDataTable.DefaultView;

            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        void LoadComponent()
        {
            int iIndex = 0;

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NAMHOC";
                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    iIndex++;
                    comboBox1.Items.Add(drDataReader.GetString(0));
                    ComboBoxa.Items.Add(drDataReader.GetString(0));
                }
            }
            catch (SqlException)
            {

                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            conn.Close();

            comboBox1.SelectedIndex = (iIndex - 1);
            ComboBoxa.SelectedIndex = (iIndex - 1);

            //Load treeView Coomponent
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = "Khối 10";
            treeView.Items.Add(item1);

            TreeViewItem item2 = new TreeViewItem();
            item2.Header = "Khối 11";
            treeView.Items.Add(item2);

            TreeViewItem item3 = new TreeViewItem();
            item3.Header = "Khối 12";
            treeView.Items.Add(item3);

            //Load from DataBase 
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select * from LOP";
                
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string str = dr.GetString(0);

                    if (str.Substring(0, 2) == "10")
                        item1.Items.Add(str);

                    if (str.Substring(0, 2) == "11")
                        item2.Items.Add(str);

                    if (str.Substring(0, 2) == "12")
                        item3.Items.Add(str);

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conn.Close();
            
        }

        


        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;
            this.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            this.MainDataGrid.IsReadOnly = true;
            LoadComponent();
        }


        void AnalyzeTenDN (string str)
        {
            int i = str.LastIndexOf('-');
            strTenDN = str.Substring(0, i);
            strQuyen = str.Substring(i + 1);

            if (int.Parse(strQuyen) == 2)
            {
                ThietLap.IsEnabled = false;
            }
            else if (int.Parse(strQuyen) == 3)
            {
                ThietLap.IsEnabled = false;
                buttonTHEM.IsEnabled = false;
                buttonXOA.IsEnabled = false;
                buttonSUA.IsEnabled = false;
                QuanLyKhoa.IsEnabled = false;

            }
        }

        public MainWindow(string str)
        {
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;
            this.ResizeMode = System.Windows.ResizeMode.CanMinimize;
            this.MainDataGrid.IsReadOnly = true;
            LoadComponent();

            AnalyzeTenDN(str);
        }

        private void btnnGiaoVien_Click(object sender, RoutedEventArgs e)
        {
            iBang = 2;
            buttonREFRES_Click(sender, e);
        }

        private void btnLop_Click(object sender, RoutedEventArgs e)
        {
            iBang = 1;
            buttonREFRES_Click(sender, e);
        }

        private void btnK10_Click(object sender, RoutedEventArgs e)
        {
            iBang = 4;
            buttonREFRES_Click(sender, e);
        }

        private void btnK11_Click(object sender, RoutedEventArgs e)
        {
            iBang = 5;
            buttonREFRES_Click(sender, e);
        }

        private void btnK12_Click(object sender, RoutedEventArgs e)
        {
            iBang = 6;
            buttonREFRES_Click(sender, e);
        }

        private void btnMonHoc_Click(object sender, RoutedEventArgs e)
        {
            iBang = 3;
            buttonREFRES_Click(sender, e);
        }

        private void buttonTHEM_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:

                    LopInfo windowLopInfo = new LopInfo(1);
                    windowLopInfo.ShowDialog();

                    break;
                case 2:
                    ThemGiaoVien _themgv = new ThemGiaoVien();
                    _themgv.ShowDialog();
                    break;
                case 3:
                    MonHocInfo newMonHocInfo = new MonHocInfo(1);
                    newMonHocInfo.ShowDialog();
                    break;
                case 4:
                    HocSinhInfo windowHocSinhInfo = new HocSinhInfo(1, comboBox1.SelectedItem.ToString());
                    windowHocSinhInfo.ShowDialog();

                    break;
                case 5:
                    HocSinhInfo windowHocSinhInfo2 = new HocSinhInfo(1, comboBox1.SelectedItem.ToString());
                    windowHocSinhInfo2.ShowDialog();
                    break;
                case 6:
                    HocSinhInfo windowHocSinhInfo3 = new HocSinhInfo(1, comboBox1.SelectedItem.ToString());
                    windowHocSinhInfo3.ShowDialog();
                    break;
            }
            buttonREFRES_Click(sender, e);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            iBang = 1;
            buttonREFRES_Click(sender, e);

            //hien icon vs phan quyen goc trai tren
            switch (strQuyen)
            {
                //Sep
                case "1":
                    image.Source = new BitmapImage(new Uri("/icon/admin.png", UriKind.Relative));
                    expaPhaQuyen.Header = "Ban giám hiệu";
                    break;
                //Linh
                case "2":
                    image.Source = new BitmapImage(new Uri("/icon/officer.png", UriKind.Relative));
                    expaPhaQuyen.Header = "Văn phòng";
                    break;
                //Bo cua sep
                case "3":
                    image.Source = new BitmapImage(new Uri("/icon/teacher.png", UriKind.Relative));
                    expaPhaQuyen.Header = "Giáo viên";
                    break;
            }
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Doing nothing
        }

        private void buttonXEM_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaLop = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        LopInfo windowLopInfo = new LopInfo(0, strMaLop);
                        if ((bool)windowLopInfo.ShowDialog())
                        {
                            //doing nothing
                        }
                    }
                    break;
                case 2:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        DataRowView row = (DataRowView)MainDataGrid.SelectedItem;
                        SuaGiaoVien _suagv = new SuaGiaoVien(row["Mã giáo viên"].ToString(), row["Họ tên"].ToString(), row["Giới tính"].ToString(), row["Khoa"].ToString(), row["CMND"].ToString(), row["Ngày vào làm"].ToString(), row["Hệ số"].ToString(), row["Mức lương"].ToString(), row["Địa chỉ"].ToString(), 0);
                        _suagv.ShowDialog();
                    }
                    break;
                case 3:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        string strMaMH = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();
                        MonHocInfo windowMonHocInfo = new MonHocInfo(0, strMaMH);
                        windowMonHocInfo.ShowDialog();
                    }
                    break;
                case 4:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(0, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                    }
                    break;
                case 5:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(0, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                    }
                    break;
                case 6:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(0, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                    }
                    break;

            }
        }

        private void buttonSUA_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaLop = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        LopInfo windowLopInfo = new LopInfo(-1, strMaLop);
                        windowLopInfo.ShowDialog();
                    }
                    break;
                case 2:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        DataRowView row = (DataRowView)MainDataGrid.SelectedItem;
                        SuaGiaoVien _suagv = new SuaGiaoVien(row["Mã giáo viên"].ToString(), row["Họ tên"].ToString(), row["Giới tính"].ToString(), row["Khoa"].ToString(), row["CMND"].ToString(), row["Ngày vào làm"].ToString(), row["Hệ số"].ToString(), row["Mức lương"].ToString(), row["Địa chỉ"].ToString(), 1);
                        _suagv.ShowDialog();
                    }
                    break;
                case 3:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        string strMaMH = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();
                        MonHocInfo windowMonHocInfo = new MonHocInfo(-1, strMaMH);
                        windowMonHocInfo.ShowDialog();
                    }
                    break;
                case 4:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(-1, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                        //if ((bool)windowHocSinhInfo.ShowDialog())
                        //    buttonREFRES_Click(sender, e);
                            //LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH");
                    }
                    break;
                case 5:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(-1, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                        //if ((bool)windowHocSinhInfo.ShowDialog())
                        //    buttonREFRES_Click(sender, e);
                        //LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH");
                    }
                    break;
                case 6:
                    if (MainDataGrid.SelectedIndex > -1)
                    {
                        //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                        string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                        HocSinhInfo windowHocSinhInfo = new HocSinhInfo(-1, strMaHS, comboBox1.SelectedItem.ToString());
                        windowHocSinhInfo.ShowDialog();
                        //if ((bool)windowHocSinhInfo.ShowDialog())
                        //    buttonREFRES_Click(sender, e);
                        //LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH");
                    }
                    break;
            }

            buttonREFRES_Click(sender, e);
        }

        #region Delete_Method

        //vì lớp ở bảng HOCSINH là khóa ngoại tham chiếu đến lớp của bảng LOP nên sẽ kg thể tùy tiện xóa 1 dữ liệu của lớp trong bảng LOP được
        //sẽ trả về true nếu kg có lớp đó trong bảng HOCSINH, false thì ngược lại
        //Hàm kiểm tra coi có Lop nào trong bảng HS kg
        private bool CheckLop(string strMaLop)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select count(*) from HOCSINH where MALOP = @MALOP";
                cmd.Parameters.AddWithValue("@MALOP", strMaLop);

                SqlDataReader drDataReader = cmd.ExecuteReader();

                while (drDataReader.Read())
                {
                    if (drDataReader.GetInt32(0) > 0)
                    {
                        MessageBox.Show(string.Concat("Mời bạn xóa hết hay thay đổi các dữ liệu học sinh đang học trong lớp " + strMaLop + " để có thể xóa dữ liệu của lớp"), "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                conn.Close();
            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong việc kết nối cơ sở dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        //Hàm Xóa 1 dữ liệu Lop được chọn trong datagrid  
        private void DeleteLop()
        {

            if (MainDataGrid.SelectedIndex > -1)
            {
                //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                string strMaLop = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                // kiểm tra coi có khóa ngoại nào trỏ đến lớp cần xóa kh
                if (!CheckLop(strMaLop))
                    return;

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from LOP where MALOP = @MALOP";
                    cmd.Parameters.AddWithValue("@MALOP", strMaLop);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    //hiện thông báo và sau khi bấm ok thì refesh lại dữ liệu để thể hiện là đã xóa
                    MessageBoxResult result = MessageBox.Show("Bạn đã xóa dữ liệu của lớp thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (result == MessageBoxResult.OK)
                    {
                        LoadData("SELECT MALOP AS 'Mã lớp', TENLOP AS 'Tên lớp', SISO AS 'Sĩ số', MAGVCN AS 'Mã GVCN', HOTEN AS 'Họ và tên GVCN' FROM LOP, GIAOVIEN WHERE LOP.MAGVCN = GIAOVIEN.MAGV ");
                    }
                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Xảy ra lỗi trong quá trình xóa dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
        }

        private void DeleteHocSinh()
        {
            if (MainDataGrid.SelectedIndex > -1)
            {
                //cách lấy thông tin của 1 cột bất kỳ khi chọn row đó
                string strMaHS = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from HOCSINH where MAHS = @MAHS and NAMHOC = @NAMHOC";
                    cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                    cmd.Parameters.AddWithValue("@NAMHOC", comboBox1.SelectedItem.ToString());
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    //hiện thông báo và sau khi bấm ok thì refesh lại dữ liệu để thể hiện là đã xóa
                    MessageBoxResult result = MessageBox.Show("Bạn đã xóa dữ liệu của lớp thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (result == MessageBoxResult.OK)
                    {
                        LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH");
                    }
                }
                catch (SqlException)
                {
                    conn.Close();
                    MessageBox.Show("Xảy ra lỗi trong quá trình xóa dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
        }

        void DeleteGiaoVien()
        {
            if (MainDataGrid.SelectedIndex > -1) // PN: danh cho xoa GIAOVIEN
            { // PN: maingridflag de tranh exception khi nguoi dung chua click vao row nao, ban Nguyen da thu va bi except =(( !!!
                //string _magv = MainDataGrid.SelectedValue.ToString();
                MessageBoxResult mes = MessageBox.Show("Bạn có muốn xóa giáo viên " + ((DataRowView)MainDataGrid.SelectedItem).Row[1].ToString(), "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mes == MessageBoxResult.Yes)
                {
                    string strMAGV = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();
                    conn.Open();
                    try
                    {

                        string query = ("DELETE from GIAOVIEN WHERE MAGV ='" + strMAGV + "'"); // thiet lap cau lenh xoa
                        SqlCommand com = new SqlCommand(query, conn);
                        com.CommandType = CommandType.Text;
                        com.ExecuteNonQuery();
                        MessageBox.Show("Đã xóa");

                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Có lỗi trong việc thực hiện xóa dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    conn.Close();
                }
            }
        }

        void DeleteMonHoc()
        {
            if (MainDataGrid.SelectedIndex < 0)
                return;
            string strMaMH = ((DataRowView)MainDataGrid.SelectedItem).Row[0].ToString();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM MONHOC WHERE MAMH = @MAMH", conn);
                cmd.Parameters.AddWithValue("@MAMH", strMaMH);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Bạn đã xóa môn học thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException)
            {
                MessageBox.Show("Bạn không xóa được môn học này vì môn học này được nhiều bảng khác làm tham chiếu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            conn.Close();
        }
        #endregion
        private void buttonXOA_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    DeleteLop();
                    break;
                case 2:
                    DeleteGiaoVien();
                    break;
                case 3:
                    DeleteMonHoc();
                    break;
                case 4:
                    DeleteHocSinh();
                    break;
                case 5:
                    DeleteHocSinh();
                    break;
                case 6:
                    DeleteHocSinh();
                    break;
            }

            buttonREFRES_Click(sender, e);
        }

        private void buttonREFRES_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    LoadData("SELECT MALOP AS 'Mã lớp', TENLOP AS 'Tên lớp', SISO AS 'Sĩ số', MAGVCN AS 'Mã GVCN', HOTEN AS 'Họ và tên GVCN' FROM LOP, GIAOVIEN WHERE LOP.MAGVCN = GIAOVIEN.MAGV ");
                    break;
                case 2:
                    LoadData("SELECT MAGV AS 'Mã giáo viên', HOTEN AS 'Họ tên', GIOITINH AS 'Giới tính', TENKHOA AS 'Khoa', CMND AS 'CMND', NGVL AS 'Ngày vào làm', DIACHI AS 'Địa chỉ',HESO as 'Hệ số', MUCLUONG as 'Mức lương' FROM GIAOVIEN, KHOA WHERE KHOA = MAKHOA");
                    break;
                case 3:
                    LoadData("SELECT MAMH AS 'Mã môn học', TENMH AS 'Tên môn học', TENKHOA AS 'Khoa quản lý', HESO1 AS 'Số điểm HS1', HESO2 AS 'Số điểm HS2' FROM MONHOC, KHOA WHERE MONHOC.KHOA = KHOA.MAKHOA");
                    break;
                case 4:
                    LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH WHERE LEFT(MALOP,2) = 10 AND NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'");
                    break;
                case 5:
                    LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH WHERE LEFT(MALOP,2) = 11 AND NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'");
                    break;
                case 6:
                    LoadData("SELECT MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' FROM HOCSINH WHERE LEFT(MALOP,2) = 12 AND NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'");
                    break;
            }

        }


        //hàm dùng để search khi gõ chữ vào textbox search
        private void SearchInfo(string strCommand)
        {
            try
            {
                SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(strCommand, strConnectionString);
                DataTable dtDataTable = new DataTable();
                dtDataTable.Clear();
                sdaDataAdapter.Fill(dtDataTable);
                MainDataGrid.ItemsSource = dtDataTable.DefaultView;

            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtboxTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    SearchInfo(string.Concat("select * from LOP where MALOP like '%" + txtSearch.Text + "%' or TENLOP like N'%" + txtSearch.Text + "%'"));
                    break;
                case 2:
                    SearchInfo(string.Concat("SELECT MAGV AS 'Mã giáo viên', HOTEN AS 'Họ tên', GIOITINH AS 'Giới tính', TENKHOA AS 'Khoa quản lý', CMND AS 'CMND', NGVL AS 'Ngày vào làm', DIACHI AS 'Địa chỉ',HESO as 'Hệ số', MUCLUONG as 'Mức lương' from GIAOVIEN, KHOA where (MAGV like '%" + txtSearch.Text + "%' or HOTEN like N'%" + txtSearch.Text + "%' ) AND KHOA = MAKHOA"));
                    break;
                case 3:
                    SearchInfo(string.Concat("SELECT MAMH AS 'Mã môn học', TENMH AS 'Tên môn học', HESO1 AS 'Số điểm HS1', HESO2 AS 'Số điểm HS2' from MONHOC where MAMH like '%" + txtSearch.Text + "%' or TENMH like N'%" + txtSearch.Text + "%'"));
                    break;
                case 4:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 10 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
                case 5:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 11 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
                case 6:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 12 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
            }
        }

        private void CapQuyenTruyCap_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow NewWindow = new AddAccountWindow();
            NewWindow.Show();
        }

        private void buttonIn_Click(object sender, RoutedEventArgs e)
        {
            switch (iBang)
            {
                case 1:
                    frmBaoCaoLop newWindow = new frmBaoCaoLop();
                    newWindow.ShowDialog();
                    break;
                case 2:
                    Report_GiaoVien NewWindow = new Report_GiaoVien();
                    NewWindow.ShowDialog();
                    break;
                case 3:
                    frmBaoCaoMonHoc newWindow1 = new frmBaoCaoMonHoc();
                    newWindow1.ShowDialog();
                    break;
                case 4:
                    frmHocSinhBaoCao newWindow2 = new frmHocSinhBaoCao(10, comboBox1.Text);
                    newWindow2.ShowDialog();
                    break;
                case 5:
                    frmHocSinhBaoCao newWindow3 = new frmHocSinhBaoCao(11, comboBox1.Text);
                    newWindow3.ShowDialog();
                    break;
                case 6:
                    frmHocSinhBaoCao newWindow4 = new frmHocSinhBaoCao(12, comboBox1.Text);
                    newWindow4.ShowDialog();
                    break;
            }
        }

        private void ThayDoiPass_Click(object sender, RoutedEventArgs e)
        {
            ChangePass _changepass = new ChangePass();
            _changepass.ShowDialog();
        }

        private void ThongTinTaiKhoan_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Concat("Xin chào bạn " + strTenDN + ". Bạn có quyền hạn số:" + strQuyen ),"Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DangXuat_Click(object sender, RoutedEventArgs e)
        {
            DangNhap DNWindow = new DangNhap();
            DNWindow.Show();
            this.Close();
        }

        private string strMaLop = "";
        private void buttonINlop_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedValue == null)
                return;
            string strName = treeView.SelectedValue.ToString();
            if (strName.Substring(2, 1) != "A")
                return;

            frmDanhSachLop newDanhSachLop = new frmDanhSachLop(strMaLop, ComboBoxa.Text);
            newDanhSachLop.ShowDialog();
        }

        private void txtSearch_TextInput(object sender, TextCompositionEventArgs e)
        {
              switch (iBang)
            {
                case 1:
                    SearchInfo(string.Concat("select * from LOP where MALOP like '%" + txtSearch.Text + "%' or TENLOP like N'%" + txtSearch.Text + "%'"));
                    break;
                case 2:
                    SearchInfo(string.Concat("select * from GIAOVIEN where MAGV like '%" + txtSearch.Text + "%' or HOTEN like N'%" + txtSearch.Text + "%'"));
                    break;
                case 3:
                    SearchInfo(string.Concat("select * from MONHOC where MAMH like '%" + txtSearch.Text + "%' or TENMH like N'%" + txtSearch.Text + "%'"));
                    break;
                case 4:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 10 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
                case 5:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 11 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
                case 6:
                    SearchInfo(string.Concat("select MAHS AS 'Mã học sinh', HOTEN AS 'Họ tên', NGSINH AS 'Ngày sinh', GIOITINH AS 'Giới tính', CMND AS 'CMND', NOISINH AS 'Nơi sinh', DIACHI AS 'Địa chỉ',MALOP as 'Mã lớp' from HOCSINH where ( HOTEN like N'%" + txtSearch.Text + "%' or DIACHI like N'%" + txtSearch.Text + "%' ) and left(MALOP,2) = 12 and NAMHOC = '" + comboBox1.SelectedItem.ToString() + "'"));
                    break;
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            txtSearch.GotFocus -= txtSearch_GotFocus;

            txtSearch1.Text = "";
            txtSearch1.GotFocus -= txtSearch_GotFocus;
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "Tìm kiếm";
            txtSearch.GotFocus += txtSearch_GotFocus;

            txtSearch1.Text = "Tìm kiếm";
            txtSearch1.GotFocus += txtSearch_GotFocus;
        }

        void NewLoadData (string str)
        {
            try
            {
                SqlDataAdapter sdaDataAdapter = new SqlDataAdapter(str, strConnectionString);
                DataTable dtDataTable = new DataTable();
                dtDataTable.Clear();
                sdaDataAdapter.Fill(dtDataTable);
                MainDataGrid1.ItemsSource = dtDataTable.DefaultView;

            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong việc kết nối SQL server", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (treeView.SelectedValue == null)
                return;
            string strName = treeView.SelectedValue.ToString();
            if (strName.Substring(2, 1) != "A")
                return;
            strMaLop = strName;
            string str = String.Concat("SELECT HOCSINH.MAHS AS 'Mã học sinh', HOTEN AS 'Tên học sinh', NGSINH AS 'Ngày sinh', HOCKY AS 'Học kỳ', DTB_HOCKY AS 'Điểm trung bình học kỳ', XEPLOAI AS 'Xếp loại' FROM TONGKET, HOCSINH WHERE TONGKET.MAHS = HOCSINH.MAHS AND MALOP = '" + treeView.SelectedValue.ToString() + "' AND HOCSINH.NAMHOC = '" + ComboBoxa.Text + "'");

            NewLoadData(str);
        }

        private void ComboBoxa_DropDownClosed(object sender, EventArgs e)
        {
            string str = String.Concat("SELECT HOCSINH.MAHS AS 'Mã học sinh', HOTEN AS 'Tên học sinh', NGSINH AS 'Ngày sinh', HOCKY AS 'Học kỳ', DTB_HOCKY AS 'Điểm trung bình học kỳ', XEPLOAI AS 'Xếp loại' FROM TONGKET, HOCSINH WHERE TONGKET.MAHS = HOCSINH.MAHS AND MALOP = '" + treeView.SelectedValue.ToString() + "' AND HOCSINH.NAMHOC = '" + ComboBoxa.Text + "'");


            NewLoadData(str);
        }

        private void ThongTinTruong_Click(object sender, RoutedEventArgs e)
        {
            TruongInfo newWindow = new TruongInfo();
            newWindow.ShowDialog();
        }

        private void TaoNamHocMoi_Click(object sender, RoutedEventArgs e)
        {
            NamHocInfo newWindow = new NamHocInfo();
            newWindow.ShowDialog();
        }

        private void QuanLyKhoa_Click(object sender, RoutedEventArgs e)
        {
            KhoaInfo newWindow = new KhoaInfo();
            newWindow.ShowDialog();
        }

        private void QuyDinh_Click(object sender, RoutedEventArgs e)
        {
            QuyDinh newWindow = new QuyDinh();
            newWindow.ShowDialog();
        }

        private void NhapDiemMoi_Click(object sender, RoutedEventArgs e)
        {
            NhapDiem newNhapDiem = new NhapDiem();
            newNhapDiem.ShowDialog();
        }

        private void SuaDiem_Click(object sender, RoutedEventArgs e)
        {
            SuaDiem newSuaDiem = new SuaDiem();
            newSuaDiem.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonINq_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid1.SelectedIndex >= 0)
            {
                frmBangDiemHocSinh newBangDiem = new frmBangDiemHocSinh((DataRowView)MainDataGrid1.SelectedItem, ComboBoxa.Text);
                newBangDiem.ShowDialog();
            }

        }

        private void MainDataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
