using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MesajTest
{
    public partial class Kayıt : Form
    {
        public Kayıt()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbMesaj;Integrated Security=True");

        private void BtnKayıt_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string yeniNumara;

            // Numara daha önceden kullanılmışsa yeni bir numara oluşturulur
            do
            {
                int rastgeleSayi = rnd.Next(1000, 9999);
                yeniNumara = rastgeleSayi.ToString();
            }
            while (IsNumberUsed(yeniNumara, baglanti));

            // Numara daha önceden kullanılmış mı kontrol edilir
            bool IsNumberUsed(string numara, SqlConnection baglanti)
            {
                using (SqlCommand komut2 = new SqlCommand("SELECT COUNT(*) FROM TBLKISILER WHERE NUMARA = @P1", baglanti)) 
                {
                    baglanti.Open();
                    komut2.Parameters.AddWithValue("@P1", numara);
                    int count = (int)komut2.ExecuteScalar(); 
                    baglanti.Close();
                    return count > 0; // Numara daha önceden kullanılmışsa true döner, kullanılmamışsa false döner
                }
            }

            // Yeni numarayı kullanarak kayıt işlemini gerçekleştirin
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLKISILER (AD, SOYAD, NUMARA, SIFRE) VALUES (@P1, @P2, @P3, @P4)", baglanti);
            komut.Parameters.AddWithValue("@P1", TxtAd.Text);
            komut.Parameters.AddWithValue("@P2", TxtSoyad.Text);
            komut.Parameters.AddWithValue("@P3", yeniNumara);
            komut.Parameters.AddWithValue("@P4", TxtSifre.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show($"Kayıt İşlemi Başarılı \nNumaranız: {yeniNumara}","Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}
