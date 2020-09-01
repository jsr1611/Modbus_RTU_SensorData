using System;
using EasyModbus;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
//using System.Runtime;

namespace modbusRTU
{
    class Program
    {

        static ModbusClient modbusconnect()
        {
            ModbusClient modbusClient = new ModbusClient("COM3");
            modbusClient.UnitIdentifier = 1; //Not necessary since default slaveID = 1;
            modbusClient.Baudrate = 115200;	// Not necessary since default baudrate = 9600
            modbusClient.Parity = System.IO.Ports.Parity.None;
            modbusClient.StopBits = System.IO.Ports.StopBits.Two;
            modbusClient.ConnectionTimeout = 5000;
            modbusClient.Connect();
            Console.WriteLine("Connection Successful");
            return modbusClient;
        }
        
            
        static void Main(string[] args)
        {
            // sensor ID = 1


            /*ModbusClient modbusClient2 = modbusClient;
            modbusClient2.UnitIdentifier = 2; //Not necessary since default slaveID = 1;
            modbusClient2.Baudrate = 115200;	// Not necessary since default baudrate = 9600
            modbusClient2.Parity = System.IO.Ports.Parity.None;
            modbusClient2.StopBits = System.IO.Ports.StopBits.Two;
            modbusClient2.ConnectionTimeout = 5000;
            modbusClient2.Connect();

            ModbusClient modbusClient3 = modbusClient;
            modbusClient3.UnitIdentifier = 3; //Not necessary since default slaveID = 1;
            modbusClient3.Baudrate = 115200;	// Not necessary since default baudrate = 9600
            modbusClient3.Parity = System.IO.Ports.Parity.None;
            modbusClient3.StopBits = System.IO.Ports.StopBits.Two;
            modbusClient3.ConnectionTimeout = 5000;
            modbusClient3.Connect();

            ModbusClient modbusClient4 = modbusClient;
            modbusClient4.UnitIdentifier = 4; //Not necessary since default slaveID = 1;
            modbusClient4.Baudrate = 115200;	// Not necessary since default baudrate = 9600
            modbusClient4.Parity = System.IO.Ports.Parity.None;
            modbusClient4.StopBits = System.IO.Ports.StopBits.Two;
            modbusClient4.ConnectionTimeout = 5000;
            modbusClient4.Connect();

            */
            ModbusClient data1 = new ModbusClient();
            data1 = modbusconnect();
            Console.WriteLine("Connection successful");
            String[] sensordata = {"Id", "Temp", "Humidity", "Part03", "Part05", "DateTime"};
            Console.WriteLine(string.Join("\t", sensordata));



            //var fm = String.Format("###,###,###");

            int i = 0;
            while (i++ < 5000) {

                DateTime now = DateTime.Now;
                string timestamp0 = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                
                int[] test1 = data1.ReadInputRegisters(10, 6);
                float temp = test1[0];
                float humid = test1[1];
                int[] part03_arr = { 0, 0 };
                int[] part05_arr = { 0, 0 };
                part03_arr[0] = test1[3];
                part03_arr[1] = test1[2];
                part05_arr[0] = test1[5];
                part05_arr[1] = test1[4];

                //float temp = modbusClient.ReadInputRegisters(10, 1)[0];
                //float humid = modbusClient.ReadInputRegisters(11, 1)[0];

                Int32 part03 = ModbusClient.ConvertRegistersToInt(part03_arr);
                Int32 part05 = ModbusClient.ConvertRegistersToInt(part05_arr);
                
                
                //data2 = dataReceive(modbusClient2);
                // data3 = dataReceive(modbusClient3);
                //data4 = dataReceive(modbusClient4);


                Console.WriteLine("1\t" + (temp / 100).ToString() + "\t" + (humid / 100).ToString() + "\t\t" + String.Format("{0:n0}", part03) + "\t" + String.Format("{0:n0}", part05) + "\t" + timestamp0);   //print sensor data to screen
                //Console.WriteLine("1\t" + ((float)data2[0] / 100).ToString() + "\t" + ((float)data2[1] / 100).ToString() + "\t\t" + String.Format("{0:n0}", data2[2]) + "\t" + String.Format("{0:n0}", data2[3]) + "\t" + timestamp0);   //print sensor data to screen
               // Console.WriteLine("1\t" + ((float)data3[0] / 100).ToString() + "\t" + ((float)data3[1] / 100).ToString() + "\t\t" + String.Format("{0:n0}", data3[2]) + "\t" + String.Format("{0:n0}", data3[3]) + "\t" + timestamp0);   //print sensor data to screen
               // Console.WriteLine("1\t" + ((float)data4[0] / 100).ToString() + "\t" + ((float)data3[1] / 100).ToString() + "\t\t" + String.Format("{0:n0}", data4[2]) + "\t" + String.Format("{0:n0}", data4[3]) + "\t" + timestamp0);   //print sensor data to screen
            }
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);

            
        }
    }
}
