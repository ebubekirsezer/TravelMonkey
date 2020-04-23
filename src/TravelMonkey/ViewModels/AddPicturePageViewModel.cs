using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using TravelMonkey.Data;
using TravelMonkey.Models;
using TravelMonkey.Services;
using TravelMonkey.Views;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public class AddPicturePageViewModel : BaseViewModel,INotifyPropertyChanged
    {
        private readonly ComputerVisionService _computerVisionService = new ComputerVisionService();
        public INavigation Navigation { get; set; }

        public bool ShowImagePlaceholder => !ShowPhoto;
        public bool ShowPhoto => _photoSource != null;
        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        MediaFile _photo;
        StreamImageSource _photoSource;
        List<FaceModel> faceModels = new List<FaceModel>();
        public List<FaceModel> FaceModels
        {
            get => faceModels;
            set => Set(ref faceModels, value);
        }
        public StreamImageSource PhotoSource
        {
            get => _photoSource;
            set
            {
                if (Set(ref _photoSource, value))
                {
                    RaisePropertyChanged(nameof(ShowPhoto));
                    RaisePropertyChanged(nameof(ShowImagePlaceholder));
                }
            }
        }

        private bool _isPosting;
        public bool IsPosting
        {
            get => _isPosting;
            set => Set(ref _isPosting, value);
        }

        private Color _pictureAccentColor = Color.SteelBlue;
        public Color PictureAccentColor
        {
            get => _pictureAccentColor;
            set => Set(ref _pictureAccentColor, value);
        }

        private string _pictureDescription;
        public string PictureDescription
        {
            get => _pictureDescription;
            set => Set(ref _pictureDescription, value);
        }

        public Command TakePhotoCommand { get; }
        public Command AddPictureCommand { get; }

        public AddPicturePageViewModel(INavigation navigation)
        {
            this.Navigation = navigation;
            TakePhotoCommand = new Command(async () => await TakePhoto());
            AddPictureCommand = new Command(() =>
             {
                 MockDataStore.Pictures.Add(new PictureEntry { Description = _pictureDescription, Image = _photoSource });
                 MessagingCenter.Send(this, Constants.PictureAddedMessage);
             });
        }

        private async Task TakePhoto()
        {
            var result = await UserDialogs.Instance.ActionSheetAsync("What do you want to do?",
                "Cancel", null, null, "Take photo", "Choose photo");

            if (result.Equals("Take photo"))
            {
                _photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Small });
                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
                Url = _photo.Path;

            }
            else if (result.Equals("Choose photo"))
            {
                _photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Small });

                PhotoSource = (StreamImageSource)ImageSource.FromStream(() => _photo.GetStream());
                MakeAnalysisRequest(_photo.Path);
            }
            else
            {
                return;
            }

            if (_photo != null)
                await Post();
        }

        private async Task Post()
        {
            if (_photo == null)
            {
                await UserDialogs.Instance.AlertAsync("Please select an image first", "No image selected");
                return;
            }

            IsPosting = true;

            try
            {
                var pictureStream = _photo.GetStreamWithImageRotatedForExternalStorage();
                var result = await _computerVisionService.AddPicture(pictureStream);

                if (!result.Succeeded)
                {
                    MessagingCenter.Send(this, Constants.PictureFailedMessage);
                    return;
                }

                PictureAccentColor = result.AccentColor;

                PictureDescription = result.Description;

                if (!string.IsNullOrWhiteSpace(result.LandmarkDescription))
                    PictureDescription += $". {result.LandmarkDescription}";
            }
            finally
            {
                IsPosting = false;
            }
        }

        public async void MakeAnalysisRequest(string imageFilePath)
        {

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKeys.FaceDetectionApiKey);

            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
               "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
               "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            string uri = ApiKeys.FaceDetectionEndPoint + "?" + requestParameters;
            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();

                //Display Result
                faceModels = JsonConvert.DeserializeObject<List<FaceModel>>(contentString);
                if (faceModels.Count != 0)
                {
                    var res =await Application.Current.MainPage.DisplayAlert("Hmm I see a Face", "Do you want me to Analyze Face?", "OK", "CANCEL");
                    if (res)
                        await Navigation.PushModalAsync(new FaceAnalysisPage(faceModels));
                    //var a = faceModels[0].faceAttributes.gender;
                }
            }
        }
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}