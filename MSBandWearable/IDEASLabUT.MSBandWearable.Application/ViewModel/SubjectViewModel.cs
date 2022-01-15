namespace IDEASLabUT.MSBandWearable.Application.ViewModel
{
    /// <summary>
    /// View model for subject id running IOS application,
    /// current view the subject is using and the Empatica e4
    /// band which subject is wearing
    /// </summary>
    public class SubjectViewModel : BaseModel
    {
        private string subjectId;
        public string SubjectId
        {
            get => subjectId;
            set => UpdateAndNotify(ref subjectId, value);
        }

        private string currentView;
        public string CurrentView
        {
            get => currentView;
            set => UpdateAndNotify(ref currentView, value);
        }

        private string e4SerialNumber;
        public string E4SerialNumber
        {
            get => e4SerialNumber;
            set => UpdateAndNotify(ref e4SerialNumber, value);
        }

        private string msBandSerialNumber;
        public string MSBandSerialNumber
        {
            get => msBandSerialNumber;
            set => UpdateAndNotify(ref msBandSerialNumber, value);
        }
    }
}
