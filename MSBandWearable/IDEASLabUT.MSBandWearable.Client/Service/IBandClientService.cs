﻿
using Microsoft.Band;

using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// An interface for MS Band 2 client service to connect to band and subscribe available supported sensors
    /// </summary>
    public interface IBandClientService
    {
        /// <summary>
        /// A connected MS Band 2 client 
        /// </summary>
        IBandClient BandClient { get; }

        /// <summary>
        /// Connects the given selected index from the available paired MS bands
        /// </summary>
        /// <param name="bandName">A MS band name to connect</param>
        /// <returns>A task that can be awaited</returns>
        Task ConnectBand(string bandName);
    }
}
