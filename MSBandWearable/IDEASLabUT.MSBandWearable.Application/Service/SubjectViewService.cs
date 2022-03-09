using IDEASLabUT.MSBandWearable.Core.Service;

using System;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class SubjectViewService : ISubjectViewService
    {
        private static readonly Lazy<SubjectViewService> Instance = new Lazy<SubjectViewService>(() => new SubjectViewService());
        private static string subjectId = "Not Available";
        private static string currentView = "Not Available";
        private static object sessionInProgress = false;

        // Lazy singleton pattern
        public static SubjectViewService Singleton => Instance.Value;

        private SubjectViewService()
        {
            // private initialization
        }

        public string SubjectId 
        {
            get => subjectId;
            set => Interlocked.Exchange(ref subjectId, value);
        }

        public string CurrentView
        {
            get => currentView;
            set => Interlocked.Exchange(ref currentView, value);
        }

        public bool SessionInProgress
        {
            get => (bool) sessionInProgress;
            set => Interlocked.Exchange(ref sessionInProgress, value);
        }
    }
}
