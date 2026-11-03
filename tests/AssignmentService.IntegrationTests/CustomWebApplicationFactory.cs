using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Xunit;

namespace AssignmentService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Priprava začasnega PostgreSQL vsebnika
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("assignments_test_db")
        .WithUsername("postgres")
        .WithPassword("testpassword")
        .Build();

    public async Task InitializeAsync()
    {
        // Zagon testnega vsebnika pred začetkom testov
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Povezovalni niz iz appsettings.json prepišemo z začasnim testnim vsebnikom
        builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());
    }

    public new async Task DisposeAsync()
    {
        // Zaustavitev in uničenje vsebnika po končanih testih
        await _dbContainer.DisposeAsync();
    }
}