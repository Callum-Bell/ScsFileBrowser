using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScsFileBrowser.AvaloniaUI.Views;

namespace ScsFileBrowser.AvaloniaUI;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow(desktop.Args);

        base.OnFrameworkInitializationCompleted();
    }
}
