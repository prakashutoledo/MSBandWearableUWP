﻿using HyperMock;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

using static System.Reflection.BindingFlags;

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
        private readonly IDictionary<Type, Mock> mocks;

        protected T Subject
        {
            get; private set;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseHyperMock{T}"/>
        /// </summary>
        protected BaseHyperMock()
        {
            mocks = new Dictionary<Type, Mock>();
        }

        [TestInitialize]
        public void SetupMocks()
        {
            // Looks for both public and private constructor
            var subjectConstructor = typeof(T).GetConstructors(Instance | Public | NonPublic).First();
            foreach(var parameter in subjectConstructor.GetParameters())
            {
                var parameterType = parameter.ParameterType;
                mocks.Add(parameterType, Mock.Create(parameterType));
            }
            Subject = subjectConstructor.Invoke(mocks.Values.Select(mock => mock.Object).ToArray()) as T;
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
            if (!mocks.ContainsKey(type))
            {
                mocks.Add(type, Mock.Create<R>());
            }
            return mocks[type] as Mock<R>;
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
            mocks.Clear();
        }
    }
}