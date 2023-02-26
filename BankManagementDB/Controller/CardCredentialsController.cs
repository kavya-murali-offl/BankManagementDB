using BankManagementDB.Interface;
using BankManagementDB.Model;

namespace BankManagementDB.Controller
{
    public class CardCredentialsController
    {
        public CardCredentialsController(ICardController cardController) {
            CardController = cardController;
        }  

        public ICardController CardController { get; set; }    

        public bool Authenticate(string cardNumber, string pin)
        {
            Card card = CardController.GetCard(cardNumber);
            if (card.Pin == pin) return true;
            return false;
        }

        public bool ValidateCardNumber(string cardNumber)
        {
            if(CardController.GetCard(cardNumber) == null) return false;
            return true;
        }
    }
}
