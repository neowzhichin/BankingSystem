using Microsoft.Extensions.Configuration.Json;

public class Appsettings
    {
        static IConfiguration Configuration { get; set; }

        public Appsettings()
        {
            string Path = "appsettings.json";

            Configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })
               .Build();
        }

        public static string app(params string[] sections)
        {
            try
            {

                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }

            return "";
        }

        public static List<T> app<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }
    }