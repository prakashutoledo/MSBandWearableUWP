using IDEASLabUT.MSBandWearable.Model;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="BandType"/>
    /// </summary>
    public class BandTypeConverter : BaseEnumJsonConverter<BandType>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BandTypeConverter"/>
        /// </summary>
        public BandTypeConverter() : base(description => description.ToBandType(), bandType => bandType.GetDescription())
        {
        }
    }
}
