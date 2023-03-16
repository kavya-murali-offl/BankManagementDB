using System.IO;
using System.Reflection;
using System.Resources;
using BankManagementDB.Controller;
using BankManagementDB.DatabaseAdapter;
using BankManagementDB.DataManager;
using BankManagementDB.DatabaseHandler;
using BankManagementDB.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System;

namespace BankManagementDB.Config
{
    public static class DependencyContainer
    {

        static DependencyContainer()
        {
            ServiceProvider = 
                new ServiceCollection()
                .AddLocalization()
                .ConfigureFactories()
                .ConfigureDBServices()
                .ConfigureDBAdapter()
                .ConfigureCustomerServices()
                .ConfigureAccountServices()
                .ConfigureTransactionServices()
                .ConfigureCardServices()
                .BuildServiceProvider();
        }

        public static ServiceProvider ServiceProvider { get; set; }

        public static IConfiguration Config { get; private set; }

        private static ResourceSet ResourceSet { get;  set; }

        public static string GetResource(string key) => ResourceSet.GetString(key);

        public static IServiceCollection ConfigureDBAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDatabaseAdapter, SQLiteDatabaseAdapter>();
            serviceCollection.AddSingleton<IDBHandler, DBHandler>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureDBServices(this IServiceCollection serviceCollection)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            return serviceCollection;
        }

        public static IServiceCollection AddLocalization(this IServiceCollection serviceCollection)
        {
            ResourceManager manager = new ResourceManager(typeof(Properties.Resources));
            var culture = CultureInfo.CurrentCulture;

            var resourceSet = manager.GetResourceSet(culture, true, false);
            if (resourceSet == null)
                resourceSet = manager.GetResourceSet(CultureInfo.InvariantCulture, true, false);

            ResourceSet = resourceSet;
            return serviceCollection;
        }

        public static IServiceCollection ConfigureCardServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetCardDataManager, GetCardDataManager>();
            serviceCollection.AddScoped<IInsertCardDataManager, InsertCardDataManager>();
            serviceCollection.AddScoped<IInsertCreditCardDataManager, InsertCreditCardDataManager>();
            serviceCollection.AddScoped<IInsertDebitCardDataManager, InsertDebitCardDataManager>();
            serviceCollection.AddScoped<IUpdateCardDataManager, UpdateCardDataManager>();
            serviceCollection.AddScoped<IUpdateCreditCardDataManager, UpdateCreditCardDataManager>();
            return serviceCollection;
        
        }
            public static IServiceCollection ConfigureFactories(this IServiceCollection serviceCollection)
         {
            serviceCollection.AddSingleton<IAccountFactory, AccountFactory>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureCustomerServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetCustomerCredentialsDataManager, GetCustomerCredentialsDataManager>();
            serviceCollection.AddScoped<IInsertCredentialsDataManager, InsertCredentialsDataManager>();
            serviceCollection.AddScoped<IGetCustomerDataManager, GetCustomerDataManager>();
            serviceCollection.AddScoped<IInsertCustomerDataManager, InsertCustomerDataManager>();
            serviceCollection.AddScoped<IUpdateCustomerDataManager, UpdateCustomerDataManager>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureAccountServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IInsertAccountDataManager, InsertAccountDataManager>();
            serviceCollection.AddScoped<IGetAccountDataManager, GetAccountDataManager>();
            serviceCollection.AddScoped<IUpdateAccountDataManager, UpdateAccountDataManager>();
            serviceCollection.AddScoped<ITransferAmountDataManager, TranferAmountDataManager>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureTransactionServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IInsertTransactionDataManager, InsertTransactionDataManager>();
            serviceCollection.AddScoped<IGetTransactionDataManager, GetTransactionDataManager>();
            return serviceCollection;
        }
    }
}
