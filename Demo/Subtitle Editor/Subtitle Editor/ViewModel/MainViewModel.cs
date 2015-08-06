using GalaSoft.MvvmLight;
using AssLoader;
using AssLoader.Collections;
using SubtitleEditor.Model;
using Windows.Storage;
using System;
using System.IO;
using Windows.Foundation;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;

namespace SubtitleEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    class MainViewModel : ViewModelBase
    {
        private static ViewModelLocator locator = new ViewModelLocator();

        private FileOpenPicker openPicker;
        private FileSavePicker savePicker;
        private MessageDialog saveDialog;

        private enum dialogResult
        {
            Yes, No, Cancel
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".ass");

            savePicker = new FileSavePicker() { DefaultFileExtension = ".ass" };
            savePicker.FileTypeChoices.Add(LocalizedStrings.AssFileName, new[] { ".ass" });

            saveDialog = new MessageDialog(LocalizedStrings.SaveDialogContent, LocalizedStrings.SaveDialogTitle);
            saveDialog.Commands.Add(new UICommand(LocalizedStrings.SaveDialogYes, null, dialogResult.Yes));
            saveDialog.Commands.Add(new UICommand(LocalizedStrings.SaveDialogNo, null, dialogResult.No));
            saveDialog.Commands.Add(new UICommand(LocalizedStrings.SaveDialogCancel, null, dialogResult.Cancel));
            saveDialog.DefaultCommandIndex = 0;
            saveDialog.CancelCommandIndex = 2;

            newFile = new RelayCommand(async () =>
            {
                if(!await CleanUpBeforeNewOrOpen())
                    return;
                doc.NewSubtitle();
            });
            openFile = new RelayCommand(async () =>
            {
                if(!await CleanUpBeforeNewOrOpen())
                    return;
                var file = await openPicker.PickSingleFileAsync();
                if(file == null)
                    return;
                await doc.OpenFileAsync(file);
            });
            saveFile = new RelayCommand(async () => await Save(), () => doc.IsModified);

            SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE160",
                Content = LocalizedStrings.SplitViewButtonNew,
                Command = newFile
            });
            SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE8E5",
                Content = LocalizedStrings.SplitViewButtonOpen,
                Command = openFile
            });
            SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE105",
                Content = LocalizedStrings.SplitViewButtonSave,
                Command = saveFile
            });

            DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE1CB",
                Content = LocalizedStrings.SplitViewTabScriptInfo,
                PageType = typeof(View.ScriptInfoPage),
                IsChecked = true
            });
            DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE2B1",
                Content = LocalizedStrings.SplitViewTabStyle,
                PageType = typeof(View.StylePage)
            });
            DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE292",
                Content = LocalizedStrings.SplitViewTabEvent,
                PageType = typeof(View.SubEventPage)
            });
            doc.PropertyChanged += documentPropertyChanged;
        }

        private Document doc = ViewModelLocator.GetForCurrentView().Document;

        public async Task<bool> Save()
        {
            if(doc.CanSave)
                await doc.SaveAsync();
            else
            {
                savePicker.SuggestedFileName = doc.Subtitle.ScriptInfo.Title ?? "";
                var file = await savePicker.PickSaveFileAsync();
                if(file == null)
                    return false;
                await doc.SaveFileAsync(file);
            }
            return true;
        }

        /// <summary>
        /// Ask user to save file if needed.
        /// </summary>
        /// <returns>True if can continue.</returns>
        public async Task<bool> CleanUpBeforeNewOrOpen()
        {
            if(doc.IsModified)
                switch(await showSaveDialog())
                {
                case dialogResult.Yes:
                    return await Save();
                case dialogResult.No:
                    return true;
                default:
                    return false;
                }
            return true;
        }

        private async Task<dialogResult> showSaveDialog()
        {
            return (dialogResult)(await saveDialog.ShowAsync()).Id;
        }

        private void documentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.PropertyName))
            {
                saveFile.RaiseCanExecuteChanged();
                return;
            }
            switch(e.PropertyName)
            {
            case nameof(doc.IsModified):
                saveFile.RaiseCanExecuteChanged();
                break;
            }
        }

        private RelayCommand newFile, openFile, saveFile;

        public ObservableCollection<SplitViewButtonData> SplitViewButtons
        {
            get;
        } = new ObservableCollection<SplitViewButtonData>();

        public ObservableCollection<SplitViewTabData> DocumentTabs
        {
            get;
        } = new ObservableCollection<SplitViewTabData>();

        public SplitViewTabData Preferences
        {
            get;
        } = new SplitViewTabData()
        {
            Icon = "\xE115",
            Content = LocalizedStrings.SplitViewTabPreferences,
            PageType = typeof(View.PreferencesPage)
        };

        private static bool isPreferencesShown;

        private static Window preferencesShownWindow;

        public bool IsPreferencesShown
        {
            get
            {
                return isPreferencesShown;
            }
            set
            {
                if(value)
                    preferencesShownWindow = Window.Current;
                Set(ref isPreferencesShown, value);
            }
        }

        public async Task<bool> ShowPreferences()
        {
            if(isPreferencesShown)
            {
                var from= ApplicationView.GetForCurrentView().Id;
                await preferencesShownWindow.Dispatcher.RunIdleAsync(async e =>
                {
                    preferencesShownWindow.Activate();
                    var to = ApplicationView.GetApplicationViewIdForWindow(preferencesShownWindow.CoreWindow);
                    if(from == to)
                        return;
                    await ApplicationViewSwitcher.SwitchAsync(to, from, ApplicationViewSwitchingOptions.Default);
                });
                return true;
            }
            else
                return false;
        }
    }
}