using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT MESAJID,CONCAT (AD, ' ', SOYAD) as 'Gönderen', BASLIK,MESAJ FROM TBLMESAJLAR inner join TBLKISILER ON TBLMESAJLAR.GONDEREN = TBLKISILER.NUMARA WHERE ALICI = " + numara, baglanti);
            DataTable dt1 = new DataTable();        
            da1.Fill(dt1);
            dataGridView1.DataSource = dt1;
        }

        void GidenKutusu()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("SELECT MESAJID,CONCAT(AD, ' ', SOYAD) as 'Alıcı', BASLIK,MESAJ FROM TBLMESAJLAR\r\ninner join TBLKISILER\r\nON TBLMESAJLAR.ALICI = TBLKISILER.NUMARA WHERE GONDEREN = " + numara, baglanti);
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
            SqlCommand komut = new SqlCommand("SELECT AD,SOYAD FROM TBLKISILER WHERE NUMARA=" + numara,baglanti);
            SqlDataReader dr = komut.ExecuteReader();   
            while (dr.Read())
            {
                LblAdSoyad.Text = dr[0] + " " + dr[1];
            }   
            baglanti.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("INSERT INTO TBLMESAJLAR (GONDEREN,ALICI,BASLIK,MESAJ) VALUES(@P1,@P2,@P3,@P4)",baglanti);
            komut.Parameters.AddWithValue("@P1", numara);
            komut.Parameters.AddWithValue("@P2", MskAlıcı.Text);
            komut.Parameters.AddWithValue("@P3", TxtBaslık.Text);   
            komut.Parameters.AddWithValue("@P4", RchTxtMesaj.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Mesajınız Gönderildi","Bilgi", MessageBoxButtons.OK,MessageBoxIcon.Information);
            GidenKutusu();
        }
    }
}
