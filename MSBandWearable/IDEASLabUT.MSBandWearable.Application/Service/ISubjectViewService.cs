﻿namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public interface ISubjectViewService
    {
        string SubjectId { get; set; }
        string CurrentView { get; set; }
        bool IsSessionInProgress { get; set; }
    }
}
