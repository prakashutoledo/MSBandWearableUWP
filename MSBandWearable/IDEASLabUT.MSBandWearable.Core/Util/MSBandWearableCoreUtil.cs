using IDEASLabUT.MSBandWearable.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Core utility class
    /// </summary>
    public static class MSBandWearableCoreUtil
    {
        static MSBandWearableCoreUtil()
        {
            // Default json converter settings to ignore null value, unknown properties resolving members in camel case
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// Runs the given priority action in main core dispatcher thread asynchronously. Null action will not be invoked in core dispatcher thread
        /// </summary>
        /// <param name="action">An action to be invoked in core dispatcher thread</param>
        /// <param name="coreDispatcherPriority">A core dispatcher priority for invoked action which defaults to normal</param>
        /// <returns>A task that can be awaited</returns>
        public static async Task RunLaterInUIThread(Action action, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal)
        {
            if (action == null)
            {
                return;
            }

            var coreDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (coreDispatcher.HasThreadAccess)
            {
                action.Invoke();
                return;
            }

            await coreDispatcher.RunAsync(coreDispatcherPriority, new DispatchedHandler(action));
        }

        /// <summary>
        /// Runs the given priority action in main core dispatcher thread asynchronously by passing given input value
        /// </summary>
        /// <typeparam name="T">A parameter of type <see cref="BaseEvent"/> </typeparam>
        /// <param name="action">An action to be invoked in core dispatcher thread</param>
        /// <param name="inputValue">An input value for given action</param>
        /// <param name="coreDispatcherPriority">A core dispatcher priority for invoked action which defaults to normal</param>
        /// <returns>A task that can be awaited</returns>
        public static async Task RunLaterInUIThread<T>(Action<T> action, T inputValue, CoreDispatcherPriority coreDispatcherPriority = CoreDispatcherPriority.Normal) where T : BaseEvent
        {
            await RunLaterInUIThread(() => action.Invoke(inputValue), coreDispatcherPriority);
        }

        /// <summary>
        /// An extension which serizes object to json representation
        /// </summary>
        /// <param name="value">A value of object to be serilized</param>
        /// <returns>A serilized json string representation</returns>
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Remove nth character from given index from last
        /// </summary>
        /// <param name="value">A string value to remove character from</param>
        /// <param name="indexFromLast">An index value from last</param>
        /// <returns>A string with removed nth character from last if valid otherwise returns itself</returns>
        /// <remarks>Index from last is 1 based not zero based</remarks>
        public static string RemoveNthCharacterFromLast(this string value, int indexFromLast)
        {
            return indexFromLast <= 0 || indexFromLast > value.Length ? value : value.Remove(value.Length - indexFromLast, 1);
        }
    }
}
