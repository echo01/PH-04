using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using System.IO.Ports;

namespace Calibrate_PB_04
{
    public class PH04_testpoint
    {
        public float[] DC0mA =new float[8];
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

        public Int16[] AnalogCH = new Int16[8];
        

        public void Cal_DCmA(float[] an,Int16[] value)
        {
            int i = 0;
            for(i=0; i < value.Length; i++)
            {
                an[i] = value[i]/1000;
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
            for(i=0;i<8;i++)
            {
                Cal_Result[i] = CH_calibrate_mAmVResult(i);
            }
            test_result = Cal_Result[0] & Cal_Result[1] & Cal_Result[2] & Cal_Result[3] & Cal_Result[4] & Cal_Result[5] & Cal_Result[6] & Cal_Result[7];
            return test_result;
        }

    }

    public partial class PH04 : Component
    {


        int nreadbyte;
        CoreModbusTCP modbusTcp = new CoreModbusTCP();

        public PH04_testpoint TestTable = new PH04_testpoint();

        public Int16[] AnalogCH = new Int16[8];
        public Int16 InputsStatus;
        public Int16 InputOverRange;
        public Int16[] InputType = new Int16[8];
        public Int16[] PV = new Int16[8];
        public Int16[] Reg_config = new Int16[16];
        public bool reg_ready;

        byte[] cmd_msg = new byte[64];
        //byte[] rx_msg = new byte[96];
        byte[] CheckingMsg = { 1, 0, 0, 0, 0, 6, 1, 3, 0, 0, 0, 1 };

        public SerialPort sPort1 = new SerialPort();
        public bool Ready;
        public bool IsPortOpen;
        public bool IsResponded;
        public byte[] rx_message = new byte[96];
        public int nrx_byte;
        public byte SlaveId;


        public PH04()
        {
            InitializeComponent();

            IsPortOpen = false;
            IsResponded = false;
            Ready = false;
        }

