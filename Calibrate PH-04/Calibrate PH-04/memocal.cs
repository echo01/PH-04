using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Calibrate_PB_04;

namespace Calibrate_PB_04
{
    public partial class memocal : Component
    {
        public SerialPort sPort1 = new SerialPort();
        public bool Ready;
        public bool IsPortOpen;
        public bool IsResponded;
        public byte[] rx_message = new byte[64];
        public int nrx_byte;

        byte[] Memocal_Mode_Remote = { 1, 6, 0, 100, 0, 11, 137, 210 };
        //Write Enter  01 06 00 66 00 01 a8 15  
        byte[] Memocal_Enter_write = { 1, 6, 0, 102, 0, 1, 168, 21 };
        //Read Enter  01 06 00 66 00 00 69 d5 
        byte[] Memocal_Enter_read = { 1, 6, 0, 102, 0, 0, 105, 213 };
        //01 06 00 64 00 03 88 14   stand by 
        byte[] Memocal_stand_by = { 1, 6, 0, 100, 0, 3, 136, 20 };
        //01 06 01 2c 00 03 09 fe  Write Add300 >> meas type >>mV  
        byte[] Memocal_type_mV = { 1, 6, 1, 44, 0, 3, 9, 254 };
        byte[] Memocal_type_mA = { 1, 6, 1, 44, 0, 2, 200, 62 };
        byte[] Memocal_deci_mA = { 1, 6, 1, 55, 0, 3, 121, 249 };
        byte[] Memocal_500ohms_mA = { 1, 6, 1, 144, 0, 2, 9, 218 };
        //01 06 01 31 00 00 d9 f9  Write Add305 >> rangeable non rangeable 
        byte[] Memocal_non_rangeable = { 1, 6, 1, 49, 0, 0, 217, 249 };
        //01 06 01 30 00 01 49 F9   Write Add304 >> selecltion Auto 
        byte[] Memocal_selecltion_Auto = { 1, 6, 1, 48, 0, 1, 73, 249 };
        //01 06 01 91 27 10 c3 e7   Write Add401 >>  Value Output 10 VDC
        byte[] Memocal_Value_Output_10000mV = { 1, 6, 1, 145, 39, 16, 195, 231 };
        byte[] Memocal_Value_Output_5000mV = { 1, 6, 1, 145, 19, 136, 212, 141 };
        byte[] Memocal_Value_Output_2500mV = { 1, 6, 1, 145, 9, 196, 222, 24 };
        byte[] Memocal_Value_Output_1000mV = { 1, 6, 1, 145, 3, 232, 217, 101 };
        byte[] Memocal_Value_Output_500mV = { 1, 6, 1, 145, 1, 244, 217, 204 };
        byte[] Memocal_Value_Output_0mV = { 1, 6, 1, 145, 0, 0, 217, 219 };

        byte[] Memocal_Value_Output_20mA = { 1, 6, 1, 145, 78, 32, 237, 163 };
        byte[] Memocal_Value_Output_12mA = { 1, 6, 1, 145, 46, 224, 197, 243 };
        byte[] Memocal_Value_Output_0mA = { 1, 6, 1, 145, 0, 0, 217, 219 };
        byte[] Memocal_Value_Output_4mA = { 1, 6, 1, 145, 15, 160, 220, 83 };
        //01 06 00 64 00 02 49 d4   generate 
        byte[] Memocal_generate = { 1, 6, 0, 100, 0, 2, 73, 212 };

        byte[] Memocal_read = {1,3,0,100,0,3,68,20 };

        public memocal()
        {
            InitializeComponent();
            sPort1 = new SerialPort();
            IsPortOpen = false;
            Ready=false;
        }

        public memocal(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Set_Comport(SerialPort serialport)
        {
            sPort1 = serialport;
  
            try
            {
                sPort1.Open();
                IsPortOpen = true;
                Start_Remote();
            }
            catch(Exception ex)
            {
                IsPortOpen = false;
                MessageBox.Show(ex.Message);   
            }
        }

        public bool close_comport()
        {
            if (sPort1.IsOpen)
                try{
                sPort1.Close();
                this.IsPortOpen=false;
                return true;
                }
                catch
                {
                return false;   
                }
            return false;
        }

        public int port_read(byte[] buffer, int offset, int count)
        {
            try
            {
                return sPort1.Read(buffer, offset, count);
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

        public void Start_Remote()
        {
            this.port_write(Memocal_read, 0, Memocal_read.Length);
            Thread.Sleep(50);
            this.Ready = false;
            nrx_byte=this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 3, 1,nrx_byte))
                return;
            //  send command set mode = Remote
            this.port_write(Memocal_Mode_Remote, 0, Memocal_Mode_Remote.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1,nrx_byte))
                return;
            // send command Enter
            this.port_write(Memocal_Enter_write, 0, Memocal_Enter_write.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1,nrx_byte))
                return;
            this.Ready = true;
        }

        public bool Memocal_Gen_mA(int mAvalue)
        {
            if (!this.Ready)
                return false;
            //  send command set mode = Set Type mV
            this.port_write(Memocal_type_mV, 0, Memocal_type_mV.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Set Type mA
            this.port_write(Memocal_type_mA, 0, Memocal_type_mA.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_non_rangeable
            this.port_write(Memocal_non_rangeable, 0, Memocal_non_rangeable.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_deci_mA
            this.port_write(Memocal_deci_mA, 0, Memocal_deci_mA.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_Value_Output_0mA
            Memocal_Value_Output_0mA = CoreModbus.Write(1, 401, mAvalue);
            this.port_write(Memocal_Value_Output_0mA, 0, Memocal_Value_Output_0mA.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_500ohms_mA
            this.port_write(Memocal_500ohms_mA, 0, Memocal_500ohms_mA.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_generate
            this.port_write(Memocal_generate, 0, Memocal_generate.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_generate
            this.port_write(Memocal_Enter_write, 0, Memocal_Enter_write.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            return true;

        }


        public bool Memocal_Gen_mV(int mVvalue)
        {
            if (!this.Ready)
                return false;
            //  send command set mode = Set Type mV
            this.port_write(Memocal_type_mV, 0, Memocal_type_mV.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;



            //  send command set mode = Memocal_non_rangeable
            this.port_write(Memocal_non_rangeable, 0, Memocal_non_rangeable.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_deci_mA
            this.port_write(Memocal_selecltion_Auto, 0, Memocal_selecltion_Auto.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_Value_Output_0mA
            Memocal_Value_Output_0mA = CoreModbus.Write(1, 401, mVvalue);
            this.port_write(Memocal_Value_Output_0mA, 0, Memocal_Value_Output_0mA.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;


            //  send command set mode = Memocal_generate
            this.port_write(Memocal_generate, 0, Memocal_generate.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            //  send command set mode = Memocal_generate
            this.port_write(Memocal_Enter_write, 0, Memocal_Enter_write.Length);
            Thread.Sleep(50);
            nrx_byte = this.port_read(rx_message, 0, 11);
            if (!CoreModbus.MsgCheck(rx_message, 6, 1, nrx_byte))
                return false;

            return true;

        }


    }
}
