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
    public Func<string>? SelectRootFolderFunc { get; set; }
    public Action? ExitAction { get; set; }
    public Action? ShowAboutWindowAction { get; set; }
    public Action? ShowFNExtWindowAction { get; set; }
    public Action? CloseFNExtWindowAction { get; set; }
    public Action? ClearSelectionAction { get; set; }

        #endregion UI Delegates


        #region Commands
    public ICommand SelectRootFolderCommand { get; private set; } = null!;
    public ICommand SaveCommand { get; private set; } = null!;
    public ICommand SearchOnlineCommand { get; private set; } = null!;
    public ICommand ExitCommand { get; private set; } = null!;
    public ICommand OpenAboutCommand { get; private set; } = null!;
    public ICommand TagFromFileNameCommand { get; private set; } = null!;
    public ICommand SaveFromFNCommand { get; private set; } = null!;
    public ICommand ClearCommand { get; private set; } = null!;
    public ICommand ClearSelectionCommand { get; private set; } = null!;

        #endregion Commands


        #region View Properties
    private ObservableCollection<MusicFileTag> _musicFileTags = new();
    private List<MusicFileTag> _selectedItems = new();
    private MusicFileTag? _itemSelected;
    private string _rootPath = string.Empty;
    private string _fileNamePattern = string.Empty;
    private string _resultPreview = string.Empty;
    private string _notificationText = string.Empty;
    private string _notificationColor = "Green";
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
        public MusicFileTag? ItemSelected
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
            ExitCommand = new RelayCommand(() => ExitAction?.Invoke());
            OpenAboutCommand = new RelayCommand(this.OpenAbout);
            ClearCommand = new RelayCommand(this.ClearList);
            ClearSelectionCommand = new RelayCommand(() => ClearSelectionAction?.Invoke());
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
                var acc = loadedFiles.Data ??= new List<MusicFileTag>();
                if (filesFromDirectory.Data != null)
                    acc.AddRange(filesFromDirectory.Data);
            }
            ProcessLoadedMusicFiles(loadedFiles);
            SetDefaultNotification();
        }
        #endregion DragDrop Handler


        #region Command Implementation
        private void SelectRootFolder()
        {
            this.ClearList();

            if (SelectRootFolderFunc == null)
            {
                SetNotification("Root path selector not available", "Red");
                return;
            }
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

        private void Save(MusicFileTag? musicFileTag)
        {
                if (musicFileTag == null)
                {
                    SetNotification("No item selected", "Orange");
                    return;
                }
                Result result = Logic.SaveTagToFile(musicFileTag);
                if (result.Status == Status.Success)
                    SetNotification("Tags saved", "Green");
                else
                    SetNotification(result.Message, "Red");
        }

        private void SaveFromFN()
        {
            CloseFNExtWindowAction?.Invoke();

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
            ResultPreview = SelectedItems.FirstOrDefault()?.FileName ?? string.Empty;
            ShowFNExtWindowAction?.Invoke();
        }

        public void SelectionChanged(IList items)
        {
            if (items == null) return;
            SelectedItems.Clear();
            foreach(MusicFileTag item in items)
            {
                SelectedItems.Add(item);
            }
            // if select only one file we show the details
            if (SelectedItems.Count == 1)
            {
                var first = SelectedItems.FirstOrDefault();
                if (first != null)
                {
                    ItemSelected = first;
                    DetailVM.SetItemToShow(first);
                    DetColWidth = new Avalonia.Controls.GridLength(400, Avalonia.Controls.GridUnitType.Pixel);
                }
                else
                {
                    DetColWidth = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Pixel);
                    ItemSelected = null;
                }
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
            return Task.CompletedTask;
        }

        private void ClearList()
        {
            this.MusicFileTags.Clear();
            this.RootPath = string.Empty;
            SetDefaultNotification();
        }

        private void OpenAbout()
        {
            ShowAboutWindowAction?.Invoke();
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
                var err = loadedFiles.Exception?.Message ?? "Unknown";
                SetNotification($"Error {err} in {loadedFiles.Message}", "Red");
                return;
            }

            if (loadedFiles.Data != null)
                MusicFileTags.AddRange(loadedFiles.Data.Where(w => w.IsValid()));
        }
    }
}