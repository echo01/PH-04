using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Calibrate_PB_04
{


    class master_modbus
    {
        private static byte[] CRCTableHigh = new byte[] { 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40};

        private static byte[] CRCTableLow = new byte[] {0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2,
        0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
        0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8,
        0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F,
        0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6,
        0xD2, 0x12, 0x13, 0xD3, 0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1,
        0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
        0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB,
        0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA,
        0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5,
        0x27, 0xE7, 0xE6, 0x26, 0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0,
        0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
        0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE,
        0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79,
        0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C,
        0xB4, 0x74, 0x75, 0xB5, 0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73,
        0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
        0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D,
        0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98,
        0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F,
        0x8D, 0x4D, 0x4C, 0x8C, 0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86,
        0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80, 0x40};

        static bool sr_port_status;
        public static SerialPort _serialPort;

        public static byte[] rx_message;                                //  rx buffer for receive byte from port
        static int rx_nbyte;                                            //  total number of rx byte
        static List<int> rx_expect_nbyte = new List<int>();               //  list of number of rx_expect_read_byte
        static int nbyte;                                               //  number of byte receive from port
        public static bool Is_rx_buff_receive;                          //  true when receive some byte respond from port
        public static bool Is_rx_buff_ready;                            //  true when receive all byte respond and equal expect number of respond byte

        static bool req_wr_mb;                                          //  true for interrput msg cmd read queq for send
        static bool req_wr_mb_queq;                                     //  
        public static bool Is_wait_msg;                                 //  true after send cmd and wait respond
        public static int mb_err;

        //static byte[] mb_msg_table1 = multi_preset_cmd(1, 0, 25);
        public static List<byte[]> mb_msg_table = new List<byte[]>();     //  msg cmd read queq 
        public static List<byte[]> msg_data = new List<byte[]>();       //  msg respond queq
        public static byte[] mb_msg_data;                               //  msg buffer for receive respond
        public static int mb_t_index;                                   //  number of index for msg cmd read queq
        public static int indx_queue;                                   //  index for msg cmd read queq

        public static byte[] mb_msg_send_cmd;                              //  msg cmd send write data
        public static int expect_n_resp_byte;                            //  expect number of respond byte
        public static byte[] msg_wr_respond_data;

        public static void mb_ini(SerialPort serialPort)
        {
            _serialPort = serialPort;
            sr_port_status = false;
            req_wr_mb = false;
            Is_rx_buff_receive = false;
            Is_wait_msg = false;
            rx_nbyte = 0;
            indx_queue = 0;
            req_wr_mb_queq = false;
        }

        public static void reg_rd_qeue()
        {
            Master_mgs_read(1, 0, 25);
            Master_mgs_read(1, 36864, 17);
        }

        public static void Master_Port_start()
        {
            sr_port_status = true;
        }

        public static void Master_Port_stop()
        {
            sr_port_status = false;
        }

        public static void Master_mgs_read(int slave_id, int start_reg, int length)
        {
            mb_msg_table.Add(multi_read_cmd(slave_id, start_reg, length));
            rx_expect_nbyte.Add(length * 2 + 5);
            msg_data.Add(new byte[2]);
            mb_t_index++;

        }

        public static void master_next_queq_read()
        {
            if (indx_queue < mb_t_index - 1) indx_queue++;
            else indx_queue = 0;
        }

        public static void master_msg_single_write(int slave_id, int start_reg, int reg_data)
        {
            mb_msg_send_cmd = Write(slave_id, start_reg, reg_data);
            expect_n_resp_byte = mb_msg_send_cmd.Length;
            //req_wr_mb = true;
            req_wr_mb_queq = true;
        }

        public static void master_msg_mul_write(int slave_id, int start_reg, List<int> reg_data)
        {
            mb_msg_send_cmd = WriteMultiple(slave_id, start_reg, reg_data);
            expect_n_resp_byte = 8;
            //req_wr_mb = true;
            req_wr_mb_queq = true;
        }


        public static void Master_Read()
        {
            while (sr_port_status)
            {
                if (Is_wait_msg)
                    try
                    {
                        nbyte = 0;
                        nbyte = _serialPort.BytesToRead;
                        if (nbyte > 0)
                        {
                            rx_message = new byte[nbyte];
                            _serialPort.Read(rx_message, 0, nbyte);
                        }
                        if (rx_nbyte == 0 && nbyte > 0)
                        {
                            mb_msg_data = new byte[rx_message.Length];
                            rx_message.CopyTo(mb_msg_data, 0);
                            rx_nbyte += nbyte;
                            Is_rx_buff_receive = true;
                        }
                        else
                            if (req_wr_mb)
                        {
                            if (nbyte > 0 && rx_nbyte < expect_n_resp_byte)
                            {
                                byte[] buff_msg = new byte[nbyte + rx_nbyte];
                                rx_message.CopyTo(buff_msg, 0);
                                mb_msg_data.CopyTo(buff_msg, rx_message.Length);
                                mb_msg_data = buff_msg;
                                rx_nbyte += nbyte;
                                Is_rx_buff_receive = true;
                            }
                        }
                        else
                        {
                            if (nbyte > 0 && rx_nbyte < rx_expect_nbyte[indx_queue])
                            {
                                byte[] buff_msg = new byte[nbyte + rx_nbyte];
                                rx_message.CopyTo(buff_msg, 0);
                                mb_msg_data.CopyTo(buff_msg, rx_message.Length);
                                mb_msg_data = buff_msg;
                                rx_nbyte += nbyte;
                                Is_rx_buff_receive = true;
                            }
                        }
                    }
                    catch (TimeoutException) { }
                Thread.Sleep(100);
            }
        }

        public static void Master_Mb()
        {
            while (sr_port_status)
            {
                if (Is_wait_msg)
                {
                    mb_err = 0;
                    if (!Is_rx_buff_receive)
                    { mb_err = 1; Is_rx_buff_ready = false; }             // Time out error
                    else
                    {
                        if (req_wr_mb)
                        {
                            if (rx_nbyte < expect_n_resp_byte)
                            {
                                mb_err = 1; Is_rx_buff_ready = false;
                            }             // Time out error   
                            else
                            {
                                Is_rx_buff_ready = true;
                                rx_nbyte = 0;
                                msg_wr_respond_data = mb_msg_data;
                                req_wr_mb = false;
                            }
                        }
                        else
                        {
                            if (rx_nbyte < rx_expect_nbyte[indx_queue])
                            {
                                mb_err = 1; Is_rx_buff_ready = false;
                            }             // Time out error   
                            else
                            {
                                Is_rx_buff_ready = true;
                                rx_nbyte = 0;
                                msg_data[indx_queue] = mb_msg_data;
                            }
                            master_next_queq_read();
                        }

                    }
                    Is_wait_msg = false;
                }
                else
                {
                    if (req_wr_mb_queq)
                    {
                        req_wr_mb_queq = false;
                        req_wr_mb = true;
                    }

                    if (req_wr_mb)
                    {
                        _serialPort.Write(mb_msg_send_cmd, 0, mb_msg_send_cmd.Length);
                        rx_nbyte = 0;
                        Is_wait_msg = true; Is_rx_buff_ready = false;
                    }
                    else
                    {
                        //master_mb.master_modbus.Master_mgs_read(1, 0, 25);
                        _serialPort.Write(mb_msg_table[indx_queue], 0, mb_msg_table[indx_queue].Length);
                        rx_nbyte = 0;
                        Is_wait_msg = true; Is_rx_buff_ready = false;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public static byte[] multi_read_cmd(int slave_id, int start_reg, int length)
        {
            List<int> msg = new List<int>();
            msg.Add(slave_id);
            msg.Add(3);
            msg.Add(start_reg / 256);
            msg.Add(start_reg % 256);
            msg.Add(length / 256);
            msg.Add(length % 256);
            int[] crc = CRC(msg, msg.Count);
            msg.Add(crc[0]);
            msg.Add(crc[1]);

            byte[] byteMsg = new byte[msg.Count];
            int i = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[i] = (byte)m;
                i++;
            }
            return byteMsg;
        }

        public static bool MsgCheck(byte[] value, int function)
        {
            bool state = false;
            if (value == null)
                return false;
            if (value[0] == 1)
            {
                if (value[1] == function)
                {
                    byte[] crc = MB_CRC16(value, value.Length - 2);
                    if (value[value.Length - 2] == crc[0] && value[value.Length - 1] == crc[1])
                    {
                        state = true;
                    }
                }
            }
            return state;
        }

        private static byte[] MB_CRC16(byte[] data, int n)
        {
            byte crcHi = 0xff;
            byte crcLo = 0xff;
            int j = 0;

            for (int i = 0; i < n; i++)
            {
                j = crcHi ^ data[i];
                crcHi = (byte)(crcLo ^ CRCTableHigh[j]);
                //crcHi = crcLo ^ CRCTableHigh[j];
                crcLo = CRCTableLow[j];
            }

            byte[] crc = new byte[2];
            crc[0] = crcHi;
            crc[1] = crcLo;
            return crc;
        }

        public static byte[] Read(int address, int lenght)
        {
            List<int> msg = new List<int>();

            msg.Add(1);
            msg.Add(3);
            msg.Add(address / 256);
            msg.Add(address % 256);
            msg.Add(lenght / 256);
            msg.Add(lenght % 256);

            int[] crc = CRC(msg, msg.Count);

            msg.Add(crc[0]);
            msg.Add(crc[1]);

            byte[] byteMsg = new byte[msg.Count];

            int i = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[i] = (byte)m;
                i++;
            }

            return byteMsg;
        }

        public static byte[] Write(int slave_id, int address, int value)
        {
            List<int> msg = new List<int>();
            msg.Add(slave_id);
            msg.Add(6);
            msg.Add(address / 0x100);
            msg.Add(address & 0xFF);

            int[] value1 = bitshift(value);

            msg.Add(value1[0]);
            msg.Add(value1[1]);

            int[] crc = CRC(msg, msg.Count);

            msg.Add(crc[0]);
            msg.Add(crc[1]);

            byte[] byteMsg = new byte[msg.Count];

            int i = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[i] = (byte)m;
                i++;
            }

            return byteMsg;
        }

        public static byte[] WriteMultiple(int slave_id, int address, List<int> value)
        {
            List<int> msg = new List<int>();
            msg.Add(slave_id);
            msg.Add(16);
            msg.Add(address / 0x100);
            msg.Add(address & 0xFF);
            msg.Add(value.Count / 256);
            msg.Add(value.Count % 256);
            msg.Add(value.Count * 2);

            for (int i = 0; i < value.Count; i++)
            {
                int[] value1 = bitshift(value[i]);
                msg.Add(value1[0]);
                msg.Add(value1[1]);
            }

            int[] crc = CRC(msg, msg.Count);

            msg.Add(crc[0]);
            msg.Add(crc[1]);

            byte[] byteMsg = new byte[msg.Count];

            int j = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[j] = (byte)m;
                j++;
            }

            return byteMsg;
        }



        public static bool Check(List<int> value, int function)
        {
            bool state = false;
            if (value[0] == 1)
            {
                if (value[1] == function)
                {
                    int[] crc = CRC(value, value.Count - 2);
                    if (value[value.Count - 2] == crc[0] && value[value.Count - 1] == crc[1])
                    {
                        state = true;
                    }
                }
            }
            return state;
        }

        private static int[] bitshift(int value1)
        {
            if (value1 < 0)
            {
                value1 += 65536;
            }

            int[] value = new int[2];
            value[0] = value1 / 256;
            value[1] = value1 % 256;
            return value;
        }

        public static int convert(int data)
        {
            if (data > 32498)
            {
                data = data - 65536;
            }
            return data;
        }

        private static int[] CRC(List<int> data, int n)
        {
            int crcHi = 0xff;
            int crcLo = 0xff;
            int j = 0;

            for (int i = 0; i < n; i++)
            {
                j = crcHi ^ data[i];
                crcHi = crcLo ^ CRCTableHigh[j];
                crcLo = CRCTableLow[j];
            }

            int[] crc = new int[2];
            crc[0] = crcHi;
            crc[1] = crcLo;
            return crc;
        }
    }
}
