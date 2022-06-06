using IDEASLabUT.MSBandWearable.Core.ViewModel;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.ComponentModel;

namespace IDEASLabUT.MSBandWearable.Test.ViewModel
{
    /// <summary>
    /// Base view model test providing property changed handler for all classes inheriting <see cref="BaseViewModel"/>
    /// </summary>
    [TestClass]
    public class BaseViewModelTest
    {
        private readonly IDictionary<string, int> propertyMap;

        /// <summary>
        /// Creates a new instance of <see cref="BaseViewModelTest"/>
        /// </summary>
        public BaseViewModelTest() => propertyMap = new Dictionary<string, int>();

        [TestCleanup]
        public void Cleanup()
        {
            propertyMap.Clear();
        }

        /// <summary>
        /// A test callback for property changed event
        /// </summary>
        /// <param name="sender">A sender of this event</param>
        /// <param name="eventArgs">A property changed event arguments</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            var propertyName = eventArgs.PropertyName;
            if (!propertyMap.ContainsKey(propertyName))
            {
                propertyMap.Add(propertyName, 0);
            }

            propertyMap[propertyName]++;
        }

        protected void VerifyProperty(string propertyName, int expectedCount, bool expectedExists = true)
        {
            Assert.AreEqual(expectedExists, propertyMap.Contains(new KeyValuePair<string, int>(propertyName, expectedCount)));
        }
    }
}
