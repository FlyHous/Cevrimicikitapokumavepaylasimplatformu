using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        private Database database;
        private Kitap secilenKitap;

        public Form1()
        {
            InitializeComponent();
            database = new Database(); // Veritabanı bağlantısını başlat
            KitaplariYukle(); // Form açıldığında kitapları yükle
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kitapAdi = textBox1.Text;
            string yazar = textBox2.Text;
            string yayinEvi = textBox3.Text;
            string yorum = textBox4.Text;

            if (!string.IsNullOrEmpty(kitapAdi) && !string.IsNullOrEmpty(yazar) && !string.IsNullOrEmpty(yayinEvi))
            {
                Kitap yeniKitap = new Kitap(kitapAdi, yazar, yayinEvi, yorum);
                database.KitapEkle(yeniKitap); // Kitabı veritabanına ekle

                listBox1.Items.Add(yeniKitap); // ListBox'a ekle

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
            }
            else
            {
                MessageBox.Show("Lütfen kitap adı, yazar ve yayınevi alanlarını doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ListBox'tan seçilen öğeyi al
            if (listBox1.SelectedItem != null)
            {
                Kitap secilenKitap = (Kitap)listBox1.SelectedItem;

                // Veritabanından kitabı sil
                database.KitapSil(secilenKitap);

                // ListBox'tan kitabı sil
                listBox1.Items.Remove(secilenKitap);
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir kitap seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // ListBox'tan seçilen öğeyi al
            if (listBox1.SelectedItem != null)
            {
                secilenKitap = (Kitap)listBox1.SelectedItem;

                // Seçilen kitabın bilgilerini TextBox'lara yükle
                textBox1.Text = secilenKitap.Adi;
                textBox2.Text = secilenKitap.Yazari;
                textBox3.Text = secilenKitap.Yayinevi;
                textBox4.Text = secilenKitap.Yorum;
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek için bir kitap seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Düzenlenen kitabı al
            if (secilenKitap != null)
            {
                secilenKitap.Adi = textBox1.Text;
                secilenKitap.Yazari = textBox2.Text;
                secilenKitap.Yayinevi = textBox3.Text;
                secilenKitap.Yorum = textBox4.Text;

                // Veritabanında düzenle
                database.KitapDüzenle(secilenKitap);

                // ListBox'ta da güncelle
                listBox1.Items[listBox1.SelectedIndex] = secilenKitap;

                // TextBox'ları temizle
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();

                MessageBox.Show("Kitap başarıyla düzenlendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Düzenlemek için bir kitap seçmelisiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void KitaplariYukle()
        {
            List<Kitap> kitaplar = database.KitaplariGetir(); // Veritabanından kitapları getir
            foreach (var kitap in kitaplar)
            {
                listBox1.Items.Add(kitap); // ListBox'a ekle
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // Düzenlenen kitabı al
            if (secilenKitap != null)
            {
                // Seçilen kitabın bilgilerini TextBox'lardan al ve güncelle
                secilenKitap.Adi = textBox1.Text;
                secilenKitap.Yazari = textBox2.Text;
                secilenKitap.Yayinevi = textBox3.Text;
                secilenKitap.Yorum = textBox4.Text;

                // Veritabanında düzenle
                database.KitapDüzenle(secilenKitap);

                // ListBox'ta da güncelle
                listBox1.Items[listBox1.SelectedIndex] = secilenKitap;

                // TextBox'ları temizle
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();

                MessageBox.Show("Kitap başarıyla düzenlendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Düzenlemek için bir kitap seçmelisiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    public class Kitap
    {
        public string Adi { get; set; }
        public string Yazari { get; set; }
        public string Yayinevi { get; set; }
        public string Yorum { get; set; }

        public Kitap(string adi, string yazari, string yayinEvi, string yorum)
        {
            Adi = adi;
            Yazari = yazari;
            Yayinevi = yayinEvi;
            Yorum = yorum;
        }

        public override string ToString()
        {
            return $"{Adi} - {Yazari} - {Yayinevi} - {Yorum}";
        }
    }

    public class Database
    {
        private SQLiteConnection connection;

        public Database()
        {
            // Veritabanı dosyasını oluştur veya bağlan
            string dbPath = "kitaplar.db";
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");

            if (!System.IO.File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath); // Veritabanı dosyasını oluştur
            }

            connection.Open();

            // Kitaplar tablosunu oluştur
            string createTableQuery = @" 
                CREATE TABLE IF NOT EXISTS Kitaplar (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Adi TEXT NOT NULL,
                    Yazari TEXT NOT NULL,
                    Yayinevi TEXT NOT NULL,
                    Yorum TEXT
                );";

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void KitapEkle(Kitap kitap)
        {
            string insertQuery = @"
                INSERT INTO Kitaplar (Adi, Yazari, Yayinevi, Yorum)
                VALUES (@Adi, @Yazari, @Yayinevi, @Yorum);";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Adi", kitap.Adi);
                command.Parameters.AddWithValue("@Yazari", kitap.Yazari);
                command.Parameters.AddWithValue("@Yayinevi", kitap.Yayinevi);
                command.Parameters.AddWithValue("@Yorum", kitap.Yorum);
                command.ExecuteNonQuery();
            }
        }

        public List<Kitap> KitaplariGetir()
        {
            List<Kitap> kitaplar = new List<Kitap>();

            string selectQuery = "SELECT * FROM Kitaplar;"; 
            using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kitaplar.Add(new Kitap(
                            reader["Adi"].ToString(),
                            reader["Yazari"].ToString(),
                            reader["Yayinevi"].ToString(),
                            reader["Yorum"].ToString()
                        ));
                    }
                }
            }

            return kitaplar;
        }

        public void KitapSil(Kitap kitap)
        {
            string deleteQuery = "DELETE FROM Kitaplar WHERE Adi = @Adi AND Yazari = @Yazari AND Yayinevi = @Yayinevi AND Yorum = @Yorum;";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Adi", kitap.Adi);
                command.Parameters.AddWithValue("@Yazari", kitap.Yazari);
                command.Parameters.AddWithValue("@Yayinevi", kitap.Yayinevi);
                command.Parameters.AddWithValue("@Yorum", kitap.Yorum);
                command.ExecuteNonQuery();
            }
        }

        public void KitapDüzenle(Kitap kitap)
        {
            string updateQuery = @"UPDATE Kitaplar SET 
                                    Adi = @Adi, 
                                    Yazari = @Yazari, 
                                    Yayinevi = @Yayinevi, 
                                    Yorum = @Yorum
                                    WHERE Adi = @Adi AND Yazari = @Yazari AND Yayinevi = @Yayinevi;";

            using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Adi", kitap.Adi);
                command.Parameters.AddWithValue("@Yazari", kitap.Yazari);
                command.Parameters.AddWithValue("@Yayinevi", kitap.Yayinevi);
                command.Parameters.AddWithValue("@Yorum", kitap.Yorum);
                command.ExecuteNonQuery();
            }
        }
    }
}
