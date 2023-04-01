using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ComApp.Wpf;

public partial class MainWindow : Window
{
	#region Properties

	public string? LogText
	{
		get => (string?)GetValue(LogTextProperty);
		set => SetValue(LogTextProperty, value);
	}
	public static readonly DependencyProperty LogTextProperty =
		DependencyProperty.Register("LogText", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

	public string? InboundText
	{
		get => (string?)GetValue(InboundTextProperty);
		set => SetValue(InboundTextProperty, value);
	}
	public static readonly DependencyProperty InboundTextProperty =
		DependencyProperty.Register("InboundText", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

	public string? OutboundText
	{
		get => (string?)GetValue(OutboundTextProperty);
		set => SetValue(OutboundTextProperty, value);
	}
	public static readonly DependencyProperty OutboundTextProperty =
		DependencyProperty.Register("OutboundText", typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

	#endregion

	public MainWindow()
	{
		InitializeComponent();
		this.Loaded += OnLoaded;
	}

	private async void OnLoaded(object sender, RoutedEventArgs e)
	{
		await ConnectAsync();
	}

	private AppServiceConnection? _appServiceConnection;

	private async ValueTask<bool> ConnectAsync()
	{
		if (_appServiceConnection is not null)
			return true;

		var appServiceConnection = new AppServiceConnection();
		appServiceConnection.AppServiceName = "InProcessAppService";
		appServiceConnection.PackageFamilyName = Package.Current.Id.FamilyName;

		var status = await appServiceConnection.OpenAsync();
		switch (status)
		{
			case AppServiceConnectionStatus.Success:
				_appServiceConnection = appServiceConnection;
				appServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
				return true;

			default:
				LogText = $"Failed: {status}";
				return false;
		}
	}

	private void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
	{
		var text = args.Request.Message["Output"] as string;

		if (Dispatcher.CheckAccess())
		{
			InboundText = text;
		}
		else
		{
			Dispatcher.Invoke(() => InboundText = text);
		}
	}

	private async void Send_Click(object sender, RoutedEventArgs e)
	{
		if (!(await ConnectAsync()))
			return;

		if (_appServiceConnection is null)
			return;

		var response = await _appServiceConnection.SendMessageAsync(new ValueSet
		{
			["Input"] = OutboundText,
		});
		LogText = response.Message?["Result"] as string;
	}
}
