using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;

namespace ComApp.Wpf2;

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

	public string? FamilyName
	{
		get => (string?)GetValue(FamilyNameProperty);
		set => SetValue(FamilyNameProperty, value);
	}
	public static readonly DependencyProperty FamilyNameProperty =
		DependencyProperty.Register("FamilyName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

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

		FamilyName = PackageHelper.GetFamilyName(Properties.Settings.Default.IdentityName);
		OutboundText = @$"Input:{DateTimeOffset.Now}";
	}

	private async void Launch_Click(object sender, RoutedEventArgs e)
	{
		var result = await Launcher.LaunchUriAsync(new Uri($"{Properties.Settings.Default.ProtocolName}:"));
		LogText = $"Launched: {result}";
	}

	private AppServiceConnection? _appServiceConnection;

	private async ValueTask<bool> ConnectAsync()
	{
		if (_appServiceConnection is not null)
			return true;

		if (string.IsNullOrWhiteSpace(FamilyName))
		{
			LogText = "Failed: FamilyName must be provided.";
			return false;
		}

		var appServiceConnection = new AppServiceConnection
		{
			AppServiceName = Properties.Settings.Default.ServiceName,
			PackageFamilyName = FamilyName
		};

		var status = await appServiceConnection.OpenAsync();
		switch (status)
		{
			case AppServiceConnectionStatus.Success:
				_appServiceConnection = appServiceConnection;
				_appServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
				_appServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
				return true;

			default:
				LogText = $"Failed: {status}";
				return false;
		}
	}

	private void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
	{
		var text = GetString(args.Request.Message);

		if (Dispatcher.CheckAccess())
		{
			InboundText = text;
		}
		else
		{
			Dispatcher.Invoke(() => InboundText = text);
		}
	}

	private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
	{
		Dispatcher.Invoke(() => LogText = $"Closed: {args.Status}");

		sender.RequestReceived -= AppServiceConnection_RequestReceived;
		sender.ServiceClosed -= AppServiceConnection_ServiceClosed;
		_appServiceConnection?.Dispose();
		_appServiceConnection = null;
	}

	private async void Send_Click(object sender, RoutedEventArgs e)
	{
		if (!(await ConnectAsync()))
			return;

		var requestMessage = new ValueSet();

		foreach (var item in OutboundText?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
		{
			var elements = item.Split(':', 2);
			if (elements.Length >= 2)
			{
				requestMessage.Add(elements[0].Trim(), elements[1].Trim());
			}
		}

		Debug.Assert(_appServiceConnection is not null);

		var response = await _appServiceConnection.SendMessageAsync(requestMessage);

		LogText = GetString(response.Message);
	}

	public static string GetString(ValueSet? message)
	{
		if (message is null)
			return string.Empty;

		var buffer = new StringBuilder();
		foreach (var item in message)
		{
			buffer.AppendLine($"{item.Key}:{item.Value}");
		}
		return buffer.ToString().TrimEnd();
	}
}
