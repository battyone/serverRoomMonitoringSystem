using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab13App
{
    public partial class ucDashboard : UserControl
    {
        

        public ucDashboard()
        {
            InitializeComponent();
            
        }


        
        
        

        private void ucDashboard_Load(object sender, EventArgs e)
        {
            //mendapatkan semua list port yang ada pada komputer
            String[] portList = System.IO.Ports.SerialPort.GetPortNames();
            foreach (String portName in portList)
                comboBox1.Items.Add(portName);
            comboBox1.Text = comboBox1.Items[comboBox1.Items.Count - 1].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();

            labelDate.Text = DateTime.Now.ToLongDateString();
            labelTime.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();

           
            
           

        }

       

        bool btn = true;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //pengondisian 
            if (btn)
            {
                
                //keadaan button ketika tombol connect ditekan
                //membuka koneksi port
                btnConnect.Text = "Disconnect";
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Int32.Parse(comboBox2.Text);
                    serialPort1.NewLine = "\r\n";
                    serialPort1.Open();
                    toolStripStatusLabel1.Text = serialPort1.PortName + " is connected.";
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "ERROR: " + ex.Message.ToString();
                }
                btn = false;
            }
            else
            {
                timer2.Stop();
                //menutup koneksi port
                btnConnect.Text = "Connect";
                serialPort1.Close();
                toolStripStatusLabel1.Text = serialPort1.PortName + " is closed.";
                btn = true;
            }

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadLine();
            // Invokes the delegate on the UI thread, and sends the data that was received to the invoked method 
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
        }

        // delegate is used to write to a UI control from a non-UI thread  
        private delegate void SetTextDeleg(string text);

        private void si_DataReceived(string data)
        {
            splitData(data);
            
        }
       

        private void splitData(object item)
        {
            //data yang diterima di pisah dengan method split sehingga berbentuk array
            String[] data = item.ToString().Split(',');
            //data array yang sudah dipisah diambil berdasarkan index dan ditampilkan masing masing

            if (data.Length != 0)
            {
                timer2.Start();
            }

            if (radioAll.Checked == true)
            {
                cardTemp.Enabled = true;
                cardHum.Enabled = true;
                cardAir.Enabled = true;
                cardSmoke.Enabled = true;
                cardWater.Enabled = true;
                cardPower.Enabled = true;
                labelTemp.Text = data[1]; // textbox untuk data suhu
                labelHum.Text = data[2]; // textbox untuk data kelembaban
                //labelAir.Text = data[]; // textbox untuk data aliran udara
                if (data[3] == "0")
                {
                    labelAir.Text = "Undetected";
                }
                else
                {
                    labelAir.Text = "Detected";
                }

                if (data[4] == "0")
                {
                    labelSmoke.Text = "Undetected";
                }
                else
                {
                    labelSmoke.Text = "Detected";
                }

                if (data[5] == "0")
                {
                    labelWater.Text = "Undetected";
                }
                else
                {
                    labelWater.Text = "Detected";
                }

                if (data[6] == "0")
                {
                    labelPower.Text = "Undetected";
                }
                else
                {
                    labelPower.Text = "Detected";
                }
            }
            else if (radioTemp.Checked == true)
            {
                labelTemp.Text = data[1];
                cardTemp.Enabled = true;
                cardHum.Enabled = false;
                cardAir.Enabled = false;
                cardSmoke.Enabled = false;
                cardWater.Enabled = false;
                cardPower.Enabled = false;
            }
            else if (radioHum.Checked == true)
            {
                
                labelHum.Text = data[2];
                cardTemp.Enabled = false;
                cardHum.Enabled = true;
                cardAir.Enabled = false;
                cardSmoke.Enabled = false;
                cardWater.Enabled = false;
                cardPower.Enabled = false;
            }
            else if (radioAir.Checked == true)
            {
                cardTemp.Enabled = false;
                cardHum.Enabled = false;
                cardAir.Enabled = true;
                cardSmoke.Enabled = false;
                cardWater.Enabled = false;
                cardPower.Enabled = false;
                if (data[3] == "0")
                {
                    labelAir.Text = "Undetected";
                }
                else
                {
                    labelAir.Text = "Detected";
                }
            }
            else if (radioSmoke.Checked == true)
            {
                cardTemp.Enabled = false;
                cardHum.Enabled = false;
                cardAir.Enabled = false;
                cardSmoke.Enabled = true;
                cardWater.Enabled = false;
                cardPower.Enabled = false;
                if (data[4] == "0")
                {
                    labelSmoke.Text = "Undetected";
                }
                else
                {
                    labelSmoke.Text = "Detected";
                }
            }
            else if (radioWater.Checked == true)
            {
                cardTemp.Enabled = false;
                cardHum.Enabled = false;
                cardAir.Enabled = false;
                cardSmoke.Enabled = false;
                cardWater.Enabled = true;
                cardPower.Enabled = false;
                if (data[5] == "0")
                {
                    labelWater.Text = "Undetected";
                }
                else
                {
                    labelWater.Text = "Detected";
                }
            }
            else if (radioPower.Checked == true)
            {
                cardTemp.Enabled = false;
                cardHum.Enabled = false;
                cardAir.Enabled = false;
                cardSmoke.Enabled = false;
                cardWater.Enabled = false;
                cardPower.Enabled = true;
                if (data[6] == "0")
                {
                    labelPower.Text = "Undetected";
                }
                else
                {
                    labelPower.Text = "Detected";
                }
            }
        }

       
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();

            

        }

        private int time = 0; 
        private void timer2_Tick(object sender, EventArgs e)
        {
            //menambahkan data temperature pada pada grafik
            chartTemp.Series[0].Points.AddXY(labelTime.Text, labelTemp.Text);
            
            time++;
            //jika point grafik lebih besar dari 40 maka nilai x axis akan bergerak
            if (chartTemp.Series[0].Points.Count > 40)
            {
                chartTemp.ChartAreas[0].AxisX.Minimum = time - 40;
                chartTemp.ChartAreas[0].AxisX.Maximum = time;
            }

            //menambahkan data temperature pada pada grafik
            
            chartHum.Series[0].Points.AddXY(labelTime.Text, labelHum.Text);
            //jika point grafik lebih besar dari 40 maka nilai x axis akan bergerak
            if (chartHum.Series[0].Points.Count > 40)
            {
                chartHum.ChartAreas[0].AxisX.Minimum = time - 40;
                chartHum.ChartAreas[0].AxisX.Maximum = time;
            }
        }




    }
}
