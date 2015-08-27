using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssLoader;

namespace SubtitleEditor.Model
{
    delegate void DocumentActionDo(Subtitle<ScriptInfo> subtitle);
    delegate void DocumentActionUndo(Subtitle<ScriptInfo> subtitle);

    interface IDocumentAction
    {
        void Do(Subtitle<ScriptInfo> subtitle);

        void Undo(Subtitle<ScriptInfo> subtitle);

        string ActionFriendlyName
        {
            get;
        }
    }

    class DocumentAction : IDocumentAction
    {
        public DocumentAction(string name, DocumentActionDo doAction, DocumentActionUndo undoAction)
        {
            ActionFriendlyName = name;
            this.doAction = doAction;
            this.undoAction = undoAction;
        }

        private DocumentActionDo doAction;
        private DocumentActionUndo undoAction;

        public string ActionFriendlyName
        {
            get;
            private set;
        }

        public void Do(Subtitle<ScriptInfo> subtitle)
        {
            doAction(subtitle);
        }

        public void Undo(Subtitle<ScriptInfo> subtitle)
        {
            undoAction(subtitle);
        }
    }
}
