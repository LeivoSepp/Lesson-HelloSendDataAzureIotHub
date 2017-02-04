using Microsoft.Azure.Devices.Client;
using Microsoft.Devices.Tpm;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace HelloSendDataAzureIotHub
{
    public sealed class StartupTask : IBackgroundTask
    {
        private void initDevice()
        {
            TpmDevice device = new TpmDevice(0);
            string hubUri = device.GetHostName();
            string deviceId = device.GetDeviceId();
            string sasToken = device.GetSASToken();
            _sendDeviceClient = DeviceClient.Create(hubUri, AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
        }
        private DeviceClient _sendDeviceClient;
        private async void SendMessages()
        {
            string messageString = "Hello World";
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await _sendDeviceClient.SendEventAsync(message);
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            initDevice();

            while (true)
            {
                SendMessages();
                Task.Delay(1000).Wait();
            }
        }
    }
}
