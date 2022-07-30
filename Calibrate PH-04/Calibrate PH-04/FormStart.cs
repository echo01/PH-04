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
        public Stopwatch stopWatch = new Stopwatch();
        //memocal Memocal2000 = new memocal();
        //public PH04 ph04 = new PH04();
        //public PH07 ph07id2 = new PH07();
        //public PH07 ph07id3 = new PH07();
        //public PH07 ph07id4 = new PH07();
        CalibrateProcess process = new();

        public FormStart()
        {
            InitializeComponent();
            Get_compoart_list();
            sPort1 = new SerialPort();
            sPort2 = new SerialPort();

        }

        private void Get_compoart_list()
        {
            string[] ArrayComPortsNames = null;
            int index = -1;
            string ComPortName = null;
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
            button2.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
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
                } 
            else
                {
                LED2.Image = Properties.Resources.red;
                return;
                }
                              
            button6.Enabled = false;
            button8.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
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
                    MessageBox.Show("Error !! Please check Comport or restart app");
                    listBox1.Items.Add("Error !! Please check Comport or restart app");
                }
            }

            ledph07id1.Image = Properties.Resources.red;
            ledph07id2.Image = Properties.Resources.red;
            ledph07id3.Image = Properties.Resources.red;
            ledph04id1.Image = Properties.Resources.red;
            LED2.Image = Properties.Resources.red;

            button6.Enabled = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //process.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
            //process.Select_PH04_Channel= Int16.Parse(comboBox12.Text);
            //process.SetInput_test(process.Select_PH04_Channel, 1);
            backgroundWorker_manualmA.RunWorkerAsync();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //process.Memocal2000.Memocal_Gen_mV(int.Parse(textBox1.Text));
            //process.Select_PH04_Channel = Int16.Parse(comboBox3.Text);
            //process.SetInput_test(process.Select_PH04_Channel, 1);
            backgroundWorker_manualmV.RunWorkerAsync();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            bkgWorker_GetPH04value.RunWorkerAsync();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            /*process.ph04.InputType[0] = ((short)TypeCh1.SelectedIndex);
            process.ph04.InputType[1] = ((short)TypeCh2.SelectedIndex);
            process.ph04.InputType[2] = ((short)TypeCh3.SelectedIndex);
            process.ph04.InputType[3] = ((short)TypeCh4.SelectedIndex);
            process.ph04.InputType[4] = ((short)TypeCh5.SelectedIndex);
            process.ph04.InputType[5] = ((short)TypeCh6.SelectedIndex);
            process.ph04.InputType[6] = ((short)TypeCh7.SelectedIndex);
            process.ph04.InputType[7] = ((short)TypeCh8.SelectedIndex);
            List<int> update_reg = new List<int>();
            update_reg.Add(process.ph04.InputType[0]);
            update_reg.Add(process.ph04.InputType[1]);
            update_reg.Add(process.ph04.InputType[2]);
            update_reg.Add(process.ph04.InputType[3]);
            update_reg.Add(process.ph04.InputType[4]);
            update_reg.Add(process.ph04.InputType[5]);
            update_reg.Add(process.ph04.InputType[6]);
            update_reg.Add(process.ph04.InputType[7]);
            process.ph04.write_input_type(update_reg);
            button11_Click(sender,e);*/
            bkgWorker_WRPH04.RunWorkerAsync();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // set channel to calibrate
            process.Select_PH04_Channel= Int16.Parse(comboBox3.Text);
            process.SetInput_test(process.Select_PH04_Channel,0);
            stopWatch.Restart();
            stopWatch.Start();
            richTextBox1.Clear();
            richTextBox1.AppendText("Start Calibrate ..\r\n");
            process.start_calibrate_thread(checkBox1.Checked,checkBox2.Checked);
            timer1.Enabled = true;
            
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

                this.labelStopwatch.Invoke(new MethodInvoker(delegate () { labelStopwatch.Text = elapsedTime; }));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bkgWorker_Stopwatch.RunWorkerAsync();
            if(process.Is_update_msg)
            {
                richTextBox1.AppendText(process.Upate_msg());
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();

                progressBar1.Value = process.Calibrate_progress;
            }
            if(!process.Calibrate_thread_run)
            {
                timer1.Enabled = false;
            }
        }

        private void backgroundWorker_manualmA_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            process.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
            this.Invoke(new MethodInvoker(delegate () {
                process.Select_PH04_Channel = Int16.Parse(comboBox12.Text);
            }));
            
            process.SetInput_test(process.Select_PH04_Channel, 1);
        }

        private void backgroundWorker_manualmV_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            process.Memocal2000.Memocal_Gen_mA(int.Parse(textBox1.Text));
            this.Invoke(new MethodInvoker(delegate () {
                process.Select_PH04_Channel = Int16.Parse(comboBox12.Text);
            }));       
            process.SetInput_test(process.Select_PH04_Channel, 1);
        }

        private void bkgWorker_GetPH04value_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
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
                    process.ph04.write_input_type(update_reg);
                    this.richTextBox1.AppendText("Write udate value to PH-04..\r\n");

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
    }
}