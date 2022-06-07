using Polly;
using Polly.Extensions.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Resilliency
{
    public class CommonPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(retryCount: 8,
                    sleepDurationProvider: attempt => { return TimeSpan.FromSeconds(Math.Pow(2, attempt)); },
                    onRetry: (exception, retryCount, context) => { Log.Error($"Retry Count {retryCount} of {context.PolicyKey} due to exception:{exception}"); }

                    );

        }
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (result, timespan) => { Log.Error($"Circuit is open due to exception of type {result.Exception.GetType().ToString()}"); },
                        onReset: () => Log.Information("Circuit is closed again"));

        }
    }
}
