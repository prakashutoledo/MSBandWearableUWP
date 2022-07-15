/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Unit test for <see cref="HeartRateModel"/>
    /// </summary>
    [TestClass]
    public class HeartRateModelTest : BaseViewModelTest<HeartRateModel>
    {
        [DataTestMethod]
        [DataRow("HeartRateStatus")]
        public void ShouldHavePropertyChangedForHeartRateStatus(string propertyName)
        {
            SetProperty(propertyName, true);
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, true);
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, false);
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }

        [DataTestMethod]
        [DataRow("Bpm")]
        [DataRow("MinBpm")]
        [DataRow("MaxBpm")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            SetProperty(propertyName, 67);
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 67);
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 71);
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
