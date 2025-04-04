namespace PhoneBookApi.Settings
{
    public class DbConnectInfo
    {

        private string _connectionString_Test = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

        private string _connectionString_Local = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

        public string connectionString_Test { get; }

        public string connectionString_Local { get; }

        public DbConnectInfo()
        {
            connectionString_Test = _connectionString_Test;

            connectionString_Local = _connectionString_Local;
        }

    }
}
