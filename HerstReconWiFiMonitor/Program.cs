using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace HerstReconWiFiMonitor
{
    class Program
    {
        
        public static void Main(string[] args) => BuildAvaloniaApp()
             .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new X11PlatformOptions
                {
                    EnableMultiTouch = true,
                    UseDBusMenu = true,
                    UseEGL = true,
                    UseGpu = true,
                    UseDeferredRendering = true
                })
                .UseSkia()
                .UseX11()
                .LogToTrace();
    }
}
