using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

public class DatabaseManager
{
    private readonly string dbPath;

public DatabaseManager()
{
    string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    string dataDir = Path.Combine(projectRoot, "Data");

    if (!Directory.Exists(dataDir))
        Directory.CreateDirectory(dataDir);

    dbPath = Path.Combine(dataDir, "warga.db");

    InitializeDatabase();
}


    private void InitializeDatabase()
    {
        if (!File.Exists(dbPath)) SQLiteConnection.CreateFile(dbPath);
        using var conn = GetConnection();
        conn.Open();
        string query = @"CREATE TABLE IF NOT EXISTS Warga (
            NIK TEXT PRIMARY KEY NOT NULL,
            NamaLengkap TEXT NOT NULL,
            TanggalLahir TEXT,
            JenisKelamin TEXT NOT NULL,
            Alamat TEXT,
            Pekerjaan TEXT,
            StatusPerkawinan TEXT
        );";
        using var cmd = new SQLiteCommand(query, conn);
        cmd.ExecuteNonQuery();
    }

    public SQLiteConnection GetConnection() =>
        new SQLiteConnection($"Data Source={dbPath};Version=3;");

    public bool SaveWarga(string nik, string nama, DateTime tgl, string jk, string alamat, string pekerjaan, string status)
    {
        using var conn = GetConnection();
        try
        {
            conn.Open();
            string sql = @"INSERT OR REPLACE INTO Warga 
                (NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan)
                VALUES (@nik, @nama, @tgl, @jk, @alamat, @pekerjaan, @status);";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nik", nik);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@tgl", tgl.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@jk", jk);
            cmd.Parameters.AddWithValue("@alamat", alamat);
            cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public DataTable GetAllWarga()
    {
        DataTable dt = new();
        using var conn = GetConnection();
        conn.Open();
        string sql = "SELECT * FROM Warga ORDER BY NamaLengkap ASC;";
        using var da = new SQLiteDataAdapter(sql, conn);
        da.Fill(dt);
        return dt;
    }

    public bool DeleteWarga(string nik)
    {
        using var conn = GetConnection();
        conn.Open();
        string sql = "DELETE FROM Warga WHERE NIK = @nik;";
        using var cmd = new SQLiteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nik", nik);
        return cmd.ExecuteNonQuery() > 0;
    }
}