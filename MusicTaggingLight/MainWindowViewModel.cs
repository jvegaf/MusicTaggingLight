 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicTaggingLight.Models;
using MusicTaggingLight.Logic;
using TagLib;
using File = TagLib.File;
using System.Collections;
using MusicTaggingLight.ViewModels;
using MusicTaggingLight.UI;
using Avalonia.Controls;

namespace MusicTaggingLight
{
    public class MainWindowViewModel : ObservableObject
    {
        public TaggingLogic Logic { get; set; }
        public DetailViewModel DetailVM { get; set; }
        public DetailView DetailView { get; set; }

        #region UI Delegates
        public Func<string> SelectRootFolderFunc { get; set; }
        public Action ExitAction { get; set; }
        public Action ShowAboutWindowAction { get; set; }
        public Action ShowFNExtWindowAction { get; set; }
        public Action CloseFNExtWindowAction { get; set; }
        public Action ClearSelectionAction { get; set; }

        #endregion UI Delegates


        #region Commands
        public ICommand SelectRootFolderCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SearchOnlineCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand OpenAboutCommand { get; set; }
        public ICommand TagFromFileNameCommand { get; set; }
        public ICommand SaveFromFNCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand ClearSelectionCommand { get; set; }

        #endregion Commands


        #region View Properties
        private ObservableCollection<MusicFileTag> _musicFileTags;
        private List<MusicFileTag> _selectedItems;
        private MusicFileTag _itemSelected;
        private string _rootPath;
        private string _fileNamePattern;
        private string _resultPreview;
        private string _notificationText;
        private string _notificationColor;
        private Avalonia.Controls.GridLength _detColWidth;

        public string NotificationColor
        {
            get { return _notificationColor; }
            set { SetProperty(ref _notificationColor, value); }
        }
        public string NotificationText
        {
            get { return _notificationText; }
            set { SetProperty(ref _notificationText, value); }
        }
        public string RootPath
        {
            get { return _rootPath; }
            set { SetProperty(ref _rootPath, value); }
        }

