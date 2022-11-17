using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Vehicle_Parking_Manager_final_.Forms
{
    public partial class FormHome : Form
    {
        public static FormHome instance;

        SQLiteConnection cn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;

        //global Variables
        string license_plate_no;
        string QUERY;

        public FormHome()
        {
            InitializeComponent();
            instance = this;
        }

        private void FormHome_Load(object sender, EventArgs e)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
            //MessageBox.Show(dbPath);

            //cn = new SQLiteConnection (@"URI=file:"+Application.StartupPath+"\\Database1.db");
            cn = new SQLiteConnection($@"URI=file:{dbPath}");
            GetAllRecords();

            cn.Open();
            cmd = new SQLiteCommand("Select park_name from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string park_name = string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            label4.Text = park_name;

            cmd = new SQLiteCommand("Select address from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            da.Fill(dt2);
            string address = string.Join(Environment.NewLine, dt2.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            label5.Text = address;

            cmd = new SQLiteCommand("Select phone_number from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt3 = new DataTable();
            da.Fill(dt3);
            string phone_number = string.Join(Environment.NewLine, dt3.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            label13.Text = $"Tel : {phone_number}";
            cn.Close();

            status();
        }
        private void status()
        {
            //cn.Open();
            var cmd = new SQLiteCommand("Select parking_slots from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt4 = new DataTable();
            da.Fill(dt4);
            string parking_slots = string.Join(Environment.NewLine, dt4.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));


            cmd = new SQLiteCommand("SELECT COUNT(license_plate_no) FROM ParkingStatus", cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt5 = new DataTable();
            da.Fill(dt5);
            string used_slots = string.Join(Environment.NewLine, dt5.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            label14.Text = $"Used Parking Slots {used_slots}/{parking_slots}";
            progressBar1.Maximum = Convert.ToInt32(parking_slots);
            progressBar1.Minimum = 0;
            //progressBar1.Step = 1;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = Convert.ToInt32(used_slots);
            //cn.Close();

        }
        private void GetAllRecords()
        {
            cmd = new SQLiteCommand("Select * from ParkingStatus", cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            //cn.Close();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            license_plate_no = (textBox1.Text);

            if (license_plate_no == "")
            {
                MessageBox.Show("Invalid License Plate No");
            }

            else
            {
                //MessageBox.Show(license_plate_no);
                DateTime localDate = DateTime.Now;
                string entry_time = (localDate.ToString());
                cn.Open();
                string QUERY = "INSERT INTO ParkingStatus " +
                    "(license_plate_no,entry_time)" +
                    "VALUES (@license_plate_no,@entry_time)";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                CMD.Parameters.AddWithValue("@license_plate_no", license_plate_no);
                CMD.Parameters.AddWithValue("@entry_time", entry_time);


                try
                {
                    CMD.ExecuteNonQuery();
                    errorProvider1.SetError(textBox1, String.Empty);
                    errorProvider1.SetError(textBox4, String.Empty);
                    textBox1.Text = "";
                    AvailableSlot(license_plate_no);
                    status();
                    GetAllRecords();
                }
                catch
                {
                    errorProvider1.SetError(textBox1, "This license plate number already parked!");
                    errorProvider1.SetError(textBox4, String.Empty);
                    textBox4.Text = "";
                }
                //cn.Close();
                
            }
            cn.Close();
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string license_plate_no2 = (textBox4.Text);
            cn.Open();
            SQLiteCommand commandToChecklicense_plate_number = new SQLiteCommand("SELECT license_plate_no from ParkingStatus where license_plate_no = '" + license_plate_no2 + "'", cn);
            string o = (string)commandToChecklicense_plate_number.ExecuteScalar();
            MessageBox.Show(o);
            if (o == license_plate_no2)
            {
                cmd = new SQLiteCommand("Select entry_time from ParkingStatus where license_plate_no = '" + license_plate_no2 + "'", cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                string entry_timeStr = string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                cmd = new SQLiteCommand("Select currency from settings where Id = " + 1, cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                string currency = string.Join(Environment.NewLine, dt2.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                cmd = new SQLiteCommand("Select parking_charge from settings where Id = " + 1, cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt3 = new DataTable();
                da.Fill(dt3);
                string parking_charge = string.Join(Environment.NewLine, dt3.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                DateTime entry_time = Convert.ToDateTime(entry_timeStr);
                DateTime leaving_time = DateTime.Now;
                String Date = DateTime.Now.ToString("MM/dd/yyyy");
                double parking_time = Convert.ToDouble(leaving_time.Subtract(entry_time).TotalMinutes);
                double parking_timeRounded = (Math.Round(parking_time, 2));
                label10.Text = Convert.ToString(parking_timeRounded);
                label9.Text = license_plate_no2;
                label11.Text = ($"{currency} {parking_charge}");
                double total = (Convert.ToDouble(parking_timeRounded) * Convert.ToDouble(parking_charge));
                label12.Text = $"{currency} {(Math.Round(total, 2)).ToString()}";
                label9.Text = license_plate_no2;

                textBox4.Text = "";

                string QUERY = "Delete from ParkingStatus where license_plate_no = @license_plate_no";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                CMD.Parameters.AddWithValue("@license_plate_no", license_plate_no2);
                CMD.ExecuteNonQuery();
                errorProvider1.SetError(textBox4, String.Empty);
                errorProvider1.SetError(textBox1, String.Empty);


                //dataGridView1.DataSource = dt;

                //cn.Open();
                QUERY = "INSERT INTO Parking_History " +
                    "(License_Plate_No,Entry_Time,Date,Leaving_Time,Parking_Duration,Total_Charge)" +
                    "VALUES (@License_Plate_No,@Entry_Time,@Date,@Leaving_Time,@Parking_Duration,@Total_Charge)";
                CMD = new SQLiteCommand(QUERY, cn);
                CMD.Parameters.AddWithValue("@License_Plate_No", license_plate_no2);
                CMD.Parameters.AddWithValue("@Entry_Time", entry_time.ToString());
                CMD.Parameters.AddWithValue("@Date", Date.ToString());
                CMD.Parameters.AddWithValue("@Leaving_Time", leaving_time.ToString());
                CMD.Parameters.AddWithValue("@Parking_Duration", parking_timeRounded.ToString());
                CMD.Parameters.AddWithValue("@Total_Charge", (Math.Round(total, 2)).ToString());
                CMD.ExecuteNonQuery();
                MessageBox.Show("ok");

            }
            else
            {

                errorProvider1.SetError(textBox4, "The number you entered is incorect or the vehicle is not in the park!.");
                errorProvider1.SetError(textBox1, String.Empty);
            }
            textBox1.Text = "";
            
            

            QUERY = $"UPDATE Slot_Details SET Slot_status = 'Empty' WHERE Slot_status = '{license_plate_no2}';";
            SQLiteCommand CMD2 = new SQLiteCommand(QUERY, cn);
            CMD2.ExecuteNonQuery();
            cn.Close();
            GetAllRecords();
            status();

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void AvailableSlot(string licensePlateNo)
        {
            //int rowCount = 1;
            //cn.Open();
            QUERY = $"SELECT COUNT(*) FROM Slot_Details;";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            int totalSlots = Convert.ToInt32(CMD.ExecuteScalar());
            //MessageBox.Show(totalSlots.ToString());

            if (totalSlots == 0)
            {
                cmd = new SQLiteCommand("Select parking_slots from settings where Id = " + 1, cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                string slots = string.Join(Environment.NewLine, dt2.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                for (int i = 1; i < (Convert.ToInt32(slots)) + 1; i++)
                {
                    string QUERY3 = $"INSERT INTO Slot_Details VALUES('{i}','Empty');";
                    SQLiteCommand CMD3 = new SQLiteCommand(QUERY3, cn);
                    CMD3.ExecuteNonQuery();
                }

                QUERY = $"SELECT COUNT(*) FROM Slot_Details;";
                CMD = new SQLiteCommand(QUERY, cn);
                CMD.ExecuteNonQuery();
                totalSlots = Convert.ToInt32(CMD.ExecuteScalar());
            }



            for (int i = 1; i < (totalSlots + 1); i++)
            {
                cmd = new SQLiteCommand($"Select Slot_status from Slot_Details where Slot_Address = '{i}'", cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                string Slot_status = string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                if (Slot_status == "Empty")
                {
                    label15.Text = i.ToString();
                    QUERY = $"UPDATE Slot_Details SET Slot_status = '{licensePlateNo}' WHERE Slot_Address = '{i}';";
                    SQLiteCommand CMD2 = new SQLiteCommand(QUERY, cn);
                    CMD2.ExecuteNonQuery();
                    //Update
                    QUERY = $"UPDATE ParkingStatus SET Slot_No = '{label15.Text}' WHERE license_plate_no = '{licensePlateNo}';";
                    CMD2 = new SQLiteCommand(QUERY, cn);
                    CMD2.ExecuteNonQuery();


                    break;
                }
                else
                {
                    continue;
                }
            }
            //cn.Close();
        }
    }   
}
