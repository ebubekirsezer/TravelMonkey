using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TravelMonkey.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
	{
		private INavigation navigation;

		public INavigation Navigation
		{
			get { return navigation; }
			set { navigation = value; }
		}
		protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value))
                return false;

			field = value;
			RaisePropertyChanged(propertyName);

			return true;
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}