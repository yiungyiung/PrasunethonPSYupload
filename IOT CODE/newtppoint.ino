#include "FastIMU.h"
#include "Madgwick.h"
#include <Wire.h>
#include <WiFi.h>
#include <WebSocketsServer.h>

#define MPU6050_ADDRESS1 0x68  // Address of the first MPU6050
#define MPU6050_ADDRESS2 0x69  // Address of the second MPU6050
#define PERFORM_CALIBRATION    // Enable startup calibration

MPU6050 IMU1;               // Instance for the first MPU6050
MPU6050 IMU2;               // Instance for the second MPU6050

// Calibration data and sensor data for both IMUs
calData calib1 = { 0 };
calData calib2 = { 0 };
AccelData IMUAccel1, IMUAccel2;
GyroData IMUGyro1, IMUGyro2;
Madgwick filter1, filter2;

// WiFi and WebSocket server setup
WebSocketsServer webSocket = WebSocketsServer(8080);

void setup() {
  Wire.begin(21, 22); // SDA, SCL
  Wire.setClock(400000); // 400kHz clock

  Serial.begin(115200);
  while (!Serial) {
    ;
  }

  // Set up custom IP configuration
  IPAddress localIP(192, 168, 43, 1);  // Set your desired IP address here
  IPAddress gateway(192, 168, 43, 1);
  IPAddress subnet(255, 255, 255, 0);

  // Set up hotspot with custom IP configuration
  WiFi.softAPConfig(localIP, gateway, subnet);
  WiFi.softAP("NodeMCU_Hotspot", "HotspotPassword");
  Serial.println("Hotspot IP: " + WiFi.softAPIP().toString());

  // Set up WebSocket server
  webSocket.begin();

  // Initialize the first MPU6050
  int err1 = IMU1.init(calib1, MPU6050_ADDRESS1);
  if (err1 != 0) {
    Serial.print("Error initializing IMU1: ");
    Serial.println(err1);
    while (true) { }
  }

  // Initialize the second MPU6050
  int err2 = IMU2.init(calib2, MPU6050_ADDRESS2);
  if (err2 != 0) {
    Serial.print("Error initializing IMU2: ");
    Serial.println(err2);
    while (true) { }
  }

#ifdef PERFORM_CALIBRATION
  Serial.println("Calibrating IMU1...");
  Serial.println("Keep IMU1 level.");
  delay(5000);
  IMU1.calibrateAccelGyro(&calib1);
  Serial.println("Calibration done for IMU1!");
  Serial.println("Accel biases X/Y/Z for IMU1: ");
  Serial.print(calib1.accelBias[0]);
  Serial.print(", ");
  Serial.print(calib1.accelBias[1]);
  Serial.print(", ");
  Serial.println(calib1.accelBias[2]);
  Serial.println("Gyro biases X/Y/Z for IMU1: ");
  Serial.print(calib1.gyroBias[0]);
  Serial.print(", ");
  Serial.print(calib1.gyroBias[1]);
  Serial.print(", ");
  Serial.println(calib1.gyroBias[2]);

  IMU1.init(calib1, MPU6050_ADDRESS1);

  Serial.println("Calibrating IMU2...");
  Serial.println("Keep IMU2 level.");
  delay(5000);
  IMU2.calibrateAccelGyro(&calib2);
  Serial.println("Calibration done for IMU2!");
  Serial.println("Accel biases X/Y/Z for IMU2: ");
  Serial.print(calib2.accelBias[0]);
  Serial.print(", ");
  Serial.print(calib2.accelBias[1]);
  Serial.print(", ");
  Serial.println(calib2.accelBias[2]);
  Serial.println("Gyro biases X/Y/Z for IMU2: ");
  Serial.print(calib2.gyroBias[0]);
  Serial.print(", ");
  Serial.print(calib2.gyroBias[1]);
  Serial.print(", ");
  Serial.println(calib2.gyroBias[2]);

  IMU2.init(calib2, MPU6050_ADDRESS2);

  filter1.begin(0.2f);
  filter2.begin(0.2f);
#endif
}

void loop() {
  webSocket.loop();

  IMU1.update();
  IMU2.update();

  IMU1.getAccel(&IMUAccel1);
  IMU1.getGyro(&IMUGyro1);
  
  IMU2.getAccel(&IMUAccel2);
  IMU2.getGyro(&IMUGyro2);

  filter1.updateIMU(IMUGyro1.gyroX, IMUGyro1.gyroY, IMUGyro1.gyroZ, IMUAccel1.accelX, IMUAccel1.accelY, IMUAccel1.accelZ);
  filter2.updateIMU(IMUGyro2.gyroX, IMUGyro2.gyroY, IMUGyro2.gyroZ, IMUAccel2.accelX, IMUAccel2.accelY, IMUAccel2.accelZ);

  // Prepare data strings for WebSocket transmission
  String data1 = String(filter1.getQuatW()) + "," + String(filter1.getQuatX()) + "," + String(filter1.getQuatY()) + "," + String(filter1.getQuatZ());
  String data2 = String(filter2.getQuatW()) + "," + String(filter2.getQuatX()) + "," + String(filter2.getQuatY()) + "," + String(filter2.getQuatZ());

  Serial.println(data1);
  Serial.println(data2);

  // Send the data over WebSocket
  webSocket.broadcastTXT(data1 + "," + data2);

  delay(50);
}
