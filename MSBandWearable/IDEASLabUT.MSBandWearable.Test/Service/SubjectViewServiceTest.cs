using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Service
{
    [TestClass]
    public class SubjectViewServiceTest : BaseHyperMock<SubjectViewService>
    {
        [DataTestMethod]
        [DataRow("SubjectId", "Not Available")]
        [DataRow("CurrentView", "Not Available")]
        [DataRow("SessionInProgress", false)]
        public void ShouldGetDefaultPropertyValue(string propertyName, object expectedValue)
        {
            var propertyInfo = Subject.GetType().GetProperty(propertyName);
            Assert.AreEqual(expectedValue, propertyInfo.GetValue(Subject));
        }
    }
}
