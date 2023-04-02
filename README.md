# UWP Communication Test

Test communications between UWP and WPF by AppService.

- [Create and consume an app service](https://learn.microsoft.com/en-us/windows/uwp/launch-resume/how-to-create-and-consume-an-app-service)
- [Launch an app for results](https://learn.microsoft.com/en-us/windows/uwp/launch-resume/how-to-launch-an-app-for-results)
- [Protocol Activation in UWP](https://stackoverflow.com/questions/38627876/protocol-activation-in-uwp)

[Launcher.LaunchUriAsync](https://learn.microsoft.com/ja-jp/uwp/api/windows.system.launcher.launchuriasync) method (except overload which takes ValueSet) works on WPF but [Launcher.LaunchUriForResultsAsync](https://learn.microsoft.com/ja-jp/uwp/api/windows.system.launcher.launchuriforresultsasync) method seems to always throw InvalidOperationException if called on WPF.
