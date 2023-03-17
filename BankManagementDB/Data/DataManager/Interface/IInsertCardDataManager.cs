using BankManagementDB.Model;

namespace BankManagementDB.DataManager
{
    public interface IInsertCardDataManager
    {
        bool InsertCard(Card card);

    }
}