/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.ComponentModel;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Base view model test providing property changed handler for all classes inheriting <see cref="BaseViewModel"/>
    /// </summary>
    [TestClass]
    public class BaseViewModelTest<ViewModel> : BaseHyperMock<ViewModel> where ViewModel : BaseViewModel
    {
        private readonly IDictionary<string, int> propertyMap;

        /// <summary>
        /// Creates a new instance of <see cref="BaseViewModelTest"/>
        /// </summary>
        public BaseViewModelTest() => propertyMap = new Dictionary<string, int>();

        [TestInitialize]
        public void PropertySetup()
        {
            Subject.PropertyChanged += OnPropertyChanged;
        }

        [TestCleanup]
        public void CleanupProperty()
        {
            propertyMap.Clear();
        }

        /// <summary>
        /// Sets the propertyName with given value using reflection
        /// </summary>
        /// <param name="propertyName">A property name to be called</param>
        /// <param name="value">A new value of given property name</param>
        protected void SetProperty(string propertyName, object value)
        {
            var propertyInfo = Subject.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(Subject, value);
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

        /// <summary>
        /// Verifies the given propertyName as been called with expected count
        /// </summary>
        /// <param name="propertyName">A property name to verify if it is in the map</param>
        /// <param name="expectedCount">An expected count of times which property name was invoked as a value</param>
        /// <param name="expectedExists">An exists flag to check if propertyName as key contains expected count as value</param>
        protected void VerifyProperty(string propertyName, int expectedCount, bool expectedExists = true)
        {
            Assert.AreEqual(expectedExists, propertyMap.Contains(new KeyValuePair<string, int>(propertyName, expectedCount)));
        }
    }
}
