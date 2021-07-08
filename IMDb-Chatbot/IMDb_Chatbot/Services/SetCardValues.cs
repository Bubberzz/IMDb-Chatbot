using System.Linq;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Services
{
    public static class SetCardValues
    {
        public static string SetTitle(ImdbSearch.Root response, int i, string title)
        {
            if (response.results[i].name is not null)
            {
                title = response.results[i].name;
            }
            if (response.results[i].title is not null)
            {
                title = response.results[i].title;
            }
            return title;
        }

        public static string SetFilmId(ImdbSearch.Root response, int i, string filmId)
        {
            if (response.results[i].id is not null)
            {
                filmId = response.results[i].id;
            }
            return filmId;
        }

        public static string SetImageUrl(ImdbSearch.Root response, int i, string imageUrl)
        {
            if (response.results[i].image is not null)
            {
                imageUrl = response.results[i].image.url;
            }
            return imageUrl;
        }

        public static string SetType(ImdbSearch.Root response, int i, string type)
        {
            if (response.results[i].titleType is not null)
            {
                type = response.results[i].titleType;
            }
            return type;
        }

        public static string SetMinutes(ImdbSearch.Root response, int i, string minutes)
        {
            if (response.results[i].runningTimeInMinutes is not null)
            {
                minutes = response.results[i].runningTimeInMinutes;
            }
            return minutes;
        }

        public static string SetRating(ImdbSearch.Root response, int i, string rating)
        {
            if (response.results[i].rating is not null)
            {
                rating = response.results[i].rating;
            }
            return rating;
        }

        public static string SetYear(ImdbSearch.Root response, int i, string year)
        {
            if (response.results[i].year is not null)
            {
                year = response.results[i].year;
            }
            return year;
        }

        public static string SetPlot(ImdbSearch.Root response, int i, string plot)
        {
            if (response.results[i].plot is null) return plot;
            plot = response.results[i].plot;
            return plot.Length <= 300 ? plot : plot.Substring(0, 300) + "..";
        }

        public static string SetActors(ImdbSearch.Root response, int i, string actors)
        {
            if (response.results[i].principals is null) return actors;
            actors = "";
            var actorList = response.results[i].principals.Select(p => p.name).ToList();
            const string separator = ", ";
            actors += string.Join(separator, actorList);
            return actors;
        }

        public static string SetGenre(ImdbSearch.Root response, int i, string genre)
        {
            if (response.results[i].genre is not null)
            {
                genre = response.results[i].genre;
            }
            return genre;
        }

        public static string SetName(ImdbSearch.Root response, int i, string name)
        {
            if (response.results[i].name is not null)
            {
                name = response.results[i].name;
            }
            return name;
        }

        public static string SetRank(ImdbSearch.Root response, int i, string rank)
        {
            if (response.results[i].actorRank is not 0)
            {
                rank = response.results[i].actorRank.ToString();
            }
            return rank;
        }

        public static string SetBio(ImdbSearch.Root response, int i, string bio)
        {
            if (response.results[i].bio is null) return bio;
            bio = response.results[i].bio;
            return bio.Length <= 300 ? bio : bio.Substring(0, 300) + "..";
        }

        public static string SetBirthDate(ImdbSearch.Root response, int i, string born)
        {
            if (response.results[i].born is not null)
            {
                born = response.results[i].born;
            }
            return born;
        }

        public static string SetKnownFor(ImdbSearch.Root response, int i, string knownFor)
        {
            if (response.results[i].knownFor is null) return knownFor;
            var films = response.results[i].knownFor.Select(t => t.title).ToList();
            knownFor = string.Join(", ", films.ToList());
            return knownFor;
        }

        public static string SetRealName(ImdbSearch.Root response, int i, string realName)
        {
            if (response.results[i].realName is not null)
            {
                realName = response.results[i].realName;
            }
            return realName;
        }
    }
}