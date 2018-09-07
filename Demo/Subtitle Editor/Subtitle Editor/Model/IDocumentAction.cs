using Opportunity.AssLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleEditor.Model
{
    internal delegate void DocumentActionDo(Subtitle<ScriptInfo> subtitle);

    internal delegate void DocumentActionUndo(Subtitle<ScriptInfo> subtitle);

    internal interface IDocumentAction
    {
        void Do(Subtitle<ScriptInfo> subtitle);

        void Undo(Subtitle<ScriptInfo> subtitle);

        string ActionFriendlyName
        {
            get;
        }
    }

    internal class DocumentAction : IDocumentAction
    {
        public DocumentAction(string name, DocumentActionDo doAction, DocumentActionUndo undoAction)
        {
            this.ActionFriendlyName = name;
            this.doAction = doAction;
            this.undoAction = undoAction;
        }

        private readonly DocumentActionDo doAction;
        private readonly DocumentActionUndo undoAction;

        public string ActionFriendlyName
        {
            get;
            private set;
        }

        public void Do(Subtitle<ScriptInfo> subtitle)
        {
            this.doAction(subtitle);
        }

        public void Undo(Subtitle<ScriptInfo> subtitle)
        {
            this.undoAction(subtitle);
        }
    }
}
