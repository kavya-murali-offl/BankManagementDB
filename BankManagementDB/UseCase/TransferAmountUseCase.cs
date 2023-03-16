using BankManagementDB.Config;
using BankManagementDB.DataManager;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using BankManagementDB.Properties;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.UseCase
{
    public class TransferAmountUseCase
    {
        public TransferAmountUseCase()
        {
            UpdateAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateAccountDataManager>();
        }

        public IUpdateAccountDataManager UpdateAccountDataManager { get; set; }

        public void TransferAmount(Account account, Account transferAccount, decimal amount)
        {
           
                    IList<Action> actions = new List<Action>();
                    Action action1 = () => {
                        account.Withdraw(amount);
                        UpdateAccountDataManager.UpdateAccount(account);
                    };

                    Action action2 = () =>
                    {
                        account.Deposit(amount);
                        UpdateAccountDataManager.UpdateAccount(transferAccount);
                    };

                    actions.Add(action1);
                    actions.Add(action2);

                    ITransferAmountDataManager transferAmountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<ITransferAmountDataManager>();
                    transferAmountDataManager.PerformTransaction(actions);

                    //if (UpdateBalance(SelectedAccount, amount, TransactionType.WITHDRAW))
                    //{
                    //    if (UpdateBalance(transferAccount, amount, TransactionType.DEPOSIT))
                    //    {
                    //        Notification.Success(string.Format(Resources.TransferSuccess, amount));
                    //        TransactionView.RecordTransaction("Transferred", amount, SelectedAccount.Balance, TransactionType.TRANSFER, SelectedAccount.AccountNumber, modeOfPayment, cardNumber, null);
                    //        TransactionView.RecordTransaction("Received", amount, SelectedAccount.Balance, TransactionType.RECEIVED,null, modeOfPayment, cardNumber, SelectedAccount.AccountNumber);
                    //    }
                    //    else
                    //    {
                    //        Notification.Error(Resources.TransferFailure);
                    //        UpdateBalance(SelectedAccount, amount, TransactionType.DEPOSIT);
                    //    }
                    //}
                    //else
                    //    Notification.Error(Resources.TransferFailure);
        }
    }
}
