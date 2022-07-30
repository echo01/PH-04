using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Calibrate_PB_04
{
    public partial class PH07 : Component
    {
        public SerialPort sPort1 = new SerialPort();
        public bool Ready;
        public bool IsPortOpen;
        public bool IsResponded;
        public byte[] rx_message = new byte[32];
        public int nrx_byte;
        public byte SlaveId;
        public Int16 DO1;
        public Int16 DO2;
        public Int16 DO3;
        public Int16 DO4;
        public Int16 DO5;
        public Int16 DO6;
        public Int16 DO;
        public Int16 Baud;
        public Int16 Parity;
        public Int16 Stopbit;
        public Int16 Delayreply;
        public Int16 SofwareVerstion;
        byte[] cmd_msg = new byte[32];

        public PH07()
        {
            InitializeComponent();
            IsPortOpen = false;
            IsResponded = false;
            Ready = false;
        }

        public PH07(IContainer container)
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

        public int port_read(byte[] buffer, int offset, int count)
        {
            int n_rx;
            try
            {
                int counttry = 0;
                for(n_rx = 0; n_rx < count;)
                {
                    int n = sPort1.Read(buffer, n_rx, count- n_rx);
                    n_rx += n;
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
        
        public void Clear_all_DO()
        {
            DO1 = 0;
            DO2 = 0;
            DO3 = 0;
            DO4 = 0;
            DO5 = 0;
            DO6 = 0;
        }

        public void Update_Output()
        {
            if (!this.Ready) return;
            List<int> msg = new List<int>();
            msg.Add(this.DO1);
            msg.Add(this.DO2);
            msg.Add(this.DO3);
            msg.Add(this.DO4);
            msg.Add(this.DO5);
            msg.Add(this.DO6);
            this.cmd_msg = CoreModbus.WriteMultiple(this.SlaveId, 0, msg);
            this.port_write(this.cmd_msg, 0, this.cmd_msg.Length);
            Thread.Sleep(100);
            nrx_byte = this.port_read(rx_message, 0, 8);
            if (!CoreModbus.MsgCheck(rx_message, 3, this.SlaveId, nrx_byte))
                return;

        }
    }
}
