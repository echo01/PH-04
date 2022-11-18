using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Calibrate_PB_04;

namespace Calibrate_PH_04
{
    public class PB04_testpoint
    {
        public float[] DC0mA = new float[8];
        public float[] DC10mA = new float[8];
        public float[] DC20mA1 = new float[8];
        public float[] DC4mA = new float[8];
        public float[] DC12mA = new float[8];
        public float[] DC20mA = new float[8];

        public bool[] DC0mAresult = new bool[8];
        public bool[] DC10mAresult = new bool[8];
        public bool[] DC20mA1result = new bool[8];

        public bool[] DC4mAresult = new bool[8];
        public bool[] DC12mAresult = new bool[8];
        public bool[] DC20mAresult = new bool[8];


        public float[] DC0mV = new float[8];
        public float[] DC500mV = new float[8];
        public float[] DC1000mV = new float[8];

        public bool[] DC0mVresult = new bool[8];
        public bool[] DC500mVresult = new bool[8];
        public bool[] DC1000mVresult = new bool[8];

        public float[] DCb100mV = new float[8];
        public float[] DCb500mV = new float[8];
        public float[] DCb1000mV = new float[8];

        public bool[] DCb100mVresult = new bool[8];
        public bool[] DCb500mVresult = new bool[8];
        public bool[] DCb1000mVresult = new bool[8];

        public float[] DC00mV = new float[8];
        public float[] DC2500mV = new float[8];
        public float[] DC5000mV = new float[8];

        public bool[] DC00mVresult = new bool[8];
        public bool[] DC2500mVresult = new bool[8];
        public bool[] DC5000mVresult = new bool[8];

        public float[] DCb00mV = new float[8];
        public float[] DCb2500mV = new float[8];
        public float[] DCb5000mV = new float[8];

        public bool[] DCb00mVresult = new bool[8];
        public bool[] DCb2500mVresult = new bool[8];
        public bool[] DCb5000mVresult = new bool[8];

        public float[] DC0V = new float[8];
        public float[] DC5V = new float[8];
        public float[] DC10V = new float[8];

        public bool[] DC0Vresult = new bool[8];
        public bool[] DC5Vresult = new bool[8];
        public bool[] DC10Vresult = new bool[8];

        public bool[] Cal_Result = new bool[8];

        public Int32[] AnalogCH = new Int32[8];


        public void Cal_DCmA(float[] an, Int16[] value)
        {
            int i = 0;
            for (i = 0; i < value.Length; i++)
            {
                an[i] = value[i] / 1000;
            }
        }

        public void Cal_DCmV(float[] an, Int16[] value)
        {
            int i = 0;
            for (i = 0; i < value.Length; i++)
            {
                an[i] = value[i] / 1000;
            }
        }

        public bool CH_Calibrate_mAResult(int ch)
        {
            bool test_result;
            test_result = DC0mAresult[ch] & DC10mAresult[ch] & DC20mA1result[ch] & DC4mAresult[ch] & DC12mAresult[ch] & DC20mAresult[ch];
            return test_result;
        }

        public bool CH_Calibrate_mVResult(int ch)
        {
            bool test_result;
            test_result = DC0mVresult[ch] & DC500mVresult[ch] & DC1000mVresult[ch] & DCb100mVresult[ch] & DCb500mVresult[ch] & DCb1000mVresult[ch];
            test_result = test_result & DC00mVresult[ch] & DC2500mVresult[ch] & DC5000mVresult[ch] & DCb00mVresult[ch] & DCb2500mVresult[ch] & DCb5000mVresult[ch];
            test_result = test_result & DC0Vresult[ch] & DC5Vresult[ch] & DC10Vresult[ch];
            return test_result;
        }

        public bool CH_calibrate_mAmVResult(int ch)
        {
            bool test_result;
            test_result = CH_Calibrate_mAResult(ch) & CH_Calibrate_mVResult(ch);
            return test_result;
        }

        public bool Report_result()
        {
            bool test_result;
            int i;
            for (i = 0; i < 8; i++)
            {
                Cal_Result[i] = CH_calibrate_mAmVResult(i);
            }
            test_result = Cal_Result[0] & Cal_Result[1] & Cal_Result[2] & Cal_Result[3] & Cal_Result[4] & Cal_Result[5] & Cal_Result[6] & Cal_Result[7];
            return test_result;
        }

