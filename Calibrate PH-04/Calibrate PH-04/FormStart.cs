using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using Calibrate_PB_04;

namespace Calibrate_PH_04
{

    public partial class FormStart : Form
    {
        static SerialPort sPort1, sPort2;
        public Stopwatch stopWatch = new();
        //memocal Memocal2000 = new memocal();
        //public PH04 ph04 = new PH04();
        //public PH07 ph07id2 = new PH07();
        //public PH07 ph07id3 = new PH07();
        //public PH07 ph07id4 = new PH07();
        CalibrateProcess process = new();
        CalibratePb04 process2 = new();
        Form1 ResultForm = new();
        bool wait_button6;
        //PB04 devicepb04 =new();


        public FormStart()
        {
            InitializeComponent();
            Get_compoart_list();
            sPort1 = new SerialPort();
            sPort2 = new SerialPort();

        }

        private void Get_compoart_list()
        {
            string[] ArrayComPortsNames;
            int index = -1;
            string ComPortName = "";
            ArrayComPortsNames = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            if (ArrayComPortsNames.Length <= 0) return;
            do
            {
                index += 1;
                comboBox1.Items.Add(ArrayComPortsNames[index]);
                comboBox2.Items.Add(ArrayComPortsNames[index]);
            }
            while (!((ArrayComPortsNames[index] == ComPortName)
                          || (index == ArrayComPortsNames.GetUpperBound(0))));
            Array.Sort(ArrayComPortsNames);
            comboBox1.Text = ArrayComPortsNames[0];
            comboBox2.Text = ArrayComPortsNames[0];
            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Get_compoart_list();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                if (process.Memocal2000.IsPortOpen)
                {
                    if (process.Memocal2000.close_comport())
                    {
                        LedMemocal.Image = Properties.Resources.red;
                        listBox1.Items.Add("Disconnect Memocal2000... ");
                    }
                    else
                    {
                        MessageBox.Show("Error !! Please check Comport or restart app");
                        listBox1.Items.Add("Please check Comport & Memocal2000");
                    }
                }
            }

            if (radioButton2.Checked)
            {
                if (process2.Memocal2000.IsPortOpen)
                {
                    if (process2.Memocal2000.close_comport())
                    {
                        LedMemocal.Image = Properties.Resources.red;
                        listBox1.Items.Add("Disconnect Memocal2000... ");
                    }
                    else
                    {
                        MessageBox.Show("Error !! Please check Comport or restart app");
                        listBox1.Items.Add("Please check Comport & Memocal2000");
                    }
                }
            }

            button2.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            wait_button6 = true;
            if(radioButton1.Checked)
            {
                try
                {
                    if (!sPort2.IsOpen)
                    {
                        sPort2.PortName = comboBox2.Text;
                        sPort2.BaudRate = 9600;
                        sPort2.Parity = Parity.None;
                        sPort2.DataBits = 8;
                        sPort2.StopBits = StopBits.One;
                        sPort2.Handshake = Handshake.None;
                        sPort2.WriteTimeout = 1000;
                        sPort2.ReadTimeout = 1000;
                        sPort2.Open();
                    }

                }
                catch
                {
                    if (sPort2.IsOpen)
                        sPort2.Close();
                    process.IsConnected = false;
                    return;
                }
                try
                {
                    process.ph07id2.Set_SlaveID(byte.Parse(textId1.Text));
                    process.ph07id3.Set_SlaveID(byte.Parse(textId2.Text));
                    process.ph07id4.Set_SlaveID(byte.Parse(textId3.Text));
                    process.ph04.Set_SlaveID(byte.Parse(textPHId1.Text));
                }
                catch
                {
                    MessageBox.Show("PH07 & PH04 Slave ID can be number only!!");
                    wait_button6 = false;
                    return;
                }
                process.ph07id2.Start_port(sPort2);
                process.ph07id3.Start_port(sPort2);
                process.ph07id4.Start_port(sPort2);
                process.ph04.Start_port(sPort2);

                if (process.ph07id2.Ready)
                {
                    ledph07id1.Image = Properties.Resources.green;
                    listBox1.Items.Add("PH-07 module 1 ready ...");
                }
                else
                {
                    ledph07id1.Image = Properties.Resources.red;
                    listBox1.Items.Add("PH-07 module 1 not ready ...");
                    return;
                }
                if (process.ph07id3.Ready)
                {
                    ledph07id2.Image = Properties.Resources.green;
                    listBox1.Items.Add("PH-07 module 2 ready ...");
                }
                else
                {
                    ledph07id2.Image = Properties.Resources.red;
                    listBox1.Items.Add("PH-07 module 2 not ready ...");
                    return;
                }
                if (process.ph07id4.Ready)
                {
                    ledph07id3.Image = Properties.Resources.green;
                    listBox1.Items.Add("PH-07 module 3 ready ...");
                }
                else
                {
                    ledph07id3.Image = Properties.Resources.red;
                    listBox1.Items.Add("PH-07 module 3 not ready ...");
                    return;
                }
                if (process.ph04.Ready)
                {
                    ledph04id1.Image = Properties.Resources.green;
                    listBox1.Items.Add("PH-04 module ready ...");
                }
                else
                {
                    ledph04id1.Image = Properties.Resources.red;
                    listBox1.Items.Add("PH-04 module not ready ...");
                    return;
                }

                if (process.IsCalibrate_tool_ready())
                {
                    LED2.Image = Properties.Resources.green;
                    splitContainer1.Enabled = true;
                    splitContainer2.Enabled = true;
                    pictureBox1.Image = Properties.Resources.green;
                }
                else
                {
                    LED2.Image = Properties.Resources.red;
                    return;
                }
                button6.Enabled = false;
                button8.Enabled = true;
            }

