using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.EnumerationType
{
    public enum EntryCases
    {
        LOGIN,
        SIGNUP,
        EXIT
    }

    public enum DashboardCases
    {
        PROFILE_SERVICES,
        CREATE_ACCOUNT,
        LIST_ACCOUNTS,
        ACCOUNT_SERVICES,
        CARD_SERVICES,
        SIGN_OUT
    }

    public enum ProfileServiceCases
    {
        VIEW_PROFILE,
        EDIT_PROFILE,
        EXIT
    }

    public enum AccountCases
    {
        DEPOSIT,
        WITHDRAW,
        TRANSFER,
        CHECK_BALANCE,
        VIEW_STATEMENT,
        PRINT_STATEMENT,
        VIEW_ACCOUNT_DETAILS,
        BACK
    }

    public enum CardCases
    {
        VIEW_CARDS,
        ADD_CARD,
        RESET_PIN,
        CREDIT_CARD_SERVICES,
        EXIT
    }

    public enum CreditCardCases
    {
        PURCHASE,
        PAYMENT,
        VIEW_STATEMENT,
        EXIT
    }

    public enum MoneyServices
    {
        CASH,
        CREDIT_CARD,
        DEBIT_CARD,
    }
}
