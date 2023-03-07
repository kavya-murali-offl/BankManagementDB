﻿using System.IO;
using BankManagementDB.Controller;
using BankManagementDB.DatabaseAdapter;
using BankManagementDB.DataManager;
using BankManagementDB.DBHandler;
using BankManagementDB.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.Config
{
    public static class DependencyContainer
    {

        static DependencyContainer()
        {
            ServiceProvider = 
                new ServiceCollection()
                .ConfigureFactories()
                .ConfigureDBAdapter()
                .ConfigureHelperServices()
                .ConfigureCustomerServices()
                .ConfigureAccountServices()
                .ConfigureTransactionServices()
                .ConfigureCardServices()
                .BuildServiceProvider();
        }

        public static ServiceProvider ServiceProvider { get; set; }
        public static IConfiguration Config { get; private set; }

        public static IServiceCollection ConfigureDBAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDatabaseAdapter, SQLiteDBAdapter>();
            serviceCollection.AddSingleton<IDBHandler, DBHandler.DBHandler>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureDBServices(this IServiceCollection serviceCollection)
        {
            Config = new ConfigurationBuilder().Build();
            var AppSettings = new AppSettings();
            //Config.Bind("AppSettings", AppSettings);
            string connectionString = Config.GetConnectionString("SQLiteConnection");
            return serviceCollection;

        }

        public static IServiceCollection ConfigureHelperServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IObjectMappingDataManager, ObjectMappingDataManager>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureCardServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetCardDataManager, GetCardDataManager>();
            serviceCollection.AddScoped<IInsertCardDataManager, InsertCardDataManager>();
            serviceCollection.AddScoped<IUpdateCardDataManager, UpdateCardDataManager>();
            return serviceCollection;
        
        }
            public static IServiceCollection ConfigureFactories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountFactory, AccountFactory>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureCustomerServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IGetCustomerDataManager, GetCustomerDataManager>();
            serviceCollection.AddScoped<IInsertCustomerDataManager, InsertCustomerDataManager>();
            serviceCollection.AddScoped<IUpdateCustomerDataManager, UpdateCustomerDataManager>();
            serviceCollection.AddScoped<IValidatePasswordDataManager, ValidatePasswordDataManager>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureAccountServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IInsertAccountDataManager, InsertAccountDataManager>();
            serviceCollection.AddScoped<IGetAccountDataManager, GetAccountDataManager>();
            serviceCollection.AddScoped<IUpdateAccountDataManager, UpdateAccountDataManager>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureTransactionServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionDataManager, TransactionDataManager>();
            return serviceCollection;
        }
    }
}
