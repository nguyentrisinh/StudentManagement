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
using Microsoft.Reporting.WinForms;

namespace QLTH
{
    /// <summary>
    /// Interaction logic for frmBangDiemHocSinh.xaml
    /// </summary>
    public partial class frmBangDiemHocSinh : Window
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        DataRowView drvData;
        string strNamHoc;
        private string strHoTen, strLop, strDTB, strXepLoai;
        public frmBangDiemHocSinh()
        {
            InitializeComponent();
        }

        public frmBangDiemHocSinh(DataRowView drv,string NamHoc)
        {
            InitializeComponent();
            drvData = drv;
            strNamHoc = NamHoc;
        }

        private DataTable GetData()
        {
            DataTable dtDataTable = new DataTable();

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TENMH AS 'MAMH', H1D1,H1D2,H1D3,H1D4,H1D5,H2D1,H2D2,H2D3,H2D4,H2D5,THI,DTB FROM KETQUAMON, MONHOC WHERE MONHOC.MAMH = KETQUAMON.MAMH AND NAMHOC = @NAMHOC AND HOCKY = @HOCKY AND KETQUAMON.MAHS = @MAHS", conn);
                cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                cmd.Parameters.AddWithValue("@HOCKY", drvData.Row[3].ToString());
                cmd.Parameters.AddWithValue("@MAHS", drvData.Row[0].ToString());
                dtDataTable.Load(cmd.ExecuteReader());
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình tạo báo cáo, mời bạn thử lại", "Thông báo", MessageBoxButton.OK);
            }

            conn.Close();
            return dtDataTable;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReportViewerDemo.Reset();
            DataTable dt = GetData();

            ReportDataSource dsDataSource = new ReportDataSource("DataSet1", dt); //ReportDataSource("", dt);
            ReportViewerDemo.LocalReport.DataSources.Add(dsDataSource);

            //embbedded to RDLC report file
            ReportViewerDemo.LocalReport.ReportEmbeddedResource = "QLTH.BangDiem.rdlc";
            //ReportViewerDemo.LocalReport.ReportPath = "\\RDLC_Report\\GiaoVien.rdlc";


            //Get information about School such as school's name and principal's name
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT HOTEN, MALOP, DTB_HOCKY, XEPLOAI FROM HOCSINH, TONGKET WHERE HOCSINH.MAHS = TONGKET.MAHS AND HOCSINH.NAMHOC = @NAMHOC AND HOCKY = @HOCKY AND TONGKET.MAHS = @MAHS";
                cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                cmd.Parameters.AddWithValue("@HOCKY", drvData.Row[3].ToString());
                cmd.Parameters.AddWithValue("@MAHS", drvData.Row[0].ToString());
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    strHoTen = dr.GetString(0);
                    strLop = dr.GetString(1);
                    strDTB = dr.GetDouble(2).ToString();
                    strXepLoai = dr.GetString(3);
                }

            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình liên kết cơ sở dữ liệu");
            }

            conn.Close();




            //Parameters
            ReportParameter[] rptParameter = new ReportParameter[]
            {
                new ReportParameter("HoTenHS", strHoTen),
                new ReportParameter("Lop", strLop),
                new ReportParameter("NamHoc", strNamHoc),
                new ReportParameter("HocKy", drvData.Row[3].ToString()),
                new ReportParameter("DTB", strDTB),
                new ReportParameter("XepLoai", strXepLoai)
            };
            ReportViewerDemo.LocalReport.SetParameters(rptParameter);


            ReportViewerDemo.RefreshReport();

        }
    }
}
