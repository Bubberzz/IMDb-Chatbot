using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Moq;

namespace IMDb_Chatbot.Tests.Common
{
    public static class SimpleMockFactory
    {
        public static Mock<T> CreateMockDialog<T>(object expectedResult = null, params object[] constructorParams)
            where T : Dialog
        {
            var mockDialog = new Mock<T>(constructorParams);
            var mockDialogNameTypeName = typeof(T).Name;
            mockDialog
                .Setup(x => x.BeginDialogAsync(It.IsAny<DialogContext>(), It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                .Returns(async (DialogContext dialogContext, object options, CancellationToken cancellationToken) =>
                {
                    await dialogContext.Context.SendActivityAsync($"{mockDialogNameTypeName} mock invoked",
                        cancellationToken: cancellationToken);

                    return await dialogContext.EndDialogAsync(expectedResult, cancellationToken);
                });

            return mockDialog;
        }

        public static Mock<TRecognizer> CreateMockLuisRecognizer<TRecognizer, TReturns>(TReturns returns,
            params object[] constructorParams)
            where TRecognizer : class, IRecognizer
            where TReturns : IRecognizerConvert, new()
        {
            var mockRecognizer = new Mock<TRecognizer>(constructorParams);
            mockRecognizer
                .Setup(x => x.RecognizeAsync<TReturns>(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(returns));
            return mockRecognizer;
        }

        public static Mock<TRecognizer> CreateMockLuisRecognizer<TRecognizer>(RecognizerResult returns,
            params object[] constructorParams)
            where TRecognizer : class, IRecognizer
        {
            var mockRecognizer = new Mock<TRecognizer>(constructorParams);
            mockRecognizer
                .Setup(x => x.RecognizeAsync(It.IsAny<ITurnContext>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(returns));
            return mockRecognizer;
        }
    }
}