namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// View model for subject id running IOS application,
    /// current view the subject is using and the Empatica e4
    /// band which subject is wearing
    /// </summary>
    public class SubjectViewModel : BaseModel
    {
        private string subjectId;
        private string currentView;
        private string e4SerialNumber;
        private string msBandSerialNumber;

        public string SubjectId
        {
            get => subjectId;
            set => UpdateAndNotify(ref subjectId, value);
        }

        public string CurrentView
        {
            get => currentView;
            set => UpdateAndNotify(ref currentView, value);
        }

        public string E4SerialNumber
        {
            get => e4SerialNumber;
            set => UpdateAndNotify(ref e4SerialNumber, value);
        }

        public string MSBandSerialNumber
        {
            get => msBandSerialNumber;
            set => UpdateAndNotify(ref msBandSerialNumber, value);
        }
    }
}
