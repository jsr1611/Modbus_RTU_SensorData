using System;
using EasyModbus;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;

namespace modbusRTU
{
    class Program
    {
        static void Main(string[] args)
        {
            ModbusClient modbusClient = new ModbusClient("COM3");
            modbusClient.Baudrate = 115200;	// Not necessary since default baudrate = 9600
            modbusClient.Parity = System.IO.Ports.Parity.None;
            modbusClient.StopBits = System.IO.Ports.StopBits.Two;
            modbusClient.ConnectionTimeout = 5000;
            modbusClient.Connect();
            Console.WriteLine("Device Connection Successful");
            
            String[] sensordata = {"ID", "Temp", "Humidity", "Part03", "Part05", "DateTime"};
            Console.WriteLine("Count\t" + string.Join("\t", sensordata));

            SqlConnection myConnection = new SqlConnection(@"Data Source=DESKTOP-JQMGA3H;Initial Catalog=MyDatabase01;Integrated Security=True");
            myConnection.Open();
            
            int i = 0;
            while (i < 1000000) {
                try {
                    i += 1;

                    for(byte j=1; j<4; j++){
                    modbusClient.UnitIdentifier = j;
                        DateTime now = DateTime.Now;
                        string timestamp0 = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        int[] test1 = modbusClient.ReadInputRegisters(10, 6);
                        float temp = test1[0];
                        float humid = test1[1];
                        int[] part03_arr = { 0, 0 };
                        int[] part05_arr = { 0, 0 };
                        part03_arr[0] = test1[3];
                        part03_arr[1] = test1[2];
                        part05_arr[0] = test1[5];
                        part05_arr[1] = test1[4];

                        Int64 part03 = ModbusClient.ConvertRegistersToInt(part03_arr);
                        Int64 part05 = ModbusClient.ConvertRegistersToInt(part05_arr);
                        Console.WriteLine(i + "\t" + j + "\t" + (temp / 100).ToString() + "\t" + (humid / 100).ToString() + "\t\t" + String.Format("{0:n0}", part03) + "\t" + String.Format("{0:n0}", part05) + "\t" + timestamp0);
                            
                        SqlCommand myCommand = new SqlCommand("INSERT INTO SensorData (ID, Temperature, Humidity, Particle03, Particle05, DateAndTime) " + "Values (@ID, @Temperature, @Humidity, @Particle03, @Particle05, @DateAndTime)", myConnection);
                        myCommand.Parameters.AddWithValue("@ID", j.ToString() );
                        myCommand.Parameters.AddWithValue("@Temperature", (temp / 100).ToString());
                        myCommand.Parameters.AddWithValue("@Humidity", (humid / 100).ToString());
                        myCommand.Parameters.AddWithValue("@Particle03", String.Format("{0:n0}", part03));
                        myCommand.Parameters.AddWithValue("@Particle05", String.Format("{0:n0}", part05));
                        myCommand.Parameters.AddWithValue("@DateAndTime", timestamp0);
                        myCommand.ExecuteNonQuery();
                     }  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            myConnection.Close();
            modbusClient.Disconnect();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