            if(radioButton2.Checked)
            {
                //listBox1.Items.Add("Plase Wait for Connect to IP PB-04 ...");
                if (!process2.pb04.IsPortOpen)
                    process2.pb04.start_tcp(pb04ip.Text,501);
                if(process2.pb04.Is_module_ready())
                {
                    listBox1.Items.Add("PB-04 module ready ...");
                    ledpb04.Image = Properties.Resources.green;
                    LED2.Image = Properties.Resources.green;
                    pictureBox1.Image = Properties.Resources.green;
                    splitContainer1.Enabled = true;
                    splitContainer2.Enabled = true;
                    button6.Enabled = false;
                    button8.Enabled = true;
                }
            }
            
            wait_button6 = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                try
                {
                    if (sPort2.IsOpen)
                    {
                        try
                        {
                            sPort2.Close();
                            listBox1.Items.Add("Tools disconnected...");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error !! Please check Comport or restart app" + ex.ToString());
                            listBox1.Items.Add("Error !! Please check Comport or restart app");
                        }
                    }

                    ledph07id1.Image = Properties.Resources.red;
                    ledph07id2.Image = Properties.Resources.red;
                    ledph07id3.Image = Properties.Resources.red;
                    ledph04id1.Image = Properties.Resources.red;
                    LED2.Image = Properties.Resources.red;
                }
                catch (Exception ex)
                {           MessageBox.Show(ex.ToString());             }


            if (radioButton2.Checked)
                try
                {
                    ledpb04.Image = Properties.Resources.red;
                    LED2.Image = Properties.Resources.red;
                    pictureBox1.Image = Properties.Resources.red;
                    process2.pb04.stop_tcp();
                }
                catch(Exception ex)
                {        MessageBox.Show(ex.ToString());             }
            

