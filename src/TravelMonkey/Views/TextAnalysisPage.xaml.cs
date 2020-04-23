using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelMonkey.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelMonkey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextAnalysisPage : ContentPage
    {
        private readonly TranslationService _translationService =
            new TranslationService();

        string _inputText;
        string _code;
        public TextAnalysisPage(string inputText,string code)
        {
            InitializeComponent();
            countryImage.Source = code;
            _inputText = inputText;
            _code = code;
            TranslateSomeKeys();
            var res = authenticateClient();
            SentimentAnalysis(res);
            KeyPhraseExtraction(res);
        }

        async void TranslateSomeKeys()
        {
            var phrase = await _translationService.TranslateText(lblTitlePhrase.Text);

            foreach (var res in phrase.Translations)
            {
                if (res.Key == _code)
                    lblTitlePhrase.Text = res.Value;
            }
            var score = await _translationService.TranslateText(lblTitleScore.Text);
            foreach (var res in score.Translations)
            {
                if (res.Key == _code)
                    lblTitleScore.Text = res.Value;
            }


        }

        class Phrase
        {
            public string KeyPhrase { get; set; }
        }

        static TextAnalyticsClient authenticateClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(ApiKeys.TextAnalysisApiKey);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = ApiKeys.TextAnalysisEndPoint
            };
            return client;
        }

        public void SentimentAnalysis(ITextAnalyticsClient client)
        {
            var result = client.Sentiment(_inputText, _code);
            lblScore.Text = result.Score.ToString();
        }

        public async void KeyPhraseExtraction(TextAnalyticsClient client)
        {
            var result = client.KeyPhrases(_inputText);

            List<Phrase> phrases = new List<Phrase>();

            if (result.KeyPhrases.Count != 0)
            {
                foreach (string keyphrase in result.KeyPhrases)
                {
                    var phrase =  await _translationService.TranslateText(keyphrase);
                    foreach(var res in phrase.Translations)
                        if (res.Key == _code)
                            phrases.Add(new Phrase { KeyPhrase = res.Value });
                }

                stackPhrases.BindingContext = phrases;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}