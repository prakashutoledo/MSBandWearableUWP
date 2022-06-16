using System;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for current experiment subject, view and session details
    /// </summary>
    public sealed class SubjectViewService : ISubjectViewService
    {
        private static readonly Lazy<SubjectViewService> Instance = new Lazy<SubjectViewService>(() => new SubjectViewService());
        private static string subjectId = "Not Available";
        private static string currentView = "Not Available";
        private static object sessionInProgress = false;

        // Lazy singleton pattern
        internal static SubjectViewService Singleton => Instance.Value;

        /// <summary>
        /// Initializes new instance of <see cref="SubjectViewService"/>
        /// </summary>
        private SubjectViewService()
        {
            // private initialization
        }

        /// <summary>
        /// The unique identifier for current subject performing experiment
        /// </summary>
        public string SubjectId 
        {
            get => subjectId;
            set => Interlocked.Exchange(ref subjectId, value);
        }

        /// <summary>
        /// A current iOS view name which subject is using
        /// </summary>
        public string CurrentView
        {
            get => currentView;
            set => Interlocked.Exchange(ref currentView, value);
        }

        /// <summary>
        /// A status for current session in progress
        /// </summary>
        public bool SessionInProgress
        {
            get => (bool) sessionInProgress;
            set => Interlocked.Exchange(ref sessionInProgress, value);
        }
    }
}
