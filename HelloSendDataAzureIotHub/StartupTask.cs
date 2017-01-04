using Microsoft.Azure.Devices.Client;
using Microsoft.Devices.Tpm;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

namespace HelloSendDataAzureIotHub
{
    public sealed class StartupTask : IBackgroundTask
    {
        int LED_PIN_GEEN = 47;
        int LED_PIN_RED = 35;
        int BTN_GREEN = 5;
        int BTN_RED = 6;
        GpioPin pinGreen;
        GpioPin pinRed;
        GpioPin btnGreen;
        GpioPin btnRed;
        private void init()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                return;
            }
            pinGreen = gpio.OpenPin(LED_PIN_GEEN);
            pinRed = gpio.OpenPin(LED_PIN_RED);
            pinRed.Write(GpioPinValue.High);
            pinRed.SetDriveMode(GpioPinDriveMode.Output);
            pinGreen.Write(GpioPinValue.High);
            pinGreen.SetDriveMode(GpioPinDriveMode.Output);

            btnGreen = gpio.OpenPin(BTN_GREEN);
            btnGreen.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnGreen.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            btnRed = gpio.OpenPin(BTN_RED);
            btnRed.SetDriveMode(GpioPinDriveMode.InputPullUp);
            btnRed.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            btnGreen.ValueChanged += BtnGreen_ValueChanged;
            btnRed.ValueChanged += BtnRed_ValueChanged;
        }

        private void BtnGreen_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPinValue pinValue = pinGreen.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                pinGreen.Write(pinValue);
            }
        }

        private void BtnRed_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            GpioPinValue pinValue = pinRed.Read();
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                pinValue = (pinValue == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                pinRed.Write(pinValue);
            }
        }
        private void initDevice()
        {
            TpmDevice device = new TpmDevice(0);
            string hubUri = device.GetHostName();
            string deviceId = device.GetDeviceId();
            string sasToken = device.GetSASToken();
            _sendDeviceClient = DeviceClient.Create(hubUri, AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId, sasToken), TransportType.Amqp);
        }
        private DeviceClient _sendDeviceClient;
        private async Task SendMessages()
        {
            string messageString = "Hello World";
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await _sendDeviceClient.SendEventAsync(message);
        }

        internal static BackgroundTaskDeferral Deferral = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            init();
            initDevice();

            while (true)
            {
                SendMessages();
                Task.Delay(1000).Wait();
            }
        }
    }
}
