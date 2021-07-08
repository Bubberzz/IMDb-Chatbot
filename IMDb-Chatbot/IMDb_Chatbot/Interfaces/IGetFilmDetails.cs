using System.Threading.Tasks;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Interfaces
{
    public interface IGetFilmDetails
    {
        public Task GetDetails(ImdbSearch.Root response, string resultId, int i = 0);
    }
}