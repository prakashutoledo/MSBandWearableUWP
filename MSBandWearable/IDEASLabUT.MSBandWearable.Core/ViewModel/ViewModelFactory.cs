using System;

namespace IDEASLabUT.MSBandWearable.ViewModel
{
    public abstract class ViewModelFactory
    {
        private sealed class ViewModelFactoryImpl : ViewModelFactory
        {
            public ViewModelFactoryImpl()
            {
                GetHeartRateModel = new HeartRateModel();
                GetGSRModel = new GSRModel();
                GetSubjectViewModel = new SubjectViewModel();
            }

            public sealed override HeartRateModel GetHeartRateModel { get; }

            public sealed override GSRModel GetGSRModel { get; }

            public sealed override SubjectViewModel GetSubjectViewModel { get; }
        }

        private static readonly Lazy<ViewModelFactory> ViewModeFactoryInstance;

        static ViewModelFactory()
        {
            ViewModeFactoryInstance = new Lazy<ViewModelFactory>(new ViewModelFactoryImpl());
        }

        public static ViewModelFactory Singleton => ViewModeFactoryInstance.Value;

        public abstract HeartRateModel GetHeartRateModel { get; }

        public abstract GSRModel GetGSRModel { get; }

        public abstract SubjectViewModel GetSubjectViewModel { get; }
    }
}
