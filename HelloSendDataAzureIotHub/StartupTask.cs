using Microsoft.Azure.Devices.Client;
using Microsoft.Devices.Tpm;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace HelloSendDataAzureIotHub
{
    public sealed class StartupTask : IBackgroundTask
    {

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            TpmDevice device = new TpmDevice(0);
            string hubUri = device.GetHostName();
            string deviceId = device.GetDeviceId();
            string sasToken = device.GetSASToken();
            DeviceClient _sendDeviceClient = DeviceClient.Create(hubUri, AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
            while (true)
            {
                string messageString = "Hello World";
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                _sendDeviceClient.SendEventAsync(message);
                Task.Delay(1000).Wait();
            }
        }
    }
}
