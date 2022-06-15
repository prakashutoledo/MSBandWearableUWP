using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// Unit test for <see cref="JsonStringExtension"/>
    /// </summary>
    [TestClass]
    public class JsonStringExtensionTest
    {
        private class SomeClass
        {
            public string Value { get; set; }
        }

        [TestMethod]
        public void JsonSerialization()
        {
            var someClass = new SomeClass
            {
                Value = "Some Value"
            };
            var expectedJson = "{\"value\":\"Some Value\"}";
            Assert.AreEqual(expectedJson, someClass.ToJson(), "Expected json should match actual json string");
        }

        [TestMethod]
        public void JsonDeserializationToType()
        {
            var jsonString = "{\"value\":\"Any Value\"}";
            var someClass = jsonString.FromJson<SomeClass>();
            Assert.IsNotNull(someClass, "Deserialized value is not null");
            Assert.AreEqual(someClass.Value, "Any Value", "Test properties should match");
        }

        [TestMethod]
        public void JsonDeserializationToObject()
        {
            var jsonString = "{\"value\":\"Any Value1\"}";
            var someObject = jsonString.FromJson(typeof(SomeClass));
            Assert.IsNotNull(someObject, "Deserialized value is not null");
            Assert.IsInstanceOfType(someObject, typeof(SomeClass), "Object is of type SomeClass");
            Assert.AreEqual((someObject as SomeClass).Value, "Any Value1", "Test properties should match");
        }
    }
}