        public bool Report_resultmA()
        {
            bool test_result;
            int i;
            for (i = 0; i < 8; i++)
            {
                Cal_Result[i] = CH_Calibrate_mAResult(i);
            }
            test_result = Cal_Result[0] & Cal_Result[1] & Cal_Result[2] & Cal_Result[3] & Cal_Result[4] & Cal_Result[5] & Cal_Result[6] & Cal_Result[7];
            return test_result;
        }

        public bool Report_resultmV()
        {
            bool test_result;
            int i;
            for (i = 0; i < 8; i++)
            {
                Cal_Result[i] = CH_Calibrate_mVResult(i);
            }
            test_result = Cal_Result[0] & Cal_Result[1] & Cal_Result[2] & Cal_Result[3] & Cal_Result[4] & Cal_Result[5] & Cal_Result[6] & Cal_Result[7];
            return test_result;
        }


    }
    public partial class PB04 : Component
    {

        public PB04_testpoint TestTable = new PB04_testpoint();
        public IPAddress ip;
        
        public Int32[] AnalogCH = new Int32[8];
        public Int16 InputsStatus;
        public Int16 InputOverRange;
        public Int16[] InputType = new Int16[8];
        public Int16[] PV = new Int16[8];
        public Int16[] Reg_config = new Int16[16];
        public bool reg_ready;

        byte[] cmd_msg = new byte[64];
        //byte[] rx_msg = new byte[96];
        byte[] CheckingMsg = { 1, 0, 0, 0, 0, 6, 1, 3, 0, 0, 0, 1 };

        public bool Ready;
        public bool IsPortOpen;
        public bool IsResponded;
        public byte[] rx_message = new byte[96];
        public Int32 nrx_byte;
        public byte SlaveId;
        public Int16 TransctionID;
        public Int16 ProtocolID;

        public TcpClient client;
        //public NetworkStream Tcp_stream;
        private Thread tcp_thread;
        CoreModbusTCP modbusTcp = new CoreModbusTCP();
        public int port;
        public String server;

        public PB04()
        {
            InitializeComponent();
        }

