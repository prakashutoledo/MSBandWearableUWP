using IDEASLabUT.MSBandWearable.Model;
using IDEASLabUT.MSBandWearable.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Unit test for <see cref="JsonUtil"/>
    /// </summary>
    [TestClass]
    public class JsonUtilTest : AwaitableTest
    {
        private class SomeClass
        {
            public string Test { get; set; }
        }

        [TestMethod]
        public void JsonSerialization()
        {
            var eventTime = new DateTime(2009, 8, 1, 1, 1, 1, 100);
            var gsrEvent = new GSREvent
            {
                Gsr = 1.0,
                AcquiredTime = eventTime,
                ActualTime = eventTime,
                FromView = "Fake View",
                SubjectId = "Fake Id",
            };

            var expectedJson = "{\"gsr\":1.0,\"acquiredTime\":\"2009-08-01T01:01:01.100000-0400\",\"actualTime\":\"2009-08-01T01:01:01.100000-0400\",\"bandType\":\"MSBAND\",\"fromView\":\"Fake View\",\"subjectId\":\"Fake Id\"}";
            Assert.AreEqual(expectedJson, gsrEvent.ToJson(), "Expected json should match actual json string");
        }

        [TestMethod]
        public void JsonDeserializationToType()
        {
            var jsonString = "{\"test\":\"Any Value\"}";
            var someClass = jsonString.FromJson<SomeClass>();
            Assert.IsNotNull(someClass, "Deserialized value is not null");
            Assert.AreEqual(someClass.Test, "Any Value", "Test properties should match");
        }

        [TestMethod]
        public void JsonDeserializationToObject()
        {
            var jsonString = "{\"test\":\"Any Value1\"}";
            var someObject = jsonString.FromJson(typeof(SomeClass));
            Assert.IsNotNull(someObject, "Deserialized value is not null");
            Assert.IsInstanceOfType(someObject, typeof(SomeClass), "Object is of type SomeClass");
            Assert.AreEqual((someObject as SomeClass).Test, "Any Value1", "Test properties should match");
        }
    }
}
