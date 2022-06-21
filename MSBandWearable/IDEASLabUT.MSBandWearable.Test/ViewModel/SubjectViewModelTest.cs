using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="SubjectViewModel"/>
    /// </summary>
    [TestClass]
    public class SubjectViewModelTest : BaseHyperMock<SubjectViewModel>
    {

        [DataTestMethod]
        [DataRow("SubjectId")]
        [DataRow("CurrentView")]
        [DataRow("E4SerialNumber")]
        [DataRow("MSBandSerialNumber")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            var propertyInfo = Subject.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(Subject, "Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            propertyInfo.SetValue(Subject, "Fake Value");
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            propertyInfo.SetValue(Subject, "New Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
