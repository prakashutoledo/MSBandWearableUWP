namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// An interface for providing subject view service
    /// </summary>
    public interface ISubjectViewService
    {
        /// <summary>
        /// The unique id for current subject performing experiment
        /// </summary>
        string SubjectId { get; set; }

        /// <summary>
        /// A current iOS view name which subject is using
        /// </summary>
        string CurrentView { get; set; }

        /// <summary>
        /// A status for current session in progress
        /// </summary>
        bool SessionInProgress { get; set; }
    }
}
