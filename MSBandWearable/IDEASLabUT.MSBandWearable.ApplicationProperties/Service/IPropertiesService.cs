namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// Service class for getting application properties based on given key
    /// </summary>
    public interface IPropertiesService
    {
        string GetProperty(string key);
    }
}
