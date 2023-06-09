﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ComApp.Uwp;

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

	public static MainPage Current { get; private set; }

	public static readonly Size DefaultSize = new(500, 320);

	public MainPage()
	{
		this.InitializeComponent();
		Current = this;

		ApplicationView.PreferredLaunchViewSize = DefaultSize;
		ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
	}

	private async void Launch_Click(object sender, RoutedEventArgs e)
	{
		await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
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
