using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelMonkey.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelMonkey.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaceAnalysisPage : ContentPage
    {
        string isMakeup = "There is no makeup on the Face";
        string isEmotion = "No Emotion";
        public FaceAnalysisPage(List<FaceModel> faceModels)
        {
            InitializeComponent();
            if (faceModels[0].faceAttributes.makeup.eyeMakeup == true || faceModels[0].faceAttributes.makeup.lipMakeup == true)
                isMakeup = "There is make up on the Face";
            if (faceModels[0].faceAttributes.emotion.sadness >= 0.5)
                isEmotion = "Person is Sad";
            if (faceModels[0].faceAttributes.emotion.happiness >= 0.5)
                isEmotion = "Person is Happy";
            if (faceModels[0].faceAttributes.emotion.anger >= 0.5)
                isEmotion = "Person is Angry";
            personCount.Text = faceModels.Count.ToString();
            personAge.Text = faceModels[0].faceAttributes.age.ToString();
            personGender.Text = faceModels[0].faceAttributes.gender;
            personMakeup.Text = isMakeup;
            personGlasses.Text = faceModels[0].faceAttributes.glasses;
            personEmotion.Text = isEmotion;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}