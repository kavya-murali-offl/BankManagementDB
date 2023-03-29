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
        EXIT,
        SIGN_OUT
    }

    public enum ProfileServiceCases
    {
        VIEW_PROFILE,
        EDIT_PROFILE,
        GO_BACK,
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
        GO_BACK,
        EXIT
    }

    public enum CardCases
    {
        VIEW_CARDS,
        ADD_CARD,
        RESET_PIN,
        VIEW_TRANSACTIONS,
        CREDIT_CARD_SERVICES,
        GO_BACK,
        EXIT
    }

    public enum CreditCardCases
    {
        PURCHASE,
        PAYMENT,
        GO_BACK,
        EXIT
    }
}
