using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicTaggingLight.Logic;
using MusicTaggingLight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicTaggingLight.ViewModels
{
    public class DetailViewModel : ObservableObject
    {
		private MainWindowViewModel mwvm;
		private MusicFileTag? musicFileTag;

		public MusicFileTag? MusicfileTag
		{
			get { return musicFileTag; }
			set 
			{
				SetProperty(ref musicFileTag, value); 
			}
		}


		#region Commands

		public ICommand SaveTagsCommand { get; private set; } = null!;
		public ICommand CancelCommand { get; private set; } = null!;

        #endregion

		private void initCommands()
		{
			SaveTagsCommand = new RelayCommand(this.Save);
			CancelCommand = new RelayCommand(this.Cancel);
		}


        public DetailViewModel(MainWindowViewModel parent)
		{
			this.mwvm = parent;
			initCommands();
		}

		public void SetItemToShow(MusicFileTag fileTag)
		{
			MusicfileTag = fileTag;

		}
	
		public void Save()
		{
			mwvm.SaveCommand.Execute(MusicfileTag);
		}

		private void Cancel()
		{
			// Collapse detail pane and clear selection via parent VM behavior
			mwvm.DetColWidth = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Pixel);
			mwvm.ClearSelectionAction?.Invoke();
		}
	}
}