        public PB04(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public void start_tcp(String server, int port)
        {
            IsPortOpen = false;
            try
            {
                client = new TcpClient(server, port);
                client.ReceiveTimeout = 1000;
                client.SendTimeout = 1000;
                //Tcp_stream = client.GetStream();
                this.server = server;
                this.port = port;
                this.SlaveId = 1;
                //client.Close();
                IsPortOpen = true;
            }
            catch (ArithmeticException e)
            {
                MessageBox.Show(e.Message); ;
            }
            if(IsPortOpen)
                client.Dispose();
        }

        public void stop_tcp()
        {
            if (!IsPortOpen) return;
            try
            {
                IsPortOpen=false;
                client.Close();
            }
            catch (ArithmeticException e)
            {
                MessageBox.Show(e.Message); 
            }

        }

        public Boolean send_receive_msg(byte[] data)
        {
            if (!IsPortOpen) return false;
            try
            {
                client = new TcpClient(server, port);
                NetworkStream Tcp_stream = client.GetStream();
                Tcp_stream.Write(data, 0, data.Length);
                //Thread.Sleep(1000);
                nrx_byte = Tcp_stream.Read(rx_message, 0, rx_message.Length);
                IsResponded = true;
                Tcp_stream.Dispose();
                client.Dispose();    
            }
            catch (ArithmeticException e)
            {
                IsResponded = false;
                MessageBox.Show(e.Message);
                return false;
            }
                        
            return (true);
        }

        public void transmitte(byte[] data)
        {
            Thread send_task;
            send_task = new Thread(() => send_receive_msg(data));
            send_task.IsBackground = true;
            send_task.Start();
        }

        public void Table1_get()
        {
            IsResponded = false;
            transmitte(modbusTcp.read_holding(SlaveId,0,42));
        }

        public Boolean Is_module_ready()
        {
            Table1_get();
            Thread.Sleep(1000);
            if (!modbusTcp.check_resp_msg(rx_message, 1, this.SlaveId,3))
                return false;
            update_reg(rx_message);
            return IsResponded;
        }
        public Boolean Is_PV_notZeor()
        {
            Boolean result=true;
            for (int i=0; i <= 7; i++)
                {
                if (this.AnalogCH[i] > 1000)
                    result &= true;
                else
                    result &= false;
                }
            return result;
        }

        public static Int16 swap(Int16 input)
        {

            return ((Int16)(((0xFF00 & input) >> 8) | ((0x00FF & input) << 8)));
        }

        public static int swap_int(Int16 input)
        {
            return ((int)(((0xFF00 & input) >> 8) | ((0x00FF & input) << 8)));
        }

        private void update_reg(byte[] value)
        {

            Int16 TranID = swap(BitConverter.ToInt16(value, 0));
            Int16 ProID = swap(BitConverter.ToInt16(value,2)); 
            Int16 DataLength = swap(BitConverter.ToInt16(value,4));
            byte UnitID = value[6];
            byte FuncCode = value[7];
            byte rxlength = value[8];
            byte[] Data = new byte[rxlength];
            Array.Copy(value, 9, Data, 0, rxlength);

            Int16[] reg_raw = new Int16[rxlength / 2];
            for (int i = 0; i < rxlength / 2; i++)
            {
                reg_raw[i] = BitConverter.ToInt16(Data, i * 2);
            }

            this.AnalogCH[0] = swap_int(reg_raw[0]);
            this.AnalogCH[1] = swap_int(reg_raw[1]);
            this.AnalogCH[2] = swap_int(reg_raw[2]);
            this.AnalogCH[3] = swap_int(reg_raw[3]);
            this.AnalogCH[4] = swap_int(reg_raw[4]);
            this.AnalogCH[5] = swap_int(reg_raw[5]);
            this.AnalogCH[6] = swap_int(reg_raw[6]);
            this.AnalogCH[7] = swap_int(reg_raw[7]);
            this.InputsStatus = swap(reg_raw[8]);
            this.InputOverRange = swap(reg_raw[9]);
            this.InputType[0] = swap(reg_raw[10]);
            this.InputType[1] = swap(reg_raw[11]);
            this.InputType[2] = swap(reg_raw[12]);
            this.InputType[3] = swap(reg_raw[13]);
            this.InputType[4] = swap(reg_raw[14]);
            this.InputType[5] = swap(reg_raw[15]);
            this.InputType[6] = swap(reg_raw[16]);
            this.InputType[7] = swap(reg_raw[17]);
            this.PV[0] = swap(reg_raw[18]);
            this.PV[1] = swap(reg_raw[19]);
            this.PV[2] = swap(reg_raw[20]);
            this.PV[3] = swap(reg_raw[21]);
            this.PV[4] = swap(reg_raw[22]);
            this.PV[5] = swap(reg_raw[23]);
            this.PV[6] = swap(reg_raw[24]);
            this.PV[7] = swap(reg_raw[25]);
            this.Reg_config[0] = swap(reg_raw[26]);
            this.Reg_config[1] = swap(reg_raw[27]);
            this.Reg_config[2] = swap(reg_raw[28]);
            this.Reg_config[3] = swap(reg_raw[29]);
            this.Reg_config[4] = swap(reg_raw[30]);
            this.Reg_config[5] = swap(reg_raw[31]);
            this.Reg_config[6] = swap(reg_raw[32]);
            this.Reg_config[7] = swap(reg_raw[33]);
        }

        public Boolean write_input_type(List<int> value)
        {
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId,10,value);
            try
            {
                transmitte(this.cmd_msg);
                Thread.Sleep(1000);
                if (!modbusTcp.check_resp_msg(rx_message, 1, this.SlaveId, 16))
                    return false;
                update_reg(rx_message);
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool send_data_cmd()
        {
            try
            {
                transmitte(this.cmd_msg);
                Thread.Sleep(1000);
                if (!modbusTcp.check_resp_msg(rx_message, 1, this.SlaveId, 16))
                    return false;
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool Calibrate_enable()
        {
            List<int> cmd = new List<int>();
            cmd.Add(1);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x500, cmd);
            return this.send_data_cmd();
        }
        public bool read_enable()
        {
            List<int> cmd = new List<int>();
            cmd.Add(0);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x500, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Mux_channel(int channel)
        {
            List<int> cmd = new List<int>();
            cmd.Add(channel);
            cmd.Add(1);
            //this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x501, cmd);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x501, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(2);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(3);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_4mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(4);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_4to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(5);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to1000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(6);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to1000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(7);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to5000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(8);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to5000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(9);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(10);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(11);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_SaveEEPROM()
        {
            List<int> cmd = new List<int>();
            cmd.Add(12);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }


        public bool Setup_test0to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(0);             //  input1 type =0  
            cmd.Add(0);             //  input2 type =0 
            cmd.Add(0);             //  input3 type =0 
            cmd.Add(0);             //  input4 type =0 
            cmd.Add(0);             //  input5 type =0 
            cmd.Add(0);             //  input6 type =0 
            cmd.Add(0);             //  input7 type =0 
            cmd.Add(0);             //  input8 type =0 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            //this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test4to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(1);             //  input1 type =1  
            cmd.Add(1);             //  input2 type =1 
            cmd.Add(1);             //  input3 type =1 
            cmd.Add(1);             //  input4 type =1 
            cmd.Add(1);             //  input5 type =1 
            cmd.Add(1);             //  input6 type =1 
            cmd.Add(1);             //  input7 type =1 
            cmd.Add(1);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test0to1V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(2);             //  input1 type =1  
            cmd.Add(2);             //  input2 type =1 
            cmd.Add(2);             //  input3 type =1 
            cmd.Add(2);             //  input4 type =1 
            cmd.Add(2);             //  input5 type =1 
            cmd.Add(2);             //  input6 type =1 
            cmd.Add(2);             //  input7 type =1 
            cmd.Add(2);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test_bipola0to1V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(3);             //  input1 type =1  
            cmd.Add(3);             //  input2 type =1 
            cmd.Add(3);             //  input3 type =1 
            cmd.Add(3);             //  input4 type =1 
            cmd.Add(3);             //  input5 type =1 
            cmd.Add(3);             //  input6 type =1 
            cmd.Add(3);             //  input7 type =1 
            cmd.Add(3);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test_0to5V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(4);             //  input1 type =1  
            cmd.Add(4);             //  input2 type =1 
            cmd.Add(4);             //  input3 type =1 
            cmd.Add(4);             //  input4 type =1 
            cmd.Add(4);             //  input5 type =1 
            cmd.Add(4);             //  input6 type =1 
            cmd.Add(4);             //  input7 type =1 
            cmd.Add(4);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test_Bipolar0to5V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(5);             //  input1 type =1  
            cmd.Add(5);             //  input2 type =1 
            cmd.Add(5);             //  input3 type =1 
            cmd.Add(5);             //  input4 type =1 
            cmd.Add(5);             //  input5 type =1 
            cmd.Add(5);             //  input6 type =1 
            cmd.Add(5);             //  input7 type =1 
            cmd.Add(5);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand


            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test_0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(6);             //  input1 type =1  
            cmd.Add(6);             //  input2 type =1 
            cmd.Add(6);             //  input3 type =1 
            cmd.Add(6);             //  input4 type =1 
            cmd.Add(6);             //  input5 type =1 
            cmd.Add(6);             //  input6 type =1 
            cmd.Add(6);             //  input7 type =1 
            cmd.Add(6);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool Setup_test_Bipolar0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(7);             //  input1 type =1  
            cmd.Add(7);             //  input2 type =1 
            cmd.Add(7);             //  input3 type =1 
            cmd.Add(7);             //  input4 type =1 
            cmd.Add(7);             //  input5 type =1 
            cmd.Add(7);             //  input6 type =1 
            cmd.Add(7);             //  input7 type =1 
            cmd.Add(7);             //  input8 type =1 

            cmd.Add(0);             //  Display input 1 =2  zero spand
            cmd.Add(0);             //  Display input 2 =2  zero spand
            cmd.Add(0);             //  Display input 3 =2  zero spand
            cmd.Add(0);             //  Display input 4 =2  zero spand
            cmd.Add(0);             //  Display input 5 =2  zero spand
            cmd.Add(0);             //  Display input 6 =2  zero spand
            cmd.Add(0);             //  Display input 7 =2  zero spand
            cmd.Add(0);             //  Display input 8 =2  zero spand

            this.cmd_msg = modbusTcp.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }


    }
}
