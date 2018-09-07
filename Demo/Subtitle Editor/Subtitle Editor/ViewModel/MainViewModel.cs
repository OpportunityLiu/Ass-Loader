using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using SubtitleEditor.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
    internal class MainViewModel : EditorViewModelBase
    {
        private static readonly ViewModelLocator locator = new ViewModelLocator();

        private FileOpenPicker openPicker;
        private readonly FileSavePicker savePicker;
        private readonly MessageDialog saveDialog;

        private readonly bool isMobile;

        private enum dialogResult
        {
            Yes, No, Cancel
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                this.isMobile = true;
            this.openPicker = new FileOpenPicker();
            this.openPicker.FileTypeFilter.Add(".ass");

            this.savePicker = new FileSavePicker() { DefaultFileExtension = ".ass" };
            this.savePicker.FileTypeChoices.Add(LocalizedStrings.Resources.AssFileName, new[] { ".ass" });

            this.saveDialog = new MessageDialog(LocalizedStrings.Resources.SaveDialogContent, LocalizedStrings.Resources.SaveDialogTitle);
            this.saveDialog.Commands.Add(new UICommand(LocalizedStrings.Resources.SaveDialogYes, null, dialogResult.Yes));
            this.saveDialog.Commands.Add(new UICommand(LocalizedStrings.Resources.SaveDialogNo, null, dialogResult.No));
            this.saveDialog.DefaultCommandIndex = 0;
            if (!this.isMobile)
            {
                this.saveDialog.Commands.Add(new UICommand(LocalizedStrings.Resources.SaveDialogCancel, null, dialogResult.Cancel));
                this.saveDialog.CancelCommandIndex = 2;
            }

            this.newFile = new RelayCommand(async () =>
            {
                if (!await this.CleanUpBeforeNewOrOpen())
                    return;
                this.Document.NewSubtitle();
            });
            this.openFile = new RelayCommand(async () =>
            {
                if (!await this.CleanUpBeforeNewOrOpen())
                    return;
                var file = await this.openPicker.PickSingleFileAsync();
                if (file == null)
                    return;
                await this.Document.OpenFileAsync(file);
            });
            this.saveFile = new RelayCommand(async () => await this.Save(), () => this.Document.IsModified);

            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE160",
                Content = LocalizedStrings.Resources.SplitViewButtonNew,
                Command = newFile
            });
            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE8E5",
                Content = LocalizedStrings.Resources.SplitViewButtonOpen,
                Command = openFile
            });
            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE105",
                Content = LocalizedStrings.Resources.SplitViewButtonSave,
                Command = saveFile
            });

            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE1CB",
                Content = LocalizedStrings.Resources.SplitViewTabScriptInfo,
                PageType = typeof(View.ScriptInfoPage),
                IsChecked = true
            });
            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE2B1",
                Content = LocalizedStrings.Resources.SplitViewTabStyle,
                PageType = typeof(View.StylePage)
            });
            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE292",
                Content = LocalizedStrings.Resources.SplitViewTabEvent,
                PageType = typeof(View.SubEventPage)
            });
            this.Document.PropertyChanged += this.documentPropertyChanged;
        }

        public async Task<bool> Save()
        {
            if (this.Document.CanSave)
            {
                try
                {
                    await this.Document.SaveAsync();
                    return true;
                }
                catch (Exception) { }
            }
            this.savePicker.SuggestedFileName = this.Document.Subtitle.ScriptInfo.Title ?? "";//TODO: locolized default new name.
            var file = await this.savePicker.PickSaveFileAsync();
            if (file == null)
                return false;
            await this.Document.SaveFileAsync(file);
            return true;
        }

        /// <summary>
        /// Ask user to save file if needed.
        /// </summary>
        /// <returns>True if can continue.</returns>
        public async Task<bool> CleanUpBeforeNewOrOpen()
        {
            if (this.Document.IsModified)
                switch (await this.showSaveDialog())
                {
                case dialogResult.Yes:
                    return await this.Save();
                case dialogResult.No:
                    return true;
                default:
                    return false;
                }
            return true;
        }

        public bool NeedCleanUp => this.Document.IsModified;

        private async Task<dialogResult> showSaveDialog()
        {
            return (dialogResult)((await this.saveDialog.ShowAsync())?.Id ?? dialogResult.Cancel);
        }

        private void documentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.saveFile.RaiseCanExecuteChanged();
                return;
            }
            switch (e.PropertyName)
            {
            case nameof(this.Document.IsModified):
                this.saveFile.RaiseCanExecuteChanged();
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
            Content = LocalizedStrings.Resources.SplitViewTabPreferences,
            PageType = typeof(View.PreferencesPage)
        };

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Title) || e.PropertyName == nameof(this.Document.IsModified))
                this.RaisePropertyChanged(nameof(this.Title));
        }

        public string Title => $"{(this.Document.IsModified ? "● " : "")}{this.Document.Title ?? LocalizedStrings.Resources.Untitled}";

        public override void Cleanup()
        {
            if (this.Document != null)
                this.Document.PropertyChanged -= this.documentPropertyChanged;
            base.Cleanup();
        }
    }
}