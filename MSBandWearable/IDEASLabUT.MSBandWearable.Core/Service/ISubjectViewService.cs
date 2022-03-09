namespace IDEASLabUT.MSBandWearable.Core.Service
{
    public interface ISubjectViewService
    {
        string SubjectId { get; set; }
        string CurrentView { get; set; }
        bool SessionInProgress { get; set; }
    }
}
