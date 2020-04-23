namespace TravelMonkey
{
	public static class ApiKeys
	{
        #warning You need to set up your API keys.
		public static string ComputerVisionApiKey = "2e9223ee697640f7becd960bdab55f77";
		public static string TranslationsApiKey = "8e45a578f3b74d41977c1baf5b7cf4a7";
		public static string BingImageSearch = "0cde39557e7243d1b9dd4da4c3047b97";
		public static string TextAnalysisApiKey = "294dedcfddda47d2b131c9a08fe40c9c";
		public static string FaceDetectionApiKey = "d960b7f532cc4c70a70c3abe5b74dcd7";

		// Change this to the Azure Region you are using
		public static string ComputerVisionEndpoint = "https://westus2.api.cognitive.microsoft.com/";
		public static string TranslationsEndpoint = "https://api.cognitive.microsofttranslator.com/";
		public static string TextAnalysisEndPoint = "https://ebubekiranalysis.cognitiveservices.azure.com/";
		public static string FaceDetectionEndPoint = "https://ebubekirdetection.cognitiveservices.azure.com/face/v1.0/detect";
	}
}