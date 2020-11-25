using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace IoTHubIngestion
{
    public class FunctionConfig : IConfiguration
    {
        public static string ConnectionString = "";
        public static string Environment = "dev";
        public static string SbConnectionString;
        public static string HeimdallUrl;
        public static string InvocationId;
        public string this[string key]
        {
            get
            {
                switch (key)
                {
                    default:
                        return ConnectionString;
                }
               
            }
            set => throw new System.NotImplementedException();
        }

        public static string HeimdallUpCode;

        public static string HeimdallDownCode;

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new System.NotImplementedException();
        }

        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken()
        {
            throw new System.NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}