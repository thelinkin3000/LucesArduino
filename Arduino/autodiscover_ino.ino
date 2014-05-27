#include <SPI.h>
#include <Client.h>
#include <Ethernet.h>
#include <Server.h>
#include <Udp.h>
#include <EthernetUdp.h>

// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192,168,2,10);

// Initialize the Ethernet server library
// with the IP address and port you want to use 
// (port 80 is default for HTTP):
EthernetServer server(88);
unsigned int localPort(8888);
int led1= 13; //Luz1
int led2= 26; //Luz2

char packetBuffer[UDP_TX_PACKET_MAX_SIZE]; //buffer to hold incoming packet,
char ReplyBuffer[] = "ArduinoMEGA";       //el nombre que el arduino va a responder en el paquete UDP

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP Udp;


String readString = String(100);
String luz1;
String luz2;


void setup()
{
  // start the Ethernet connection and the server:
  Ethernet.begin(mac, ip); //Parece que si no ponemos la ip, la pide por DHCP, veremos
  server.begin();
  Udp.begin(localPort); //Acá iniciamos un servidor UDP que escuche en el puerto localport
  pinMode(led1,OUTPUT);
  pinMode(led2,OUTPUT);
}

void loop()
{
  // Acá escucha por algún cliente UDP (En este caso el celular)
  int packetSize = Udp.parsePacket();
  if (packetSize)
  {
    IPAddress remote = Udp.remoteIP();
    
    // Lee el paquete y lo pone en packetBuffer
    Udp.read(packetBuffer,UDP_TX_PACKET_MAX_SIZE);
    // Manda una respuesta. Lo que se puede hacer del lado del cliente es recibir esta respuesta y sacar de ahí la ip del arduino
	// Tambien lo que se puede hacer y lo hago yo acá, es devolver dentro de la respuesta el nombre del arduino que en este
	// caso está en la variable "replybuffer". 
        Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
        Udp.write(ReplyBuffer);
        Udp.endPacket();    
  }
  
  
  EthernetClient client = server.available();
  if (client) {
    // an http request ends with a blank line
    boolean currentLineIsBlank = true;
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        if (readString.length()<100) //leer peticion HTTP caracter por caracter
        {
          readString += c;
        }
        if (c=='\n') //Si la peticion HTTP ha finalizado
        {
          //Determinar lo que se recibe mediante GET para encender el Led o apagarlo
          if(readString.indexOf("led1=On")>0){
            digitalWrite(led1,HIGH);
            luz1="Prendido";
          }
          if(readString.indexOf("led1=Off")>0){
            digitalWrite(led1,LOW);
            luz1="Apagado";
          }
          if(readString.indexOf("led2=On")>0){
            digitalWrite(led2,HIGH);
            luz2="Prendido";
          }
          if(readString.indexOf("led2=Off")>0){
            digitalWrite(led2,LOW);
            luz2="Apagado";
          }

          readString=""; //Vaciar el string que se uso para la lectura
          //Enviar cabecera HTTP estandar

          client.println("HTTP/1.1 200 OK");
          client.println("Content-Type: text/html");
          client.println();

          //Devolver estado de las luces

          client.println("Ok");
          client.println("Luz 1 : " + luz1);
          client.println("Luz 2 : " + luz2);


          client.stop();

        }
      }
    }
  }
}

