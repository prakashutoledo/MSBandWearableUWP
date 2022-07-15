/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

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

        private class DerivedClass : SomeClass
        {
            public string DerivedValue { get; set; }
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

        [TestMethod]
        public async Task ShouldSerializedAsync()
        {
            SomeClass someClass = new DerivedClass
            {
                Value = "Some Value",
                DerivedValue = "Some Derived Value"
            };

            var expectedJson = "{\"derivedValue\":\"Some Derived Value\",\"value\":\"Some Value\"}";

            var actualJson = await someClass.ToJsonAsync();
            Assert.AreEqual(expectedJson, actualJson, "Expected json value should match actual value");
        }

        [TestMethod]
        public async Task ShouldDeserializedAsync()
        {
            var json = "{\"derivedValue\":\"Async Derived Value\",\"value\":\"Some Async Value\"}";
            var deserializedObject = await json.FromJsonAsync(typeof(DerivedClass));
            Assert.IsInstanceOfType(deserializedObject, typeof(DerivedClass));
            var actualObject = deserializedObject as DerivedClass;
            Assert.AreEqual("Async Derived Value", actualObject.DerivedValue, "Derived Value");
            Assert.AreEqual("Some Async Value", actualObject.Value, "Some Value");
        }

        [TestMethod]
        public async Task ShouldDeserializedAsyncWithExplicitType()
        {
            var json = "{\"derivedValue\":\"Async Derived Value\",\"value\":\"Some Async Value\"}";
            var deserializedObject = await json.FromJsonAsync<SomeClass>();
            Assert.IsInstanceOfType(deserializedObject, typeof(SomeClass));
            Assert.AreEqual("Some Async Value", deserializedObject.Value);
        }
    }
}