            button6.Enabled = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //process.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
            process.Select_PH04_Channel= Int16.Parse(comboBox12.Text);
            process2.Select_Channel= Int16.Parse(comboBox12.Text);
            //process.SetInput_test(process.Select_PH04_Channel, 1);
            if (!backgroundWorker_manualmA.IsBusy)
                backgroundWorker_manualmA.RunWorkerAsync();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //process.Memocal2000.Memocal_Gen_mV(int.Parse(textBox1.Text));
            process.Select_PH04_Channel = Int16.Parse(comboBox3.Text);
            process2.Select_Channel = Int16.Parse(comboBox3.Text);
            //process.SetInput_test(process.Select_PH04_Channel, 1);
            if (!backgroundWorker_manualmV.IsBusy)
                backgroundWorker_manualmV.RunWorkerAsync();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //if(!bkgWorker_GetPH04value.IsBusy)
            if(radioButton1.Checked)
                bkgWorker_GetPH04value.RunWorkerAsync();
            if(radioButton2.Checked)
                bkgWork_GetPB04value.RunWorkerAsync();
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            bkgWorker_WRPH04.RunWorkerAsync();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                // set channel to calibrate
                process.Select_PH04_Channel = Int16.Parse(comboBox3.Text);
                process.SetInput_test(process.Select_PH04_Channel, 0);
                stopWatch.Restart();
                stopWatch.Start();
                richTextBox1.Clear();
                richTextBox1.AppendText("Start Calibrate ..\r\n");
                process.start_calibrate_thread(checkBox1.Checked, checkBox2.Checked);
                timer1.Enabled = true;
            }
           if(radioButton2.Checked)
            {
                process2.Select_Channel = Int16.Parse(comboBox3.Text);
                //process.SetInput_test(process.Select_PH04_Channel, 0);
                stopWatch.Restart();
                stopWatch.Start();
                richTextBox1.Clear();
                richTextBox1.AppendText("Start Calibrate ..\r\n");
                process2.start_calibrate_thread(checkBox1.Checked, checkBox2.Checked);
                timer1.Enabled = true;
            }
            
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                this.labelStopwatch.Invoke(new MethodInvoker(delegate () {
                    labelStopwatch.Text = elapsedTime; 
                    label_calibrate.Text = elapsedTime;
                    }
                ));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!bkgWorker_Stopwatch.IsBusy)
                bkgWorker_Stopwatch.RunWorkerAsync();
            if(radioButton1.Checked)
            {
                if (process.Is_update_msg)
                {
                    richTextBox1.AppendText(process.Upate_msg());
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.ScrollToCaret();
                    richTextBox1.Refresh();

                    progressBar1.Value = process.Calibrate_progress;
                    //progressBar2.Value = process.Auto_Cal_progress;
                    //progressBar3.Value = process.Calibrate_progress;
                }
                if ((!process.Calibrate_thread_run) && (!process.Test_thread_run))
                {
                    timer1.Enabled = false;
                }
            }
            
            if(radioButton2.Checked)
            {
                if (process2.Is_update_msg)
                {
                    richTextBox1.AppendText(process2.Upate_msg());
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.ScrollToCaret();
                    richTextBox1.Refresh();

                    progressBar1.Value = process2.Calibrate_progress;
                    //progressBar2.Value = process.Auto_Cal_progress;
                    //progressBar3.Value = process.Calibrate_progress;
                }
                if ((!process2.Calibrate_thread_run) && (!process2.Test_thread_run))
                {
                    timer1.Enabled = false;
                }
            }
            

        }

        private void backgroundWorker_manualmA_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if(radioButton1.Checked)
            {
                process.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
                this.Invoke(new MethodInvoker(delegate () {
                    process.Select_PH04_Channel = Int16.Parse(comboBox12.Text);
                }));
                process.SetInput_test(process.Select_PH04_Channel, 0);
            }
            if (radioButton2.Checked)
            {
                process2.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
                this.Invoke(new MethodInvoker(delegate () {
                    process2.Select_Channel = Int16.Parse(comboBox12.Text);
                }));
            }

        }

        private void backgroundWorker_manualmV_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if(radioButton1.Checked)
            {
                process.Memocal2000.Memocal_Gen_mV(int.Parse(textBox1.Text));
                this.Invoke(new MethodInvoker(delegate () {
                    process.Select_PH04_Channel = Int16.Parse(comboBox12.Text);
                }));
                process.SetInput_test(process.Select_PH04_Channel, 1);
            }
            if (radioButton2.Checked)
            {
                process2.Memocal2000.Memocal_Gen_mV(int.Parse(textBox1.Text));
                this.Invoke(new MethodInvoker(delegate () {
                    process2.Select_Channel = Int16.Parse(comboBox12.Text);
                }));
            }
        }

        private void bkgWorker_GetPH04value_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if(radioButton1.Checked)
                process.ph04.update_reg();

            this.Invoke(
                new MethodInvoker( delegate() {
                    this.AnCh1.Text = process.ph04.AnalogCH[0].ToString();
                    this.AnCh2.Text = process.ph04.AnalogCH[1].ToString();
                    this.AnCh3.Text = process.ph04.AnalogCH[2].ToString();
                    this.AnCh4.Text = process.ph04.AnalogCH[3].ToString();
                    this.AnCh5.Text = process.ph04.AnalogCH[4].ToString();
                    this.AnCh6.Text = process.ph04.AnalogCH[5].ToString();
                    this.AnCh7.Text = process.ph04.AnalogCH[6].ToString();
                    this.AnCh8.Text = process.ph04.AnalogCH[7].ToString();

                    this.TypeCh1.SelectedIndex = process.ph04.InputType[0];
                    this.TypeCh2.SelectedIndex = process.ph04.InputType[1];
                    this.TypeCh3.SelectedIndex = process.ph04.InputType[2];
                    this.TypeCh4.SelectedIndex = process.ph04.InputType[3];
                    this.TypeCh5.SelectedIndex = process.ph04.InputType[4];
                    this.TypeCh6.SelectedIndex = process.ph04.InputType[5];
                    this.TypeCh7.SelectedIndex = process.ph04.InputType[6];
                    this.TypeCh8.SelectedIndex = process.ph04.InputType[7];
                    this.richTextBox1.AppendText("Get value update form PH-04..\r\n");
                })
                );

            
        }

        private void bkgWorker_WRPH04_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.Invoke(
                new MethodInvoker(delegate () {
                    process.ph04.InputType[0] = ((short)this.TypeCh1.SelectedIndex);
                    process.ph04.InputType[1] = ((short)this.TypeCh2.SelectedIndex);
                    process.ph04.InputType[2] = ((short)this.TypeCh3.SelectedIndex);
                    process.ph04.InputType[3] = ((short)this.TypeCh4.SelectedIndex);
                    process.ph04.InputType[4] = ((short)this.TypeCh5.SelectedIndex);
                    process.ph04.InputType[5] = ((short)this.TypeCh6.SelectedIndex);
                    process.ph04.InputType[6] = ((short)this.TypeCh7.SelectedIndex);
                    process.ph04.InputType[7] = ((short)this.TypeCh8.SelectedIndex);
                    List<int> update_reg = new List<int>();
                    update_reg.Add(process.ph04.InputType[0]);
                    update_reg.Add(process.ph04.InputType[1]);
                    update_reg.Add(process.ph04.InputType[2]);
                    update_reg.Add(process.ph04.InputType[3]);
                    update_reg.Add(process.ph04.InputType[4]);
                    update_reg.Add(process.ph04.InputType[5]);
                    update_reg.Add(process.ph04.InputType[6]);
                    update_reg.Add(process.ph04.InputType[7]);
                    if(radioButton1.Checked)
                    {
                        process.ph04.write_input_type(update_reg);
                        this.richTextBox1.AppendText("Write udate value to PH-04..\r\n");
                    }
                    if(radioButton2.Checked)
                    {
                        process2.pb04.write_input_type(update_reg);
                        this.richTextBox1.AppendText("Write udate value to PB-04..\r\n");
                    }
                    

                    button11_Click(sender, e);
                })
                );    
        }

        private void button17_Click(object sender, EventArgs e)
        {
            process.Select_PH04_Channel = Int16.Parse(comboBox3.Text);
            //process.SetInput_test(process.Select_PH04_Channel, 0);
            process.ph04.Calibrate_Mux_channel(process.Select_PH04_Channel);
            process.ph04.Calibrate_Duplicate_to_all_CH();
            richTextBox1.AppendText("Force all channel use same calibrate value..\r\n");
        }

        private void btn_autoCalibrate_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            if(radioButton1.Checked)
            {
                process.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process.Channel_need_to_calibrate[7] = check_ch8.Checked;


                process.Auto_Cal_Run = true;
                process.start_Auto_cal_thread(checkBox3.Checked, checkBox4.Checked);
            }
            if(radioButton2.Checked)
            {
                process2.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process2.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process2.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process2.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process2.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process2.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process2.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process2.Channel_need_to_calibrate[7] = check_ch8.Checked;


                process2.Auto_Cal_Run = true;
                process2.start_Auto_cal_thread(checkBox3.Checked, checkBox4.Checked);
            }
            
            timer2.Enabled = true;
            stopWatch.Restart();
            stopWatch.Start();
            btn_autoCalibrate.Enabled = false;
            btn_autoTest.Enabled=false;
            btn_stop_calibrate.Enabled = true;
            button9.Enabled = false;
            if (timer3.Enabled) timer3.Enabled = false;
        }

        private void process_test_result(bool t_result)
        {
            if (check_allch.Checked)
            {
                process.Count_test += 1;
                if (t_result)
                {
                    process.Pass_test += 1;
                    if (ResultForm.IsDisposed)
                        ResultForm = new Form1();
                    ResultForm.label1.Visible = false;
                    ResultForm.label2.Visible = true;
                    ResultForm.Visible = true;
                    ResultForm.Show();
                    label40.Text = "PASS";
                    label40.ForeColor = Color.Green;
                }
                else
                {
                    if (ResultForm.IsDisposed)
                        ResultForm = new Form1();
                    ResultForm.label1.Visible = true;
                    ResultForm.label2.Visible = false;
                    ResultForm.Visible = true;
                    ResultForm.Show();
                    label40.Text = "FAIL";
                    label40.ForeColor = Color.Red;
                }
                text_count.Text = process.Count_test.ToString();
                text_pass.Text = process.Pass_test.ToString();
            }
            else
            {
                richTextBox2.AppendText("Test Done...");
                richTextBox2.AppendText(Environment.NewLine);
                richTextBox2.ScrollToCaret();
                richTextBox2.Refresh();
            }
            if (check_ch1.Checked)
                if (process.ph04.TestTable.Cal_Result[0])
                    Result_CH1.Image = Properties.Resources.ok2;
                else
                    Result_CH1.Image = Properties.Resources.fail2;
            else
                Result_CH1.Image = Properties.Resources.none1;

            if (check_ch2.Checked)
                if (process.ph04.TestTable.Cal_Result[1])
                    Result_CH2.Image = Properties.Resources.ok2;
                else
                    Result_CH2.Image = Properties.Resources.fail2;
            else
                Result_CH2.Image = Properties.Resources.none1;

            if (check_ch3.Checked)
                if (process.ph04.TestTable.Cal_Result[2])
                    Result_CH3.Image = Properties.Resources.ok2;
                else
                    Result_CH3.Image = Properties.Resources.fail2;
            else
                Result_CH3.Image = Properties.Resources.none1;

            if (check_ch4.Checked)
                if (process.ph04.TestTable.Cal_Result[3])
                    Result_CH4.Image = Properties.Resources.ok2;
                else
                    Result_CH4.Image = Properties.Resources.fail2;
            else
                Result_CH4.Image = Properties.Resources.none1;

            if (check_ch5.Checked)
                if (process.ph04.TestTable.Cal_Result[4])
                    Result_CH5.Image = Properties.Resources.ok2;
                else
                    Result_CH5.Image = Properties.Resources.fail2;
            else
                Result_CH5.Image = Properties.Resources.none1;

            if (check_ch6.Checked)
                if (process.ph04.TestTable.Cal_Result[5])
                    Result_CH6.Image = Properties.Resources.ok2;
                else
                    Result_CH6.Image = Properties.Resources.fail2;
            else
                Result_CH6.Image = Properties.Resources.none1;

            if (check_ch7.Checked)
                if (process.ph04.TestTable.Cal_Result[6])
                    Result_CH7.Image = Properties.Resources.ok2;
                else
                    Result_CH7.Image = Properties.Resources.fail2;
            else
                Result_CH7.Image = Properties.Resources.none1;

            if (check_ch8.Checked)
                if (process.ph04.TestTable.Cal_Result[7])
                    Result_CH8.Image = Properties.Resources.ok2;
                else
                    Result_CH8.Image = Properties.Resources.fail2;
            else
                Result_CH8.Image = Properties.Resources.none1;
        }

        private void process2_test_result(bool t_result)
        {
            if (check_allch.Checked)
            {
                process2.Count_test += 1;
                if (t_result)
                {
                    process2.Pass_test += 1;
                    if (ResultForm.IsDisposed)
                        ResultForm = new Form1();
                    ResultForm.label1.Visible = false;
                    ResultForm.label2.Visible = true;
                    ResultForm.Visible = true;
                    ResultForm.Show();
                    label40.Text = "PASS";
                    label40.ForeColor = Color.Green;
                }
                else
                {
                    if (ResultForm.IsDisposed)
                        ResultForm = new Form1();
                    ResultForm.label1.Visible = true;
                    ResultForm.label2.Visible = false;
                    ResultForm.Visible = true;
                    ResultForm.Show();
                    label40.Text = "FAIL";
                    label40.ForeColor = Color.Red;
                }
                text_count.Text = process2.Count_test.ToString();
                text_pass.Text = process2.Pass_test.ToString();
            }
            else
            {
                richTextBox2.AppendText("Test Done...");
                richTextBox2.AppendText(Environment.NewLine);
                richTextBox2.ScrollToCaret();
                richTextBox2.Refresh();
            }

            if (check_ch1.Checked)
                if (process2.pb04.TestTable.Cal_Result[0])
                    Result_CH1.Image = Properties.Resources.ok2;
                else
                    Result_CH1.Image = Properties.Resources.fail2;
            else
                Result_CH1.Image = Properties.Resources.none1;

            if (check_ch2.Checked)
                if (process2.pb04.TestTable.Cal_Result[1])
                    Result_CH2.Image = Properties.Resources.ok2;
                else
                    Result_CH2.Image = Properties.Resources.fail2;
            else
                Result_CH2.Image = Properties.Resources.none1;

            if (check_ch3.Checked)
                if (process2.pb04.TestTable.Cal_Result[2])
                    Result_CH3.Image = Properties.Resources.ok2;
                else
                    Result_CH3.Image = Properties.Resources.fail2;
            else
                Result_CH3.Image = Properties.Resources.none1;

            if (check_ch4.Checked)
                if (process2.pb04.TestTable.Cal_Result[3])
                    Result_CH4.Image = Properties.Resources.ok2;
                else
                    Result_CH4.Image = Properties.Resources.fail2;
            else
                Result_CH4.Image = Properties.Resources.none1;

            if (check_ch5.Checked)
                if (process2.pb04.TestTable.Cal_Result[4])
                    Result_CH5.Image = Properties.Resources.ok2;
                else
                    Result_CH5.Image = Properties.Resources.fail2;
            else
                Result_CH5.Image = Properties.Resources.none1;

            if (check_ch6.Checked)
                if (process2.pb04.TestTable.Cal_Result[5])
                    Result_CH6.Image = Properties.Resources.ok2;
                else
                    Result_CH6.Image = Properties.Resources.fail2;
            else
                Result_CH6.Image = Properties.Resources.none1;

            if (check_ch7.Checked)
                if (process2.pb04.TestTable.Cal_Result[6])
                    Result_CH7.Image = Properties.Resources.ok2;
                else
                    Result_CH7.Image = Properties.Resources.fail2;
            else
                Result_CH7.Image = Properties.Resources.none1;

            if (check_ch8.Checked)
                if (process2.pb04.TestTable.Cal_Result[7])
                    Result_CH8.Image = Properties.Resources.ok2;
                else
                    Result_CH8.Image = Properties.Resources.fail2;
            else
                Result_CH8.Image = Properties.Resources.none1;
        }

        private void Show_test_result()
        {
            bool t_result;
            if(radioButton1.Checked)
            {
                t_result = process.ph04.TestTable.Report_result();
                process_test_result(t_result);
            }

            if (radioButton2.Checked)
            {
                if (checkBox3.Checked && checkBox4.Checked)
                    t_result = process2.pb04.TestTable.Report_result();
                else if (checkBox3.Checked)
                    t_result = process2.pb04.TestTable.Report_resultmA();
                else
                    t_result = process2.pb04.TestTable.Report_resultmV();
                process2_test_result(t_result);
            }


        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //if (timer3.Enabled) return;
            if (!bkgWorker_Stopwatch.IsBusy)
                bkgWorker_Stopwatch.RunWorkerAsync();
            if(radioButton1.Checked)
            {
                if (process.Is_update_msg)
                {
                    richTextBox2.AppendText(process.Upate_msg());
                    richTextBox2.AppendText(Environment.NewLine);
                    richTextBox2.ScrollToCaret();
                    richTextBox2.Refresh();

                    //progressBar1.Value = process.Calibrate_progress;
                    progressBar2.Value = process.Auto_Cal_progress;
                    progressBar3.Value = process.Calibrate_progress;
                }
                if (!process.Auto_Cal_Run)
                {
                    //Show_test_result();
                    timer2.Enabled = false;
                    if (checkBox5.Checked)
                    {             start_test();            }
                    else
                    {            button9.Enabled = true;      }

                }
            }

            if(radioButton2.Checked)
            {
                if (process2.Is_update_msg)
                {
                    richTextBox2.AppendText(process2.Upate_msg());
                    richTextBox2.AppendText(Environment.NewLine);
                    richTextBox2.ScrollToCaret();
                    richTextBox2.Refresh();

                    //progressBar1.Value = process.Calibrate_progress;
                    progressBar2.Value = process2.Auto_Cal_progress;
                    progressBar3.Value = process2.Calibrate_progress;
                }
                if (!process2.Auto_Cal_Run)
                {
                    //Show_test_result();
                    timer2.Enabled = false;
                    if (checkBox5.Checked)
                    { start_test(); }
                    else
                    { button9.Enabled = true; }

                }
            }
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //if (timer2.Enabled) return;
            if(!bkgWorker_Stopwatch.IsBusy)
                bkgWorker_Stopwatch.RunWorkerAsync();

            if(radioButton1.Checked)
            {
                if (process.Is_update_msg)
                {
                    richTextBox2.AppendText(process.Upate_msg());
                    richTextBox2.AppendText(Environment.NewLine);
                    richTextBox2.ScrollToCaret();
                    richTextBox2.Refresh();

                    //progressBar1.Value = process.Calibrate_progress;
                    progressBar2.Value = process.Auto_Cal_progress;
                    progressBar3.Value = process.Calibrate_progress;
                }
                if (!process.Auto_Cal_Run)
                {
                    timer3.Enabled = false;
                    //btn_autoCalibrate.Enabled = true;
                    //btn_autoTest.Enabled = true;
                    btn_stop_calibrate.Enabled = false;
                    button9.Enabled = true;
                    Show_test_result();
                }
            }

            if(radioButton2.Checked)
            {
                if (process2.Is_update_msg)
                {
                    richTextBox2.AppendText(process2.Upate_msg());
                    richTextBox2.AppendText(Environment.NewLine);
                    richTextBox2.ScrollToCaret();
                    richTextBox2.Refresh();

                    //progressBar1.Value = process.Calibrate_progress;
                    progressBar2.Value = process2.Auto_Cal_progress;
                    progressBar3.Value = process2.Calibrate_progress;
                }
                if (!process2.Auto_Cal_Run)
                {
                    timer3.Enabled = false;
                    //btn_autoCalibrate.Enabled = true;
                    //btn_autoTest.Enabled = true;
                    btn_stop_calibrate.Enabled = false;
                    button9.Enabled = true;
                    Show_test_result();
                }
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            process.Select_PH04_Channel = Int16.Parse(comboBox3.Text);
            process.SetInput_test(process.Select_PH04_Channel, 0);
            stopWatch.Restart();
            stopWatch.Start();
            richTextBox1.Clear();
            richTextBox1.AppendText("Start test ..\r\n");
            process.start_test_thread(checkBox1.Checked, checkBox2.Checked);
            timer1.Enabled = true;
        }
         

        void start_test()
        {
            if(radioButton1.Checked)
            {
                process.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process.Channel_need_to_calibrate[7] = check_ch8.Checked;

                Result_CH1.Image = Properties.Resources.none1;
                Result_CH2.Image = Properties.Resources.none1;
                Result_CH3.Image = Properties.Resources.none1;
                Result_CH4.Image = Properties.Resources.none1;
                Result_CH5.Image = Properties.Resources.none1;
                Result_CH6.Image = Properties.Resources.none1;
                Result_CH7.Image = Properties.Resources.none1;
                Result_CH8.Image = Properties.Resources.none1;

                process.Auto_Cal_Run = true;
                btn_stop_calibrate.Enabled = true;
                process.start_Auto_test_thread(checkBox3.Checked, checkBox4.Checked);
                timer3.Enabled = true;
            }
            if(radioButton2.Checked)
            {
                process2.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process2.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process2.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process2.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process2.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process2.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process2.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process2.Channel_need_to_calibrate[7] = check_ch8.Checked;

                Result_CH1.Image = Properties.Resources.none1;
                Result_CH2.Image = Properties.Resources.none1;
                Result_CH3.Image = Properties.Resources.none1;
                Result_CH4.Image = Properties.Resources.none1;
                Result_CH5.Image = Properties.Resources.none1;
                Result_CH6.Image = Properties.Resources.none1;
                Result_CH7.Image = Properties.Resources.none1;
                Result_CH8.Image = Properties.Resources.none1;

                process2.Auto_Cal_Run = true;
                btn_stop_calibrate.Enabled = true;
                process2.start_Auto_test_thread(checkBox3.Checked, checkBox4.Checked);
                timer3.Enabled = true;
            }

            
            //if (timer2.Enabled) timer2.Enabled = false;
        }

        private void btn_autoTest_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            start_test();
            label40.Text = "WAIT";

            if (radioButton1.Checked)
            {
                process.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process.Channel_need_to_calibrate[7] = check_ch8.Checked;
                process.Select_PH04_Channel = 0;
            }
            if (radioButton2.Checked)
            {
                process2.Channel_need_to_calibrate[0] = check_ch1.Checked;
                process2.Channel_need_to_calibrate[1] = check_ch2.Checked;
                process2.Channel_need_to_calibrate[2] = check_ch3.Checked;
                process2.Channel_need_to_calibrate[3] = check_ch4.Checked;

                process2.Channel_need_to_calibrate[4] = check_ch5.Checked;
                process2.Channel_need_to_calibrate[5] = check_ch6.Checked;
                process2.Channel_need_to_calibrate[6] = check_ch7.Checked;
                process2.Channel_need_to_calibrate[7] = check_ch8.Checked;
                process2.Select_Channel = 0;
            }


            btn_autoCalibrate.Enabled = false;
            btn_autoTest.Enabled = false;
            btn_stop_calibrate.Enabled = true;
            button9.Enabled = false;
            stopWatch.Restart();
            stopWatch.Start();
        }

        private void btn_stop_calibrate_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                process.stop_auto_cal_thread();
                process.stop_auto_test_thread();
            }
            if(radioButton2.Checked)
            {
                process2.stop_auto_cal_thread();
                process2.stop_auto_test_thread();
            }
            //btn_autoTest.Enabled = true;
            //btn_autoCalibrate.Enabled = true;
            //btn_stop_calibrate.Enabled = false;
            button9.Enabled=true;
            timer2.Enabled = false;
            timer3.Enabled = false;
        }

        private void check_ch1_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch1.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked& check_ch2.Checked& check_ch3.Checked& check_ch4.Checked& check_ch5.Checked& check_ch6.Checked& check_ch7.Checked& check_ch8.Checked;
            }

        }

        private void check_allch_CheckedChanged(object sender, EventArgs e)
        {
            if(check_allch.Checked)
            {
                check_ch1.Checked = true;
                check_ch2.Checked = true;
                check_ch3.Checked = true;
                check_ch4.Checked = true;
                check_ch5.Checked = true;
                check_ch6.Checked = true;
                check_ch7.Checked = true;
                check_ch8.Checked = true;
            }
            
        }

        private void check_ch2_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch2.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch3_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch3.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch4_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch4.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch5_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch5.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch6_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch6.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch7_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch7.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void check_ch8_CheckedChanged(object sender, EventArgs e)
        {
            if (!check_ch8.Checked)
                check_allch.Checked = false;
            else
            {
                check_allch.Checked = check_ch1.Checked & check_ch2.Checked & check_ch3.Checked & check_ch4.Checked & check_ch5.Checked & check_ch6.Checked & check_ch7.Checked & check_ch8.Checked;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button6.Enabled = true;
            comboBox2.Enabled = true;
            panel3.Enabled = true;
            if (process2.Memocal2000.IsPortOpen)
            {
                if (process2.Memocal2000.close_comport())
                {
                    LedMemocal.Image = Properties.Resources.red;
                    listBox1.Items.Add("Disconnect Memocal2000... ");
                }
                else
                {
                    MessageBox.Show("Error !! Please check Comport or restart app");
                    listBox1.Items.Add("Please check Comport & Memocal2000");
                }
            }

            try
            {
                ledpb04.Image = Properties.Resources.red;
                LED2.Image = Properties.Resources.red;
                pictureBox1.Image = Properties.Resources.red;
                process2.pb04.stop_tcp();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }

            label7.Enabled = radioButton1.Checked;
            label8.Enabled = radioButton1.Checked;
            textPHId1.Enabled = radioButton1.Checked;
            ledph04id1.Enabled = radioButton1.Checked;
            //button3_Click(sender, e);
            //button8_Click(sender, e);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //button3_Click(sender, e);
            //button8_Click(sender, e);
            panel3.Enabled = false;
            comboBox2.Enabled = false;
            button2.Enabled = true;
            button6.Enabled = true;
            if (process.Memocal2000.IsPortOpen)
            {
                if (process.Memocal2000.close_comport())
                {
                    LedMemocal.Image = Properties.Resources.red;
                    listBox1.Items.Add("Disconnect Memocal2000... ");
                }
                else
                {
                    MessageBox.Show("Error !! Please check Comport or restart app");
                    listBox1.Items.Add("Please check Comport & Memocal2000");
                }
            }

            try
            {
                if (sPort2.IsOpen)
                {
                    try
                    {
                        sPort2.Close();
                        listBox1.Items.Add("Tools disconnected...");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error !! Please check Comport or restart app" + ex.ToString());
                        listBox1.Items.Add("Error !! Please check Comport or restart app");
                    }
                }

                ledph07id1.Image = Properties.Resources.red;
                ledph07id2.Image = Properties.Resources.red;
                ledph07id3.Image = Properties.Resources.red;
                ledph04id1.Image = Properties.Resources.red;
                LED2.Image = Properties.Resources.red;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }


            label35.Enabled = radioButton2.Checked;
            pb04ip.Enabled = radioButton2.Checked;
            ledpb04.Enabled = radioButton2.Checked;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox1.Image= Properties.Resources.red;
            richTextBox2.Text = "";
            richTextBox2.AppendText("restart connection port....");
            richTextBox2.AppendText(Environment.NewLine);

            progressBar2.Value = 0;
            progressBar3.Value = 0;

            label40.Text = "----";
            label40.ForeColor = Color.Black;

            Result_CH1.Image = Properties.Resources.none1;
            Result_CH2.Image = Properties.Resources.none1;
            Result_CH3.Image = Properties.Resources.none1;
            Result_CH4.Image = Properties.Resources.none1;
            Result_CH5.Image = Properties.Resources.none1;
            Result_CH6.Image = Properties.Resources.none1;
            Result_CH7.Image = Properties.Resources.none1;
            Result_CH8.Image = Properties.Resources.none1;
            bkgNextModule.RunWorkerAsync();
            button3_Click(sender, e);       //  disconnect memocal
            button8_Click(sender, e);       //  disconnect tool and device
            button2_Click(sender, e);       //  connect memocal
            button6_Click(sender, e);       //  connect device
            
        }

        private void bkgNextModule_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int wait_count=0;
            while(wait_button6)
            {
                if(wait_count < 3)
                {
                    wait_count++;
                    Thread.Sleep(500);
                }
                else
                {
                    wait_button6 = false;
                }
            }
            this.Invoke(
                new MethodInvoker(delegate () {
                    btn_autoTest.Enabled = true;
                    btn_autoCalibrate.Enabled = true;
                    btn_stop_calibrate.Enabled = false;
                })
                );
            
            
        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void textId3_TextChanged(object sender, EventArgs e)
        {

        }

        private void bkgWork_GetPB04value_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (radioButton2.Checked)
                process2.pb04.Is_module_ready();
            this.Invoke(
                new MethodInvoker(delegate () {
                    this.AnCh1.Text = process2.pb04.AnalogCH[0].ToString();
                    this.AnCh2.Text = process2.pb04.AnalogCH[1].ToString();
                    this.AnCh3.Text = process2.pb04.AnalogCH[2].ToString();
                    this.AnCh4.Text = process2.pb04.AnalogCH[3].ToString();
                    this.AnCh5.Text = process2.pb04.AnalogCH[4].ToString();
                    this.AnCh6.Text = process2.pb04.AnalogCH[5].ToString();
                    this.AnCh7.Text = process2.pb04.AnalogCH[6].ToString();
                    this.AnCh8.Text = process2.pb04.AnalogCH[7].ToString();

                    this.TypeCh1.SelectedIndex = process2.pb04.InputType[0];
                    this.TypeCh2.SelectedIndex = process2.pb04.InputType[1];
                    this.TypeCh3.SelectedIndex = process2.pb04.InputType[2];
                    this.TypeCh4.SelectedIndex = process2.pb04.InputType[3];
                    this.TypeCh5.SelectedIndex = process2.pb04.InputType[4];
                    this.TypeCh6.SelectedIndex = process2.pb04.InputType[5];
                    this.TypeCh7.SelectedIndex = process2.pb04.InputType[6];
                    this.TypeCh8.SelectedIndex = process2.pb04.InputType[7];
                    this.richTextBox1.AppendText("Get value update form PB-04..\r\n");
                })
                );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                sPort1.PortName = comboBox1.Text;
                sPort1.BaudRate = 9600;
                sPort1.Parity = Parity.None;
                sPort1.DataBits = 8;
                sPort1.StopBits = StopBits.One;
                sPort1.Handshake = Handshake.None;
                sPort1.ReadTimeout = 500;
                sPort1.WriteTimeout = 500;
            }
            catch
            {
                if (sPort1.IsOpen)
                    sPort1.Close();
                return;
            }
            if(radioButton1.Checked)
            {
                process.Memocal2000.Set_Comport(sPort1);
                if (process.Memocal2000.Ready)
                {
                    LedMemocal.Image = Properties.Resources.green;
                    button2.Enabled = false;
                    //button3.Enabled = true;
                    listBox1.Items.Add("Memocal2000 connected..");
                }
                else
                {
                    LedMemocal.Image = Properties.Resources.red;
                    MessageBox.Show("Please check Comport & Memocal2000");
                    listBox1.Items.Add("Please check Comport & Memocal2000");
                    button2.Enabled = true;
                    //button3.Enabled = true;
                    if (process.Memocal2000.IsPortOpen)
                        try
                        {
                            sPort1.Close();
                        }
                        catch { }
                }
            }

            if (radioButton2.Checked)
            {
                process2.Memocal2000.Set_Comport(sPort1);
                if (process2.Memocal2000.Ready)
                {
                    LedMemocal.Image = Properties.Resources.green;
                    button2.Enabled = false;
                    //button3.Enabled = true;
                    listBox1.Items.Add("Memocal2000 connected..");
                }
                else
                {
                    LedMemocal.Image = Properties.Resources.red;
                    MessageBox.Show("Please check Comport & Memocal2000");
                    listBox1.Items.Add("Please check Comport & Memocal2000");
                    button2.Enabled = true;
                    //button3.Enabled = true;
                    if (process2.Memocal2000.IsPortOpen)
                        try
                        {
                            sPort1.Close();
                        }
                        catch { }
                }
            }

        }




    }
}