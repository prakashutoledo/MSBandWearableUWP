using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Formatter
{
    /// <summary>
    /// Base test class for formatting serilog events
    /// </summary>
    public class BaseFormatterTest
    {
        protected string actualReason;

        [TestInitialize]
        public void BaseSetup()
        {
            actualReason = null;
        }

        /// <summary>
        /// Sets the reason for invalid action during formatting serilog events
        /// </summary>
        /// <param name="reason">A reason to set</param>
        protected void InvalidAction(string reason)
        {
            actualReason = reason;
        }
    }
}
