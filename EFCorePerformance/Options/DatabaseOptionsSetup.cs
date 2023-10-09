using Microsoft.Extensions.Options;

namespace EFCorePerformance.Options;

public class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
{
    private const string ConfigurationSectionName = "DatabaseOptions";
    private readonly IConfiguration _configuration = configuration;

    public void Configure(DatabaseOptions options)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        options.ConnectionString = connectionString;

        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}