/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using HyperMock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

using static System.Reflection.BindingFlags;
using static HyperMock.MockBehavior;
using System.Reflection;

namespace IDEASLabUT.MSBandWearable.Test
{
    /// <summary>
    /// Creates a subject type under test which can invoke both public or private constructors using reflection
    /// to create subject which can be default or with parameters. If parameters are present in 
    /// the constructor, it will create mocks for them
    /// </summary>
    /// <typeparam name="T">A type of subject under test</typeparam>
    public abstract class BaseHyperMock<T> : AwaitableTest where T : class
    {
        private IDictionary<Type, Mock> mockByType;

        protected T Subject
        {
            get; private set;
        }

        public virtual void OverrideMockSetup()
        {
        }

        [TestInitialize]
        public void SetupMocks()
        {
            // Looks for both public and private constructor
            var subjectConstructor = typeof(T).GetConstructors(Instance | Public | NonPublic).First();
            mockByType = subjectConstructor.GetParameters()
                .Select(parameter => ToMock(parameter))
                .ToDictionary(typeAndMock => typeAndMock.type, typeMockTuple => typeMockTuple.mock);
            OverrideMockSetup();
            Subject = subjectConstructor.Invoke(mockByType.Values.Select(mock => mock.Object).ToArray()) as T;
        }

        /// <summary>
        /// Converts the given parameter infor to Mock
        /// </summary>
        /// <param name="parameter">A parameter info</param>
        /// <returns>A tuple of type and mock</returns>
        private (Type type, Mock mock) ToMock(ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var mockParameter = typeof(Mock)
                // Look for public static method 'Create'
                .GetMethod("Create", Public | Static, null, new Type[] { typeof(MockBehavior) }, null)
                // Make generic method Create<T>
                .MakeGenericMethod(parameterType)
                .Invoke(null, new object[] { Loose }) as Mock;
            return (parameterType, mockParameter);
        }
        /// <summary>
        /// Gets the underlying mock value created for given type. If no mock is created subject initialization,
        /// this will create a mock and then return the associated mock value
        /// </summary>
        /// <typeparam name="R">A type of value represented by the mock to return</typeparam>
        /// <returns>A value holding by mock object for given type</returns>
        protected R MockValue<R>()
        {
            return GetOrCreateMock<R>().Object;
        }

        /// <summary>
        /// Creates and add the mock for given type if no such mock exists then return the mock object associated
        /// to given type or will return the existing mock which was created during subject initialization
        /// </summary>
        /// <typeparam name="R">A type of mock object to return</typeparam>
        /// <returns>Returns the newly created mock or returns the existing mock for given type</returns>
        protected Mock<R> GetOrCreateMock<R>()
        {
            var type = typeof(R);
            if (!mockByType.ContainsKey(type))
            {
                mockByType.Add(type, Mock.Create<R>());
            }
            return mockByType[type] as Mock<R>;
        }

        /// <summary>
        /// Apply the given action represented by given type. If mock do not exists for given type, it will create and add 
        /// then apply the given action
        /// </summary>
        /// <typeparam name="R">A type of mock object to apply given action</typeparam>
        /// <param name="mockAction">A mock action to invoke</param>
        protected void MockFor<R>(Action<Mock<R>> mockAction)
        {
            mockAction?.Invoke(GetOrCreateMock<R>());
        }

        [TestCleanup]
        public void MockCleanUp()
        {
            mockByType.Clear();
        }
    }
}
