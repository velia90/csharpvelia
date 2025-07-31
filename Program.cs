using System;
using System.Windows.Forms;

namespace AplikasiPencatatanWarga
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inisialisasi database
            DatabaseManager dbManager = new DatabaseManager();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}