        public string FileNamePattern
        {
            get { return _fileNamePattern; }
            set 
            { 
                SetProperty(ref _fileNamePattern, value); 
            }
        }
        public string ResultPreview
        {
            get { return _resultPreview; }
            set 
            { 
                SetProperty(ref _resultPreview, value); 
            }
        }
        public List<MusicFileTag> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                SetProperty(ref _selectedItems, value);
            }
        }
        public MusicFileTag ItemSelected
        {
            get { return _itemSelected; }
            set
            {
                SetProperty(ref _itemSelected, value);
            }
        }
        public Avalonia.Controls.GridLength DetColWidth
        {
            get { return _detColWidth; }
            set
            {
                SetProperty(ref _detColWidth, value);
            }
        }
        public ObservableCollection<MusicFileTag> MusicFileTags
        {
            get { return _musicFileTags; }
            set { SetProperty(ref _musicFileTags, value); }
        }
        #endregion View Properties

        public MainWindowViewModel()
        {
            DetailVM = new DetailViewModel(this);
            DetailView = new DetailView(DetailVM);
            MusicFileTags = new ObservableCollection<MusicFileTag>();
            SelectedItems = new List<MusicFileTag>();
            Logic = new TaggingLogic();
            DetColWidth = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Pixel);
            InitCommands();
            SetDefaultNotification();
        }
        private void InitCommands()
        {
            SelectRootFolderCommand = new RelayCommand(SelectRootFolder);
            SaveCommand = new RelayCommand<MusicFileTag?>(Save);
            SearchOnlineCommand = new AsyncRelayCommand(SearchOnline);
            TagFromFileNameCommand = new RelayCommand(this.TagFromFilename);
            SaveFromFNCommand = new RelayCommand(this.SaveFromFN);
            ExitCommand = new RelayCommand(() => ExitAction.Invoke());
            OpenAboutCommand = new RelayCommand(this.OpenAbout);
            ClearCommand = new RelayCommand(this.ClearList);
            ClearSelectionCommand = new RelayCommand(() => ClearSelectionAction.Invoke());
        }


        #region DragDrop Handler
        internal void DragDropFiles(List<string> musicFiles)
        {
            Result<List<MusicFileTag>> loadedFiles = Logic.LoadMusicFilesFromList(musicFiles);
            ProcessLoadedMusicFiles(loadedFiles);
            SetDefaultNotification();
        }

        internal void DragDropDirectory(List<string> directories)
        {
            Result<List<MusicFileTag>> loadedFiles = new Result<List<MusicFileTag>>(new List<MusicFileTag>());
            foreach (var dir in directories)
            {
                Result<List<MusicFileTag>> filesFromDirectory = Logic.LoadMusicFilesFromRoot(dir);
                loadedFiles.Data.AddRange(filesFromDirectory.Data);
            }
            ProcessLoadedMusicFiles(loadedFiles);
            SetDefaultNotification();
        }
        #endregion DragDrop Handler


        #region Command Implementation
        private void SelectRootFolder()
        {
            this.ClearList();

            RootPath = SelectRootFolderFunc.Invoke();
            if (String.IsNullOrEmpty(RootPath))
            {
                SetNotification("Root path not set", "Red");
                return;
            }

            SetDefaultNotification();

            Result<List<MusicFileTag>> loadedFiles = Logic.LoadMusicFilesFromRoot(RootPath);
            ProcessLoadedMusicFiles(loadedFiles);
        }

        private void Save(MusicFileTag musicFileTag)
        {
                Result result = Logic.SaveTagToFile(musicFileTag);
                if (result.Status == Status.Success)
                    SetNotification("Tags saved", "Green");
                else
                    SetNotification(result.Message, "Red");
        }

        private void SaveFromFN()
        {
            CloseFNExtWindowAction.Invoke();

            foreach ( var item in SelectedItems)
            {
                Result result = Logic.SaveTagsExtractedFromFilename(FileNamePattern, item);
                if (result.Status == Status.Success)
                    SetNotification("Tags saved", "Green");
                else
                    SetNotification(result.Message, "Red");
            }
            SelectedItems.Clear();
        }
        
        private void TagFromFilename()
        {
            if (SelectedItems.Count < 1)
                return;
            FileNamePattern = "%artist%-%title%";
            ResultPreview = SelectedItems.FirstOrDefault().FileName;
            ShowFNExtWindowAction.Invoke();
        }

        public void SelectionChanged(IList items)
        {
            SelectedItems.Clear();
            foreach(MusicFileTag item in items)
            {
                SelectedItems.Add(item);
            }
            // if select only one file we show the details
            if (SelectedItems.Count.Equals(1))
            {
                ItemSelected = SelectedItems.First();
                DetailVM.SetItemToShow(ItemSelected);
                DetColWidth = new Avalonia.Controls.GridLength(400, Avalonia.Controls.GridUnitType.Pixel);
            }
            else
            {
                DetColWidth = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Pixel);
                ItemSelected = null;
            }
        }

        private Task SearchOnline()
        {
            // not implemented yet
            return null;
        }

        private void ClearList()
        {
            this.MusicFileTags.Clear();
            this.RootPath = string.Empty;
            SetDefaultNotification();
        }

        private void OpenAbout()
        {
            ShowAboutWindowAction.Invoke();
        }
        #endregion Command Implementation


        #region Notification Methods
        private void SetDefaultNotification()
        {
            this.SetNotification("Ready", "Green");
        }

        public void SetNotification(string text, string color)
        {
            this.NotificationText = text;
            this.NotificationColor = color;
        }

        #endregion Notification Methods

        private void ProcessLoadedMusicFiles(Result<List<MusicFileTag>> loadedFiles)
        {
            if (loadedFiles.Status != Status.Success)
            {
                SetNotification($"Error {loadedFiles.Exception.Message} in {loadedFiles.Message}", "Red");
                return;
            }

            MusicFileTags.AddRange(loadedFiles.Data.Where(w => w.IsValid()));
        }
    }
}