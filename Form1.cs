using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AplikasiPencatatanWarga
{
    public partial class Form1 : Form
    {
        private DatabaseManager dbManager = new DatabaseManager();

        // Komponen Form
        private TextBox txtNIK, txtNamaLengkap, txtAlamat, txtPekerjaan;
        private ComboBox cmbJenisKelamin, cmbStatusPerkawinan;
        private DateTimePicker dtpTanggalLahir;
        private Button btnSimpan, btnReset, btnHapus, btnUbah;
        private DataGridView dgvWarga;

        private string selectedNIK = string.Empty;

        public Form1()
        {
            this.Text = "Aplikasi Pencatatan Warga";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 600);
            this.Load += Form1_Load;

            InitComponents();
        }

        private void InitComponents()
        {
            Label lblNIK = new Label() { Text = "NIK", Location = new Point(20, 20) };
            txtNIK = new TextBox() { Location = new Point(150, 20), Width = 200, MaxLength = 16 };

            Label lblNama = new Label() { Text = "Nama Lengkap", Location = new Point(20, 50) };
            txtNamaLengkap = new TextBox() { Location = new Point(150, 50), Width = 200 };

            Label lblTanggal = new Label() { Text = "Tanggal Lahir", Location = new Point(20, 80) };
            dtpTanggalLahir = new DateTimePicker() { Location = new Point(150, 80), Format = DateTimePickerFormat.Long };

            Label lblKelamin = new Label() { Text = "Jenis Kelamin", Location = new Point(20, 110) };
            cmbJenisKelamin = new ComboBox() { Location = new Point(150, 110), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbJenisKelamin.Items.AddRange(new string[] { "Laki-laki", "Perempuan" });

            Label lblAlamat = new Label() { Text = "Alamat", Location = new Point(20, 140) };
            txtAlamat = new TextBox() { Location = new Point(150, 140), Width = 200, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            Label lblPekerjaan = new Label() { Text = "Pekerjaan", Location = new Point(20, 210) };
            txtPekerjaan = new TextBox() { Location = new Point(150, 210), Width = 200 };

            Label lblStatus = new Label() { Text = "Status Perkawinan", Location = new Point(20, 240) };
            cmbStatusPerkawinan = new ComboBox() { Location = new Point(150, 240), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatusPerkawinan.Items.AddRange(new string[] { "Belum Kawin", "Kawin", "Cerai Hidup", "Cerai Mati" });

            // Tombol
            btnSimpan = new Button() { Text = "Simpan", Location = new Point(20, 280), Width = 75 };
            btnReset = new Button() { Text = "Reset", Location = new Point(110, 280), Width = 75 };
            btnUbah = new Button() { Text = "Ubah", Location = new Point(200, 280), Width = 75, Enabled = false };
            btnHapus = new Button() { Text = "Hapus", Location = new Point(290, 280), Width = 75, Enabled = false };

            btnSimpan.Click += BtnSimpan_Click;
            btnReset.Click += BtnReset_Click;
            btnHapus.Click += BtnHapus_Click;
            btnUbah.Click += BtnUbah_Click;

            // DataGridView
            dgvWarga = new DataGridView()
            {
                Location = new Point(380, 20),
                Size = new Size(390, 500),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvWarga.CellClick += DgvWarga_CellClick;

            // Tambah semua kontrol ke Form
            this.Controls.AddRange(new Control[]
            {
                lblNIK, txtNIK,
                lblNama, txtNamaLengkap,
                lblTanggal, dtpTanggalLahir,
                lblKelamin, cmbJenisKelamin,
                lblAlamat, txtAlamat,
                lblPekerjaan, txtPekerjaan,
                lblStatus, cmbStatusPerkawinan,
                btnSimpan, btnReset, btnUbah, btnHapus,
                dgvWarga
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            DataTable dt = dbManager.GetAllWarga();
            dgvWarga.DataSource = dt;
            dgvWarga.AutoResizeColumns();
            dgvWarga.ClearSelection();
            selectedNIK = "";
            txtNIK.ReadOnly = false;
            btnUbah.Enabled = false;
            btnHapus.Enabled = false;
        }

        private void ResetForm()
        {
            txtNIK.Text = "";
            txtNamaLengkap.Text = "";
            txtAlamat.Text = "";
            txtPekerjaan.Text = "";
            dtpTanggalLahir.Value = DateTime.Now;
            cmbJenisKelamin.SelectedIndex = 0;
            cmbStatusPerkawinan.SelectedIndex = 0;

            selectedNIK = "";
            txtNIK.ReadOnly = false;
            btnUbah.Enabled = false;
            btnHapus.Enabled = false;
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNIK.Text) || string.IsNullOrWhiteSpace(txtNamaLengkap.Text))
            {
                MessageBox.Show("NIK dan Nama wajib diisi.", "Peringatan");
                return;
            }

            bool success = dbManager.SaveWarga(
                txtNIK.Text.Trim(),
                txtNamaLengkap.Text.Trim(),
                dtpTanggalLahir.Value,
                cmbJenisKelamin.Text,
                txtAlamat.Text,
                txtPekerjaan.Text,
                cmbStatusPerkawinan.Text
            );

            if (success)
            {
                MessageBox.Show("Data disimpan.");
                LoadData();
                ResetForm();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void BtnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedNIK)) return;

            if (MessageBox.Show($"Hapus NIK {selectedNIK}?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (dbManager.DeleteWarga(selectedNIK))
                {
                    MessageBox.Show("Data dihapus.");
                    LoadData();
                    ResetForm();
                }
            }
        }

        private void BtnUbah_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Silakan ubah data, lalu klik Simpan.");
        }

        private void DgvWarga_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvWarga.Rows[e.RowIndex];
                selectedNIK = row.Cells["NIK"].Value.ToString();
                txtNIK.Text = selectedNIK;
                txtNamaLengkap.Text = row.Cells["NamaLengkap"].Value.ToString();
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtPekerjaan.Text = row.Cells["Pekerjaan"].Value.ToString();
                cmbJenisKelamin.Text = row.Cells["JenisKelamin"].Value.ToString();
                cmbStatusPerkawinan.Text = row.Cells["StatusPerkawinan"].Value.ToString();

                if (DateTime.TryParse(row.Cells["TanggalLahir"].Value.ToString(), out DateTime tgl))
                    dtpTanggalLahir.Value = tgl;

                txtNIK.ReadOnly = true;
                btnUbah.Enabled = true;
                btnHapus.Enabled = true;
            }
        }
    }
}