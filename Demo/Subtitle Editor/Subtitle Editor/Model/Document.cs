﻿using Opportunity.AssLoader;
using Opportunity.MvvmUniverse;
using Opportunity.MvvmUniverse.Views;
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
    internal class Document : ObservableObject
    {
        public Document()
        {
            this.reset();
        }

        public string Title => this.SubtitleFile?.Name;

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
            if (this.subtitle is null)
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
            OnPropertyChanged(nameof(CanSave), nameof(UndoAction), nameof(RedoAction), nameof(IsModified));
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
                if (value is null)
                    throw new ArgumentNullException();
                this.Set(nameof(this.CanSave), ref this.subtitle, value);
            }
        }

        private StorageFile subtitleFile;

        public StorageFile SubtitleFile
        {
            get => this.subtitleFile;
            private set
            {
                if (this.Set(nameof(this.CanSave), nameof(Title), ref this.subtitleFile, value) && value != null)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Add(value, value.Name, RecentStorageItemVisibility.AppAndSystem);
                }
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
            var data = await FileIO.ReadTextAsync(file);
            this.Subtitle = Opportunity.AssLoader.Subtitle.Parse<ScriptInfo>(data).Result;
            this.reset();
            this.modified();
        }

        public bool CanSave => this.subtitle != null && this.subtitleFile != null;

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
            OnPropertyChanged(nameof(this.IsModified));
        }

        public async Task SaveFileAsync(StorageFile file)
        {
            this.SubtitleFile = file ?? throw new ArgumentNullException(nameof(file));
            await this.SaveAsync();
        }
    }
}
