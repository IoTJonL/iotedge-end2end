using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace CloudFunctions
{
    public static class IotHubMessageProcessor
    {
        // AppInsights TelemetryClient
        private static TelemetryClient telemetry = new TelemetryClient();

        /// <summary>
        /// Function that processes messages from the IoT Hub events-endpoint
        /// </summary>
        /// <param name="message"></param>
        /// <param name="log"></param>
        [FunctionName("IotHubMessageProcessor")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "iothubevents_cs", ConsumerGroup = "receiverfunction")]EventData message, ILogger log)
        {
            log.LogInformation($"IotHubMessageProcessor received a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            if (message.Properties.ContainsKey("correlationId"))
            {
                var correlationId = message.Properties["correlationId"].ToString();
                log.LogInformation($"Message correlationId={correlationId}");

                var properties = new Dictionary<string, string> { { "correlationId", correlationId } };
                telemetry.TrackEvent("100-ReceivedIoTHubMessage", properties);
            }
            else
            {
                log.LogWarning("Message received without correlationId property");
            }
        }
    }
}