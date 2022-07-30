using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrate_PB_04;


namespace Calibrate_PH_04
{
    public partial class CalibrateProcess : Component
    {
        public memocal Memocal2000 = new();
        public PH04 ph04 = new();
        public PH07 ph07id2 = new();
        public PH07 ph07id3 = new();
        public PH07 ph07id4 = new();
        public bool IsConnected;
        public bool IsReadyOnPH07_Module1;
        public bool IsReadyOnPH07_Module2;
        public bool IsReadyOnPH07_Module3;

        public int Select_PH04_Channel;
        public string msg;
        public bool Is_update_msg;
        public int Calibrate_Step;
        public bool Calibrate_thread_run;
        public int Calibrate_progress;
        private Thread Calibrate_thread;

        public CalibrateProcess()
        {
            InitializeComponent();
        }

        public CalibrateProcess(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void setPH07tool(PH07 module1,PH07 module2, PH07 module3)
        {
            ph07id2=module1;
            ph07id3=module2;    
            ph07id4=module3;
        }

        public void setPH04_Device(PH04 dev)
        {
            ph04 = dev;
        }
        public void setMemocal_device(memocal dev)
        {
            Memocal2000 = dev;
        }

        public bool IsCalibrate_tool_ready()
        {
            if (ph04.Ready && ph07id2.Ready && ph07id3.Ready && ph07id4.Ready)
                return true;
            return false;
        }


        public void SetInput_test(int ch, int input_type)
        {
            ph07id2.Clear_all_DO();
            ph07id3.Clear_all_DO();
            ph07id4.Clear_all_DO();

            ph07id4.DO3 = (Int16)input_type;
            switch (ch)
            {
                case 0:     //  all off
                    break;
                case 1:
                    if (input_type == 0)
                    {
                        ph07id2.DO1 = 1;
                        ph07id2.DO2 = 0;
                    }
                    else
                    {
                        ph07id2.DO1 = 0;
                        ph07id2.DO2 = 1;
                    }
                    break;
                case 2:
                    if (input_type == 0)
                    {
                        ph07id2.DO3 = 1;
                        ph07id3.DO1 = 0;
                    }
                    else
                    {
                        ph07id2.DO3 = 0;
                        ph07id3.DO1 = 1;
                    }
                    break;
                case 3:
                    if (input_type == 0)
                    {
                        ph07id3.DO2 = 1;
                        ph07id3.DO3 = 0;
                    }
                    else
                    {
                        ph07id3.DO2 = 0;
                        ph07id3.DO3 = 1;
                    }
                    break;
                case 4:
                    if (input_type == 0)
                    {
                        ph07id4.DO1 = 1;
                        ph07id4.DO3 = 0;
                    }
                    else
                    {
                        ph07id4.DO1 = 0;
                        ph07id4.DO2 = 1;
                    }
                    break;
                case 5:
                    if (input_type == 0)
                    {
                        ph07id4.DO4 = 1;
                        ph07id4.DO5 = 0;
                    }
                    else
                    {
                        ph07id4.DO4 = 0;
                        ph07id4.DO5 = 1;
                    }
                    break;
                case 6:
                    if (input_type == 0)
                    {
                        ph07id4.DO6 = 1;
                        ph07id3.DO4 = 0;
                    }
                    else
                    {
                        ph07id4.DO6 = 0;
                        ph07id3.DO4 = 1;
                    }
                    break;
                case 7:
                    if (input_type == 0)
                    {
                        ph07id3.DO5 = 1;
                        ph07id3.DO6 = 0;
                    }
                    else
                    {
                        ph07id3.DO5 = 0;
                        ph07id3.DO6 = 1;
                    }
                    break;
                case 8:
                    if (input_type == 0)
                    {
                        ph07id2.DO4 = 1;
                        ph07id2.DO5 = 0;
                    }
                    else
                    {
                        ph07id2.DO4 = 0;
                        ph07id2.DO5 = 1;
                    }
                    break;
                case 9: //  all on
                    if (input_type == 0)
                    {
                        ph07id2.DO1 = 1;        //  CH1
                        ph07id2.DO3 = 1;        //  CH2
                        ph07id3.DO2 = 1;        //  CH3
                        ph07id4.DO1 = 1;        //  CH4
                        ph07id4.DO4 = 1;        //  CH5
                        ph07id4.DO6 = 1;        //  CH6
                        ph07id3.DO5 = 1;        //  CH7
                        ph07id2.DO4 = 1;        //  CH8

                    }
                    else
                    {
                        ph07id2.DO2 = 1;        //  CH1    
                        ph07id3.DO1 = 1;        //  CH2
                        ph07id3.DO3 = 1;        //  CH3
                        ph07id4.DO2 = 1;        //  CH4
                        ph07id4.DO5 = 1;        //  CH5
                        ph07id3.DO4 = 1;        //  CH6
                        ph07id3.DO6 = 1;        //  CH7
                        ph07id2.DO5 = 1;        //  CH8

                    }
                    break;
            }
            SetSignal_type(input_type);
            Thread.Sleep(300);
            ph07id2.Update_Output();
            Thread.Sleep(300);
            ph07id3.Update_Output();
            Thread.Sleep(300);
            ph07id4.Update_Output();
            Thread.Sleep(300);
        }

        public void SetSignal_type(int input_type)
        {
            switch (input_type)
            {
                case 0://   mA
                    ph07id4.DO3 = 0;
                    ph07id4.Update_Output();
                    break;
                case 1://   mV
                    ph07id4.DO3 = 1;
                    ph07id4.Update_Output();
                    break;
            }
        }

        public void start_calibrate_thread(bool mA,bool mV)
        {
            if (mA && mV)
                Calibrate_thread = new Thread(RunCalibrate_all_manual_one_ch);
            else if (mA)
                Calibrate_thread = new Thread(RunCalibrate_mA_manual_one_ch);
            else if (mV)
                Calibrate_thread = new Thread(RunCalibrate_mV_manual_one_ch);
            else
                {
                MessageBox.Show("Please select one calibrate method");
                return;
                }
                
            Calibrate_thread.IsBackground = true;
            Calibrate_Step = 0;
            Calibrate_thread.Start();
            Calibrate_thread_run = true;
        }

        public string Upate_msg()
        {
            if (Is_update_msg)
            {
                Is_update_msg = false;  
                return msg;
            }
                
            return "";
        }

        public void RunCalibrate_all_manual_one_ch()
        {
            int delay_after_memocal = 1250;
            int delay_after_cmd = 250;
            int waitingloop=0;
            while (Calibrate_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1)*100)/14;
                if (waitingloop>0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 0);                //  force tools switch relay for calibrate each channel
                            Thread.Sleep(delay_after_cmd);
                            ph04.Calibrate_enable();                        //  set pb04 calibrate 
                            Memocal2000.Memocal_Gen_mA(0);                  //  Gen 0 mA
                                                                            //Thread.Sleep(delay_after_cmd);
                            Thread.Sleep(delay_after_cmd);
                            msg = "Set PH04 to Calibrate mode.. ";
                            Calibrate_Step = 1;
                            Is_update_msg = true;
                            break;
                        case 1:
                            if (ph04.Calibrate_Mux_channel(Select_PH04_Channel))
                            {
                                //textconsole1.Text += "Start Calibrate CH = " + PB04_input_CH.ToString();
                                msg = "Start Calibrate CH = " + Select_PH04_Channel.ToString();
                                Calibrate_Step = 2;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't start Calibrate CH = " + PB04_input_CH.ToString();
                                msg = "Can't start Calibrate CH = " + Select_PH04_Channel.ToString();
                                Calibrate_Step = 0;
                                return;
                            }
                            Is_update_msg = true;
                            break;
                        case 2: //Offset Zero 0-20mA
                            if (ph04.Calibrate_Offset_0mA())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Offset 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 3;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "can't calibrate Offset 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(20000);
                            Is_update_msg = true;
                            break;
                        case 3: //Gain 0-20mA
                            if (ph04.Calibrate_Gain_0to20mA())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Gain 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 4;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Gain 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(4000);
                            Is_update_msg = true;
                            break;
                        case 4: //Gain 4-20mA
                            if (ph04.Calibrate_Offset_4mA())
                            {
                                //textconsole1.Text += "Calibrate Offset 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Offset 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";

                                Calibrate_Step = 5;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(20000);
                            Is_update_msg = true;
                            break;
                        case 5: //Gain 4-20mA
                            if (ph04.Calibrate_Gain_4to20mA())
                            {
                                //textconsole1.Text += "Calibrate Gain 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate Gain 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 6;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Can't calibrate Gain 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Is_update_msg = true;
                            break;
                        case 6: //Setup Calibrate mV
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Calibrate_Step = 7;
                            msg = "Setup Calibrate mV " + Select_PH04_Channel.ToString() + " DONE..";
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 7: //offset 0-1000mV
                            if (ph04.Calibrate_Offset_0to1000mV())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate Offset 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 8;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Can't calibrate Offset 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mV(1000);
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 8: //Gain 0-1000mV
                            if (ph04.Calibrate_Gain_0to1000mV())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate Gain 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 9;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Gain 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 9: //Offse 0 to 5000 mV =============================
                            if (ph04.Calibrate_Offset_0to5000mV())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate offset 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 10;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(5000);
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 10:
                            if (ph04.Calibrate_Gain_0to5000mV())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate Gain 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 11;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Cant't calibrate Gain 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 11:    //Offse 0 to 10V =========================
                            if (ph04.Calibrate_Offset_0to10V())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-10V CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg="Calibrate offset 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 12;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-10V CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(10000);
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 12:    //Gain 0 to 10V ========================
                            if (ph04.Calibrate_Gain_0to10V())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-10V CH = " + comboBox3.Text + " DONE..";
                                msg="Calibrate Gain 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 13;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-10V CH = " + comboBox3.Text + " DONE..";
                                msg = "Can't calibrate Gain 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            waitingloop = 1;
                            break;
                        case 13:
                            ph04.Calibrate_SaveEEPROM();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            //textconsole1.Text += "Save Calibrate CH = " + PB04_input_CH.ToString() + " DONE..";
                            msg="Save Calibrate CH = " + Select_PH04_Channel.ToString() + " DONE..";
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            break;
                        default:
                            msg = "Stop process incorrect calibrate step on value " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            return;

                    }
                }                    
                Thread.Sleep(delay_after_memocal);
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";
                
            } 
        }

