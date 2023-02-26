using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankManagementDB.Controller;
using BankManagementDB.Controller;
using BankManagementDB.Interface;
using BankManagementDB.Repository;
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
                .ConfigureCustomerServices()
                .ConfigureAccountServices()
                .ConfigureTransactionServices()
                .ConfigureCardServices()
                .BuildServiceProvider();
        }

        public static ServiceProvider ServiceProvider { get; set; }

        public static IServiceCollection ConfigureCardServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICardController, CardController>();
            serviceCollection.AddSingleton<ICardRepository, CardRepository>();  
            return serviceCollection;
        }

        public static IServiceCollection ConfigureFactories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountFactory, AccountFactory>();
            serviceCollection.AddSingleton<ICardFactory, CardFactory>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureCustomerServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IValidationServices, AuthController>();
            serviceCollection.AddScoped<ICustomerController, CustomerController>();
            serviceCollection.AddSingleton<ICustomerRepository, CustomerRepository>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureAccountServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAccountController, AccountController>();
            serviceCollection.AddSingleton<IAccountRepository, AccountRepository>();
            return serviceCollection;
        }

        public static IServiceCollection ConfigureTransactionServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITransactionProcessController, TransactionProcessController>();
            serviceCollection.AddScoped<ITransactionController, TransactionController>();
            serviceCollection.AddSingleton<ITransactionRepository, TransactionRepository>();
            return serviceCollection;
        }
    }
}
