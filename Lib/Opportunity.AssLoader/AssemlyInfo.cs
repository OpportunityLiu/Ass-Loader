using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Opportunity.AssLoader.Test")]
[assembly: InternalsVisibleTo("Opportunity.AssLoader.Universal")]
[assembly: InternalsVisibleTo("Opportunity.AssLoader.Universal.Test")]
#if DEBUG
[assembly: InternalsVisibleTo("Analyzer")]
#endif
