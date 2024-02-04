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

namespace Görsel_Ödev
{
    public partial class Form1 : Form
    {
        SqlConnection baglanti;
        SqlCommand komut;
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        void Listele()
        {
            baglanti = new SqlConnection("Data Source=BARAN;Initial Catalog=bankaa;Integrated Security=True;");
            baglanti.Open();
            da = new SqlDataAdapter("select * from hesap order by hno", baglanti);
            DataTable tablo = new DataTable();
            da.Fill(tablo);
            dataGridView1.DataSource = tablo;
            ds.Clear();
            da.Fill(ds, "hesap");
            baglanti.Close();

        }
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Giriş yapana kadar diğer hizmetlere erişemezsiniz.");
            panel4.Visible = false;
            panel2.Visible = false;
            button1.Enabled = false;
            btnkredi.Enabled = false;
            Listele();
            textBox3.MaxLength = 11;
            textBox4.MaxLength = 6;
        }

        private void btnkayit_Click(object sender, EventArgs e)
        {
            string ad = textBox1.Text;
            string soyad = textBox2.Text;
            string tc = textBox3.Text;
            string parola = textBox4.Text;

            if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) || string.IsNullOrWhiteSpace(tc) || string.IsNullOrWhiteSpace(parola))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
            }
            else
            {

                string sorgu = "Insert into hesap (ad,soyad,tc,parola,bakiye,kredi,ktaksit) values (@ad, @soyad,@tc, @parola,@bakiye,@kredi,@ktaksit)";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@ad", textBox1.Text);
                komut.Parameters.AddWithValue("@soyad", textBox2.Text);
                komut.Parameters.AddWithValue("@tc", textBox3.Text);
                komut.Parameters.AddWithValue("@parola", textBox4.Text);
                komut.Parameters.AddWithValue("@bakiye", "0");
                komut.Parameters.AddWithValue("@kredi", "0");
                komut.Parameters.AddWithValue("@ktaksit", "Kredi Çekilmedi");
                baglanti.Open();
                komut.ExecuteNonQuery();
                MessageBox.Show("Yeni Kullanıcı Başarıyla Eklendi");
                string maxHesapNoSorgu = "SELECT MAX(hno) FROM hesap";
                using (SqlCommand maxHesapNoKomut = new SqlCommand(maxHesapNoSorgu, baglanti))
                {
                    object maxHesapNo = maxHesapNoKomut.ExecuteScalar();
                    MessageBox.Show("Kullanıcı Hesap Numaranız: " + maxHesapNo.ToString());
                }
                    

                Listele();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
        }
        private void textBox4_Enter(object sender, EventArgs e)
        {
        }

        private void btngiris_Click(object sender, EventArgs e)
        {


            if (!string.IsNullOrWhiteSpace(textBox16.Text))
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("SELECT * FROM hesap WHERE hno = @hno", baglanti);
                komut.Parameters.AddWithValue("@hno", textBox16.Text);

                using (SqlDataReader read = komut.ExecuteReader())
                {
                    if (read.Read())
                    {
                        textBox1.Text = read["ad"].ToString();
                        textBox2.Text = read[2].ToString();
                        textBox3.Text = read[3].ToString();
                        textBox11.Text = read[5].ToString();
                        textBox15.Text = read[6].ToString();

                        MessageBox.Show("Kullanıcı Başarıyla Bulundu");
                        panel4.Visible = true;
                        panel2.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Bulunamadı");
                    }
                }

                baglanti.Close();
            }
            else
            {
                MessageBox.Show("Lütfen hesap numarasını girin.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(textBox14.Text) && !string.IsNullOrWhiteSpace(textBox16.Text))
                {
                    string sorgu = "UPDATE hesap SET bakiye = @bakiye WHERE hno = @hno";
                    komut = new SqlCommand(sorgu, baglanti);

                    // Eğer textBox14.Text'te bir sayı varsa, bu değeri kullan; değilse 0 olarak ayarla
                    int bakiye = int.TryParse(textBox14.Text, out int bakiyeValue) ? bakiyeValue : 0;

                    komut.Parameters.AddWithValue("@bakiye", bakiye);
                    komut.Parameters.AddWithValue("@hno", Convert.ToInt32(textBox16.Text));

                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    MessageBox.Show("Bakiye Güncellendi");
                    Listele();
                }
                else
                {
                    MessageBox.Show("Lütfen Gerekli adımları doldurun ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }


        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            textBox12.Clear();
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            textBox13.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int bakiye = Convert.ToInt32(textBox11.Text);

                int yatırılacak = 0;
                int cekilecek = 0;

                if (int.TryParse(textBox12.Text, out yatırılacak))
                {
                    int sonuc1 = bakiye + yatırılacak;
                    textBox14.Text = sonuc1.ToString();
                }

                if (int.TryParse(textBox13.Text, out cekilecek))
                {
                    if (cekilecek < bakiye)
                    {
                        int sonuc2 = bakiye - cekilecek;
                        textBox14.Text = sonuc2.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Lütfen Çekilecek Paranızı " + bakiye + "TL den büyük sayı girin.");
                    }
                }
                MessageBox.Show("Hesaplama Tamamlandı");
                button1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: Lütfen Hesaba giriş yapın");
            }

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Ana Parayı Çeker
            if (double.TryParse(txtkredi.Text, out double anapara))
            {
                // Comboboxtan vade yi alır
                if (comboBox1.SelectedItem != null && double.TryParse(comboBox1.SelectedItem.ToString(), out double secilenvaade))
                {
                    // Hesaplamayı yapar
                    double geriodenecek = anapara * (1 + (secilenvaade / 100) * 2.56);
                    double aylik = geriodenecek/secilenvaade;

                    // Sonucu Yazdırır
                    txtödeme.Text = geriodenecek.ToString("C2"); // Türk Lirası formatında gösterir
                    textBox6.Text = aylik.ToString("C2"); // Türk Lirası formatında gösterir
                    textBox5.Text = comboBox1.SelectedItem.ToString();
                    btnkredi.Enabled=true;
                }
                else
                {
                    MessageBox.Show("Lütfen bir vade seçin.");
                }
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir sayı girin.");
            }
        }
    

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtfaiz_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnkredi_Click(object sender, EventArgs e)
        {
            string sorgu = "update hesap set  kredi=@kredi,ktaksit=@ktaksit where hno=@hno";
            komut = new SqlCommand(sorgu, baglanti);
            komut.Parameters.AddWithValue("@kredi", txtödeme.Text);
            komut.Parameters.AddWithValue("@ktaksit", textBox5.Text);
            komut.Parameters.AddWithValue("@hno", textBox16.Text);


            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Kredi Bilgileri Güncellendi");

            Listele();

        }
    }
}
