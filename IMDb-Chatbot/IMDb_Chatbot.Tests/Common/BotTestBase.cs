using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace IMDb_Chatbot.Tests.Common
{
    public abstract class BotTestBase
    {
        // A lazy configuration object that gets instantiated once during execution when is needed
        private static readonly Lazy<IConfiguration> _configurationLazy = new(() =>
        {
            LoadLaunchSettingsIntoEnvVariables("Properties//launchSettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            return config.Build();
        });

        protected readonly string ExpectedComingSoonMoviesContent =
            "- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n- The Shawshank Redemption (1994) - Tim Robbins, Morgan Freeman\r\n";

        protected readonly string ExpectedMoreResults =
            "[{\"contentType\":\"application/vnd.microsoft.card.hero\",\"contentUrl\":null,\"content\":{\"title\":\"The Shawshank Redemption\",\"subtitle\":\"J.J. Carroll, Matt Tory, Jay Funsch\",\"text\":\"Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\\r\\n Year: 2020 | Rating: 9.3 | Minutes: Unknown | Genre: Drama | Type: tvEpisode\",\"images\":[{\"url\":\"https://m.media-amazon.com/images/M/MV5BMmU2YTM5ZTMtYzBjMi00ZWVmLTlmNGMtYTY1YjEwMzc2MGEzXkEyXkFqcGdeQXVyNDE5MzY5MDc@._V1_.jpg\",\"alt\":null,\"tap\":null}],\"buttons\":[{\"type\":\"openUrl\",\"title\":\"Open IMDb\",\"image\":null,\"text\":null,\"displayText\":null,\"value\":\"https://www.imdb.com/title/tt12286264/\",\"channelData\":null,\"imageAltText\":null}],\"tap\":null},\"name\":null,\"thumbnailUrl\":null},{\"contentType\":\"application/vnd.microsoft.card.hero\",\"contentUrl\":null,\"content\":{\"title\":\"The Shawshank Redemption\",\"subtitle\":\"Ron Shahar, Alon Apple, Nitzan Avital\",\"text\":\"Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\\r\\n Year: 2011 | Rating: 9.3 | Minutes: Unknown | Genre: Drama | Type: tvEpisode\",\"images\":[{\"url\":\"https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg\",\"alt\":null,\"tap\":null}],\"buttons\":[{\"type\":\"openUrl\",\"title\":\"Open IMDb\",\"image\":null,\"text\":null,\"displayText\":null,\"value\":\"https://www.imdb.com/title/tt14308968/\",\"channelData\":null,\"imageAltText\":null}],\"tap\":null},\"name\":null,\"thumbnailUrl\":null},{\"contentType\":\"application/vnd.microsoft.card.hero\",\"contentUrl\":null,\"content\":{\"title\":\"The Shawshank Redemption: Behind the Scenes\",\"subtitle\":\"Frank Darabont, Morgan Freeman, Tim Robbins\",\"text\":\"Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\\r\\n Year: 2004 | Rating: 9.3 | Minutes: 9 | Genre: Drama | Type: video\",\"images\":[{\"url\":\"https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg\",\"alt\":null,\"tap\":null}],\"buttons\":[{\"type\":\"openUrl\",\"title\":\"Open IMDb\",\"image\":null,\"text\":null,\"displayText\":null,\"value\":\"https://www.imdb.com/title/tt5443386/\",\"channelData\":null,\"imageAltText\":null}],\"tap\":null},\"name\":null,\"thumbnailUrl\":null},{\"contentType\":\"application/vnd.microsoft.card.hero\",\"contentUrl\":null,\"content\":{\"title\":\"The Shawshank Redemption: Cast Interviews\",\"subtitle\":\"Clancy Brown, Morgan Freeman, Bob Gunton\",\"text\":\"Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\\r\\n Year: 2004 | Rating: 9.3 | Minutes: 134 | Genre: Drama | Type: video\",\"images\":[{\"url\":\"https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/not-found.jpg\",\"alt\":null,\"tap\":null}],\"buttons\":[{\"type\":\"openUrl\",\"title\":\"Open IMDb\",\"image\":null,\"text\":null,\"displayText\":null,\"value\":\"https://www.imdb.com/title/tt5443390/\",\"channelData\":null,\"imageAltText\":null}],\"tap\":null},\"name\":null,\"thumbnailUrl\":null}]";

        protected readonly string ExpectedTopRatedActorsContent =
            "Rank: 1 - Sophia Di Martino\r\nRank: 2 - Sophia Di Martino\r\nRank: 3 - Sophia Di Martino\r\nRank: 4 - Sophia Di Martino\r\nRank: 5 - Sophia Di Martino\r\nRank: 6 - Sophia Di Martino\r\nRank: 7 - Sophia Di Martino\r\nRank: 8 - Sophia Di Martino\r\nRank: 9 - Sophia Di Martino\r\nRank: 10 - Sophia Di Martino\r\n";

        protected readonly string ExpectedTopRatedActorsContentLastResult =
            "Rank: 71 - Sophia Di Martino\r\nRank: 72 - Sophia Di Martino\r\nRank: 73 - Sophia Di Martino\r\nRank: 74 - Sophia Di Martino\r\nRank: 75 - Sophia Di Martino\r\nRank: 76 - Sophia Di Martino\r\nRank: 77 - Sophia Di Martino\r\nRank: 78 - Sophia Di Martino\r\nRank: 79 - Sophia Di Martino\r\nRank: 80 - Sophia Di Martino\r\n";

        protected readonly string ExpectedTopRatedMoviesContent =
            "Rating: 9.2 - The Shawshank Redemption\r\nRating: 9.1 - The Shawshank Redemption\r\nRating: 9 - The Shawshank Redemption\r\nRating: 9 - The Shawshank Redemption\r\nRating: 8.9 - The Shawshank Redemption\r\nRating: 8.9 - The Shawshank Redemption\r\nRating: 8.9 - The Shawshank Redemption\r\nRating: 8.8 - The Shawshank Redemption\r\nRating: 8.8 - The Shawshank Redemption\r\nRating: 8.8 - The Shawshank Redemption\r\n";

        protected readonly string ExpectedTopRatedMoviesContentLastResult =
            "Rating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\nRating: 8.3 - The Shawshank Redemption\r\n";

        protected BotTestBase()
            : this(null)
        {
        }

        protected BotTestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        public virtual IConfiguration Configuration => _configurationLazy.Value;

        protected ITestOutputHelper Output { get; }

        private static void LoadLaunchSettingsIntoEnvVariables(string launchSettingsFile)
        {
            if (!File.Exists(launchSettingsFile)) return;

            using (var file = File.OpenText(launchSettingsFile))
            {
                var reader = new JsonTextReader(file);
                var fileData = JObject.Load(reader);

                var variables = fileData
                    .GetValue("profiles")
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
            }
        }
    }
}