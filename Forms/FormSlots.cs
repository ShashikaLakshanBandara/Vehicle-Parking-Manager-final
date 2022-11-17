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
using System.Text.RegularExpressions;

namespace Vehicle_Parking_Manager_final_.Forms
{
    public partial class FormSlots : Form
    {
        SQLiteConnection cn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        public FormSlots()
        {
            InitializeComponent();
        }

        
        private void FormSlots_Load(object sender, EventArgs e)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
            cn = new SQLiteConnection($@"URI=file:{dbPath}");
            cn.Open();
            cmd = new SQLiteCommand("Select * from Slot_Details", cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataGridView1.DataSource = dt;
            cn.Close();

            ProgressCircle();

        }

        private void ProgressCircle() //Update Progress Circle
        {
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
            
            guna2CircleProgressBar1.Maximum = Convert.ToInt32(parking_slots);
            guna2CircleProgressBar1.Minimum = 0;
            guna2CircleProgressBar1.Value = Convert.ToInt32(used_slots);
            label10.Text = parking_slots;
            label5.Text = used_slots;
            label4.Text = ((Convert.ToInt32(parking_slots)) - (Convert.ToInt32(used_slots))).ToString();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string licennsePlateNo = guna2TextBox1.Text;
            FilterBy("Slot_status",licennsePlateNo);

        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string SlotAddress = TextBoxValidation(guna2TextBox2.Text,"sa");
            FilterBy("Slot_Address", SlotAddress);
        }
        private void FilterBy(string column,string value)
        {
            cn.Open();
            string QUERY = $"SELECT Slot_Address, Slot_status FROM Slot_Details WHERE {column} = '{value}';";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(CMD);
            da.Fill(dt);
            DataGridView1.DataSource = dt;
            cn.Close();
        }

        public string TextBoxValidation(string text,string type)
        {
            

            if (type == "sa")
            {
                if (text == "" || text=="0" || ValidateOnlyNumbers(text))
                {
                    MessageBox.Show("Please insert a valid Slot Address!");
                    
                }



            }
            else if(type=="lpn")
            {

            }

            




            return text;
            
        }
        protected bool ValidateOnlyNumbers(string number)
        {
            MessageBox.Show(number);
            bool x = false;
            try
            {
                int i = Convert.ToInt32(number);
            }
            catch
            {
                x = true;
            }
            MessageBox.Show(x.ToString());
            return x;
        }



    }
}
