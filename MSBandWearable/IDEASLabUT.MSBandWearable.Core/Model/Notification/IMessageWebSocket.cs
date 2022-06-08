using System;
using System.Threading.Tasks;

using Windows.Storage.Streams;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    public interface IUtf8MessageWebSocket : IDisposable
    {
        Task ConnectAsync(string webSocketUrl);
        Func<string, Task> OnMessageReceived { get; set; }
        void Close(ushort code, string reason);
        IDataWriter DataWriter { get; }
    }
}
