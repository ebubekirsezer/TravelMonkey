using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using System;
using TravelMonkey.ViewModels;
using Xamarin.Forms;

namespace TravelMonkey.Views
{
    public partial class TranslationResultPage : ContentPage
    {
        private readonly TranslateResultPageViewModel _translateResultPageViewModel =
            new TranslateResultPageViewModel();
        string _text;

        public TranslationResultPage(string inputText)
        {
            InitializeComponent();

            _text = inputText;
            MessagingCenter.Subscribe<TranslateResultPageViewModel>(this,
                Constants.TranslationFailedMessage,
                async (s) =>
                {
                    await DisplayAlert("Whoops!", "We lost our dictionary, something went wrong while translating", "OK");
                });

            _translateResultPageViewModel.InputText = inputText;

            BindingContext = _translateResultPageViewModel;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Country_Tapped(object sender, EventArgs e)
        {
            var tappedCountry = sender as Label;
            var countryCode = tappedCountry.BindingContext as string;   
            Navigation.PushModalAsync(new TextAnalysisPage(_text,countryCode));
        }
    }
}