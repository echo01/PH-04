using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrate_PB_04
{
    internal class CoreModbusTCP
    {
        public int TransactionID;
        public int ProtocalId;
        public int Lengthfield;
        public byte UnitID;
        public byte[] cmd_msg = new byte[96];
        public byte[] rx_msg = new byte[96];

        public CoreModbusTCP()
        {

        }

        public byte[] read_holding(int slaveid, int address, int lenght)
        {
            List<int> msg = new List<int>();
            msg.Add(TransactionID/256);
            msg.Add(TransactionID%256);
            msg.Add(ProtocalId/256);
            msg.Add(ProtocalId%256);
            Lengthfield = 6;
            msg.Add(Lengthfield/256);
            msg.Add(Lengthfield%256);
            //msg.Add(UnitID);
            msg.Add(slaveid);
            msg.Add(3);
            msg.Add((address) / 0x100);
            msg.Add(address % 256);
            msg.Add(lenght / 256);
            msg.Add(lenght % 256);

            //int[] crc = CRC(msg, msg.Count);

            //msg.Add(crc[0]);
            //msg.Add(crc[1]);

            byte[] byteMsg = new byte[msg.Count];

            int i = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[i] = (byte)m;
                i++;
            }

            return byteMsg;
        }

        public byte[] WriteMultiple(int slave_id, int address, List<int> value)
        {
            List<int> msg = new List<int>();
            msg.Add(TransactionID / 256);
            msg.Add(TransactionID % 256);
            msg.Add(ProtocalId / 256);
            msg.Add(ProtocalId % 256);
            Lengthfield = value.Count;
            msg.Add(Lengthfield / 256);
            msg.Add(Lengthfield % 256);
            //msg.Add(UnitID);
            msg.Add(slave_id);
            msg.Add(16);
            msg.Add((address) / 0x100);
            msg.Add(address & 0xFF);
            msg.Add(value.Count / 256);
            msg.Add(value.Count % 256);
            msg.Add((value.Count) * 2);


            for (int i = 0; i < value.Count; i++)
            {
                int[] value1 = bitshift(value[i]);
                msg.Add(value1[0]);
                msg.Add(value1[1]);
            }


            byte[] byteMsg = new byte[msg.Count];

            int j = 0;
            foreach (int m in msg) // Loop through all strings
            {
                byteMsg[j] = (byte)m;
                j++;
            }

            return byteMsg;
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

        public bool check_resp_msg(byte[] value, int MsgId, int SlaveID, int func)
        {
            //bool state = false;
            if (value == null)
                return (false);
            //byte[] TransactionID = new byte[2];
            TransactionID = BitConverter.ToInt16(value, 0);
            ProtocalId = BitConverter.ToInt16(value, 2);
            Lengthfield = value[5];
            UnitID = value[6];
            byte FuncCode = value[7];
            byte[] Data = new byte[Lengthfield];
            try
            {
                Array.Copy(value, 6, Data, 0, Lengthfield - 2);
            }
            catch
            {
                return (false);
            }
            
            //if (TransactionID != MsgId)
            //    return (false);
            if (Lengthfield < 5)
                return (false);
            if (FuncCode != func)
                return (false);
            return true;
        }

    }
}
