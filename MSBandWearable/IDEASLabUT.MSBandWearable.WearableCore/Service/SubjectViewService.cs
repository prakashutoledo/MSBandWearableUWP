using System;

using static System.Threading.Interlocked;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// A service class for current experiment subject, view and session details
    /// </summary>
    public class SubjectViewService : ISubjectViewService
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
            set => Exchange(ref subjectId, value);
        }

        /// <summary>
        /// A current iOS view name which subject is using
        /// </summary>
        public string CurrentView
        {
            get => currentView;
            set => Exchange(ref currentView, value);
        }

        /// <summary>
        /// A status for current session in progress
        /// </summary>
        public bool SessionInProgress
        {
            get => (bool) sessionInProgress;
            set => Exchange(ref sessionInProgress, value);
        }
    }
}
