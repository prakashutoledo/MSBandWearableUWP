/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using System.Net.Http;

namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// Http client supplier
    /// </summary>
    public interface IHttpClientSupplier
    {
        /// <summary>
        /// Supplies the new instance of <see cref="HttpClient"/>
        /// The supplied resource is not managed by this class and caller is responsible
        /// disposing managed and unmanaged resources
        /// </summary>
        /// <returns>A newly created http client</returns>
        HttpClient Supply();
    }
}
