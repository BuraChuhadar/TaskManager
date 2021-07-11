using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Task_Manager.Controller;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Task_Manager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //TODO: Not ideal for resolving dependencies find a better way:
            ProcessController = (ProcessController)App.Services.GetService(typeof(IProcessController));
        }

        private readonly IProcessController ProcessController;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                App.AppServiceConnected += App_AppServiceConnected;
                App.AppServiceDisconnected += App_AppServiceDisconnected;
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private async void App_AppServiceDisconnected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Reconnect();
            });
        }

        private async void Reconnect()
        {
            if (App.IsForeground)
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private async void App_AppServiceConnected(object sender, Windows.ApplicationModel.AppService.AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += Connection_RequestReceived;
            ValueSet message = new ValueSet();
            message.Add("CheckProcesses", "CheckProcesses");

            AppServiceResponse messageResponse = await App.Connection.SendMessageAsync(message);
            var processInfo = (string)messageResponse.Message["ProcessesInfo"];

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var result = ProcessController.ParceProcessInfos(processInfo);
                processGrid.ItemsSource = result;

            });
        }

        private async void Connection_RequestReceived(Windows.ApplicationModel.AppService.AppServiceConnection sender, Windows.ApplicationModel.AppService.AppServiceRequestReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
