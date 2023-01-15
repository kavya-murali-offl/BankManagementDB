using BankManagement.Controller;
using BankManagement.Models;
using System;


namespace BankManagement.View
{
    public class ProfileView
    {
        
        public void GetProfileDetails(ProfileController profileController)
        {
            while (true)
            {
                PrintProfileDetails(profileController);
                Console.WriteLine("Press 0 to go back to dashboard");
                try
                {
                    String input = Console.ReadLine();
                    if (input != "0")
                    {
                        Console.WriteLine("Enter a valid option");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception error)
                {
                    Console.WriteLine("Enter a valid option");
                }
            }
        }

        public void PrintProfileDetails(ProfileController profileController)
        {
            Console.WriteLine("\n PROFILE \n");
            Console.WriteLine("Name: " + profileController.Name);
            Console.WriteLine("UserName: " + profileController.UserName);
            //Console.WriteLine("Phone: " + profileController.Phone);

        }
    }
}
