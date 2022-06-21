using System.Net.Http;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// A http client supplier implementation
    /// </summary>
    internal class HttpClientSupplier : IHttpClientSupplier
    {
        /// <summary>
        /// Supplies the new instance of <see cref="HttpClient"/>
        /// The supplied resource is not managed by this class and caller is responsible
        /// disposing managed and unmanaged resources
        /// </summary>
        /// <returns>A newly created http client</returns>
        public HttpClient Supply()
        {
            return new HttpClient();
        }
    }
}
