# MusicTaggingLight

This is a small cross-platform tool, which helps to tag mp3 Files. 
You select a root Folder and all \*.mp3 song files are loaded in the UI.
There you can edit the ID3 Tags through the DataGrid or the PropertyGrid.
For saving the edited data, just click the button or use the Ctrl+S shortcut.

**Now supports both Windows and Linux!**

![preview](https://github.com/jvegaf/MusicTaggingLight/blob/master/musictagginglight.png)
![preview](https://github.com/jvegaf/MusicTaggingLight/blob/master/musictagginglight-edit.png)

## Cross-Platform Support

The application has been refactored to run on both Windows and Linux using:
- **.NET 8** - Modern cross-platform runtime
- **Avalonia UI** - Cross-platform XAML UI framework
- **Cross-platform file dialogs** - Native file system integration
- **Platform-specific URL opening** - Works on Windows, Linux, and macOS

## Running the Application

### Prerequisites
- .NET 8 Runtime or SDK

### Windows
```bash
dotnet run
```

### Linux
```bash
dotnet run
```
*Note: Requires X11 display server or Wayland*

### macOS
```bash
dotnet run
```

## For Developers - what to contribute?
This project is far from being finished. I just implemented the basic stuff for tagging \*.mp3 files.
Some ideas to make this tool a bit better are:
- implementing a "auto-tagging through online search" functionality. For this, I want to implement a search through "freedb" and maybe other APIs/WebServices.
- reading tags from CD Text
- removing the "save" button and save the tags while editing. Like- you edit a tag and when you go over to the next one, the just edited tag is already saved.
But it would definitely need the functionality to rollback everything, if something goes wrong.
- support for more file types. Currently, only \*.mp3 files are supported. It would be a more useful tool, if there would be the possibility to edit other files as well.

## Used Libraries
- CommunityToolkit.Mvvm (NuGet) - Cross-platform MVVM framework
- taglib-sharp-netstandard2.0 (NuGet) - Cross-platform audio metadata library
- Avalonia (NuGet) - Cross-platform .NET UI framework
- Avalonia.Desktop (NuGet) - Desktop platform support
- Avalonia.Themes.Fluent (NuGet) - Modern UI theme
- Avalonia.Controls.DataGrid (NuGet) - DataGrid control
- icons8 for Icons (https://icons8.com/)

## Architecture Changes

The application was migrated from WPF (.NET Framework 4.5.2) to Avalonia UI (.NET 8) for cross-platform compatibility:

### Before (WPF - Windows Only)
- .NET Framework 4.5.2
- WPF UI framework
- Ookii.Dialogs.Wpf for dialogs
- DevExpress.Mvvm for MVVM
- Windows-specific Process.Start calls

### After (Avalonia - Cross-Platform)  
- .NET 8
- Avalonia UI framework
- Native cross-platform dialogs
- CommunityToolkit.Mvvm for MVVM
- Platform-aware Process.Start calls
