

using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PhoneBookApi.Settings
{
    public interface IConnect
    {
        DataTable GetirDB(string Sorgu);

        string GonderDB(string Sorgu);
    }
    public class Connect : IConnect
    {
        private readonly ProjSettings _projSettings;

        public Connect(IOptions<ProjSettings> projSettings)
        {
            _projSettings = projSettings.Value;
        }

        public DataTable GetirDB(string query)
        {
            NpgsqlConnection baglan = null;
            DataTable dt = new DataTable();
            try
            {
                baglan = new NpgsqlConnection(_projSettings.ConnectionString);
                NpgsqlDataAdapter adap = new NpgsqlDataAdapter(query, baglan);

                adap.Fill(dt);
                baglan.Close();
            }
            catch (System.Exception xx)
            {
                string hata = xx.ToString();
            }
            finally
            {
                baglan.Dispose();
            }
            return dt;
        }

        public string GonderDB(string query)
        {
            string sonuc = "0";

            NpgsqlConnection baglan = null;
            try
            {
                baglan = new NpgsqlConnection(_projSettings.ConnectionString);
                NpgsqlCommand komut = new NpgsqlCommand();

                baglan.Open();
                komut.Connection = baglan;
                komut.CommandTimeout = 300;
                komut.CommandText = query;
                komut.CommandType = CommandType.Text;
                int deger = komut.ExecuteNonQuery();
                baglan.Close();
                sonuc = deger.ToString();
            }
            catch (Exception ex)
            {
                sonuc = ex.Message.ToString();
            }
            finally
            {
                baglan.Dispose();
            }

            //*******************************

            return sonuc;
        }
    }
}
