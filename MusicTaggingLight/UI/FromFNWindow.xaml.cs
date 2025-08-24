using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Windows.Input;

namespace MusicTaggingLight.UI
{
    /// <summary>
    /// Interaction logic for FromFNWindow.axaml
    /// </summary>
    public partial class FromFNWindow : Window
    {
        private MainWindowViewModel vm;
        public FromFNWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            this.DataContext = vm;
        }
        public FromFNWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = this.vm = vm;
            vm.CloseFNExtWindowAction = new Action(this.Close);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object? sender, RoutedEventArgs e)
        {
            // Trigger the SaveFromFN command exposed by MainWindowViewModel
            vm.SaveFromFNCommand.Execute(null);
        }
    }
}
