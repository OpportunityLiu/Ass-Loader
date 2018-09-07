using GalaSoft.MvvmLight;
using Opportunity.AssLoader;
using SubtitleEditor.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SubtitleEditor.Model
{
    internal class Document : ViewModelBase
    {
        public Document()
        {
            Application.Current.Suspending += this.onSuspending;
            this.reset();
        }

        private void updateTitle()
        {
            this.Title = this.SubtitleFile?.Name ?? this.Subtitle?.ScriptInfo.Title;
        }

        private string title;

        public string Title
        {
            get => this.title;
            private set => this.Set(nameof(this.Title), ref this.title, value);
        }

        private readonly LinkedList<IDocumentAction> actionList = new LinkedList<IDocumentAction>();
        private LinkedListNode<IDocumentAction> currentAction, savedAction;

        private void onSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

        }

        public bool TryDo(IDocumentAction action)
        {
            if (this.subtitle == null)
                return false;
            try
            {
                action.Do(this.subtitle);
            }
            catch (Exception)
            {
                return false;
            }
            while (this.actionList.First.Next != this.currentAction)
                this.actionList.Remove(this.actionList.First.Next);
            this.actionList.AddAfter(this.actionList.First, action);
            this.currentAction = this.actionList.First.Next;
            this.modified();
            return true;
        }

        public void Do(IDocumentAction action)
        {
            if (this.subtitle == null)
                throw new InvalidOperationException($"{nameof(this.Subtitle)} is null");
            action.Do(this.subtitle);
            while (this.actionList.First.Next != this.currentAction)
                this.actionList.Remove(this.actionList.First.Next);
            this.actionList.AddAfter(this.actionList.First, action);
            this.currentAction = this.actionList.First.Next;
            this.modified();
        }

        public void Undo()
        {
            this.currentAction.Value.Undo(this.subtitle);
            this.currentAction = this.currentAction.Next;
            this.modified();
        }

        public void Redo()
        {
            this.currentAction = this.currentAction.Previous;
            this.currentAction.Value.Do(this.subtitle);
            this.modified();
        }

        private void modified()
        {
            this.RaisePropertyChanged(nameof(this.CanSave));
            this.RaisePropertyChanged(nameof(this.UndoAction));
            this.RaisePropertyChanged(nameof(this.RedoAction));
            this.RaisePropertyChanged(nameof(this.IsModified));
            this.updateTitle();
        }

        public IDocumentAction UndoAction => this.currentAction.Value;

        public IDocumentAction RedoAction => this.currentAction.Previous.Value;

        public bool IsModified => this.currentAction != this.savedAction;

        private Subtitle<ScriptInfo> subtitle;

        public Subtitle<ScriptInfo> Subtitle
        {
            get => this.subtitle;
            private set
            {
                this.Set(ref this.subtitle, value);
                if (value.ScriptInfo.SubtitleEditorConfig == null)
                    value.ScriptInfo.SubtitleEditorConfig = new ProjectConfig();
                this.RaisePropertyChanged(nameof(this.CanSave));
                this.updateTitle();
            }
        }

        private StorageFile subtitleFile;

        public StorageFile SubtitleFile
        {
            get => this.subtitleFile;
            private set
            {
                this.Set(ref this.subtitleFile, value);
                this.RaisePropertyChanged(nameof(this.CanSave));
                if (value != null)
                    StorageApplicationPermissions.MostRecentlyUsedList.Add(value, value.Name, RecentStorageItemVisibility.AppAndSystem);
                this.updateTitle();
            }
        }

        private void reset()
        {
            this.actionList.Clear();
            this.actionList.AddFirst((IDocumentAction)null);
            this.actionList.AddLast((IDocumentAction)null);
            this.currentAction = this.savedAction = this.actionList.Last;
        }

        public void NewSubtitle()
        {
            this.SubtitleFile = null;
            this.Subtitle = new Subtitle<ScriptInfo>(new ScriptInfo());
            this.currentAction = null;
            this.savedAction = null;
            this.reset();
            this.modified();
        }

        public async Task OpenFileAsync(StorageFile file)
        {
            this.SubtitleFile = file ?? throw new ArgumentNullException(nameof(file));
            using (var stream = await this.SubtitleFile.OpenStreamForReadAsync())
            using (var reader = new StreamReader(stream))
                this.Subtitle = Opportunity.AssLoader.Subtitle.Parse<ScriptInfo>(reader);
            this.reset();
            this.modified();
        }

        public bool CanSave => this.subtitle != null && this.subtitleFile != null && this.IsModified;

        public async Task SaveAsync()
        {
            if (!this.CanSave)
                throw new InvalidOperationException("Can't save now.");
            try
            {
                using (var stream = await this.SubtitleFile.OpenStreamForWriteAsync())
                {
                    stream.SetLength(0);
                    using (var writer = new StreamWriter(stream))
                        this.subtitle.Serialize(writer);
                }
            }
            catch (Exception)
            {
                this.SubtitleFile = null;
                throw;
            }
            this.savedAction = this.currentAction;
            this.RaisePropertyChanged(nameof(this.IsModified));
        }

        public async Task SaveFileAsync(StorageFile file)
        {
            this.SubtitleFile = file ?? throw new ArgumentNullException(nameof(file));
            await this.SaveAsync();
        }

        public override void Cleanup()
        {
            Application.Current.Suspending -= this.onSuspending;
            this.reset();
            this.subtitle = null;
            this.subtitleFile = null;
            base.Cleanup();
        }
    }
}
