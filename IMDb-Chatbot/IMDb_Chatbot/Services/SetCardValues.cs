using System;
using System.Collections.Generic;
using System.Linq;
using CardsBot.Models;
using IMDb_Chatbot.Models;

namespace CardsBot.Services
{
    public static class SetCardValues
    {
        public static string SetTitle(ImdbResult response, int i, string title)
        {
            if (response.ImdbSearchResult.results[i].name is not null)
            {
                title = response.ImdbSearchResult.results[i].name;
            }

            if (response.ImdbSearchResult.results[i].title is not null)
            {
                title = response.ImdbSearchResult.results[i].title;
            }

            return title;
        }

        public static string SetFilmId(ImdbResult response, int i, string filmId)
        {
            if (response.ImdbSearchResult.results[i].id is not null)
            {
                filmId = response.ImdbSearchResult.results[i].id;
            }

            return filmId;
        }

        public static string SetImageUrl(ImdbResult response, int i, string imageUrl)
        {
            if (response.ImdbSearchResult.results[i].image is not null)
            {
                imageUrl = response.ImdbSearchResult.results[i].image.url;
            }

            return imageUrl;
        }

        public static string SetType(ImdbResult response, int i, string type)
        {
            if (response.ImdbSearchResult.results[i].titleType is not null)
            {
                type = response.ImdbSearchResult.results[i].titleType;
            }

            return type;
        }

        public static string SetMinutes(ImdbResult response, int i, string minutes)
        {
            if (response.ImdbSearchResult.results[i].runningTimeInMinutes is not null)
            {
                minutes = response.ImdbSearchResult.results[i].runningTimeInMinutes;
            }

            return minutes;
        }

        public static string SetRating(ImdbResult response, int i, string rating)
        {
            if (response.ImdbSearchResult.results[i].rating is not null)
            {
                rating = response.ImdbSearchResult.results[i].rating;
            }

            return rating;
        }

        public static string SetYear(ImdbResult response, int i, string year)
        {
            if (response.ImdbSearchResult.results[i].year is not null)
            {
                year = response.ImdbSearchResult.results[i].year;
            }

            return year;
        }

        public static string SetPlot(ImdbResult response, int i, string plot)
        {
            if (response.ImdbSearchResult.results[i].plot is not null)
            {
                plot = response.ImdbSearchResult.results[i].plot;
            }

            return plot;
        }

        public static string SetActors(ImdbResult response, int i, string actors)
        {
            if (response.ImdbSearchResult.results[i].principals is not null)
            {
                actors = "";
                var actorList = response.ImdbSearchResult.results[i].principals.Select(p => p.name).ToList();
                const string separator = ", ";
                actors += string.Join(separator, actorList);
            }

            return actors;
        }

        public static string SetGenre(ImdbResult response, int i, string genre)
        {
            if (response.ImdbSearchResult.results[i].genre is not null)
            {
                genre = response.ImdbSearchResult.results[i].genre;
            }

            return genre;
        }

        internal static string SetName(ImdbResult response, int i, string name)
        {
            if (response.ImdbSearchResult.results[i].name is not null)
            {
                name = response.ImdbSearchResult.results[i].name;
            }

            return name;
        }

        public static string SetRank(ImdbResult response, int i, string rank)
        {
            if (response.ImdbSearchResult.results[i].actorRank is not 0)
            {
                rank = response.ImdbSearchResult.results[i].actorRank.ToString();
            }
            
            return rank;
        }

        public static string SetBio(ImdbResult response, int i, string bio)
        {
            if (response.ImdbSearchResult.results[i].bio is not null)
            {
                bio = response.ImdbSearchResult.results[i].bio;
                return bio.Length <= 300 ? bio : bio.Substring(0, 300) + "..";
            }

            return bio;
        }

        public static string SetBornDate(ImdbResult response, int i, string born)
        {
            if (response.ImdbSearchResult.results[i].born is not null)
            {
                born = response.ImdbSearchResult.results[i].born;
            }

            return born;
        }

        public static string SetKnownFor(ImdbResult response, int i, string knownFor)
        {
            if (response.ImdbSearchResult.results[i].knownFor is not null)
            {
                var films = response.ImdbSearchResult.results[i].knownFor.Select(t => t.title).ToList();
                knownFor = string.Join(", ", films.ToList());
            }

            return knownFor;
        }

        public static string SetRealName(ImdbResult response, int i, string realName)
        {
            if (response.ImdbSearchResult.results[i].realName is not null)
            {
                realName = response.ImdbSearchResult.results[i].realName;
            }

            return realName;
        }
    }
}
