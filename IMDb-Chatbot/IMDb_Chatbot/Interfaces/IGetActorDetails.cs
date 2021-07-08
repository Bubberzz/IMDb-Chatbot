using System.Threading.Tasks;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Interfaces
{
    public interface IGetActorDetails
    {
        public Task GetDetails(ImdbSearch.Root response, string resultId, int i = 0);
    }
}