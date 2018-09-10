using System.Collections.Generic;
using ScriptInfoSerializeHelper
    = Opportunity.AssLoader.SerializeHelper<Opportunity.AssLoader.ScriptInfoCollection, Opportunity.AssLoader.ScriptInfoAttribute>;

namespace Opportunity.AssLoader
{
    internal interface ISubtitle
    {
        Dictionary<string, ScriptInfoSerializeHelper> ScriptInfoFields { get; }
    }
}