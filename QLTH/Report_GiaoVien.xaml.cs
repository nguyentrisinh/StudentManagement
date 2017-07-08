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
    /// Interaction logic for Report_GiaoVien.xaml
    /// </summary>
    public partial class Report_GiaoVien : Window
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        string strName, strHieuTruong;

        public Report_GiaoVien()
        {
            InitializeComponent();
            this.ResizeMode = System.Windows.ResizeMode.CanMinimize;
        }

        private void btnShowReport_Click(object sender, RoutedEventArgs e)
        {
            ReportViewerDemo.Reset();
            DataTable dt = GetData();

            ReportDataSource dsDataSource = new ReportDataSource("DataSet1", dt); //ReportDataSource("", dt);
            ReportViewerDemo.LocalReport.DataSources.Add(dsDataSource);

            //embbedded to RDLC report file
            ReportViewerDemo.LocalReport.ReportEmbeddedResource = "QLTH.GiaoVien.rdlc";
            //ReportViewerDemo.LocalReport.ReportPath = "\\RDLC_Report\\GiaoVien.rdlc";

            //Get information about School such as school's name and principal's name
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select * from THONGTINTRG";
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    strName = dr.GetString(1);
                    strHieuTruong = dr.GetString(3);
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
                new ReportParameter ("FromDate",dpFromDate.Text),
                new ReportParameter("ToDate", dpToDate.Text),
                new ReportParameter("TenTruong", strName.ToUpper()),
                new ReportParameter("TenHT", strHieuTruong),
            };
            ReportViewerDemo.LocalReport.SetParameters(rptParameter);
            

            ReportViewerDemo.RefreshReport();
        }

        private DataTable GetData()
        {
            DateTime FromDate = DateTime.Parse(dpFromDate.Text) ;
            DateTime ToDate = DateTime.Parse(dpToDate.Text);

            DataTable dtDataTable = new DataTable();

            try
            {

                string strCommand = string.Concat("Select MAGV, HOTEN, GIOITINH, KHOA.TENKHOA as 'KHOA', CMND, NGVL, DIACHI, HESO, MUCLUONG from GIAOVIEN, KHOA where NGVL >= '" + dpFromDate.Text + "' and NGVL <= '" + dpToDate.Text + "' and GIAOVIEN.KHOA = KHOA.MAKHOA ");

                SqlDataAdapter daDataAdapter = new SqlDataAdapter(strCommand,strConnectionString);
                //daDataAdapter = cmd.ExecuteNonQuery();
                daDataAdapter.Fill(dtDataTable);
            }
            catch(SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình tạo báo cáo, mời bạn thử lại", "Thông báo", MessageBoxButton.OK);
            }

            //conn.Close();
            return dtDataTable;

        }

        private SqlDataReader GetInfo()
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "Select * from THONGTINTRG";
                SqlDataReader dr = cmd.ExecuteReader();
                conn.Close();
                return dr;
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình liên kết cơ sở dữ liệu");
            }
            conn.Close();

            return null;
        }
    }
}
