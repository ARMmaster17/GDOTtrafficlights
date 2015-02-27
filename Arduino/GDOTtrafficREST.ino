#include <SPI.h>
#include <Ethernet.h>
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
char server[] = "192.168.1.148";
IPAddress ip(192,168,0,177);
EthernetClient client;
int refreshPeriod;

void setup() {
  refreshPeriod = 1000;
   Serial.begin(9600);
    while (!Serial) {
     ;
   }
   if (Ethernet.begin(mac) == 0) {
     Serial.println("Failed to configure Ethernet using DHCP");
     Ethernet.begin(mac, ip);
   }
   delay(3000);
 /*Serial.println("connecting...");
   if (client.connect(server, 81)) {
     Serial.println("connected");
     client.println("GET /updateperiod HTTP/1.1");
     client.println("Host: 1920168.1.148");
     client.println("Connection: close");
     client.println();
   } 
   else {
     Serial.println("connection failed");
   }
   bool isFirstNum = true;
   while (!client.available())
   {
     
   }
   while (client.available())
   {
     String tempString;
     char c = client.read();
     if (isFirstNum)
     {
       refreshPeriod = int(c);
       isFirstNum = false;
     }
     else
     {
       refreshPeriod = refreshPeriod * 10;
       refreshPeriod = refreshPeriod + int(c);
     }
   }
   client.stop();*/
}

void loop()
{
   Serial.println("Refreshing data");
   if (client.connect(server, 81)) {
     Serial.println("connected");
     client.println("GET /0/lightstat HTTP/1.1");
     client.println("Host: 1920168.1.148");
     client.println("Connection: close");
     client.println();
   } 
   else {
     Serial.println("connection failed");
   }
   while (!client.available())
   {}
   while (client.available())
   {
     char c = client.read();
     Serial.print('Status:');
     Serial.print(c);
     Serial.print('/n');
   }
   client.stop();
   delay(refreshPeriod);
}
