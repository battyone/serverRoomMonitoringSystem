using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab13App
{
    

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();          
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            
            //mendapatkan semua list port yang ada pada komputer
            String[] portList = System.IO.Ports.SerialPort.GetPortNames();
            foreach (String portName in portList)
                comboBox1.Items.Add(portName);
            comboBox1.Text = comboBox1.Items[comboBox1.Items.Count - 1].ToString();
            comboBox2.Text = comboBox2.Items[0].ToString();

            //menampilkan hari, tanggal, dan waktu pada form
            labelDate.Text = DateTime.Now.ToLongDateString();
            labelTime.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();    
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void navDashboard_Click(object sender, EventArgs e)
        {
            //warna text ketika navdashboard ditekan
            navDashboard.ForeColor = Color.FromArgb(35, 145, 255);
            navConfig.ForeColor = Color.DarkGray;
            navLog.ForeColor = Color.DarkGray;
            //keadaan panel saat navdashboard ditekan
            panelDashboard.Visible = true;
            panelConfig.Visible = false;
            panelLog.Visible = false;
            panelDashboard.BringToFront();     
        }

        private void navConfig_Click(object sender, EventArgs e)
        {
            //warna text ketika navconfig ditekan
            navDashboard.ForeColor = Color.DarkGray;
            navConfig.ForeColor = Color.FromArgb(35, 145, 255);
            navLog.ForeColor = Color.DarkGray;
            //keadaan panel saat navconfig ditekan
            panelDashboard.Visible = false;
            panelConfig.Visible = true;
            panelLog.Visible = false;
            panelConfig.BringToFront();      
        }

        private void navLog_Click(object sender, EventArgs e)
        {
            //warna text ketika navconfig ditekan
            navDashboard.ForeColor = Color.DarkGray;
            navConfig.ForeColor = Color.DarkGray;
            navLog.ForeColor = Color.FromArgb(35, 145, 255);
            //keadaan panel saat navconfig ditekan
            panelDashboard.Visible = false;
            panelConfig.Visible = false;
            panelLog.Visible = true;
            panelLog.BringToFront();
        }

        private void ucConfig1_Load(object sender, EventArgs e)
        {
           
        }

        bool btn = true;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //pengondisian 
            if (btn)
            {

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
            //data yang diterima dikirimkan pada masing masing label pada tab dashboard
            string data = serialPort1.ReadLine();
            // Invokes the delegate on the UI thread, and sends the data that was received to the invoked method 
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });

            //membaca data, data yang diterima dikirimkan ke lisbox pada tab Log
            string receivedMsg = serialPort1.ReadLine();
            Tampilkan(receivedMsg);

        }

        private delegate void TampilkanDelegate(object item);

        private void Tampilkan(object item)
        {
            if (InvokeRequired)
                // This is a worker thread so delegate the task.
                listBox1.Invoke(new TampilkanDelegate(Tampilkan), item);
            else
            {
                // This is the UI thread so perform the task.
                listBox1.Items.Add(item);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                
            }
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

            //jika ada data yang masuk maka timer2 start
            if (data.Length != 0)
            {
                timer2.Start();
            }

            //jika data temperature kurang dari atau lebih dari data set point
            if (Int32.Parse(data[1]) < Int32.Parse(textMinTemp.Text))
            {
                toolStripStatusLabel1.Text = "test kurang";
                cardTemp.BackColor = Color.Red;
            }
            else if (Int32.Parse(data[1]) > Int32.Parse(textMaxTemp.Text))
            {
                toolStripStatusLabel1.Text = "test lebih";
                cardTemp.BackColor = Color.Red;
            }
            else
            {
                toolStripStatusLabel1.Text = "test ok";
                cardTemp.BackColor = Color.White;
            }

            //jika data humidity kurang dari atau lebih dari data set point
            if (Int64.Parse(data[2]) < Int64.Parse(textMinHum.Text))
            {
                cardHum.BackColor = Color.Red;
            }
            else if (Int64.Parse(data[2]) > Int64.Parse(textMaxHum.Text))
            {
                
                cardHum.BackColor = Color.Red;
            }
            else
            {
                
                cardHum.BackColor = Color.White;
            }

            //jika radiobutton All di check
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
                
                //mengubah nilai keluaran sensor menjadi tulisan
                if (data[3] == "0")
                {
                    labelAir.Text = "Undetected";
                    cardAir.BackColor = Color.White;
                }
                else
                {
                    labelAir.Text = "Detected";
                    cardAir.BackColor = Color.Red;
                }

                //mengubah nilai keluaran sensor menjadi tulisan
                if (data[4] == "0")
                {
                    cardSmoke.BackColor = Color.White;
                    labelSmoke.Text = "Undetected";
                }
                else
                {
                    cardSmoke.BackColor = Color.Red;
                    labelSmoke.Text = "Detected";
                }

                //mengubah nilai keluaran sensor menjadi tulisan
                if (data[5] == "0")
                {
                    cardWater.BackColor = Color.White;
                    labelWater.Text = "Undetected";
                }
                else
                {
                    cardWater.BackColor = Color.Red;
                    labelWater.Text = "Detected";
                }

                //mengubah nilai keluaran sensor menjadi tulisan
                if (data[6] == "0")
                {
                    cardPower.BackColor = Color.White;
                    labelPower.Text = "Undetected";
                }
                else
                {
                    cardPower.BackColor = Color.Red;
                    labelPower.Text = "Detected";
                }
            }
            //jika radiobutton temp yang di check
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
            //jika radiobutton hum yang di check
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
            //jika radiobutton airflow yang di check
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
            //jika radiobutton smoke yang di check
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
            //jika radiobutton water leak yang di check
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
            //jika radiobutton power failure yang di check
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
            //mengaktifkan timer pada waktu yang ditampilkan oleh form
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

            //menambahkan data humidity pada pada grafik
            chartHum.Series[0].Points.AddXY(labelTime.Text, labelHum.Text);
            //jika point grafik lebih besar dari 40 maka nilai x axis akan bergerak
            if (chartHum.Series[0].Points.Count > 40)
            {
                chartHum.ChartAreas[0].AxisX.Minimum = time - 40;
                chartHum.ChartAreas[0].AxisX.Maximum = time;
            }
        }

        private void textPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        

        private void textMinTemp_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textMaxTemp_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        

        private void textMinHum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textMaxHum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Configuration Saved!";    
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //membuat save dialog control
            SaveFileDialog savefile = new SaveFileDialog();
            //mendapatkan waktu dan hari sekarang
            string time = DateTime.Now.ToLongTimeString();
            string date = DateTime.Now.ToShortDateString();
            string dt = DateTime.Now.ToString("yyyyMMddTHHmmss");
            //menyimpan file txt
            savefile.FileName = "serverRoom-"+ dt +".txt";
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                //menulis data pada file
                using (StreamWriter sw = new StreamWriter(savefile.FileName))

                    //perulangan terhadap data yang terdapat pada listbox
                    foreach (var item in listBox1.Items)
                    {
                        //data kemudian ditulis pada file txt. terdapat keterangan hari dan waktu data tersebut ditulis
                        sw.WriteLine(date + "," + time + "," +item);
                    }
            }    
                 
        }

        //membuat data table controller
        DataTable table = new DataTable();

        private string pilihFile = "";
        private void btnImport_Click(object sender, EventArgs e)
        {
           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //mendapatkan path file
                pilihFile = openFileDialog1.FileName;
                //membuat class fileinfo untuk mendapatkan informasi file
                FileInfo fileinfo = new FileInfo(pilihFile);
                //menampilkan info nama file 
                labelName.Text = fileinfo.Name;
                //mendapatkan waktu terakhir file diedit
                string lasttime = fileinfo.LastWriteTime.ToLongTimeString();
                //mendapatkan tanggal terakhir file diedit
                string lastdate = fileinfo.LastWriteTime.ToLongDateString();
                //menampilkan tanggal dan waktu terakhir file diedit
                labelModified.Text = lastdate + " " + lasttime;
                //mendapatkan waktu pembuatan file
                string timecreate = fileinfo.CreationTime.ToLongTimeString();
                //mendapatkan tanggal pembuatan file
                string datecreate = fileinfo.CreationTime.ToLongDateString();
                //menampilkan tanggal dan waktu pembuatan file
                labelCreated.Text = datecreate + " " + timecreate;

                //sumber datagrid
                dataGridView1.DataSource = table;

                //membuat colum header dari tabel
                DataColumn colId = table.Columns.Add("No", typeof(int));
                DataColumn colDatetime = table.Columns.Add("DateTime", typeof(string));
                DataColumn colTemp = table.Columns.Add("Temperature (*C)", typeof(string));
                DataColumn colHum = table.Columns.Add("Humidity (%)", typeof(string));
                DataColumn colAir = table.Columns.Add("Air Flow OFF", typeof(string));
                DataColumn colSmoke = table.Columns.Add("Smoke", typeof(string));
                DataColumn colWater = table.Columns.Add("Water Leak", typeof(string));
                DataColumn colPower = table.Columns.Add("Power Failure", typeof(string));

                //membaca semua isi file yang dipilih
                string[] lines = File.ReadAllLines(pilihFile);
                int i = 0;
                foreach (var line in lines)
                {
                    //men-split data yang ada pada file berdasarkan tanda koma
                    string[] split = line.Split(',');

                    DataRow row = table.NewRow();
                    //split data kemudian dimasukkan pada masing masing kolom yang sesuai dengan header kolom
                    row.SetField(colId, ++i);
                    row.SetField(colDatetime, split[0] + " " + split[1]);
                    row.SetField(colTemp, split[3]);
                    row.SetField(colHum, split[4]);
                    //merubah nilai dari sensor menjadi tulisan
                    if (split[5] == "0")
                    {
                        row.SetField(colAir, "Undetected");
                    }
                    else
                    {
                        row.SetField(colAir, "Detected");
                    }

                    if (split[6] == "0")
                    {
                        row.SetField(colSmoke, "Undetected");
                    }
                    else
                    {
                        row.SetField(colSmoke, "Detected");
                    }

                    if (split[7] == "0")
                    {
                        row.SetField(colWater, "Undetected");
                    }
                    else
                    {
                        row.SetField(colWater, "Detected");
                    }

                    if (split[8] == "0")
                    {
                        row.SetField(colPower, "Undetected");
                    }
                    else
                    {
                        row.SetField(colPower, "Detected");
                    }
                    //menambahkan data pada setiap baris
                    table.Rows.Add(row);
                }

            }
                
        }

    }
}
