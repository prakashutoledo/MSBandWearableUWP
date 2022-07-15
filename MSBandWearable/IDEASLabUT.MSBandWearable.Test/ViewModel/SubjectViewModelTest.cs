/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="SubjectViewModel"/>
    /// </summary>
    [TestClass]
    public class SubjectViewModelTest : BaseViewModelTest<SubjectViewModel>
    {

        [DataTestMethod]
        [DataRow("SubjectId")]
        [DataRow("CurrentView")]
        [DataRow("E4SerialNumber")]
        [DataRow("MSBandSerialNumber")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            SetProperty(propertyName, "Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, "Fake Value");
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, "New Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
