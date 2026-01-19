using Reqnroll;
using Reqnroll.BoDi;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Application.Services;      
using Infrastructure.Repositories;
using Infrastructure.Context;
using Domain.Repository;
using Domain.Interfaces;
using Application.Service;
using Application.Interfaces;
using Infrastructure.Security;

namespace Tests.E2E.Support
{
    [Binding]
    public class CreateUserSupport
    {
        private readonly IObjectContainer _container;

        private static readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("webapisociety_test")
            .WithUsername("tester")
            .WithPassword("password123")
            .Build();

        public CreateUserSupport(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeTestRun]
        public static async Task RunBeforeAllTests()
        {
            await _postgreSqlContainer.StartAsync();
        }

        [BeforeScenario]
        public async Task InitializeDependencies()
        {
            var connectionString = _postgreSqlContainer.GetConnectionString();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var dbContext = new AppDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();

           
            _container.RegisterInstanceAs<AppDbContext>(dbContext);

            var userRepository = new UserRepository(dbContext);
            _container.RegisterInstanceAs<IUserRepository>(userRepository);

            var securityService = new BCryptoSecurityService();
            _container.RegisterInstanceAs<ISecurityService>(securityService);

            _container.RegisterTypeAs<UserService, IUserServices>();
        }

        [AfterScenario]
        public async Task CleanUp()
        {
            if (_container.IsRegistered<AppDbContext>())
            {
                var dbContext = _container.Resolve<AppDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
            }
        }

        [AfterTestRun]
        public static async Task RunAfterAllTests()
        {
            await _postgreSqlContainer.StopAsync();
        }
    }
}