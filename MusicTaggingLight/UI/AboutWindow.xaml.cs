using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MusicTaggingLight.UI
{
    /// <summary>
    /// Interaction logic for AboutWindow.axaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TxtIcons_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                OpenUrl(textBlock.Text);
            }
        }

        private void TxtGithub_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                OpenUrl(textBlock.Text);
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
            }
            catch (Exception)
            {
                // Handle exception or ignore
            }
        }
    }
}
