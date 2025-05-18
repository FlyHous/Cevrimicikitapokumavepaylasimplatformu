using System;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Uygulama başlatılmadan önce gerekli ayarları yapıyoruz
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // İlk olarak Form2'yi açıyoruz
            Application.Run(new Form2());
        }
    }
}
