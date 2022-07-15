/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Unit test for <see cref="TemperatureModel"/>
    /// </summary>
    [TestClass]
    public class TemperatureModelTest : BaseViewModelTest<TemperatureModel>
    {
        [DataTestMethod]
        [DataRow("Temperature")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            SetProperty(propertyName, 37.0);
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 37.0);
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 38.2);
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