        public PH04(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Set_SlaveID(byte slaveid)
        {
            this.SlaveId = slaveid;
        }

        public bool Start_port(SerialPort serialport)
        {
            sPort1 = serialport;
            try
            {
                this.IsPortOpen = sPort1.IsOpen;
                if (!this.IsPortOpen)
                {
                    this.sPort1.Open();
                    this.IsPortOpen = true;
                }
                Start_Remote();
            }
            catch (Exception ex)
            {
                IsPortOpen = false;
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        public void Start_Remote()
        {
            this.Ready = false;
            this.cmd_msg = CoreModbus.Read(this.SlaveId, 0, 1);
            this.port_write(this.cmd_msg, 0, this.cmd_msg.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 7);
            Thread.Sleep(300);
            if (!CoreModbus.MsgCheck(rx_message, 3, this.SlaveId, nrx_byte))
                return;
            this.Ready = true;
        }


        public int port_read(byte[] buffer, int offset, int count)
        {
            int n_rx;
            try
            {
                int counttry = 0;
                for (n_rx = 0; n_rx < count;)
                {
                    int n = sPort1.Read(buffer, n_rx, count - n_rx);
                    n_rx += n;
                    if (n != 0) counttry = 0;
                    counttry += 1;
                    if (counttry > 3)
                        break;
                }
                return n_rx;
            }
            catch
            {
                return 0;
            }
        }

        public void port_write(byte[] buffer, int offset, int count)
        {
            try
            {
                sPort1.Write(buffer, offset, count);
            }
            catch
            {
                return;
            }
        }


        public void init_connection_server(string ServerIp, int port)
        {

        }

        public static Int16 swap(Int16 input)
        {
  
            return ((Int16)(((0xFF00 & input) >> 8) | ((0x00FF & input) << 8)));
        }

        void update_pb04(byte[] value)
        {

            byte UnitID = value[0];
            byte FuncCode = value[1];
            byte DataLength = value[2]; 
            byte[] Data = new byte[DataLength];
            Array.Copy(value, 3, Data, 0, DataLength);

            Int16[] reg_raw = new Int16[DataLength / 2];
            for (int i = 0; i < DataLength/ 2; i++)
            {
                reg_raw[i] = BitConverter.ToInt16(Data, i * 2);
            }
            
            this.AnalogCH[0] = swap(reg_raw[0]);
            this.AnalogCH[1] = swap(reg_raw[1]);
            this.AnalogCH[2] = swap(reg_raw[2]);
            this.AnalogCH[3] = swap(reg_raw[3]);
            this.AnalogCH[4] = swap(reg_raw[4]);
            this.AnalogCH[5] = swap(reg_raw[5]);
            this.AnalogCH[6] = swap(reg_raw[6]);
            this.AnalogCH[7] = swap(reg_raw[7]);
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

        public void update_reg()
        {
            if (!this.Ready) return;
            this.cmd_msg = CoreModbus.Read(this.SlaveId, 0, 42);
            try
            {
                this.port_write(this.cmd_msg, 0, this.cmd_msg.Length);
                Thread.Sleep(50);
                nrx_byte = this.port_read(rx_message, 0, 42*2+5);
                Thread.Sleep(300);

                if (!CoreModbus.MsgCheck(rx_message, 3, this.SlaveId, nrx_byte))
                    return;
                update_pb04(rx_message);
            }
            catch
            {

            }
        }

        public void write_input_type(List<int> value)
        {
            if (!this.Ready) return;
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10,value);
            try
            {
                this.port_write(this.cmd_msg, 0, this.cmd_msg.Length);
                Thread.Sleep(50);
                nrx_byte = this.port_read(rx_message, 0, 8);
                if (!CoreModbus.MsgCheck(rx_message, 16, this.SlaveId, nrx_byte))
                    return;
                update_pb04(rx_message);
            }
            catch
            {

            }
        }


        public bool send_data_cmd()
        {
            if (!this.Ready) return false;
            //this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, value);
            try
            {
                this.port_write(this.cmd_msg, 0, this.cmd_msg.Length);
                Thread.Sleep(80);
                nrx_byte = this.port_read(rx_message, 0, 8);
                return CoreModbus.MsgCheck(rx_message, 16, this.SlaveId, nrx_byte);
            }
            catch
            {
                return false;
            }
        }

        public bool Calibrate_enable()
        {
            List<int> cmd = new List<int>();
            cmd.Add(1);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x500, cmd);
            return this.send_data_cmd();
        }
        public bool read_enable()
        {
            List<int> cmd = new List<int>();
            cmd.Add(0);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x500, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Mux_channel(int channel)
        {
            List<int> cmd = new List<int>();
            cmd.Add(channel);
            cmd.Add(1);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x501, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(2);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId,0x502,cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(3);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_4mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(4);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_4to20mA()
        {
            List<int> cmd = new List<int>();
            cmd.Add(5);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to1000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(6);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to1000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(7);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to5000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(8);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to5000mV()
        {
            List<int> cmd = new List<int>();
            cmd.Add(9);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Offset_0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(10);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Gain_0to10V()
        {
            List<int> cmd = new List<int>();
            cmd.Add(11);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_SaveEEPROM()
        {
            List<int> cmd = new List<int>();
            cmd.Add(12);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool Calibrate_Duplicate_to_all_CH()
        {
            List<int> cmd = new List<int>();
            cmd.Add(13);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
            return this.send_data_cmd();
        }

        public bool load_current_calibate()
        {
            List<int> cmd = new List<int>();
            cmd.Add(14);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0x502, cmd);
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

            /*cmd.Add(0);             // Zero PV1 =0  zero
            cmd.Add(20000);         // Zero PV1 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV2 =0  zero
            cmd.Add(20000);         // Zero PV2 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV3 =0  zero
            cmd.Add(20000);         // Zero PV3 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV4 =0  zero
            cmd.Add(20000);         // Zero PV4 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV5 =0  zero
            cmd.Add(20000);         // Zero PV5 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV6 =0  zero
            cmd.Add(20000);         // Zero PV6 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV7 =0  zero
            cmd.Add(20000);         // Zero PV7 =20000/1000 = 20.000  spand

            cmd.Add(0);             // Zero PV8 =0  zero
            cmd.Add(20000);         // Zero PV8 =20000/1000 = 20.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(4000);          // Zero PV1 =0  zero
            cmd.Add(20000);         // Zero PV1 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV2 =0  zero
            cmd.Add(20000);         // Zero PV2 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV3 =0  zero
            cmd.Add(20000);         // Zero PV3 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV4 =0  zero
            cmd.Add(20000);         // Zero PV4 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV5 =0  zero
            cmd.Add(20000);         // Zero PV5 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV6 =0  zero
            cmd.Add(20000);         // Zero PV6 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV7 =0  zero
            cmd.Add(20000);         // Zero PV7 =20000/1000 = 20.000  spand

            cmd.Add(4000);          // Zero PV8 =0  zero
            cmd.Add(20000);         // Zero PV8 =20000/1000 = 20.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(0);          // Zero PV1 =0  zero
            cmd.Add(1000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV2 =0  zero
            cmd.Add(1000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV3 =0  zero
            cmd.Add(1000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV4 =0  zero
            cmd.Add(1000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV5 =0  zero
            cmd.Add(1000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV6 =0  zero
            cmd.Add(1000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV7 =0  zero
            cmd.Add(1000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV8 =0  zero
            cmd.Add(1000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(-1000);          // Zero PV1 =0  zero
            cmd.Add(1000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV2 =0  zero
            cmd.Add(1000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV3 =0  zero
            cmd.Add(1000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV4 =0  zero
            cmd.Add(1000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV5 =0  zero
            cmd.Add(1000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV6 =0  zero
            cmd.Add(1000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV7 =0  zero
            cmd.Add(1000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(-1000);          // Zero PV8 =0  zero
            cmd.Add(1000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(0);          // Zero PV1 =0  zero
            cmd.Add(5000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV2 =0  zero
            cmd.Add(5000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV3 =0  zero
            cmd.Add(5000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV4 =0  zero
            cmd.Add(5000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV5 =0  zero
            cmd.Add(5000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV6 =0  zero
            cmd.Add(5000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV7 =0  zero
            cmd.Add(5000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV8 =0  zero
            cmd.Add(5000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(0);          // Zero PV1 =0  zero
            cmd.Add(5000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV2 =0  zero
            cmd.Add(5000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV3 =0  zero
            cmd.Add(5000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV4 =0  zero
            cmd.Add(5000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV5 =0  zero
            cmd.Add(5000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV6 =0  zero
            cmd.Add(5000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV7 =0  zero
            cmd.Add(5000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV8 =0  zero
            cmd.Add(5000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(0);          // Zero PV1 =0  zero
            cmd.Add(10000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV2 =0  zero
            cmd.Add(10000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV3 =0  zero
            cmd.Add(10000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV4 =0  zero
            cmd.Add(10000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV5 =0  zero
            cmd.Add(10000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV6 =0  zero
            cmd.Add(10000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV7 =0  zero
            cmd.Add(10000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(0);          // Zero PV8 =0  zero
            cmd.Add(10000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
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

            /*cmd.Add(-10000);          // Zero PV1 =0  zero
            cmd.Add(10000);         // Zero PV1 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV2 =0  zero
            cmd.Add(10000);         // Zero PV2 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV3 =0  zero
            cmd.Add(10000);         // Zero PV3 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV4 =0  zero
            cmd.Add(10000);         // Zero PV4 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV5 =0  zero
            cmd.Add(10000);         // Zero PV5 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV6 =0  zero
            cmd.Add(10000);         // Zero PV6 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV7 =0  zero
            cmd.Add(10000);         // Zero PV7 =1000/1000 = 1.000  spand

            cmd.Add(-10000);          // Zero PV8 =0  zero
            cmd.Add(10000);         // Zero PV8 =1000/1000 = 1.000  spand*/
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 10, cmd);
            return this.send_data_cmd();
        }

        public bool check_resp_msg(byte[] value, int MsgId, int SlaveID, int func)
        {
            //bool state = false;
            if (value == null)
                return (false);
            //byte[] TransactionID = new byte[2];
            int TransactionID = BitConverter.ToInt16(value, 0);
            int ProtocalId = BitConverter.ToInt16(value, 2);
            int Lengthfield = value[5];
            byte UnitID = value[6];
            byte FuncCode = value[7];
            byte[] Data = new byte[Lengthfield];
            Array.Copy(value, 6, Data, 0, Lengthfield - 2);
            if (TransactionID != MsgId)
                return (false);
            if (Lengthfield < 5)
                return (false);
            if (FuncCode != func)
                return (false);
            return true;
        }

    }
}
