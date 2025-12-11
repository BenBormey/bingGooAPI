namespace bingGooAPI.Databases
{
    public class ConnectionManager
    {
        public ConnectionManager()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.DefaltConnectionString = builder.Build().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }
        public ConnectionManager(string ConnectionString)
        {
            this.DefaltConnectionString = ConnectionString;
        }
        public string DefaltConnectionString { get; }
    }
}
