using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using Microsoft.Bot.Schema;

namespace IMDb_Chatbot.Services
{
    public class Cards
    {
        private readonly IImdbService _imdbService;
        private StringBuilder _stringBuilder;

        public Cards(IImdbService imdbService)
        {
            _imdbService = imdbService;
            _stringBuilder = new StringBuilder();
        }

        public static List<HeroCard> GetCard(ImdbSearch.Root response, bool showMoreResults, bool showTryAgainButton)
        {
            var result = new List<HeroCard>();

            for (var i = 0; i < response.results.Count; i++)
            {
                if (response.results[i].id.StartsWith("/title"))
                    switch (showMoreResults)
                    {
                        case true:
                            result.Add(CreateFilmCard(response, i, true, showTryAgainButton));
                            break;
                        case false:
                            result.Add(CreateFilmCard(response, i, false, showTryAgainButton));
                            break;
                    }

                if (response.results[i].id.StartsWith("/name")) result.Add(CreateActorCard(response, i));
            }

            return result;
        }

        public async Task<HeroCard> CreateTopRatedMovieCardAsync(List<TopRatedMovies.Root> list, bool isFinalCard)
        {
            _stringBuilder = new StringBuilder();

            if (isFinalCard is false)
            {
                for (var i = 0; i < 10; i++)
                {
                    var resultId = ExtractResultId.GetId(list, i);
                    var film = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"Rating: {list[i].chartRating} - {film.d[0].l}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Top rated movies")
                    }
                };
                return heroCard;
            }
            else
            {
                for (var i = 0; i < 10; i++)
                {
                    var resultId = ExtractResultId.GetId(list, i);
                    var film = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"Rating: {list[i].chartRating} - {film.d[0].l}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString()
                };
                return heroCard;
            }
        }

        public async Task<HeroCard> CreateTopRatedActorCardAsync(List<TopRatedActors.Root> list, bool isFinalCard)
        {
            _stringBuilder = new StringBuilder();

            if (isFinalCard is false)
            {
                for (var i = 0; i < 10; i++)
                {
                    var resultId = ExtractResultId.GetId(list, i);
                    var actor = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"Rank: {list[i].chartRating} - {actor.d[0].l}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Top rated actors")
                    }
                };
                return heroCard;
            }
            else
            {
                for (var i = 0; i < 10; i++)
                {
                    var resultId = ExtractResultId.GetId(list, i);
                    var actor = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"Rank: {list[i].chartRating} - {actor.d[0].l}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString()
                };
                return heroCard;
            }
        }

        private static HeroCard CreateFilmCard(ImdbSearch.Root response, int i, bool showMoreResultsButton,
            bool showTryAgainButton)
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
                "https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg";
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
                // Card that displays show more search results
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
                    Images = new List<CardImage> {new(imageUrl)},
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.OpenUrl, "Open IMDb",
                            value: "https://www.imdb.com" + filmId),
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Movie Results")
                    }
                };
                return heroCardShowMoreResults;
            }

            // Card that displays try again button
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
                    Images = new List<CardImage> {new(imageUrl)},
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.OpenUrl, "Open IMDb",
                            value: "https://www.imdb.com" + filmId),
                        new(ActionTypes.MessageBack, "Try Again?",
                            value: "IMDb Roulette")
                    }
                };
                return heroCardMovieRouletteCard;
            }

            // Card without 'show more' button
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
                Images = new List<CardImage> {new(imageUrl)},
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com" + filmId)
                }
            };
            return heroCard;
        }

        private static HeroCard CreateActorCard(ImdbSearch.Root response, int i)
        {
            // Set default values in case the API doesn't return a value
            var name = "Unknown";
            var rank = "Unknown";
            var bio = "Unknown";
            var born = "Unknown";
            var knownFor = "Unknown";
            var realName = "Unknown";
            var imageUrl =
                "https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg";
            var id = "tt013";

            // Set values returned by the API
            name = SetCardValues.SetName(response, i, name);
            realName = SetCardValues.SetRealName(response, i, name);
            rank = SetCardValues.SetRank(response, i, rank);
            bio = SetCardValues.SetBio(response, i, bio);
            born = SetCardValues.SetBirthDate(response, i, born);
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
                Images = new List<CardImage> {new(imageUrl)},
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com" + id)
                }
            };
            return heroCard;
        }

        public async Task<HeroCard> CreateComingSoonCardAsync(List<string> list, bool final)
        {
            _stringBuilder = new StringBuilder();

            if (final is false)
            {
                for (var i = 0; i < 10; i++)
                {
                    var extractId = list[i].Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"- {film.d[0].l} ({film.d[0].y}) - {film.d[0].s}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString(),
                    Buttons = new List<CardAction>
                    {
                        new(ActionTypes.MessageBack, "More Results",
                            value: "Show More: Coming Soon movies")
                    }
                };
                return heroCard;
            }
            else
            {
                for (var i = 0; i < 10; i++)
                {
                    var extractId = list[i].Split("/");
                    var resultId = extractId[2];
                    var film = await _imdbService.AutoComplete(resultId);
                    _stringBuilder.Append($"- {film.d[0].l} ({film.d[0].y}) - {film.d[0].s}");
                    _stringBuilder.AppendLine();
                }

                var heroCard = new HeroCard
                {
                    Text = _stringBuilder.ToString()
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
                    "A story about a guy talking to a girl. He wants to let her know that he will never give her up, let her down, run around, or desert her. He wouldn't make her cry, say goodbye, tell a lie, or hurt her." +
                    "\r\n " +
                    "Year: " + "1987" + " | " +
                    "Rating: " + "10/10" + " | " +
                    "Genre: " + "Drama" + " | " +
                    "Minutes: " + "3:35" + " | " +
                    "Type: " + "Movie",
                Media = new List<MediaUrl>
                {
                    new()
                    {
                        Url = "https://media.giphy.com/media/Ju7l5y9osyymQ/giphy.gif"
                    }
                }
            };
            return animationCard;
        }
    }
}