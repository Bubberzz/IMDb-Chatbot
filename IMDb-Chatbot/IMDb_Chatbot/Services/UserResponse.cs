using System.Collections.Generic;

namespace IMDb_Chatbot.Services
{
    public class UserResponse
    {
        public static List<string> UserResponseNegative()
        {
            var negativeResponse = new List<string>
            {
                "No",
                "no",
                "Nah",
                "nah",
                "Nope",
                "nope"
            };
            return negativeResponse;
        }

        public static List<string> UserResponsePositive()
        {
            var positiveResponse = new List<string>
            {
                "Yes",
                "yes",
                "ok",
                "Ok",
                "OK",
                "Sure",
                "sure",
                "Yeah",
                "yeah",
                "ye",
                "Ye",
                "Yea",
                "yea"
            };
            return positiveResponse;
        }
    }
}