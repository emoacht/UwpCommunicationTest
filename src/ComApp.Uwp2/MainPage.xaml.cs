using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ComApp.Uwp2;

public sealed partial class MainPage : Page
{
	#region Properties

	public string InputText
	{
		get => (string)GetValue(InputTextProperty);
		set => SetValue(InputTextProperty, value);
	}
	public static readonly DependencyProperty InputTextProperty =
		DependencyProperty.Register("InputText", typeof(string), typeof(MainPage), new PropertyMetadata(null));

	public string OutputText
	{
		get => (string)GetValue(OutputTextProperty);
		set => SetValue(OutputTextProperty, value);
	}
	public static readonly DependencyProperty OutputTextProperty =
		DependencyProperty.Register("OutputText", typeof(string), typeof(MainPage), new PropertyMetadata(string.Empty));

	#endregion

	public static readonly Size DefaultSize = new(500, 320);

	public MainPage()
	{
		this.InitializeComponent();

		ApplicationView.PreferredLaunchViewSize = DefaultSize;
		ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
	}

	internal async Task SetInputAsync(string text)
	{
		if (Dispatcher.HasThreadAccess)
		{
			InputText = text;
		}
		else
		{
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => InputText = text);
		}
	}

	private async void Send_Click(object sender, RoutedEventArgs e)
	{
		await App.Current.SendAsync(OutputText);
	}
}
