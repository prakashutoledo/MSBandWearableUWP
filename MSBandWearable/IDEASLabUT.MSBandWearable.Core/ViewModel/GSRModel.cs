namespace IDEASLabUT.MSBandWearable.ViewModel
{
    public class GSRModel : BaseViewModel
    {
        private double gsr;

        /// <summary>
        /// A current gsr resistance value in KOHMs
        /// </summary>
        public double Gsr
        {
            get => gsr;
            set => UpdateAndNotify(ref gsr, value);
        }
    }
}
