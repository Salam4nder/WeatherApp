using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Weather.Models;
using Weather.Services;
using ListView = Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ListView;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        GroupedForecast groupedforecast;
        private string _city = string.Empty;

        public ForecastPage()
        {
            InitializeComponent();
            
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Code here will run right before the screen appears
            //You want to set the Title or set the City
            _city = Title;

            //This is making the first load of data
            MainThread.BeginInvokeOnMainThread(async () => {await LoadForecast();});
        }

        private async Task LoadForecast()
        {
            //Heare you load the forecast
            var result = await service.GetForecastAsync(_city);
            groupedforecast.City = result.City;
            groupedforecast.Items = result.Items.GroupBy(x => x.DateTime.Date);
            WeatherDataList.ItemsSource = groupedforecast.Items;
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            await LoadForecast();
        }
    }
}