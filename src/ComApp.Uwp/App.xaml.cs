using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ComApp.Uwp;

sealed partial class App : Application
{
	public static new App Current => (App)Application.Current;

	public App()
	{
		this.InitializeComponent();
		this.Suspending += OnSuspending;
	}

	protected override void OnLaunched(LaunchActivatedEventArgs e)
	{
		ApplicationView.GetForCurrentView().TryResizeView(MainPage.DefaultSize);

		var rootFrame = Window.Current.Content as Frame;
		if (rootFrame is null)
		{
			rootFrame = new Frame();
			rootFrame.NavigationFailed += OnNavigationFailed;

			if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
			{
				//TODO: Load state from previously suspended application
			}

			Window.Current.Content = rootFrame;
		}

		if (!e.PrelaunchActivated)
		{
			if (rootFrame.Content is null)
			{
				rootFrame.Navigate(typeof(MainPage), e.Arguments);
			}

			Window.Current.Activate();
		}
	}

	private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
	{
		throw new Exception($"Failed to load Page {e.SourcePageType.FullName}");
	}

	private void OnSuspending(object sender, SuspendingEventArgs e)
	{
		var deferral = e.SuspendingOperation.GetDeferral();
		//TODO: Save application state and stop any background activity
		deferral.Complete();
	}

	private AppServiceConnection _appServiceConnection;
	private BackgroundTaskDeferral _appServiceDeferral;

	protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
	{
		base.OnBackgroundActivated(args);

		if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails appService)
		{
			_appServiceDeferral = args.TaskInstance.GetDeferral();
			args.TaskInstance.Canceled += TaskInstance_Canceled;
			_appServiceConnection = appService.AppServiceConnection;
			_appServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
			_appServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
		}
	}

	private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
	{
		var appServiceDeferral = args.GetDeferral();

		var text = args.Request.Message["Input"] as string;
		await MainPage.Current?.SetInputAsync(text);

		await args.Request.SendResponseAsync(new ValueSet
		{
			["Result"] = $"Accept: {DateTime.Now}"
		});

		appServiceDeferral.Complete();
	}

	private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
	{
		_appServiceDeferral?.Complete();
	}

	private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
	{
		_appServiceDeferral?.Complete();
	}

	internal async Task SendAsync(string text)
	{
		if (_appServiceConnection is null)
			return;

		await _appServiceConnection.SendMessageAsync(new ValueSet
		{
			["Output"] = text,
		});
	}
}
