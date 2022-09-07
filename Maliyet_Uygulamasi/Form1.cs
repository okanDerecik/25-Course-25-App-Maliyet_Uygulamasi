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

namespace Maliyet_Uygulamasi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=DESKTOP-K0C08G8;Initial Catalog=Maliyet;Integrated Security=True");

        void malzemeListe()
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * from Malzemeler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        void urunListesi()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("select * from urunler", baglanti);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView1.DataSource = dt2;
        }

        void kasa()
        {
            SqlDataAdapter da3 = new SqlDataAdapter("Select * from kasa", baglanti);
            DataTable dt3 = new DataTable();
            da3.Fill(dt3);
            dataGridView1.DataSource = dt3;
        }

        void urunler()
        {
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("select * from urunler", baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            CmbUrun.ValueMember = "URUNID";
            CmbUrun.DisplayMember = "AD";
            CmbUrun.DataSource = dt;

            baglanti.Close();
        }

        void malzemeler()
        {
            baglanti.Open();
            SqlDataAdapter da = new SqlDataAdapter("select * from malzemeler",baglanti);
            DataTable dt = new DataTable();
            da.Fill(dt);
            CmbMalzeme.ValueMember = "MALZEMEID";
            CmbMalzeme.DisplayMember = "AD";
            CmbMalzeme.DataSource = dt;
            baglanti.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            malzemeListe();
            urunler();
            malzemeler();
        }

        private void BtnUrunListesi_Click(object sender, EventArgs e)
        {
            urunListesi();
        }

        private void BtnMalzemeListesi_Click(object sender, EventArgs e)
        {
            malzemeListe();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            kasa();
        }

        private void BtnCikis_Click(object sender, EventArgs e)
        {

        }

        private void BtnMalzemeEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into malzemeler (ad,stok,fıyat,notlar) values (@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtMalzemeAd.Text);
            komut.Parameters.AddWithValue("@p2", decimal.Parse(TxtMalzemeStok.Text));
            komut.Parameters.AddWithValue("@p3", decimal.Parse(TxtMalzemeFiyat.Text));
            komut.Parameters.AddWithValue("@p4", TxtMalzemeNot.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Malzeme Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            malzemeListe();
        }

        private void BtnUrunEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("Insert into URUNLER (AD) values (@p1)",baglanti);
            komut.Parameters.AddWithValue("@p1",txtUrunAd.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Ürün Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            urunListesi();
        }

        private void BtnUrunOlustur_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into fırın (urunıd,malzemeıd,mıktar,malıyet) values (@p1,@p2,@p3,@p4)", baglanti);
            komut.Parameters.AddWithValue("@p1",CmbUrun.SelectedValue);
            komut.Parameters.AddWithValue("@p2", CmbMalzeme.SelectedValue);
            komut.Parameters.AddWithValue("@p3", decimal.Parse(txtMiktar.Text));
            komut.Parameters.AddWithValue("@p4", decimal.Parse(TxtMaliyet.Text));
            komut.ExecuteNonQuery();
            baglanti.Close();
            MessageBox.Show("Malzeme Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

            listBox1.Items.Add(CmbMalzeme.Text + " - " + TxtMaliyet.Text);
        }

        private void txtMiktar_TextChanged(object sender, EventArgs e)
        {
            double maliyet;
            if(txtMiktar.Text == "")
            {
                txtMiktar.Text = "0";
            }

            baglanti.Open();
            SqlCommand komut = new SqlCommand("select * from malzemeler where Malzemeıd=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", CmbMalzeme.SelectedValue);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                TxtMaliyet.Text = dr[3].ToString();
            }
            baglanti.Close();

            maliyet = Convert.ToDouble(TxtMaliyet.Text) / 1000 * Convert.ToDouble(txtMiktar.Text);
            TxtMaliyet.Text = maliyet.ToString();

            if (CmbMalzeme.Text == "YUMURTA")
            {
                label11.Visible = true;
                label11.Text = "Adet";
                maliyet = Convert.ToDouble(TxtMaliyet.Text) * Convert.ToDouble(txtMiktar.Text);
                txtMiktar.Text = maliyet.ToString();
            }
            else
            {
                label11.Visible = true;
                label11.Text = "Gram";
                maliyet = Convert.ToDouble(TxtMaliyet.Text) / 1000 * Convert.ToDouble(txtMiktar.Text);
                TxtMaliyet.Text = maliyet.ToString();
            }

        }

        

        private void CmbMalzeme_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;

            TxtUrunId.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            txtUrunAd.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();

            baglanti.Open();
            SqlCommand komut = new SqlCommand("select sum(malıyet) from fırın where urunıd=@p1", baglanti);
            komut.Parameters.AddWithValue("@p1", TxtUrunId.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                TxtUrunMFıyat.Text = dr[0].ToString();
            }
            baglanti.Close();
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("update urunler set MFIYAT=@p1,SFIYAT=@p2,STOK=@p3 where URUNID=@p4",baglanti);
            komut.Parameters.AddWithValue("@p1", decimal.Parse(TxtUrunMFıyat.Text));
            komut.Parameters.AddWithValue("@p2", decimal.Parse(TxtUrunSFıyat.Text));
            komut.Parameters.AddWithValue("@p3", TxtUrunStok.Text);
            komut.Parameters.AddWithValue("@p4", TxtUrunId.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            
            baglanti.Open();
            SqlCommand komut2 = new SqlCommand("update kasa set gırıs=gırıs+@s1 ,cıkıs=cıkıs+@s2,kar=@s3 ", baglanti);
            komut2.Parameters.AddWithValue("@s1", decimal.Parse(TxtUrunSFıyat.Text));
            komut2.Parameters.AddWithValue("@s2", decimal.Parse(TxtUrunMFıyat.Text));
            komut2.Parameters.AddWithValue("@s3", decimal.Parse(TxtUrunSFıyat.Text) - decimal.Parse(TxtUrunMFıyat.Text));
            komut2.ExecuteNonQuery();
            baglanti.Close(); MessageBox.Show("Ürün Güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
