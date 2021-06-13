// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using CardsBot.Models;
using CardsBot.Services;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class Cards
    {
        private IImdbService _imdbService;
        public Cards(IImdbService imdbService)
        {
            _imdbService = imdbService;
        }

        // Get hero card, title: movie, name: actor
        public static List<HeroCard> GetHeroCard(ImdbResult response, bool showMoreResults, bool showTryAgainButton)
        {
            var result = new List<HeroCard>();

            for (var i = 0; i < response.ImdbSearchResult.results.Count; i++)
            {
                if (response.ImdbSearchResult.results[i].id.StartsWith("/title"))
                {
                    switch (showMoreResults)
                    {
                        case true:
                            result.Add(CreateFilmHeroCards(response, i, true, showTryAgainButton));
                            break;
                        case false:
                            result.Add(CreateFilmHeroCards(response, i, false, showTryAgainButton));
                            break;
                    }
                }

                if (response.ImdbSearchResult.results[i].id.StartsWith("/name"))
                {
                    result.Add(CreateActorHeroCards(response, i));
                }
            }

            return result;
        }

        public async Task<HeroCard> CreateTopRatedMovieCardAsync(List<CardsBot.Models.TopRatedMovies.Root> list, bool final)
        {

            var myStringBuilder = new StringBuilder();

            if (final is false)
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].id.Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"Rating: {list[i].chartRating} - {film.d[0].l}");
                    myStringBuilder.AppendLine();
                }


                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Top rated movies")
                    },
                };
                return heroCard;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].id.Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"Rating: {list[i].chartRating} - {film.d[0].l}");
                    myStringBuilder.AppendLine();

                }

                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                };
                return heroCard;
            }
           
        }

        public async Task<HeroCard> CreateTopRatedActorCardAsync(List<CardsBot.Models.TopRatedActors.Root> list, bool final)
        {

            StringBuilder myStringBuilder = new StringBuilder();

            if (final is false)
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].id.Split("/");
                    var resultId = extractId[2];
                    var actor = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"Rank: {list[i].chartRating} - {actor.d[0].l}");
                    myStringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Top rated actors")
                    },
                };
                return heroCard;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].id.Split("/");
                    var resultId = extractId[2];
                    var actor = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"Rank: {list[i].chartRating} - {actor.d[0].l}");
                    myStringBuilder.AppendLine();

                }
                var movies = "";

                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                };
                return heroCard;
            }

        }


        private static HeroCard CreateFilmHeroCards(ImdbResult response, int i, bool showMoreResultsButton, bool showTryAgainButton)
        {
            // Set default values in case the API doesn't return a value
            var title = "Unknown";
            var actors = "Unknown";
            var plot = "Unknown";
            var year = "Unknown";
            var rating = "Unknown";
            var minutes = "Unknown";
            var type = "Unknown";
            var imageUrl =
                "https://www.publicdomainpictures.net/pictures/280000/velka/not-found-image-15383864787lu.jpg";
            var filmId = "tt013";
            var genre = "Unknown";

            // Set values returned by the API
            title = SetCardValues.SetTitle(response, i, title);
            actors = SetCardValues.SetActors(response, i, actors);
            plot = SetCardValues.SetPlot(response, i, plot);
            year = SetCardValues.SetYear(response, i, year);
            rating = SetCardValues.SetRating(response, i, rating);
            minutes = SetCardValues.SetMinutes(response, i, minutes);
            type = SetCardValues.SetType(response, i, type);
            imageUrl = SetCardValues.SetImageUrl(response, i, imageUrl);
            filmId = SetCardValues.SetFilmId(response, i, filmId);
            genre = SetCardValues.SetGenre(response, i, filmId);

            // Check if the API returned multiple films
            // If multiple films are returned, then include a show more results button
            if (showMoreResultsButton)
            {
                // Card that can show more search results
                var heroCardShowMoreResults = new HeroCard
                {
                    Title = title,
                    Subtitle = actors,
                    Text =
                        plot + "\r\n " +
                        "Year: " + year + " | " +
                        "Rating: " + rating + " | " +
                        "Genre: " + genre + " | " +
                        "Minutes: " + minutes + " | " +
                        "Type: " + type,
                    Images = new List<CardImage> { new CardImage(imageUrl) },
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.OpenUrl, "Open IMDb",
                            value: "https://www.imdb.com" + filmId),
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Movie Results")
                    },
                };
                return heroCardShowMoreResults;
            }

            if (showTryAgainButton)
            {
                var heroCardMovieRouletteCard = new HeroCard
                {
                    Title = title,
                    Subtitle = actors,
                    Text =
                        plot + "\r\n " +
                        "Year: " + year + " | " +
                        "Rating: " + rating + " | " +
                        "Genre: " + genre + " | " +
                        "Minutes: " + minutes + " | " +
                        "Type: " + type,
                    Images = new List<CardImage> { new CardImage(imageUrl) },
                    Buttons = new List<CardAction>
                        {
                            new(ActionTypes.OpenUrl, "Open IMDb",
                                value: "https://www.imdb.com" + filmId),
                            new(ActionTypes.MessageBack, "Try Again?",
                                value: "IMDb Roulette")
                        },
                };
                return heroCardMovieRouletteCard;
            }

            // Card that returns film without 'show more' button
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = actors,
                Text =
                    plot + "\r\n " +
                    "Year: " + year + " | " +
                    "Rating: " + rating + " | " +
                    "Minutes: " + minutes + " | " +
                    "Genre: " + genre + " | " +
                    "Type: " + type,
                Images = new List<CardImage> { new CardImage(imageUrl) },
                Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.OpenUrl, "Open IMDb",
                            value: "https://www.imdb.com" + filmId)
                    },
            };
            return heroCard;

        }

        private static HeroCard CreateActorHeroCards(ImdbResult response, int i)
        {
            // Set default values in case the API doesn't return a value
            var name = "Unknown";
            var rank = "Unknown";
            var bio = "Unknown";
            var born = "Unknown";
            var knownFor = "Unknown";
            var realName = "Unknown";
            var imageUrl =
                "https://www.publicdomainpictures.net/pictures/280000/velka/not-found-image-15383864787lu.jpg";
            var id = "tt013";

            // Set values returned by the API
            name = SetCardValues.SetName(response, i, name);
            realName = SetCardValues.SetRealName(response, i, name);
            rank = SetCardValues.SetRank(response, i, rank);
            bio = SetCardValues.SetBio(response, i, bio);
            born = SetCardValues.SetBornDate(response, i, born);
            knownFor = SetCardValues.SetKnownFor(response, i, knownFor);
            imageUrl = SetCardValues.SetImageUrl(response, i, imageUrl);
            id = SetCardValues.SetFilmId(response, i, id);

            var heroCard = new HeroCard
            {
                Title = name,
                Subtitle = "IMDb rank: " + rank,
                Text =
                    realName + " was born on " + born + "\r\n" +
                    bio + "\r\n " +
                    "Known for: " + knownFor + "\r\n",
                Images = new List<CardImage> {new CardImage(imageUrl)},
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com" + id)
                },
            };
            return heroCard;
        }

        public async Task<HeroCard> CreateComingSoonCardAsync(List<string> list, bool final)
        {
            StringBuilder myStringBuilder = new StringBuilder();

            if (final is false)
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"- {film.d[0].l} ({film.d[0].y}) - {film.d[0].s}");
                    myStringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Coming Soon movies")
                    },
                };
                return heroCard;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    var extractId = list[i].Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    myStringBuilder.Append($"- {film.d[0].l} ({film.d[0].y}) - {film.d[0].s}");
                    myStringBuilder.AppendLine();

                }
                var movies = "";

                var heroCard = new HeroCard
                {
                    Text = myStringBuilder.ToString(),
                };
                return heroCard;
            }
        }

        public static AnimationCard GetRickAstleyCard()
        {
            var animationCard = new AnimationCard
            {
                Title = "Never Gonna Give You Up",
                Subtitle = "Rick Astley",
                Text =
                    "A story about a guy talking to a girl, and he wants to let her know that he will never gonna give her up, let her down, run around, or desert her. He wouldn't make her cry, say goodbye, tell a lie, or hurt her." + "\r\n " +
                    "Year: " + "1987" + " | " +
                    "Rating: " + "10/10" + " | " +
                    "Genre: " + "Drama" + " | " +
                    "Minutes: " + "3:35" + " | " +
                    "Type: " + "Movie",
           
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://media.giphy.com/media/Ju7l5y9osyymQ/giphy.gif",
                    },
                },
            };

            return animationCard;
        }
    }
}
