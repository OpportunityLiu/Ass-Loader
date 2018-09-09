using Opportunity.AssLoader;
using Opportunity.AssLoader.Collections;
using Opportunity.MvvmUniverse.Collections;
using Opportunity.MvvmUniverse.Commands;
using SubtitleEditor.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        public static ResourceInfo.Resources.ViewModel.IMain Localizer { get; } = LocalizedStrings.Resources.ViewModel.Main;

        private readonly FileOpenPicker openPicker;
        private readonly FileSavePicker savePicker;
        private readonly ContentDialog saveDialog;

        private enum DialogResult
        {
            Yes, No, Cancel
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.openPicker = new FileOpenPicker();
            this.openPicker.FileTypeFilter.Add(".ass");

            this.savePicker = new FileSavePicker() { DefaultFileExtension = ".ass" };
            this.savePicker.FileTypeChoices.Add(LocalizedStrings.Resources.App.AssFileName, new[] { ".ass" });

            var locSaveDialog = LocalizedStrings.Resources.Dialog.Save;
            this.saveDialog = new ContentDialog
            {
                Content = locSaveDialog.Content,
                Title = locSaveDialog.Title,
                PrimaryButtonText = locSaveDialog.Yes,
                SecondaryButtonText = locSaveDialog.No,
                CloseButtonText = locSaveDialog.Cancel,
            };

            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE160",
                Content = Localizer.SplitView.Button.New,
                Command = AsyncCommand.Create(async c =>
                {
                    if (!await this.CleanUpBeforeNewOrOpen())
                        return;
                    this.Document.NewSubtitle();
                })
            });
            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE8E5",
                Content = Localizer.SplitView.Button.Open,
                Command = AsyncCommand.Create(async c =>
                {
                    if (!await this.CleanUpBeforeNewOrOpen())
                        return;
                    var file = await this.openPicker.PickSingleFileAsync();
                    if (file is null)
                        return;
                    await this.Document.OpenFileAsync(file);
                })
            });
            this.SplitViewButtons.Add(new SplitViewButtonData()
            {
                Icon = "\xE105",
                Content = Localizer.SplitView.Button.Save,
                Command = AsyncCommand.Create(async c => await this.Save()),
            });

            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE1CB",
                Content = Localizer.SplitView.Tab.ScriptInfo,
                PageType = typeof(View.ScriptInfoPage),
                IsChecked = true
            });
            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE2B1",
                Content = Localizer.SplitView.Tab.Style,
                PageType = typeof(View.StylePage)
            });
            this.DocumentTabs.Add(new SplitViewTabData()
            {
                Icon = "\xE292",
                Content = Localizer.SplitView.Tab.Event,
                PageType = typeof(View.SubEventPage)
            });
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
            this.savePicker.SuggestedFileName = this.Document.Subtitle.ScriptInfo.Title ?? "";
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
            if (!this.Document.IsModified)
                return true;

            switch (await this.saveDialog.ShowAsync())
            {
            case ContentDialogResult.Primary:
                return await this.Save();
            case ContentDialogResult.Secondary:
                return true;
            default:
                return false;
            }
        }

        public bool NeedCleanUp => this.Document.IsModified;

        public ObservableList<SplitViewButtonData> SplitViewButtons { get; } = new ObservableList<SplitViewButtonData>();

        public ObservableList<SplitViewTabData> DocumentTabs { get; } = new ObservableList<SplitViewTabData>();

        public SplitViewTabData Preferences { get; } = new SplitViewTabData()
        {
            Icon = "\xE115",
            Content = Localizer.SplitView.Tab.Preferences,
            PageType = typeof(View.PreferencesPage)
        };

        protected override void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(this.Document.Title) || e.PropertyName == nameof(this.Document.IsModified))
                this.OnPropertyChanged(nameof(this.Title));
        }

        public string Title => $"{(this.Document.IsModified ? "● " : "")}{this.Document.Title ?? Localizer.Untitled}";
    }
}