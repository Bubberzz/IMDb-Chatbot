using System.Collections.Generic;
using IMDb_Chatbot.Models;
using IMDb_Chatbot.Services;
using Xunit;

namespace IMDb_Chatbot.Tests.Unit_Tests.Services
{
    public class SetCardValuesTests
    {
        [Theory]
        [InlineData("Avengers", "Avengers")]
        [InlineData(null, null)]
        [InlineData("Avengers", null)]
        [InlineData(null, "Avengers")]
        public void GivenSetTitle_ThenReturnTitleOrUnknown(string name, string title)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        name = name,
                        title = title
                    }
                }
            };

            // Act
            var result = SetCardValues.SetTitle(searchResult, 0, "Unknown");

            // Assert
            if (name is null && title is null) Assert.Equal("Unknown", result);

            if (name is not null || title is not null) Assert.Equal("Avengers", result);
        }

        [Theory]
        [InlineData("tt2394834")]
        [InlineData(null)]
        public void GivenSetFilmId_ThenReturnIdOrUnknown(string id)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        id = id
                    }
                }
            };

            // Act
            var result = SetCardValues.SetFilmId(searchResult, 0, "tt013");

            // Assert
            Assert.Equal(id is null ? "tt013" : "tt2394834", result);
        }

        [Theory]
        [InlineData("https://www.imdb.com/title/tt0137523/mediaviewer/rm1412004864/")]
        [InlineData(null)]
        public void GivenSetImageUrl_ThenReturnUrlOrNotFound(string url)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root();
            if (url is not null)
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            image = new ImdbSearch.Image
                            {
                                url = url
                            }
                        }
                    }
                };
            else
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            image = null
                        }
                    }
                };

            // Act
            var result = SetCardValues.SetImageUrl(searchResult, 0,
                "https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg");

            // Assert
            Assert.Equal(
                url is null
                    ? "https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg"
                    : "https://www.imdb.com/title/tt0137523/mediaviewer/rm1412004864/", result);
        }

        [Theory]
        [InlineData("Movie")]
        [InlineData(null)]
        public void GivenSetType_ThenReturnTypeOrUnknown(string type)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        titleType = type
                    }
                }
            };

            // Act
            var result = SetCardValues.SetType(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(type is null ? "Unknown" : "Movie", result);
        }

        [Theory]
        [InlineData("127")]
        [InlineData(null)]
        public void GivenSetMinutes_ThenReturnMinutesOrUnknown(string minutes)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        runningTimeInMinutes = minutes
                    }
                }
            };

            // Act
            var result = SetCardValues.SetMinutes(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(minutes is null ? "Unknown" : "127", result);
        }

        [Theory]
        [InlineData("8.9")]
        [InlineData(null)]
        public void GivenSetRating_ThenReturnRatingOrUnknown(string year)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        rating = year
                    }
                }
            };

            // Act
            var result = SetCardValues.SetRating(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(year is null ? "Unknown" : "8.9", result);
        }

        [Theory]
        [InlineData("1999")]
        [InlineData(null)]
        public void GivenSetYear_ThenReturnYearOrUnknown(string year)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        year = year
                    }
                }
            };

            // Act
            var result = SetCardValues.SetYear(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(year is null ? "Unknown" : "1999", result);
        }

        [Theory]
        [InlineData("This is a movie plot")]
        [InlineData(null)]
        public void GivenSetPlot_ThenReturnPlotOrNull(string plot)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        plot = plot
                    }
                }
            };

            // Act
            var result = SetCardValues.SetPlot(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(plot is null ? "Unknown" : "This is a movie plot", result);
        }

        [Theory]
        [InlineData("Chris Hemsworth, Anthony Hopkins, Natalie Portman")]
        [InlineData(null)]
        public void GivenSetActors_ThenReturnActorsOrUnknown(string actors)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root();

            if (actors is not null)
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            principals = new List<ImdbSearch.Principal>
                            {
                                new()
                                {
                                    name = "Chris Hemsworth"
                                },
                                new()
                                {
                                    name = "Anthony Hopkins"
                                },
                                new()
                                {
                                    name = "Natalie Portman"
                                }
                            }
                        }
                    }
                };
            else
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            principals = null
                        }
                    }
                };

            // Act
            var result = SetCardValues.SetActors(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(actors is null ? "Unknown" : "Chris Hemsworth, Anthony Hopkins, Natalie Portman", result);
        }


        [Theory]
        [InlineData("Horror")]
        [InlineData(null)]
        public void GivenSetGenre_ThenReturnGenreOrUnknown(string genre)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        genre = genre
                    }
                }
            };

            // Act
            var result = SetCardValues.SetGenre(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(genre is null ? "Unknown" : "Horror", result);
        }

        [Theory]
        [InlineData("Will Smith")]
        [InlineData(null)]
        public void GivenSetName_ThenReturnNameOrUnknown(string name)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        name = name
                    }
                }
            };

            // Act
            var result = SetCardValues.SetName(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(name is null ? "Unknown" : "Will Smith", result);
        }

        [Theory]
        [InlineData("Unknown", 0)]
        [InlineData("Unknown", 2)]
        public void GivenSetRank_ThenReturnRankOr0(string rank, int actorRank)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root();
            if (actorRank is not 0)
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            actorRank = actorRank
                        }
                    }
                };
            else
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            actorRank = actorRank
                        }
                    }
                };

            // Act
            var result = SetCardValues.SetRank(searchResult, 0, rank);

            // Assert
            Assert.Equal(actorRank is 0 ? "Unknown" : "2", result);
        }

        [Theory]
        [InlineData("This is a biography")]
        [InlineData(null)]
        public void GivenSetBio_ThenReturnBioOrUnknown(string bio)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        bio = bio
                    }
                }
            };

            // Act
            var result = SetCardValues.SetBio(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(bio is null ? "Unknown" : "This is a biography", result);
        }

        [Theory]
        [InlineData("25 January 1968 in Philadelphia, Pennsylvania, USA")]
        [InlineData(null)]
        public void GivenSetBirthDate_ThenReturnBirthDateOrNull(string born)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        born = born
                    }
                }
            };

            // Act
            var result = SetCardValues.SetBirthDate(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(born is null ? "Unknown" : "25 January 1968 in Philadelphia, Pennsylvania, USA", result);
        }


        [Theory]
        [InlineData("Ad Astra, Fight Club")]
        [InlineData(null)]
        public void GivenSetKnownFor_ThenReturnKnownForOrNull(string movies)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root();

            if (movies is null)
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            knownFor = null
                        }
                    }
                };
            else
                searchResult = new ImdbSearch.Root
                {
                    results = new List<ImdbSearch.Result>
                    {
                        new()
                        {
                            knownFor = new List<ImdbSearch.KnownFor>
                            {
                                new()
                                {
                                    title = "Ad Astra"
                                },
                                new()
                                {
                                    title = "Fight Club"
                                }
                            }
                        }
                    }
                };

            // Act
            var result = SetCardValues.SetKnownFor(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(movies is null ? "Unknown" : "Ad Astra, Fight Club", result);
        }

        [Theory]
        [InlineData("William Smith")]
        [InlineData(null)]
        public void GivenSetRealName_ThenReturnRealNameOrNull(string name)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new()
                    {
                        realName = name
                    }
                }
            };

            // Act
            var result = SetCardValues.SetRealName(searchResult, 0, "Unknown");

            // Assert
            Assert.Equal(name is null ? "Unknown" : "William Smith", result);
        }
    }
}