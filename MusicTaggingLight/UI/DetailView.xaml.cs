using MusicTaggingLight.Models;
using MusicTaggingLight.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MusicTaggingLight.UI
{
    /// <summary>
    /// Interaction logic for DetailView.axaml
    /// </summary>
    public partial class DetailView : UserControl
    {

        private DetailViewModel dvm;

        public DetailView(DetailViewModel vm)
        {
            InitializeComponent();
            this.DataContext = this.dvm = vm;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
