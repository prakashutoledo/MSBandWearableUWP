using Microsoft.Extensions.Configuration;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Service class for getting application properties based on given key
    /// </summary>
    public interface IPropertiesService
    {
        /// <summary>
        /// Get the value for given property key
        /// </summary>
        /// <param name="key">A key to look property value</param>
        /// <returns>A value assciated to given key if exists otherwise null</returns>
        string GetProperty(string key);

        /// <summary>
        /// Gets the underling properties value
        /// </summary>
        IConfiguration GetProperties { get; }
    }
}
