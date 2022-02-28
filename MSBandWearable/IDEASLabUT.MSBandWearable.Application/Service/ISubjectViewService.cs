using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Application.Service
{
    public interface ISubjectViewService
    {
        string SubjectId { get; set; }
        string CurrentView { get; set; }
        bool IsSessionInProgress { get; set; }
    }
}
