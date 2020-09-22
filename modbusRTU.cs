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

            SqlConnection myConnection = new SqlConnection(@"Data Source=DESKTOP-JIMMY;Initial Catalog=SensorDataDB;Integrated Security=True");
            myConnection.Open();
            
            int i = 0;
            while (i < 10000000) {
                try {
                    i += 1;
                    for(byte j=1; j<4; j++){
                    modbusClient.UnitIdentifier = j;
                        string timestamp0 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        //string timestamp0 = now.ToString("yyyy-MM-dd HH:mm:ss.fff"); // 
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
                        
                        string sql_str_temp = "INSERT INTO DEV_TEMP_" + j.ToString() + " (Temperature, DateAndTime) Values (@Temperature, @DateAndTime)";
                        string sql_str_humid = "INSERT INTO DEV_HUMID_" + j.ToString() + " (Humidity, DateAndTime) Values (@Humidity, @DateAndTime)";
                        string sql_str_part03 = "INSERT INTO DEV_PART03_" + j.ToString() + " (Particle03, DateAndTime) Values (@Particle03, @DateAndTime)";
                        string sql_str_part05 = "INSERT INTO DEV_PART05_" + j.ToString() + " (Particle05, DateAndTime) Values (@Particle05, @DateAndTime)";

                        SqlCommand myCommand_temp = new SqlCommand(sql_str_temp, myConnection);
                        SqlCommand myCommand_humid = new SqlCommand(sql_str_humid, myConnection);
                        SqlCommand myCommand_part03 = new SqlCommand(sql_str_part03, myConnection);
                        SqlCommand myCommand_part05 = new SqlCommand(sql_str_part05, myConnection);

                        myCommand_temp.Parameters.AddWithValue("@Temperature ", Math.Round((temp / 100),2).ToString());
                        myCommand_temp.Parameters.AddWithValue("@DateAndTime", timestamp0);
                        myCommand_temp.ExecuteNonQuery();

                        myCommand_humid.Parameters.AddWithValue("@Humidity", Math.Round((humid / 100), 2));
                        myCommand_humid.Parameters.AddWithValue("@DateAndTime", timestamp0);
                        myCommand_humid.ExecuteNonQuery();

                        myCommand_part03.Parameters.AddWithValue("@Particle03", part03);
                        myCommand_part03.Parameters.AddWithValue("@DateAndTime", timestamp0);
                        myCommand_part03.ExecuteNonQuery();

                        myCommand_part05.Parameters.AddWithValue("@Particle05", part05);
                        myCommand_part05.Parameters.AddWithValue("@DateAndTime", timestamp0);
                        myCommand_part05.ExecuteNonQuery();
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

        /*        string DB_Selector(int S_ID, string tb_name, string column_name)
                {
                    if (S_ID == 1)
                    {

                    }
                    else if (S_ID == 2)
                    {
                        string sql_str = "INSERT INTO " + DB_Name + " (S_ID, Temperature, Humidity, Particle03, Particle05, DateAndTime) " + "Values (@S_ID, @Temperature, @Humidity, @Particle03, @Particle05, @DateAndTime)";
                    }
                    else if (S_ID == 3)
                    {

                    }
                }*/
    }
}
