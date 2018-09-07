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
            this.fields = fields.Split(splitChar, count);
            for (var i = 0; i < this.fields.Length; i++)
                this.fields[i] = this.fields[i].Trim();
        }

        internal EntryData(params string[] fields)
        {
            this.fields = fields;
            for (var i = 0; i < this.fields.Length; i++)
                FormatHelper.SingleLineStringValueValid(ref this.fields[i]);
        }

        private string[] fields;

        public override string ToString()
        {
            return string.Join(",", this.fields);
        }

        #region IReadOnlyCollection<string> 成员

        public string this[int index] => this.fields[index];

        public int Count => this.fields.Length;

        #endregion

        #region IEnumerable<string> 成员

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)this.fields).GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }

        #endregion
    }
}
