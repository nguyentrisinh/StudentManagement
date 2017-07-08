using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Windows;
using System.Data;

namespace QLTH
{
    
    class Password
    {
        public static string strConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\QLHS.mdf;Integrated Security=True";
        SqlConnection conn = new SqlConnection(strConnectionString);
        //key to encrypt and decrypt the password
        private const int keysize = 256;

        //Elements of each account
        public string strTenDN {get; set; }
        public string strMaGV { get; set; }
        public string strPass { get; set; }
        public int iQuyenHan { get; set; }
        public int iSTT { get; set; }
        public string strNewPass { get; set; }

        public Password()
        { }

        public Password (string TenDN, string MaGV, string Pass, int i)
        {
            strTenDN = TenDN;
            strMaGV = MaGV;
            strPass = Pass;
            iQuyenHan = i;
        }

        public Password(string TenDN)
        {
            strTenDN = TenDN;
        }
        public Password (string TenDN, string Pass)
        {
            strTenDN = TenDN;
            strPass = Pass;
        }

        public Password(string TenDN, string Pass, string NewPass)
        {
            strTenDN = TenDN;
            strPass = Pass;
            strNewPass = NewPass;
        }

        //Method to encrypt password text
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "silyn81996";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "silyn81996";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public bool AddUsers ()
        {

            try
            {
                conn.Open();
                SqlCommand cmd1 = new SqlCommand();
                cmd1.Connection = conn;
                cmd1.CommandText = "select count(*) from TAIKHOAN";

                //find the biggest STT x to add STT (x+1) to the following account
                SqlDataReader drDataReader1 = cmd1.ExecuteReader();
                while (drDataReader1.Read())
                {
                    iSTT = drDataReader1.GetInt32(0);
                }
                iSTT++;

                drDataReader1.Close();
                //find if there is an account has the same "account name" of new account 
                cmd1.CommandText = "select * from TAIKHOAN where TENDN = @TENDN";
                cmd1.Parameters.AddWithValue("@TENDN", strTenDN);
                drDataReader1 = cmd1.ExecuteReader(); 
                if (drDataReader1.HasRows)
                {
                    MessageBox.Show("Tên đăng nhập này đã được sử dụng cho 1 tài khoản khác, mời bạn thay đổi tên đăng nhập", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    conn.Close();
                    return false;
                }

                drDataReader1.Close();
                //insert new account to database
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = conn;
                cmd2.CommandText = "insert into TAIKHOAN values (@STT,@TENDN,@MAGV,@PASS,@PASSENCRYPT,@QUYENSUDUNG)";
                cmd2.Parameters.AddWithValue("@STT", iSTT.ToString());
                cmd2.Parameters.AddWithValue("@TENDN", strTenDN);
                cmd2.Parameters.AddWithValue("@MAGV", strMaGV);
                cmd2.Parameters.AddWithValue("@PASS", strPass);
                cmd2.Parameters.AddWithValue("@PASSENCRYPT", Encrypt(strPass));
                cmd2.Parameters.AddWithValue("@QUYENSUDUNG", iQuyenHan.ToString());

                cmd2.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình tạo tài khoản mới");
                return false;
            }

            MessageBox.Show("Bạn đã tạo tài khoản mới thành công", "Thông báo", MessageBoxButton.OK);
            return true;
        }


        public string DangNhap(string _tenDN, string _matKhau)
        {
            try
            {
                Password x = new Password(_tenDN, _matKhau);
                conn.Open();
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = "select * from TAIKHOAN where TENDN = @TENDN";
                cm.Parameters.AddWithValue("@TENDN", x.strTenDN);
                SqlDataReader drDataReader1 = cm.ExecuteReader(); 
                if (drDataReader1.HasRows)
                {
                    drDataReader1.Close();
                    cm.CommandText = "SELECT PASSENCRYPT FROM TAIKHOAN WHERE TENDN = @_TENDN";
                    cm.Parameters.AddWithValue("@_TENDN", x.strTenDN);
                    string passEncrypt = cm.ExecuteScalar().ToString();

                    if (String.Compare(Encrypt(x.strPass), passEncrypt) == 0) // Login successfull
                    {
                        cm.CommandText = "SELECT QUYENSUDUNG FROM TAIKHOAN WHERE TENDN = @__TENDN";
                        cm.Parameters.AddWithValue("@__TENDN", x.strTenDN);
                        x.iQuyenHan = Convert.ToInt32(cm.ExecuteScalar().ToString());

                        conn.Close();
                        MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButton.OK);
                        return string.Concat(x.strTenDN + "-" + x.iQuyenHan.ToString());
                    }
                    MessageBox.Show("Mật khẩu sai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return null;
                
                }

                else
                {
                    if (MessageBox.Show("Tên đăng nhập không tồn tại, bạn có muốn tạo mới ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        AddAccountWindow addNew = new AddAccountWindow();
                        addNew.ShowDialog();
                        //Application.Current.Windows[1].Close();
                    }
                    
                    return null;
                }
            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }



        }

        public Password ThayDoiPass(string TenDN, string Pass, string NewPass)
        {
            try
            {
                Password x = new Password(TenDN, Pass, NewPass);
                conn.Open();
                SqlCommand cm = new SqlCommand();
                cm.Connection = conn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = "select * from TAIKHOAN where TENDN = @TENDN";
                cm.Parameters.AddWithValue("@TENDN", x.strTenDN);
                SqlDataReader drDataReader1 = cm.ExecuteReader();
                if (drDataReader1.HasRows)
                {
                    drDataReader1.Close();
                    cm.CommandText = "SELECT PASSENCRYPT FROM TAIKHOAN WHERE TENDN = @_TENDN";
                    cm.Parameters.AddWithValue("@_TENDN", x.strTenDN);
                    string passEncrypt = cm.ExecuteScalar().ToString();

                    if (String.Compare(Encrypt(x.strPass), passEncrypt) == 0) // Login successfull
                    {
                        cm.CommandText = "SELECT QUYENSUDUNG FROM TAIKHOAN WHERE TENDN = @__TENDN";
                        cm.Parameters.AddWithValue("@__TENDN", x.strTenDN);
                        x.iQuyenHan = Convert.ToInt32(cm.ExecuteScalar().ToString());
                        cm.CommandText = "UPDATE TAIKHOAN SET PASS = @NEWPASS, PASSENCRYPT = @NEWPASSENCRYPT WHERE TENDN = @TTENDN";
                        cm.Parameters.AddWithValue("@NEWPASS", x.strNewPass);
                        cm.Parameters.AddWithValue("@NEWPASSENCRYPT", Encrypt(x.strNewPass));
                        cm.Parameters.AddWithValue("@TTENDN", x.strTenDN);
                        cm.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Thay đổi mật khẩu thành công", "Thông báo", MessageBoxButton.OK);
                        return x;
                    }

                    MessageBox.Show("Mật khẩu sai", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;

                }

                else
                {
                    MessageBox.Show("Tên đăng nhập không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
            }
            catch (SqlException)
            {
                conn.Close();
                MessageBox.Show("Có lỗi trong quá trình kết nối SQL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }



    }
}
