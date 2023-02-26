using System;
using System.Collections.Generic;
using System.Linq;
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
        GO_TO_ACCOUNT,
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
        CARD_SERVICES,
        BACK
    }

    public enum CardCases
    {
        VIEW_CARDS,
        RESET_PIN,
        EXIT
    }

    public enum MoneyServices
    {
        CASH,
        CREDIT_CARD,
        DEBIT_CARD,
    }
}
