using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Opportunity.AssLoader.Test
{
    public abstract class TestBase
    {
        public TestBase()
        {
            TestHelper.Init();
        }

        public TestHelper TestHelper;

        public TestContext TestContext
        {
            get => this.TestHelper.Context;
            set => this.TestHelper = new TestHelper(value);
        }
    }
}
