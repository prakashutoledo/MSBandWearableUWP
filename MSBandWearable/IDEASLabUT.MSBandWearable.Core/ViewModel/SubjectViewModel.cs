namespace IDEASLabUT.MSBandWearable.Core.ViewModel
{
    /// <summary>
    /// View model for subject id running IOS application,
    /// current view the subject is using and the Empatica e4
    /// band which subject is wearing
    /// </summary>
    public class SubjectViewModel : BaseViewModel
    {
        private string subjectId;
        private string currentView;
        private string e4SerialNumber;
        private string msBandSerialNumber;

        /// <summary>
        /// A unique identifier of subject in the running experiment wearing the band
        /// </summary>
        public string SubjectId
        {
            get => subjectId;
            set => UpdateAndNotify(ref subjectId, value);
        }

        /// <summary>
        /// An iOS SwiftUI view which subject is using in iPAD wearing the band
        /// </summary>
        public string CurrentView
        {
            get => currentView;
            set => UpdateAndNotify(ref currentView, value);
        }

        /// <summary>
        /// An unique serial number of the Empatica E4 band which subject is wearing 
        /// </summary>
        public string E4SerialNumber
        {
            get => e4SerialNumber;
            set => UpdateAndNotify(ref e4SerialNumber, value);
        }

        /// <summary>
        /// An unique serial number of the MS Band 2 band which subject is wearing 
        /// </summary>
        public string MSBandSerialNumber
        {
            get => msBandSerialNumber;
            set => UpdateAndNotify(ref msBandSerialNumber, value);
        }
    }
}
