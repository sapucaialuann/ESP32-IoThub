using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IoTHubIngestion.Domain.Models
{
    public interface ILoggerManager
    {

        void LogInfo(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath]string callerFilePath = "",
            Dictionary<string, object> customProperties = null);

        void LogWarn(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath]string callerFilePath = "",
            Dictionary<string, object> customProperties = null);

        void LogDebug(string message,
            [CallerMemberName] string method = "",
            [CallerFilePath]string callerFilePath = "",
            Dictionary<string, object> customProperties = null);


        void LogBusiness(string message,
            Dictionary<string, string> props,
            [CallerMemberName] string method = "",
            [CallerFilePath]string callerFilePath = "");

        void LogPerformance(string message,
            Action callback,
            Dictionary<string, string> props,
            [CallerMemberName] string method = "",
            [CallerFilePath]string callerFilePath = "");

        void LogError(string message, 
            [CallerMemberName] string method = "", 
            [CallerFilePath]string callerFilePath = "", 
            Exception ex = null, 
            Dictionary<string, object> customProperties = null);

        Task LogPerformanceAsync(                   string message, 
                                                    Stopwatch sw, 
                                                    Dictionary<string, object> props,
                                                    [CallerMemberName] string method = "",
                                                    [CallerFilePath] string callerFilePath = "");
    }
}