        public void RunCalibrate_mA_manual_one_ch()
        {
            int delay_after_memocal = 1200;
            int delay_after_cmd = 500;
            int waitingloop = 0;
            while (Calibrate_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 14;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 3;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 0);                //  force tools switch relay for calibrate each channel
                            Thread.Sleep(delay_after_cmd);
                            ph04.Calibrate_enable();                        //  set pb04 calibrate 
                            Memocal2000.Memocal_Gen_mA(0);                  //  Gen 0 mA
                                                                            //Thread.Sleep(delay_after_cmd);
                            Thread.Sleep(delay_after_cmd);
                            msg = "Set PH04 to Calibrate mode.. ";
                            Calibrate_Step = 1;
                            Is_update_msg = true;
                            break;
                        case 1:
                            if (ph04.Calibrate_Mux_channel(Select_PH04_Channel))
                            {
                                //textconsole1.Text += "Start Calibrate CH = " + PB04_input_CH.ToString();
                                msg = "Start Calibrate CH = " + Select_PH04_Channel.ToString();
                                Calibrate_Step = 2;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't start Calibrate CH = " + PB04_input_CH.ToString();
                                msg = "Can't start Calibrate CH = " + Select_PH04_Channel.ToString();
                                Calibrate_Step = 0;
                                return;
                            }
                            Is_update_msg = true;
                            break;
                        case 2: //Offset Zero 0-20mA
                            if (ph04.Calibrate_Offset_0mA())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Offset 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 3;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "can't calibrate Offset 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(20000);
                            Is_update_msg = true;
                            break;
                        case 3: //Gain 0-20mA
                            if (ph04.Calibrate_Gain_0to20mA())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Gain 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 4;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Gain 0-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(4000);
                            Is_update_msg = true;
                            break;
                        case 4: //Gain 4-20mA
                            if (ph04.Calibrate_Offset_4mA())
                            {
                                //textconsole1.Text += "Calibrate Offset 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Offset 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";

                                Calibrate_Step = 5;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mA(20000);
                            Is_update_msg = true;
                            break;
                        case 5: //Gain 4-20mA
                            if (ph04.Calibrate_Gain_4to20mA())
                            {
                                //textconsole1.Text += "Calibrate Gain 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Gain 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 13;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 4-20mA CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Gain 4-20mA CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Is_update_msg = true;
                            waitingloop = 1;
                            break;
                        case 13:
                            ph04.Calibrate_SaveEEPROM();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            //textconsole1.Text += "Save Calibrate CH = " + PB04_input_CH.ToString() + " DONE..";
                            msg = "Save Calibrate CH = " + Select_PH04_Channel.ToString() + " DONE..";
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            break;
                        default:
                            msg = "Stop process incorrect calibrate step on value " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            return;

                    }
                }
                Thread.Sleep(delay_after_memocal);
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";

            }
        }

        public void RunCalibrate_mV_manual_one_ch()
        {
            int delay_after_memocal = 1200;
            int delay_after_cmd = 500;
            int waitingloop = 0;
            while (Calibrate_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 14;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 1;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 1);                //  force tools switch relay for calibrate each channel
                            Thread.Sleep(delay_after_cmd);
                            ph04.Calibrate_enable();                        //  set pb04 calibrate 
                            //Memocal2000.Memocal_Gen_mV(0);                  //  Gen 0 mA
                                                                            //Thread.Sleep(delay_after_cmd);
                            //Thread.Sleep(delay_after_cmd);
                            msg = "Set PH04 to Calibrate mode.. ";
                            Calibrate_Step = 6;
                            Is_update_msg = true;
                            break;
                        case 6: //Setup Calibrate mV
                            SetInput_test(Select_PH04_Channel, 1);                //  force tools switch relay for calibrate each channel
                            Thread.Sleep(delay_after_cmd);
                            ph04.Calibrate_enable();                        //  set pb04 calibrate 
                            Memocal2000.Memocal_Gen_mV(0);

                            Calibrate_Step = 7;
                            msg = "Setup Calibrate mV " + Select_PH04_Channel.ToString() + " DONE..";
                            Is_update_msg = true;
                            break;
                        case 7: //offset 0-1000mV
                            if (ph04.Calibrate_Offset_0to1000mV())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Offset 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 8;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Offset 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            Memocal2000.Memocal_Gen_mV(1000);
                            Is_update_msg = true;
                            break;
                        case 8: //Gain 0-1000mV
                            if (ph04.Calibrate_Gain_0to1000mV())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Gain 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 9;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-1000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate Gain 0-1000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Is_update_msg = true;
                            break;
                        case 9: //Offse 0 to 5000 mV =============================
                            if (ph04.Calibrate_Offset_0to5000mV())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate offset 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 10;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(5000);
                            Is_update_msg = true;
                            break;
                        case 10:
                            if (ph04.Calibrate_Gain_0to5000mV())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate Gain 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 11;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-5000mV CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Cant't calibrate Gain 0-5000mV CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Is_update_msg = true;
                            break;
                        case 11:    //Offse 0 to 10V =========================
                            if (ph04.Calibrate_Offset_0to10V())
                            {
                                //textconsole1.Text += "Calibrate Offset 0-10V CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Calibrate offset 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 12;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Offset 0-10V CH = " + PB04_input_CH.ToString() + " DONE..";
                                msg = "Can't calibrate offset 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            //Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(10000);
                            Is_update_msg = true;
                            break;
                        case 12:    //Gain 0 to 10V ========================
                            if (ph04.Calibrate_Gain_0to10V())
                            {
                                //textconsole1.Text += "Calibrate Gain 0-10V CH = " + comboBox3.Text + " DONE..";
                                msg = "Calibrate Gain 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                Calibrate_Step = 13;
                            }
                            else
                            {
                                //textconsole1.Text += "Can't calibrate Gain 0-10V CH = " + comboBox3.Text + " DONE..";
                                msg = "Can't calibrate Gain 0-10V CH = " + Select_PH04_Channel.ToString() + " DONE..";
                                return;
                            }
                            waitingloop = 1;
                            break;
                        case 13:
                            ph04.Calibrate_SaveEEPROM();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            //textconsole1.Text += "Save Calibrate CH = " + PB04_input_CH.ToString() + " DONE..";
                            msg = "Save Calibrate CH = " + Select_PH04_Channel.ToString() + " DONE..";
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            break;
                        default:
                            msg = "Stop process incorrect calibrate step on value " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_thread_run = false;
                            return;

                    }
                }
                Thread.Sleep(delay_after_memocal);
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";

            }
        }
    }
}
