using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Xml.Linq;
using MusicTaggingLight.UI;
using System.Threading.Tasks;

namespace MusicTaggingLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;
        private Dictionary<string,int> OrderDict;

        public MainWindow()
        {
            InitializeComponent();
            vm = this.DataContext as MainWindowViewModel;
            vm.SelectRootFolderFunc = new Func<string>(SelectRootFolderDialog);
            vm.ExitAction = new Action(() => this.Close());
            vm.ShowAboutWindowAction = new Action(this.ShowAboutWindow);
            vm.ShowFNExtWindowAction = new Action(this.ShowFNExtrWindow);
            vm.ClearSelectionAction = new Action(this.ClearSelection);
            OrderDict = GetColumnsOrder();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns the root directory of the selected folder</returns>
        private string SelectRootFolderDialog()
        {
            var task = SelectRootFolderDialogAsync();
            return task.Result;
        }
        
        private async Task<string> SelectRootFolderDialogAsync()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
                {
                    Title = "Select root folder",
                    AllowMultiple = false
                });
                
                if (folders.Count == 1)
                {
                    return folders[0].Path.LocalPath;
                }
            }
            return "";
        }

        private Dictionary<string,int> GetColumnsOrder()
        {
            Dictionary<string, int> orderDict = new Dictionary<string, int>();
            XElement doc = XElement.Load(@"../../Resources/colsorder.xml");
            IEnumerable<XElement> items = doc.Descendants();
            foreach (var item in items)
            {
                orderDict.Add(item.Name.LocalName, int.Parse(item.Value));
            }
            return orderDict;
        }

        private void SaveColumnsOrder()
        {
            XElement el = new XElement(
                "items",
                from keyValue in OrderDict
                select new XElement(keyValue.Key, keyValue.Value));
            XDocument doc = new XDocument(el);
            doc.Save(@"../../Resources/colsorder.xml");
        }


        private async void ShowAboutWindow()
        {
            var about = new AboutWindow();
            this.Opacity = 0.7;
            await about.ShowDialog(this);
            this.Opacity = 1.0;
        }
        private async void ShowFNExtrWindow()
        {
            var dialog = new FromFNWindow(this.vm);
            this.Opacity = 0.7;
            await dialog.ShowDialog(this);
            this.Opacity = 1.0;
        }

        // This event handler will need to be attached manually or converted to work with Avalonia DataGrid
        // For now, commenting out as Avalonia DataGrid has different event model
        /*
        private void dgrFileTags_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString().ToLower() == "albumcover")
            {
                e.Cancel = true;
            }
            else
            {
                e.Column.DisplayIndex = OrderDict[e.Column.Header.ToString()];
            }
        }
        */

        private void dgrFileTags_Drop(object sender, DragEventArgs e)
        {
            vm.ClearCommand.Execute(null);

            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles();
                var data = files?.Select(f => f.Path.LocalPath).ToArray() ?? Array.Empty<string>();

                if (!CheckForMp3(data))
                    return;

                var directories = new List<string>();
                var filesList = new List<string>();

                // Validate, if dropped data are files or directories.
                // Both have different approaches how to handle the input.
                foreach (var d in data)
                {
                    if (IsDirectory(d))
                        directories.Add(d);
                    else
                        filesList.Add(d);
                }

                if (directories.Count > 0)
                    vm.DragDropDirectory(directories);

                if (filesList.Count > 0)
                    vm.DragDropFiles(filesList);
            }
        }

        /// <summary>
        /// Check if a given path is a directory or a file
        /// </summary>
        /// <param name="path">string, which has to be validated</param>
        /// <returns>true if the param is a directory, false if it's a file</returns>
        private bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                vm.SelectionChanged(dataGrid.SelectedItems);
            }
        }

        private void ClearSelection()
        {
            var dataGrid = this.FindControl<DataGrid>("dgrFileTags");
            if (dataGrid != null)
            {
                dataGrid.SelectedItems.Clear();
            }
        }

        private bool CheckForMp3(string[] data)
        {
            foreach (var d in data)
            {
                var info = new FileInfo(d);
                if (info.Extension == ".mp3")
                {
                    return true;
                }
            }

            vm.SetNotification("Only *.mp3 files supported!", "Orange");
            return false;
        }

        private void dgrFileTags_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var grid = (DataGrid)sender;
            OrderDict.Clear();
            foreach (var column in grid.Columns)
            {
                OrderDict.Add(column.Header.ToString(), column.DisplayIndex);
            }
            SaveColumnsOrder();
        }
    }
}