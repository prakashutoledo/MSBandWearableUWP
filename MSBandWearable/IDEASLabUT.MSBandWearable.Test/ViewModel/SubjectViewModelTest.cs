using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    /// <summary>
    /// View model test for <see cref="SubjectViewModel"/>
    /// </summary>
    [TestClass]
    public class SubjectViewModelTest : BaseViewModelTest
    {
        private SubjectViewModel viewModel;

        [TestInitialize]
        public void Initialize()
        {
            viewModel = new SubjectViewModel();
            viewModel.PropertyChanged += OnPropertyChanged;
        }

        [DataTestMethod]
        [DataRow("SubjectId")]
        [DataRow("CurrentView")]
        [DataRow("E4SerialNumber")]
        [DataRow("MSBandSerialNumber")]
        public void ShouldHavePropertyChanged(string propertyName)
        {
            var propertyInfo = viewModel.GetType().GetProperty(propertyName);

            propertyInfo.SetValue(viewModel, "Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            propertyInfo.SetValue(viewModel, "Fake Value");
            // Will not invoke property changed event as changed value is same as previous value
            VerifyProperty(propertyName: propertyName, expectedCount: 1);

            propertyInfo.SetValue(viewModel, "New Fake Value");
            VerifyProperty(propertyName: propertyName, expectedCount: 2);
        }
    }
}
