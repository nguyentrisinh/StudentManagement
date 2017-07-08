using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace QLTH
{
    class AverageGrade
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        int iLoai;
        float[] fDiem = new float[20];
        int iCount = 0;

        public string strMaHS, strNamHoc, strHocKy;

        public AverageGrade(string MAHS, string NAMHOC, string HOCKY)
        {
            strMaHS = MAHS;
            strNamHoc = NAMHOC;
            strHocKy = HOCKY;

        }


        //this method use to check if student's ID has already average grade 
        //if the answer is no we will use insert command to add the average grade into database
        //if the answer is yes, we will use update command to update average grade in database
        private bool CheckDiem()
        {

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from TONGKET where MAHS = @MAHS and NAMHOC = @NAMHOC and HOCKY = @HOCKY", conn);
                cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                cmd.Parameters.AddWithValue("@HOCKY", strHocKy);

                iLoai = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (SqlException)
            {

            }
            conn.Close();

            if (iLoai == 0)
                return false;
            return true;
        }

        private float GetSum()
        {
            iCount = 0;
            float fDiemTong = 0;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select DTB from KETQUAMON where MAHS = @MAHS and NAMHOC = @NAMHOC and HOCKY = @HOCKY";
                cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                cmd.Parameters.AddWithValue("@HOCKY", strHocKy);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    fDiem[iCount] = (float)dr.GetDouble(0);
                    iCount++;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Có lỗi trong quá trình lấy điểm từng môn của học sinh", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            conn.Close();

            for (int i = 0; i < iCount; i++)
                fDiemTong += fDiem[i];
            return fDiemTong;
        }

        public void UpdateDTB()
        {
            float fDTB = (float)Math.Round(GetSum() / iCount, 2);
            string strLoai;

            if (fDTB >= 8)
                strLoai = "Giỏi";
            else if (fDTB < 8 && fDTB >= 6.5)
                strLoai = "Tiên tiến";
            else if (fDTB < 6.5 && fDTB >= 5)
                strLoai = "Trung bình";
            else if (fDTB < 5 && fDTB >= 3)
                strLoai = "Yếu";
            else
                strLoai = "Kém";

            if (!CheckDiem())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "insert into TONGKET values (@MAHS,@NAMHOC,@HOCKY,@DTB,@XEPLOAI)";
                    cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                    cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                    cmd.Parameters.AddWithValue("@HOCKY", strHocKy);
                    cmd.Parameters.AddWithValue("@DTB", fDTB.ToString());
                    cmd.Parameters.AddWithValue("@XEPLOAI", strLoai);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    MessageBox.Show("Có lỗi trong quá trình lưu điểm trung bình vào cơ sở dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    cmd.CommandText = "update TONGKET set DTB_HOCKY = @DTB, XEPLOAI = @XEPLOAI where MAHS = @MAHS and NAMHOC = @NAMHOC and HOCKY = @HOCKY";
                    cmd.Parameters.AddWithValue("@MAHS", strMaHS);
                    cmd.Parameters.AddWithValue("@NAMHOC", strNamHoc);
                    cmd.Parameters.AddWithValue("@HOCKY", strHocKy);
                    cmd.Parameters.AddWithValue("@DTB", fDTB.ToString());
                    cmd.Parameters.AddWithValue("@XEPLOAI", strLoai);
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    MessageBox.Show("Có lỗi trong quá trình cập nhật điểm trung bình vào cơ sở dữ liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                conn.Close();
            }


        }


    }
}
