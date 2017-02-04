# Lesson #4 Send "Hello World" to Azure IoTHub
This project uses Raspberry PI, Windows 10 IoT Core to send message "Hello World" into Azure IoTHub with TPM authentication.

## What I will learn from this project?
* How to send message from Raspberry to cloud, Azure IoTHub?
* What is Azure IoTHub?
* How to set up Azure to accept messages?
* What is TPM (Trusted Platform Module) and why it is needed in Raspberry?

## What I need for this project?
* Raspberry PI
* Windows 10 IoT Core [Instructions to set up](https://developer.microsoft.com/en-us/windows/iot/Docs/GetStarted/rpi2/sdcard/stable/getstartedstep1)
* Azure subscription https://portal.azure.com/
* Visual Studio https://www.visualstudio.com/
* Device Explorer [Azure IoT SDK](https://github.com/Azure/azure-iot-sdks/releases)

## Let's start with the code itself
The code itself is very short and simple. The only thing which takes time, is to set up the environment.

1. Creating new TPM device, to authenticate our device. **TpmDevice device = new TpmDevice(0);**
2. Reading information from TPM module
   1. Hostname: this is used to find Azure cloud
   2. DeviceID: this is your Raspberry name (one Raspberry can have also multiple names)
   3. SASToken: this is the most valuable thing, this is the password to authenticate yourself
3. Creating new DeviceClient, which is used to send data into Azure **DeviceClient _sendDeviceClient = ...**
4. While (true) is never ending loop
5. Create a string to hold the "Hello World" which we are sending to Azure: **string messageString = "Hello World";**
6. Converting our message to ASCII code. For example Hello World in ASCII characters are "072 101 108 108 111 032 087 111 114 108 100"
7. Actual send command, to send these ASCII numbers into Azure: **_sendDeviceClient.SendEventAsync(message);**
8. Wait 1 sec and then jump into beginning of while-loop.
```C#
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
```

## What is a TPM?
   1. TPM stands for Trusted Platform Module and it is used to keep secret information in hardware
   2. We do not want to share our secret passwords and device name publicly into GitHub, but we want that the code still works
   3. We will save Azure connection password into special hardware module, called TPM module.
   4. When the password is saved into TPM module, we need just simple execute commands **GetHostName**, **GetDeviceID** and **GetSASToken**.

### How to set up Rasperry TPM?
Creating TPM in Raspberry is simple and it is explained very well in this page:
https://github.com/Azure/azure-iot-hub-vs-cs/wiki/Device-Provisioning-with-TPM 

Please follow the guidance on that page and you are all set up correctly.

Before you can save your security data into TPM, you need:
* Azure subscription and IoTHUB configured
* Device Explorer to register your device as an Azure device

### How to set up Azure IoTHUB?


### How to create a devie for Azure?