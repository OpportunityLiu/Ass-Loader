using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.AssLoader
{
    internal class EntryData : IReadOnlyList<string>
    {
        private static readonly char[] splitChar = new char[] { ',' };

        internal EntryData(string fields, int count)
        {
            this.Fields = fields.Split(splitChar, count);
            for (var i = 0; i < this.Fields.Length; i++)
                this.Fields[i] = this.Fields[i].Trim();
        }

        internal EntryData(params string[] fields)
        {
            this.Fields = fields;
            for (var i = 0; i < this.Fields.Length; i++)
                FormatHelper.SingleLineStringValueValid(ref this.Fields[i]);
        }

        internal readonly string[] Fields;

        public override string ToString()
        {
            return string.Join(",", this.Fields);
        }

        #region IReadOnlyCollection<string> 成员

        public string this[int index] => this.Fields[index];

        public int Count => this.Fields.Length;

        #endregion

        #region IEnumerable<string> 成员

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)this.Fields).GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Fields.GetEnumerator();
        }

        #endregion
    }
}
