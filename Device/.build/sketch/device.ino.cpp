#include <Arduino.h>
#line 1 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
/**
 * A simple Azure IoT example for sending telemetry to Iot Hub.
 */


#include <WiFi.h>
#include <Adafruit_Sensor.h>
#include <DHT.h>
#include "Esp32MQTTClient.h"

#define INTERVAL 10000
#define MESSAGE_MAX_LEN 256
#define DHTPIN 4
#define DHTTYPE DHT11
#define LED_BUILTIN 2
// Please input the SSID and password of WiFi
const char* ssid     = "PREDIALNET_2G_C271";
const char* password = "998534782";

DHT dht(DHTPIN, DHTTYPE);

/*String containing Hostname, Device Id & Device Key in the format:                         */
/*  "HostName=<host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"                */
/*  "HostName=<host_name>;DeviceId=<device_id>;SharedAccessSignature=<device_sas_token>"    */
static const char* connectionString = "HostName=iothub-tentativa-2.azure-devices.net;DeviceId=esp-32-withDHT;SharedAccessKey=rK/LvJTXJWNsJS0tguTvFtGC2mFCFAInDCo+OxrxaYw=";
const char *messageData = "{\"messageId\":%d, \"Temperature\":%f, \"Humidity\":%f}";
static bool hasIoTHub = false;
static bool hasWifi = false;
int messageCount = 1;
static bool messageSending = true;
static uint64_t send_interval_ms;


#line 34 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
String readDHTTemperature();
#line 50 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
String readDHTHumidity();
#line 66 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result);
#line 74 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
static void MessageCallback(const char* payLoad, int size);
#line 80 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size);
#line 94 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
static int DeviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size);
#line 125 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
void setup();
#line 170 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
void loop();
#line 34 "c:\\Users\\luann.sapucaia\\Documents\\IoTWorkbenchProjects\\examples\\exemple-telemetry\\Device\\device.ino"
String readDHTTemperature() {
  // Read temperature as Celsius (the default)
  float t = dht.readTemperature();
  // Read temperature as Fahrenheit (isFahrenheit = true)
  //float t = dht.readTemperature(true);
  // Check if any reads failed and exit early (to try again).
  if (isnan(t)) { 
    Serial.println("Failed to read from DHT sensor!");
    return "--";
  }
  else {
    Serial.println(t);
    return String(t);
  }
}

String readDHTHumidity() {
  // Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)
  float h = dht.readHumidity();
  if (isnan(h)) {
    Serial.println("Failed to read from DHT sensor!");
    return "--";
  }
  else {
    Serial.println(h);
    return String(h);
  }
}




static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result)
{
  if (result == IOTHUB_CLIENT_CONFIRMATION_OK)
  {
    Serial.println("Send Confirmation Callback finished.");
  }
}

static void MessageCallback(const char* payLoad, int size)
{
  Serial.println("Message callback:");
  Serial.println(payLoad);
}

static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
  char *temp = (char *)malloc(size + 1);
  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';
  // Display Twin message.
  Serial.println(temp);
  free(temp);
}

static int  DeviceMethodCallback(const char *methodName, const unsigned char *payload, int size, unsigned char **response, int *response_size)
{
  LogInfo("Try to invoke method %s", methodName);
  const char *responseMessage = "\"Successfully invoke device method\"";
  int result = 200;

  if (strcmp(methodName, "start") == 0)
  {
    LogInfo("Start sending temperature and humidity data");
    messageSending = true;
  }
  else if (strcmp(methodName, "stop") == 0)
  {
    LogInfo("Stop sending temperature and humidity data");
    messageSending = false;
  }
  else
  {
    LogInfo("No method %s found", methodName);
    responseMessage = "\"No method found\"";
    result = 404;
  }

  *response_size = strlen(responseMessage) + 1;
  *response = (unsigned char *)strdup(responseMessage);

  return result;
}



void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  Serial.begin(115200);
  Serial.println("ESP32 Device");
  Serial.println("Initializing...");
  Serial.println(" > WiFi");
  Serial.println("Starting connecting WiFi.");

  dht.begin();
  delay(100);

  WiFi.mode(WIFI_MODE_AP);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println(".");
    hasWifi = false;
    digitalWrite(LED_BUILTIN, HIGH);   // turn the LED on (HIGH is the voltage level)
    delay(500);
    digitalWrite(LED_BUILTIN, LOW);    // turn the LED off by making the voltage LOW
    delay(500);  
  }
  hasWifi = true;
  
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  Serial.println(" > IoT Hub");
  if (!Esp32MQTTClient_Init((const uint8_t*)connectionString, true))
  {
    hasIoTHub = false;
    Serial.println("Initializing IoT hub failed.");
    return;
  }
  hasIoTHub = true;
  Esp32MQTTClient_SetSendConfirmationCallback(SendConfirmationCallback);
  Esp32MQTTClient_SetMessageCallback(MessageCallback);
  Esp32MQTTClient_SetDeviceTwinCallback(DeviceTwinCallback);
  Esp32MQTTClient_SetDeviceMethodCallback(DeviceMethodCallback);
  Serial.println("Start sending events.");
  randomSeed(analogRead(0));
  send_interval_ms = millis();

}

void loop() {
if (hasWifi && hasIoTHub)
  {
    if (messageSending && 
        (int)(millis() - send_interval_ms) >= INTERVAL)
    {
      // Send teperature data
      char messagePayload[MESSAGE_MAX_LEN];
      float temperature = dht.readTemperature();
      float humidity = dht.readHumidity();
      snprintf(messagePayload, MESSAGE_MAX_LEN, messageData, messageCount++, temperature, humidity);
      Serial.println(messagePayload);
      EVENT_INSTANCE* message = Esp32MQTTClient_Event_Generate(messagePayload, MESSAGE);
      Esp32MQTTClient_SendEventInstance(message);
      send_interval_ms = millis();
    }
    else
    {
      Esp32MQTTClient_Check();
    }
  }
  delay(100);
}
