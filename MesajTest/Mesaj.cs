using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesajTest
{
    public partial class Mesaj : Form
    {
        public Mesaj()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbMesaj;Integrated Security=True");
        public string numara;

        void GelenKutusu()
        {
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT CONCAT (AD, ' ', SOYAD) as 'Gönderen', BASLIK as 'Başlık',MESAJ as 'Mesaj' FROM TBLMESAJLAR inner join TBLKISILER ON TBLMESAJLAR.GONDEREN = TBLKISILER.NUMARA WHERE ALICI = " + numara + "ORDER BY MESAJID ASC", baglanti);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            dataGridView1.DataSource = dt1;
        }

        void GidenKutusu()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("SELECT CONCAT(AD, ' ', SOYAD) as 'Alıcı', BASLIK as 'Başlık',MESAJ as 'Mesaj' FROM TBLMESAJLAR inner join TBLKISILER ON TBLMESAJLAR.ALICI = TBLKISILER.NUMARA WHERE GONDEREN = " + numara + "ORDER BY MESAJID ASC", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView2.DataSource = dt2;
        }

        private void Mesaj_Load(object sender, EventArgs e)
        {
            LblNumara.Text = numara;
            GelenKutusu();
            GidenKutusu();
            // Ad soydadı çekme 
            baglanti.Open();
            SqlCommand komut = new SqlCommand("SELECT AD,SOYAD FROM TBLKISILER WHERE NUMARA=" + numara, baglanti);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0] + " " + dr[1];
            }
            baglanti.Close();
        }

        string Alıcı()
        {
            string alici = TxtAlıcı.Text;
            string sorgu = "SELECT NUMARA FROM TBLKISILER WHERE AD + ' ' + SOYAD = @P1";
            baglanti.Open();
            SqlCommand komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@P1", alici);
            object sonuc = komut.ExecuteScalar();
            baglanti.Close();
            return sonuc?.ToString(); // null değilse sonucu döndür, null ise null döndür
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string aliciNumara = Alıcı();
            if (string.IsNullOrEmpty(aliciNumara)) // null veya boş ise
            {
                MessageBox.Show("Alıcı bulunamadı. Lütfen geçerli bir isim giriniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLMESAJLAR (GONDEREN,ALICI,BASLIK,MESAJ) VALUES(@P1,@P2,@P3,@P4)", baglanti);
            komut.Parameters.AddWithValue("@P1", numara);
            komut.Parameters.AddWithValue("@P2", aliciNumara);
            komut.Parameters.AddWithValue("@P3", TxtBaslık.Text);
            komut.Parameters.AddWithValue("@P4", RchTxtMesaj.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Mesajınız Gönderildi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GidenKutusu();
        }

        private void BtnTemizle_Click(object sender, EventArgs e)
        {
            TxtAlıcı.Text = string.Empty;
            TxtBaslık.Text = string.Empty;
            RchTxtMesaj.Text = string.Empty;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GelenKutusu();
            GidenKutusu();
            MessageBox.Show("Mesajlarınız Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
