using System;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public class SubjectViewService
    {
        private static readonly Lazy<SubjectViewService> Instance = new Lazy<SubjectViewService>(() => new SubjectViewService());

        // Lazy singleton pattern
        public static SubjectViewService Singleton => Instance.Value;
        private SubjectViewService()
        {
            // private initialization
        }

        public ThreadLocal<string> SubjectId { get; } = new ThreadLocal<string>(() => "Not Available");

        public ThreadLocal<string> CurrentView { get; } = new ThreadLocal<string>(() => "Not Available");

        public ThreadLocal<bool> IsSessionInProgress { get; } = new ThreadLocal<bool>(() => false);
    }
}
