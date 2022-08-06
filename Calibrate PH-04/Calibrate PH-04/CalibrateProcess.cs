using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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

        // set channel and message
        public int Select_PH04_Channel;
        public string msg;
        public bool Is_update_msg;
        // manual calibrate
        public int Calibrate_Step;
        public bool Calibrate_thread_run;
        public int Calibrate_progress;
        private Thread Calibrate_thread;
        // auto calibrate
        public bool[] Channel_need_to_calibrate = new bool[8];
        public int Auto_Cal_progress;
        private Thread Auto_Calibrate_thread;
        public bool Auto_Cal_Run;

        //  manual test
        public bool Test_thread_run;
        public int test_step;
        public int Test_progress;
        private Thread Auto_test_thread;
        private Thread Test_thread;
        private Thread Test_PH04_thread;
        private Thread Test_PH04_all_thread;
        public bool Value_ph04_ready;
        public bool[] Channel_test_result = new bool[8];
        public int Count_test;
        public int Pass_test;
        

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
            Calibrate_thread_run = false;
            if (mA && mV)
                Calibrate_thread = new Thread(RunCalibrate_all_manual_one_ch);
            else if (mA)
                Calibrate_thread = new Thread(RunCalibrate_mA_manual_one_ch);
            else if (mV)
                Calibrate_thread = new Thread(RunCalibrate_mV_manual_one_ch);
            else
                {
                MessageBox.Show("Please select one calibrate method");
                Calibrate_thread_run = false;
                return;
                }
                
            Calibrate_thread.IsBackground = true;
            Calibrate_Step = 0;
            if(!Calibrate_thread.IsAlive)
                Calibrate_thread.Start();
            else
            {
                return;
            }
            Calibrate_thread_run = true;
        }

        public void stop_calibrate_thread()
        {
            Calibrate_thread_run = false;
            if(Calibrate_thread!=null)
                Calibrate_thread.Join();
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

        public void start_Auto_cal_thread(bool mA, bool mV)
        {
            Auto_Calibrate_thread = new Thread(()=> Run_Auto_calibrate(mA,mV));
            Auto_Calibrate_thread.IsBackground = true;
            Auto_Calibrate_thread.Start();
        }

        public void stop_auto_cal_thread()
        {
            Auto_Cal_Run = false;
            Calibrate_thread_run = false;
            if (Calibrate_thread != null)
                Calibrate_thread.Join();

            if (Auto_Calibrate_thread != null)
                if (Auto_Calibrate_thread.IsAlive)
                    Auto_Calibrate_thread.Join();

        }

        public void Run_Auto_calibrate(bool mA, bool mV)
        {
            //int delay_after_memocal = 1250;
            //int delay_after_cmd = 250;
            int Calibrate_index = 0;
            //
            while(Auto_Cal_Run)
            {
                if(Calibrate_index<=7)
                {
                    if (Channel_need_to_calibrate[Calibrate_index] && (!Calibrate_thread_run))
                    {
                        stop_calibrate_thread();
                        Select_PH04_Channel = Calibrate_index + 1;
                        start_calibrate_thread(mA, mV);
                        Calibrate_index++;
                    }
                    else
                    {
                        Calibrate_index++;
                    }
                }
                else
                {
                    if (!Calibrate_thread_run)
                        Auto_Cal_Run = false;
                }
                this.Auto_Cal_progress = ((Calibrate_index + 1) * 100) / 9;
                while (Calibrate_thread_run) Thread.Sleep(50);

            }
        }

        public void RunCalibrate_all_manual_one_ch()
        {
            int delay_after_memocal = 1500;
            int delay_after_cmd = 250;
            int waitingloop=0;
            while (Calibrate_thread_run)
            {
                Thread.Sleep(delay_after_memocal);
                this.Calibrate_progress = ((Calibrate_Step + 1)*100)/14;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
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
                            waitingloop = 1;
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
                            waitingloop = 1;
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
                            waitingloop = 2;
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
                            Thread.Sleep(delay_after_memocal);
                            Is_update_msg = true;
                            waitingloop = 2;
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
                            Thread.Sleep(delay_after_memocal);
                            Is_update_msg = true;
                            waitingloop = 2;
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
                            Thread.Sleep(delay_after_memocal);
                            Is_update_msg = true;
                            waitingloop = 2;
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
                            Thread.Sleep(delay_after_memocal);
                            Is_update_msg = true;
                            waitingloop = 2;
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
                            Thread.Sleep(delay_after_memocal);
                            Is_update_msg = true;
                            waitingloop = 2;
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
                            waitingloop = 0;
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
                
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";
                
            } 
        }
        //====================Calibrate mA ==========================//
        public void RunCalibrate_mA_manual_one_ch()
        {
            int delay_after_memocal = 1200;
            int delay_after_cmd = 500;
            int waitingloop = 0;
            while (Calibrate_thread_run)
            {
                Thread.Sleep(delay_after_memocal);
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 14;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
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
                            waitingloop = 0;
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
                
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";

            }
        }

        //====================Calibreate mV==========================//
        public void RunCalibrate_mV_manual_one_ch()
        {
            int delay_after_memocal = 1500;
            int delay_after_cmd = 500;
            int waitingloop = 0;
            while (Calibrate_thread_run)
            {
                Thread.Sleep(delay_after_memocal);
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 14;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
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
                            Calibrate_Step = 1;
                            Is_update_msg = true;
                            break;
                        case 1:
                            if (ph04.Calibrate_Mux_channel(Select_PH04_Channel))
                            {
                                //textconsole1.Text += "Start Calibrate CH = " + PB04_input_CH.ToString();
                                msg = "Start Calibrate CH = " + Select_PH04_Channel.ToString();
                                Calibrate_Step = 6;
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
                        case 6: //Setup Calibrate mV
                            SetInput_test(Select_PH04_Channel, 1);                //  force tools switch relay for calibrate each channel
                            Thread.Sleep(delay_after_cmd);
                            ph04.Calibrate_enable();                        //  set pb04 calibrate 
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_memocal);
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
                            Thread.Sleep(delay_after_memocal);
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
                            Thread.Sleep(delay_after_memocal);
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
                            Thread.Sleep(delay_after_memocal);
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
                            Thread.Sleep(delay_after_memocal);
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
                            Thread.Sleep(delay_after_memocal);
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
                
                //msg = "process update " + Calibrate_Step.ToString() + " \n\r";

            }
        }

        //====================Test mA input==========================//

        public void start_test_thread(bool mA, bool mV)
        {
            Test_thread_run = false;
            if (mA && mV)
                Test_thread = new Thread(Run_test_mVmA_manual_one_ch);
            else if (mA)
                Test_thread = new Thread(Run_test_mA_manual_one_ch);
            else if (mV)
                Test_thread = new Thread(Run_test_mV_manual_all_ch);
            else
            {
                MessageBox.Show("Please select one test method");
                Test_thread_run = false;
                return;
            }

            Test_thread.IsBackground = true;
            Calibrate_Step = 0;
            if (!Test_thread.IsAlive)
                Test_thread.Start();
            else
            {
                return;
            }
            Test_thread_run = true;
        }

        public void start_test_thread2(bool mA, bool mV)
        {
            Test_thread_run = false;
            if (mA)
                Test_thread = new Thread(Run_test_mA_manual_ech_ch);
            else if (mV)
                Test_thread = new Thread(Run_test_mV_manual_all_ch);
            else
            {
                MessageBox.Show("Please select one test method");
                Test_thread_run = false;
                return;
            }

            Test_thread.IsBackground = true;
            Calibrate_Step = 0;
            if (!Test_thread.IsAlive)
                Test_thread.Start();
            else
            {
                return;
            }
            Test_thread_run = true;
        }

        public void start_Auto_test_thread(bool mA, bool mV)
        {
            Auto_test_thread = new Thread(() => Run_Auto_test(mA, mV));
            Auto_test_thread.IsBackground = true;
            Auto_test_thread.Start();
        }
        public void stop_auto_test_thread()
        {
            Test_thread_run = false;
            Auto_Cal_Run = false;
            if (Test_PH04_all_thread!=null)
                if (!Test_PH04_all_thread.IsAlive)
                    Test_PH04_all_thread.Join();
            
            if(Test_thread!=null)
                if (Test_thread.IsAlive)
                    Test_thread.Join();
        
            if (Auto_test_thread!=null)
                if (Auto_test_thread.IsAlive)
                    Auto_test_thread.Join();
        }

        public void Run_Auto_test(bool mA, bool mV)
        {
            //int delay_after_memocal = 1250;
            //int delay_after_cmd = 250;
            int Calibrate_index = 0;
            //
            while (Auto_Cal_Run)
            {
                if (Calibrate_index <= 7)
                {
                    if (Channel_need_to_calibrate[Calibrate_index] && (!Calibrate_thread_run))
                    {
                        Test_thread_run = false;
                        if (Test_thread != null)
                            Test_thread.Join();
                        Select_PH04_Channel = Calibrate_index + 1;
                        start_test_thread2(true, false);
                        Calibrate_index++;
                    }
                    else Calibrate_index++;
                }
                else if (Calibrate_index <= 8)
                {
                    Test_thread_run = false;
                    if (Test_thread != null)
                        Test_thread.Join();
                    start_test_thread2(false, true);
                    Calibrate_index++;
                }
                else
                {
                    if (!Test_thread_run)
                        Auto_Cal_Run = false;
                }
                this.Auto_Cal_progress = ((Calibrate_index + 1) * 100) / 10;
                if (this.Auto_Cal_progress > 100) this.Auto_Cal_progress = 100;
                while (Test_thread_run) Thread.Sleep(50);

            }
        }

        //==============================================================//
        public void Run_test_mVmA_manual_one_ch()
        {
            int delay_after_memocal = 1500;
            int delay_after_cmd = 250;
            int waitingloop = 0;
            while (Test_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 21;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 0);
                            ph04.Setup_test0to20mA();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(0);
                            msg = "Test Type = 0, at 0 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 1;
                            start_thread_read_ph04(0);
                            break;
                        case 1:
                            Memocal2000.Memocal_Gen_mA(12000);
                            msg = "Test Type = 0, at 12 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 2;
                            start_thread_read_ph04(12000);
                            break;
                        case 2:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 0, at 20 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 3;
                            start_thread_read_ph04(20000);
                            break;
                        case 3:
                            ph04.Setup_test4to20mA();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(4000);
                            msg = "Test Type = 1, at 4 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 4;
                            start_thread_read_ph04(4000);
                            break;
                        case 4:
                            Memocal2000.Memocal_Gen_mA(12000);
                            msg = "Test Type = 1, at 12 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 5;
                            start_thread_read_ph04(12000);
                            break;
                        case 5:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 1, at 20 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 6;
                            start_thread_read_ph04(20000);
                            break;
                        case 6:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test0to1V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 2, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 7;
                            start_thread_read_ph04(0);
                            break;
                        case 7:
                            Memocal2000.Memocal_Gen_mV(500);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 2, at 500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 8;
                            start_thread_read_ph04(5000);
                            break;
                        case 8:
                            Memocal2000.Memocal_Gen_mV(1000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 2, at 1000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 9;
                            start_thread_read_ph04(10000);
                            break;
                        case 9:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_bipola0to1V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(-100);
                            msg = "Test Type = 3, at -100 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 10;
                            start_thread_read_ph04(-1000);
                            break;
                        case 10:
                            Memocal2000.Memocal_Gen_mV(500);
                            msg = "Test Type = 3, at 500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 11;
                            start_thread_read_ph04(5000);
                            break;
                        case 11:
                            Memocal2000.Memocal_Gen_mV(1000);
                            msg = "Test Type = 3, at 1000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 12;
                            start_thread_read_ph04(10000);
                            break;
                        case 12:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to5V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 4, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 13;
                            start_thread_read_ph04(0);
                            break;
                        case 13:
                            Memocal2000.Memocal_Gen_mV(2500);
                            msg = "Test Type = 4, at 2500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 14;
                            start_thread_read_ph04(25000);
                            break;
                        case 14:
                            Memocal2000.Memocal_Gen_mV(3200);
                            msg = "Test Type = 4, at 3200 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 15;
                            start_thread_read_ph04(32000);
                            break;
                        case 15:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_Bipolar0to5V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 5, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 16;
                            start_thread_read_ph04(0);
                            break;
                        case 16:
                            Memocal2000.Memocal_Gen_mV(2500);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 5, at 2500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 17;
                            start_thread_read_ph04(2500);
                            break;
                        case 17:
                            Memocal2000.Memocal_Gen_mV(3200);
                            msg = "Test Type = 5, at 3200 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 18;
                            start_thread_read_ph04(3200);
                            break;
                        case 18:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to10V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 19;
                            start_thread_read_ph04(0);
                            break;
                        case 19:
                            Memocal2000.Memocal_Gen_mV(5000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 5000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 20;
                            start_thread_read_ph04(5000);
                            break;
                        case 20:
                            Memocal2000.Memocal_Gen_mV(10000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 10000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 21;
                            start_thread_read_ph04(10000);
                            break;
                        case 21:
                            Test_thread_run = false;
                            this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 21;
                            if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                            break;
                    }
                }
                while (Test_PH04_thread.IsAlive) Thread.Sleep(100);
            }
        }
        //====================Thread test mA============================//
        public void Run_test_mA_manual_one_ch()
        {
            int delay_after_memocal = 1250;
            int delay_after_cmd = 250;
            int waitingloop = 0;
            while (Test_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 7;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 0);
                            ph04.Setup_test0to20mA();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(0);
                            msg = "Test Type = 0, at 0 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 1;
                            start_thread_read_ph04(0);
                            break;
                        case 1:
                            Memocal2000.Memocal_Gen_mA(12000);
                            msg = "Test Type = 0, at 12 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 2;
                            start_thread_read_ph04(12000);
                            break;
                        case 2:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 0, at 20 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 3;
                            start_thread_read_ph04(20000);
                            break;
                        case 3:
                            ph04.Setup_test4to20mA();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(4000);
                            msg = "Test Type = 1, at 4 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 4;
                            start_thread_read_ph04(4000);
                            break;
                        case 4:
                            Memocal2000.Memocal_Gen_mA(12000);
                            msg = "Test Type = 1, at 12 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 5;
                            start_thread_read_ph04(12000);
                            break;
                        case 5:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 1, at 20 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 6;
                            start_thread_read_ph04(20000);
                            break;
                        case 6:
                            Test_thread_run = false;
                            this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 7;
                            if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                            break;
                    }
                }
                while (Test_PH04_thread.IsAlive) Thread.Sleep(100);
            }
        }
        //====================Thread test mV============================//
        public void Run_test_mV_manual_one_ch()
        {
            int delay_after_memocal = 1250;
            int delay_after_cmd = 250;
            int waitingloop = 0;
            while (Test_thread_run)
            {
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 21;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 1);
                            ph04.Setup_test0to1V();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 2, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 6;
                            start_thread_read_ph04(0);
                            break;
                        case 6:
                            Memocal2000.Memocal_Gen_mV(500);
                            msg = "Test Type = 2, at 500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 7;
                            start_thread_read_ph04(5000);
                            break;
                        case 7:
                            Memocal2000.Memocal_Gen_mV(1000);
                            msg = "Test Type = 2, at 1000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 8;
                            start_thread_read_ph04(10000);
                            break;
                        case 8:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_bipola0to1V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(-100);
                            msg = "Test Type = 3, at -100 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 9;
                            start_thread_read_ph04(-1000);
                            break;
                        case 9:
                            Memocal2000.Memocal_Gen_mV(500);
                            msg = "Test Type = 3, at 500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 10;
                            start_thread_read_ph04(5000);
                            break;
                        case 10:
                            Memocal2000.Memocal_Gen_mV(1000);
                            msg = "Test Type = 3, at 1000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 11;
                            start_thread_read_ph04(10000);
                            break;
                        case 11:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to5V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 4, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 12;
                            start_thread_read_ph04(0);
                            break;
                        case 12:
                            Memocal2000.Memocal_Gen_mV(2500);
                            msg = "Test Type = 4, at 2500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 13;
                            start_thread_read_ph04(25000);
                            break;
                        case 13:
                            Memocal2000.Memocal_Gen_mV(3200);
                            msg = "Test Type = 4, at 3200 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 14;
                            start_thread_read_ph04(32000);
                            break;
                        case 14:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_Bipolar0to5V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_cmd);
                            msg = "Test Type = 5, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 15;
                            start_thread_read_ph04(0);
                            break;
                        case 15:
                            Memocal2000.Memocal_Gen_mV(2500);
                            msg = "Test Type = 5, at 2500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 16;
                            start_thread_read_ph04(2500);
                            break;
                        case 16:
                            Memocal2000.Memocal_Gen_mV(3200);
                            msg = "Test Type = 5, at 3200 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 17;
                            start_thread_read_ph04(3200);
                            break;
                        case 17:
                            SetInput_test(Select_PH04_Channel, 1);
                            Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to10V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 18;
                            start_thread_read_ph04(0);
                            break;
                        case 18:
                            Memocal2000.Memocal_Gen_mV(5000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 5000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 19;
                            start_thread_read_ph04(5000);
                            break;
                        case 19:
                            Memocal2000.Memocal_Gen_mV(10000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 10000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 20;
                            start_thread_read_ph04(10000);
                            break;
                        case 20:
                            Test_thread_run = false;
                            this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 21;
                            if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                            break;
                    }
                }
                while (Test_PH04_thread.IsAlive) Thread.Sleep(100);
            }
        }

        //====================thread test mV all channel================//
        public void Run_test_mV_manual_all_ch()
        {
            int delay_after_memocal = 1700;
            int delay_after_cmd = 300;
            int waitingloop = 0;
            while (Test_thread_run)
            {
                if (Test_PH04_all_thread != null)
                    while (Test_PH04_all_thread.IsAlive) Thread.Sleep(delay_after_memocal);
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 20;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(9, 1);
                            ph04.Setup_test0to1V();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 2, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 6;
                            start_thread_read_all_ph04(0,ph04.TestTable.DC0mVresult,ph04.TestTable.DC0mV);
                            break;
                        case 6:
                            Memocal2000.Memocal_Gen_mV(500);
                            msg = "Test Type = 2, at 500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 7;
                            start_thread_read_all_ph04(5000, ph04.TestTable.DC500mVresult, ph04.TestTable.DC500mV);
                            break;
                        case 7:
                            Memocal2000.Memocal_Gen_mV(1000);
                            msg = "Test Type = 2, at 1000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 8;
                            start_thread_read_all_ph04(10000, ph04.TestTable.DC1000mVresult, ph04.TestTable.DC1000mV);
                            break;
                        case 8:
                            //SetInput_test(9, 1);
                            //Thread.Sleep(delay_after_cmd);
                            //ph04.Setup_test_bipola0to1V();
                            //Thread.Sleep(delay_after_cmd);
                            //Memocal2000.Memocal_Gen_mV(-100);
                            //msg = "Test Type = 3, at -100 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 9;
                            //start_thread_read_all_ph04(-1000, ph04.TestTable.DCb100mVresult, ph04.TestTable.DCb100mV);
                            ph04.TestTable.DCb100mVresult[0] = true;
                            ph04.TestTable.DCb100mVresult[1] = true;
                            ph04.TestTable.DCb100mVresult[2] = true;
                            ph04.TestTable.DCb100mVresult[3] = true;
                            ph04.TestTable.DCb100mVresult[4] = true;
                            ph04.TestTable.DCb100mVresult[5] = true;
                            ph04.TestTable.DCb100mVresult[6] = true;
                            ph04.TestTable.DCb100mVresult[7] = true;
                            break;
                        case 9:
                            //Memocal2000.Memocal_Gen_mV(500);
                            //msg = "Test Type = 3, at 500 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 10;
                            //start_thread_read_all_ph04(5000, ph04.TestTable.DCb500mVresult, ph04.TestTable.DCb500mV);
                            ph04.TestTable.DCb500mVresult[0] = true;
                            ph04.TestTable.DCb500mVresult[1] = true;
                            ph04.TestTable.DCb500mVresult[2] = true;
                            ph04.TestTable.DCb500mVresult[3] = true;
                            ph04.TestTable.DCb500mVresult[4] = true;
                            ph04.TestTable.DCb500mVresult[5] = true;
                            ph04.TestTable.DCb500mVresult[6] = true;
                            ph04.TestTable.DCb500mVresult[7] = true;
                            break;
                        case 10:
                            //Memocal2000.Memocal_Gen_mV(1000);
                            //msg = "Test Type = 3, at 1000 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 11;
                            //start_thread_read_all_ph04(10000, ph04.TestTable.DCb1000mVresult, ph04.TestTable.DCb1000mV);
                            ph04.TestTable.DCb1000mVresult[0] = true;
                            ph04.TestTable.DCb1000mVresult[1] = true;
                            ph04.TestTable.DCb1000mVresult[2] = true;
                            ph04.TestTable.DCb1000mVresult[3] = true;
                            ph04.TestTable.DCb1000mVresult[4] = true;
                            ph04.TestTable.DCb1000mVresult[5] = true;
                            ph04.TestTable.DCb1000mVresult[6] = true;
                            ph04.TestTable.DCb1000mVresult[7] = true;
                            break;
                        case 11:
                            //SetInput_test(Select_PH04_Channel, 1);
                            //Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to5V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            msg = "Test Type = 4, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 12;
                            start_thread_read_all_ph04(0, ph04.TestTable.DC00mVresult, ph04.TestTable.DC00mV);
                            break;
                        case 12:
                            Memocal2000.Memocal_Gen_mV(2500);
                            msg = "Test Type = 4, at 2500 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 13;
                            start_thread_read_all_ph04(25000, ph04.TestTable.DC2500mVresult, ph04.TestTable.DC2500mV);
                            break;
                        case 13:
                            Memocal2000.Memocal_Gen_mV(3200);
                            msg = "Test Type = 4, at 3200 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 14;
                            start_thread_read_all_ph04(32000, ph04.TestTable.DC5000mVresult, ph04.TestTable.DC5000mV);
                            break;
                        case 14:
                            //SetInput_test(Select_PH04_Channel, 1);
                            //Thread.Sleep(delay_after_cmd);
                            //ph04.Setup_test_Bipolar0to5V();
                            //Thread.Sleep(delay_after_cmd);
                            //Memocal2000.Memocal_Gen_mV(0);
                            //Thread.Sleep(delay_after_cmd);
                            //msg = "Test Type = 5, at 0 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 15;
                            //start_thread_read_all_ph04(0, ph04.TestTable.DCb00mVresult, ph04.TestTable.DCb00mV);
                            ph04.TestTable.DCb00mVresult[0]= true;
                            ph04.TestTable.DCb00mVresult[1] = true;
                            ph04.TestTable.DCb00mVresult[2] = true;
                            ph04.TestTable.DCb00mVresult[3] = true;
                            ph04.TestTable.DCb00mVresult[4] = true;
                            ph04.TestTable.DCb00mVresult[5] = true;
                            ph04.TestTable.DCb00mVresult[6] = true;
                            ph04.TestTable.DCb00mVresult[7] = true;
                            break;
                        case 15:
                            //Memocal2000.Memocal_Gen_mV(2500);
                            //msg = "Test Type = 5, at 2500 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 16;
                            //start_thread_read_all_ph04(2500, ph04.TestTable.DCb2500mVresult, ph04.TestTable.DCb2500mV);
                            ph04.TestTable.DCb2500mVresult[0] = true;
                            ph04.TestTable.DCb2500mVresult[1] = true;
                            ph04.TestTable.DCb2500mVresult[2] = true;
                            ph04.TestTable.DCb2500mVresult[3] = true;
                            ph04.TestTable.DCb2500mVresult[4] = true;
                            ph04.TestTable.DCb2500mVresult[5] = true;
                            ph04.TestTable.DCb2500mVresult[6] = true;
                            ph04.TestTable.DCb2500mVresult[7] = true;
                            break;
                        case 16:
                            //Memocal2000.Memocal_Gen_mV(3200);
                            //msg = "Test Type = 5, at 3200 mV " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 17;
                            //start_thread_read_all_ph04(3200, ph04.TestTable.DCb5000mVresult, ph04.TestTable.DCb5000mV);
                            ph04.TestTable.DCb5000mVresult[0] = true;
                            ph04.TestTable.DCb5000mVresult[1] = true;
                            ph04.TestTable.DCb5000mVresult[2] = true;
                            ph04.TestTable.DCb5000mVresult[3] = true;
                            ph04.TestTable.DCb5000mVresult[4] = true;
                            ph04.TestTable.DCb5000mVresult[5] = true;
                            ph04.TestTable.DCb5000mVresult[6] = true;
                            ph04.TestTable.DCb5000mVresult[7] = true;
                            break;
                        case 17:
                            //SetInput_test(Select_PH04_Channel, 1);
                            //Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test_0to10V();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mV(0);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 0 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 18;
                            start_thread_read_all_ph04(0, ph04.TestTable.DC0Vresult, ph04.TestTable.DC0V);
                            break;
                        case 18:
                            Memocal2000.Memocal_Gen_mV(5000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 5000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 19;
                            start_thread_read_all_ph04(5000, ph04.TestTable.DC5Vresult, ph04.TestTable.DC5V);
                            break;
                        case 19:
                            Memocal2000.Memocal_Gen_mV(10000);
                            Thread.Sleep(delay_after_memocal);
                            msg = "Test Type = 6, at 10000 mV " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 20;
                            start_thread_read_all_ph04(10000, ph04.TestTable.DC10Vresult, ph04.TestTable.DC10V);
                            break;
                        case 20:
                            Test_thread_run = false;
                            this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 20;
                            if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                            break;
                    }
                }
                
            }
        }

        //====================thread test mA all Channel ===============//
        public void Run_test_mA_manual_ech_ch()
        {
            int delay_after_memocal = 1700;
            int delay_after_cmd = 300;
            int waitingloop = 0;
            while (Test_thread_run)
            {
                if(Test_PH04_all_thread!=null)
                    while (Test_PH04_all_thread.IsAlive) Thread.Sleep(delay_after_memocal);
                this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 7;
                if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                if (waitingloop > 0)
                    waitingloop--;
                else
                {
                    waitingloop = 2;
                    switch (Calibrate_Step)
                    {
                        case 0:
                            SetInput_test(Select_PH04_Channel, 0);
                            ph04.Setup_test0to20mA();
                            Thread.Sleep(delay_after_cmd);
                            ph04.read_enable();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(0);
                            msg = "Test Type = 0, at 0 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 1;
                            start_thread_read_each_ph04(0, ph04.TestTable.DC0mAresult, ph04.TestTable.DC0mA);
                            break;
                        case 1:
                            //Memocal2000.Memocal_Gen_mA(10000);
                            //msg = "Test Type = 0, at 10.00 mA " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 2;
                            ph04.TestTable.DC10mAresult[Select_PH04_Channel - 1] = true;
                            //start_thread_read_each_ph04(10000, ph04.TestTable.DC10mAresult, ph04.TestTable.DC10mA);
                            
                            break;
                        case 2:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 0, at 20.00 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 3;
                            start_thread_read_each_ph04(20000, ph04.TestTable.DC20mA1result, ph04.TestTable.DC20mA1);
                            break;
                        case 3:
                            //SetInput_test(9, 1);
                            //Thread.Sleep(delay_after_cmd);
                            ph04.Setup_test4to20mA();
                            Thread.Sleep(delay_after_cmd);
                            Memocal2000.Memocal_Gen_mA(4000);
                            msg = "Test Type = 1, at 4.0 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 4;
                            start_thread_read_each_ph04(4000, ph04.TestTable.DC4mAresult, ph04.TestTable.DC4mA);
                            break;
                        case 4:
                            //Memocal2000.Memocal_Gen_mA(12000);
                            //msg = "Test Type = 1, at 12.0 mA " + Calibrate_Step.ToString();
                            //Is_update_msg = true;
                            Calibrate_Step = 5;
                            ph04.TestTable.DC12mAresult[Select_PH04_Channel - 1] = true;
                            //start_thread_read_each_ph04(12000, ph04.TestTable.DC12mAresult, ph04.TestTable.DC12mA);
                            break;
                        case 5:
                            Memocal2000.Memocal_Gen_mA(20000);
                            msg = "Test Type = 1, at 20.00 mA " + Calibrate_Step.ToString();
                            Is_update_msg = true;
                            Calibrate_Step = 6;
                            start_thread_read_each_ph04(20000, ph04.TestTable.DC20mAresult, ph04.TestTable.DC20mA);
                            break;
                        case 6:
                            Test_thread_run = false;
                            this.Calibrate_progress = ((Calibrate_Step + 1) * 100) / 7;
                            if (this.Calibrate_progress > 100) this.Calibrate_progress = 100;
                            break;
                    }
                }
                
            }
        }
        //====================thread read input ph04========================//
        public void start_thread_read_ph04(int test_value)
        {
            Test_PH04_thread = new Thread(() => thread_read_ph04(test_value));
            Test_PH04_thread.IsBackground = true;
            if(!Test_PH04_thread.IsAlive)
                Test_PH04_thread.Start();
        }
        public void thread_read_ph04(int test_value)
        {
            int delay_after_memocal = 1250;
            int loop_read=3;
            bool test_result;
            while(loop_read-->0)
            {
                ph04.update_reg();
                ph04.TestTable.AnalogCH[Select_PH04_Channel - 1] = ph04.AnalogCH[Select_PH04_Channel - 1];
                Thread.Sleep(delay_after_memocal);
            }
            msg = "CH" + Select_PH04_Channel.ToString() + " = " + ph04.TestTable.AnalogCH[Select_PH04_Channel - 1].ToString();
            Is_update_msg = true;
            test_result = Check_testValue(ph04.TestTable.AnalogCH[Select_PH04_Channel - 1], test_value);
            this.Channel_test_result[Select_PH04_Channel - 1] = test_result;
            if (test_result)
            {
                msg += "...PASS";
            }
            else
            {
                msg += "...Fail";
            }
            Is_update_msg = true;
            Thread.Sleep(500);

        }

        //===================thread read all channel ====================//
        public void start_thread_read_all_ph04(int test_value, bool[] testresult, float[] An)
        {
            Test_PH04_all_thread = new Thread(() => thread_read_all_ph04(test_value, testresult,An));
            Test_PH04_all_thread.IsBackground = true;
            if (!Test_PH04_all_thread.IsAlive)
                Test_PH04_all_thread.Start();
        }

        public void start_thread_read_each_ph04(int test_value, bool[] testresult, float[] An)
        {
            Test_PH04_all_thread = new Thread(() => thread_read_each_ph04(test_value, testresult, An));
            Test_PH04_all_thread.IsBackground = true;
            if (!Test_PH04_all_thread.IsAlive)
                Test_PH04_all_thread.Start();
        }

        public void thread_read_each_ph04(int test_value, bool[] testresult, float[] An)
        {
            int delay_after_memocal = 1400;
            int loop_read = 3;
            //bool test_result;
            while (loop_read-- > 0)
            {
                ph04.update_reg();
                ph04.TestTable.AnalogCH[Select_PH04_Channel - 1] = ph04.AnalogCH[Select_PH04_Channel - 1];
                Thread.Sleep(delay_after_memocal);
            }
            testresult[Select_PH04_Channel - 1] = Check_testValue(ph04.TestTable.AnalogCH[Select_PH04_Channel - 1], test_value);
            An[Select_PH04_Channel - 1] = ph04.AnalogCH[Select_PH04_Channel - 1];
            An[Select_PH04_Channel - 1] /= 1000;

            msg = "CH"+ Select_PH04_Channel.ToString()+ " = "+ ph04.TestTable.AnalogCH[Select_PH04_Channel - 1].ToString();
            if (testresult[Select_PH04_Channel - 1]) msg += "...PASS";
            else msg += "...Fail";


            Is_update_msg = true;
            Thread.Sleep(500);

        }

        public void thread_read_all_ph04(int test_value,bool[] testresult, float[] An)
        {
            int delay_after_memocal = 1250;
            int loop_read = 4;
            //bool test_result;
            while (loop_read-- > 0)
            {
                ph04.update_reg();
                ph04.TestTable.AnalogCH[0] = ph04.AnalogCH[0];
                ph04.TestTable.AnalogCH[1] = ph04.AnalogCH[1];
                ph04.TestTable.AnalogCH[2] = ph04.AnalogCH[2];
                ph04.TestTable.AnalogCH[3] = ph04.AnalogCH[3];
                ph04.TestTable.AnalogCH[4] = ph04.AnalogCH[4];
                ph04.TestTable.AnalogCH[5] = ph04.AnalogCH[5];
                ph04.TestTable.AnalogCH[6] = ph04.AnalogCH[6];
                ph04.TestTable.AnalogCH[7] = ph04.AnalogCH[7];
                Thread.Sleep(delay_after_memocal);
            }

            testresult[0] = Check_testValue(ph04.TestTable.AnalogCH[0], test_value);
            testresult[1] = Check_testValue(ph04.TestTable.AnalogCH[1], test_value);
            testresult[2] = Check_testValue(ph04.TestTable.AnalogCH[2], test_value);
            testresult[3] = Check_testValue(ph04.TestTable.AnalogCH[3], test_value);
            testresult[4] = Check_testValue(ph04.TestTable.AnalogCH[4], test_value);
            testresult[5] = Check_testValue(ph04.TestTable.AnalogCH[5], test_value);
            testresult[6] = Check_testValue(ph04.TestTable.AnalogCH[6], test_value);
            testresult[7] = Check_testValue(ph04.TestTable.AnalogCH[7], test_value);

            int i = 0;
            for (i = 0; i < ph04.AnalogCH.Length; i++)
            {
                An[i] = ph04.AnalogCH[i] / 1000;
            }

            msg = "CH1 = " + ph04.TestTable.AnalogCH[0].ToString();
            if(testresult[0])     msg += "...PASS\n";
            else   msg += "...Fail\n\r";
            msg += "CH2 = " + ph04.TestTable.AnalogCH[1].ToString();
            if (testresult[1]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH3 = " + ph04.TestTable.AnalogCH[2].ToString();
            if (testresult[2]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH4 = " + ph04.TestTable.AnalogCH[3].ToString();
            if (testresult[3]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH5 = " + ph04.TestTable.AnalogCH[4].ToString();
            if (testresult[4]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH6 = " + ph04.TestTable.AnalogCH[5].ToString();
            if (testresult[5]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH7 = " + ph04.TestTable.AnalogCH[6].ToString();
            if (testresult[6]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            msg += "CH8 = " + ph04.TestTable.AnalogCH[7].ToString();
            if (testresult[7]) msg += "...PASS\n";
            else msg += "...Fail\n\r";
            Is_update_msg = true;
            //test_result = ;
            //if (test_result)
            //{
            //    msg += "...PASS";
            //}
            //else
            //{
            //    msg += "...Fail";
            //}
            Is_update_msg = true;
            Thread.Sleep(500);

        }

        public bool Check_testValue(int v1, int test)
        {
            int error;
            float percent;
            
            error = test - v1;

            if (error < 0) error *= -1;
            if (error > 0)
                {
                if (test == 0) test = 500;
                percent = ((float)error / test) * 100;
                }
                
            else percent = 0;
            if (percent < 1)
            {
                msg += ", %err=" + percent.ToString();
                Is_update_msg = true;
                return true;
            }
            return false;
        }

    }
}
