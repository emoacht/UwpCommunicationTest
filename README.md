# UWP Communication Test

Test communications between UWP and WPF by AppService.

- [Create and consume an app service](https://learn.microsoft.com/en-us/windows/uwp/launch-resume/how-to-create-and-consume-an-app-service)
- [Launch an app for results](https://learn.microsoft.com/en-us/windows/uwp/launch-resume/how-to-launch-an-app-for-results)
- [Protocol Activation in UWP](https://stackoverflow.com/questions/38627876/protocol-activation-in-uwp)

[Launcher.LaunchUriAsync](https://learn.microsoft.com/ja-jp/uwp/api/windows.system.launcher.launchuriasync) method (except overload which takes ValueSet) works on WPF but [Launcher.LaunchUriForResultsAsync](https://learn.microsoft.com/ja-jp/uwp/api/windows.system.launcher.launchuriforresultsasync) method seems to always throw InvalidOperationException if called on WPF.

## How to communicate

### Case 1

Both `ComApp.Uwp` and `ComApp.Wpf` are packaged in `ComApp` packaging project.

1. Build and start `ComApp`. It will install both apps.
2. Start `ComApp.Uwp`. Hit __Launch WPF__ to start `ComApp.Wpf`.
3. Send the messages to each other.

### Case 2

`ComApp.Uwp2` and `ComApp.Wpf2` are independent apps.

1. Build and start `ComApp.Uwp2`. It will install this app.
2. Build and start `ComApp.Wpf2`. It will find the installed `ComApp.Uwp2` and fill FamilyName. Hit __Launch UWP__ to start `ComApp.Uwp2`.
3. Send the messages to each other.
