using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MusicTaggingLight.UI
{
    /// <summary>
    /// Interaction logic for FromFNWindow.axaml
    /// </summary>
    public partial class FromFNWindow : Window
    {
 
        public FromFNWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
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
    }
}
