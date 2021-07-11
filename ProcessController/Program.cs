using Microsoft.PowerShell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ProcessController
{
    class Program
    {

        private static AutoResetEvent appServiceExit;
        private static AppServiceConnection connection;

        public static async void CreateConnection()
        {
            connection = new AppServiceConnection
            {
                AppServiceName = "FilesInteropService",
                PackageFamilyName = Package.Current.Id.FamilyName
            };


            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                // something went wrong ...
                connection.Dispose();
                connection = null;
            }
        }

        static void Main(string[] args)
        {
            // Only one instance of the fulltrust process allowed
            // This happens if multiple instances of the UWP app are launched
            Mutex mutex;
            using (mutex = new Mutex(true, "FilesInteropService", out bool isNew))
            {
                if (!isNew)
                {
                    return;
                }
            }

            try
            {
                appServiceExit = new AutoResetEvent(false);
                CreateConnection();
                appServiceExit.WaitOne();
            }
            finally
            {
                connection?.Dispose();
                appServiceExit?.Dispose();
                mutex?.ReleaseMutex();
            }
        }



        

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            appServiceExit.Set();
           
        }

        private async static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDeferral = args.GetDeferral();
            if (args.Request.Message == null)
            {
                messageDeferral.Complete();
                return;
            }

            try
            {
                if (args.Request.Message.ContainsKey("CheckProcesses"))
                {
                    RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();


                    Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);


                    runspace.Open();

                    RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                    scriptInvoker.Invoke("$ExecutionPolicy = Get-ExecutionPolicy -Scope CurrentUser");
                    scriptInvoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope CurrentUser ");

                    Pipeline pipeline = runspace.CreatePipeline();

                    Command myCommand = new Command(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script/Monitor.ps1"));

                    pipeline.Commands.Add(myCommand);

                    // Execute PowerShell script
                    var results = pipeline.Invoke();
                    scriptInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser $ExecutionPolicy -Force");
                    var stringBuilder = new StringBuilder();
                    foreach (var item in results)
                    {
                        stringBuilder.Append(item);
                    }

                    var responseArray = new ValueSet
                    {
                        { "ProcessesInfo", stringBuilder.ToString() }
                    };

                    await args.Request.SendResponseAsync(responseArray);
                }
            }
            catch
            {
                var responseArray = new ValueSet
                    {
                        { "results", "" }
                    };

                await args.Request.SendResponseAsync(responseArray);
            }
        }
    }
}
