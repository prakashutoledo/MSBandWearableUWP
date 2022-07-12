using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// Unit test for <see cref="GSRModel"/>
    /// </summary>
    [TestClass]
    public class GSRViewModelTest : BaseViewModelTest<GSRModel>
    {
        [DataTestMethod]
        [DataRow("Gsr")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            SetProperty(propertyName, 1.0);
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 1.0);
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            SetProperty(propertyName, 2.1);
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
