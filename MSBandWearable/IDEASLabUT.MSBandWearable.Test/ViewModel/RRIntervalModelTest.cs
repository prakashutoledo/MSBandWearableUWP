using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Unit test for <see cref="RRIntervalModel"/>
    /// </summary>
    public class RRIntervalModelTest : BaseViewModelTest<RRIntervalModel>
    {
        [DataTestMethod]
        [DataRow("Ibi")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            SetProperty(propertyName, 22.0);
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 22.0);
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 36.1);
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
