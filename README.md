# iot-end-to-end
Simple IoT end-to-end solution for demo purposes, requires a Raspberry Pi and Grove Pi

The solution simulates rooms that send temperature data to Azure IoT Hub.

Clicking the space bar in a window causes the temperature to rise, which is processed by a WebJob that sends a message back to IoT Hub for the particular device.

For more details see my [blog entry](http://martinabbott.azurewebsites.net/2016/03/26/iot-hub-end-to-end/)
