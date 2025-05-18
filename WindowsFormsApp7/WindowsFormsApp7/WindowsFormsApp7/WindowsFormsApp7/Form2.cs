using System.Data.SQLite;
using System.Windows.Forms;
using System;



namespace WindowsFormsApp7
{
    public partial class Form2 : Form
    {
        private SQLiteConnection connection;

        public Form2()
        {
            InitializeComponent();
            connection = new SQLiteConnection("Data Source=users.db;Version=3;");
        }
        // Veritabanı bağlantısı
        
        private void Form2_Load(object sender, EventArgs e)
        {
            connection.Open(); // Bağlantıyı aç
            // Kullanıcı tablosunu oluştur
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Kullanici (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    KullaniciAdi TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Sifre TEXT NOT NULL
                );";

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();  // Tabloyu oluştur
            }
        }
        

        // Kayıt işlemi (Sign Up)
        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox3.Text;
            string email = textBox4.Text;
            string password = textBox5.Text;

            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();

            // Şifreler eşleşiyorsa
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Şifre alanı boş olamaz!");
            }
            else
            {
                if (RegisterUser(username, email, password))
                {
                    MessageBox.Show("Kayıt başarılı!");
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya e-posta zaten mevcut!");
                }
            }
        }

        // Giriş işlemi (Login)
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Giriş için kontrol
            if (CheckLogin(username, password))
            {
                MessageBox.Show("Giriş başarılı!");
                // Form 1'e geçiş
                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();  // Mevcut formu gizle
            }
            else
            {
                MessageBox.Show("Geçersiz kullanıcı adı veya şifre!");
            }
        }
        private bool CheckLogin(string username, string password)
        {
            string query = "SELECT * FROM Kullanici WHERE KullaniciAdi = @KullaniciAdi AND Sifre = @Sifre";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@KullaniciAdi", username);
                command.Parameters.AddWithValue("@Sifre", password);
                SQLiteDataReader reader = command.ExecuteReader();

                return reader.HasRows;  // Kullanıcı adı ve şifre eşleşiyorsa satır döner
            }
        }

        // Kullanıcı kaydını veritabanına ekleme
        private bool RegisterUser(string username, string email, string password)
        {
            // Kullanıcı adı veya e-posta kontrolü
            if (UserExists(username, email))
            {
                return false;  // Kullanıcı adı veya e-posta zaten mevcutsa kayıt yapılmaz
            }

            string query = "INSERT INTO Kullanici (KullaniciAdi, Email, Sifre) VALUES (@KullaniciAdi, @Email, @Sifre)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@KullaniciAdi", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Sifre", password);
                command.ExecuteNonQuery();  // Veritabanına kaydetme işlemi
            }
            return true;
        }

        // Kullanıcı adı veya e-posta mevcut mu diye kontrol etme
        private bool UserExists(string username, string email)
        {
            string query = "SELECT COUNT(*) FROM Kullanici WHERE KullaniciAdi = @KullaniciAdi OR Email = @Email";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@KullaniciAdi", username);
                command.Parameters.AddWithValue("@Email", email);

                long count = (long)command.ExecuteScalar(); // Satır sayısını döndür
                return count > 0;  // Eğer 1 veya daha fazla satır varsa kullanıcı zaten mevcut demektir
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.Close(); // Bağlantıyı kapat
        }
    }
}




