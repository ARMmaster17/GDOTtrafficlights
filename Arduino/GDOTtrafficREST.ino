#include <SPI.h>
#include <Ethernet.h>

// Enter a MAC address for your controller below.
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
//Insert IP address of tl_core server
//IPAddress server(192,168,1,148);
char server[] = "192.168.1.148"; 
// Set IP as DHCP failsafe
 IPAddress ip(192,168,0,177);
 EthernetClient client;
 int node = 0; //Set this to whatever light node this device will be
void setup() {
  // Open serial communications and wait for port to open:
   Serial.begin(9600);
    while (!Serial) {
     ; // wait for serial port to connect. Needed for Leonardo only
   }

   // start the Ethernet connection:
   if (Ethernet.begin(mac) == 0) {
     Serial.println("DHCP configuration failed. RETRYING");
     // DHCP fail
     Ethernet.begin(mac, ip);
   }
   Serial.println(Ethernet.localIP());
   delay(1000);
}

void loop()
{
  Serial.println("Attaching to REST service");
   if (client.connect(server, 81)) { //Replace 81 with whatever port your server is using (81 by default)
     Serial.println("Attached");
     //String msg = "GET /" + node + "/lightstat HTTP/1.1";
     //client.println(msg);
     client.println("GET /0/lightstat HTTP/1.1");
     client.println("Host: 192.168.1.148:81");
     client.println("Connection: close");
     client.println();
   } 
   else {
     Serial.println("Server down");
   } 
  // if there are incoming bytes available 
   // from the server, read them and print them:
   char response[] = "";
   while (client.available())
   {
     char c = client.read();
     Serial.print(c);
     if (c != "")
     {
     response += c;
     }
     else
     {
       break;
     }
   }
   //////////////////////////////////////////
     //Begin interpreting the response
     if (response == "")
     {
       
     }
     else
     {
     String resp = response;
     String removal = "<xml><trafficstatus><status id=\"" + node + "\">";
     String removal2 = "</status></trafficstatus></xml>";
     resp = resp.replace(removal, "");
     resp = resp.replace(removal2, "");
     //resp is now an int in string format that can be used to control individual pins accordingly
     Serial.print(resp);
     }
     //////////////////////////////////////////

   // if the server's disconnected, stop the client:
   if (!client.connected()) {
     Serial.println();
     Serial.println("disconnecting.");
     client.stop();

     // do nothing forevermore:
     while(true);
   }
}
