using System;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Services
{
    public class GetActorDetails : IGetActorDetails
    {
        private static IImdbService _imdbService;

        public GetActorDetails(IImdbService imdbService)
        {
            _imdbService = imdbService;
        }

        public async Task GetDetails(ImdbSearch.Root response, string resultId, int i = 0)
        {
            // get actor bio
            var bio = await _imdbService.GetBio(resultId);
            response.results[i].bio = bio.miniBios != null ? bio.miniBios[0].text : "Unknown";

            // set birth date info
            if (bio.birthDate is not null)
            {
                var birthPlace = bio.birthPlace ?? "Unknown";
                try
                {
                    var formattedDate = DateTime.ParseExact($"{bio.birthDate}", "yyyy-mm-dd",
                        null);
                    response.results[i].born =
                        formattedDate.ToString("dd MMMM yyyy") + " in " + birthPlace;
                }
                catch
                {
                    response.results[i].born = bio.birthDate + " in " + birthPlace;
                }
            }
            else
            {
                response.results[i].born = "Unknown";
            }

            // set real name
            response.results[i].realName = bio.realName ?? response.results[0].name;

            // get actor rank
            var rank = await _imdbService.AutoComplete(response.results[0].name);
            response.results[i].actorRank = rank.d?[0].rank ?? 0;
        }
    }
}