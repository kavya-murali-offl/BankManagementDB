using BankManagementDB.Model;

namespace BankManagementDB.Controller
{
    public class CardCredentialsController
    {

        public bool Authenticate(long cardNumber, long pin)
        {
            CardController cardController = new CardController();
            Card card = cardController.GetCard(cardNumber);
            if (card.Pin == pin) return true;
            return false;
        }


        public bool ValidateCardNumber(long cardNumber)
        {
            CardController cardController = new CardController();   
            if(cardController.GetCard(cardNumber) == null) return false;
            return true;
        }
    }
}
