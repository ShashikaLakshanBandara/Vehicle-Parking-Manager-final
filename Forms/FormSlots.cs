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
            ViewAllRecords();
            ProgressCircle();
        }

        private void ProgressCircle() //Update Progress Circle
        {
            cn.Open();
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
            cn.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string licennsePlateNo = TextBoxValidation(guna2TextBox1.Text,"lpn");
            
            if (licennsePlateNo != "notValid")
            {
                FilterBy("Slot_status", licennsePlateNo);
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string SlotAddress = TextBoxValidation(guna2TextBox2.Text,"sa");
            if (SlotAddress != "notValid")
            {
                FilterBy("Slot_Address", SlotAddress);
            }
            
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
                string totalSlots = CountAllTableRows("Slot_Details");
                cn.Close();
                if (text == "")
                {
                    MessageBox.Show("The input field couldn't be empty!","Validation Failed",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    text = "notValid";
                    
                }
                else if (text == "0")
                {
                    MessageBox.Show("There is not a Slot Address as 0!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (ValidateOnlyNumbers(text))
                {
                    MessageBox.Show("The Slot Address cannot include Characters!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (ValidateSpecialCharacter(text))
                {
                    MessageBox.Show("The Slot Address cannot include Special Charaters!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (Int32.Parse(text) > Int32.Parse(totalSlots))
                {
                    MessageBox.Show($"You have only {totalSlots} slots in your park!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                return text;


            }
            else if(type=="lpn")
            {
                char[] myArray = text.ToCharArray();
                
                if (text == "")
                {
                    MessageBox.Show("The input field couldn't be empty!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";

                }
                else if (9 != text.Length)
                {
                    MessageBox.Show("The license plate number hasn't valid length!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (ValidateSpecialCharacter(text))
                {
                    MessageBox.Show("The license plate number cannot include Special Charaters!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (!char.IsLetter(myArray[0]) || !char.IsLetter(myArray[1]) || !char.IsLetter(myArray[2]) || !char.IsLetter(myArray[3]) || !char.IsLetter(myArray[4]))
                {
                    MessageBox.Show("The license plate number format is not valid!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                else if (!char.IsDigit(myArray[5]) || !char.IsDigit(myArray[6]) || !char.IsDigit(myArray[7]) || !char.IsDigit(myArray[8]))
                {
                    MessageBox.Show("The license plate number format is not valid!", "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    text = "notValid";
                }
                return text;
            }
            return text;
            
        }
        protected bool ValidateOnlyNumbers(string number)
        {
            bool x = false;
            try
            {
                int i = Convert.ToInt32(number);
            }
            catch
            {
                x = true;
            }
            return x;
        }
        protected bool ValidateSpecialCharacter(string number)
        {
            bool x = false;
            char[] numberArray = number.ToCharArray();
            for (int i = 0; i < numberArray.Length; i++)
            {
                if (!char.IsLetterOrDigit(numberArray[i]))
                {
                    x = true;
                    break;
                }
            }
            return x;
        }
        protected string CountAllTableRows(string tblname)
        {
            cn.Open();
            string QUERY = $"SELECT COUNT(*) FROM {tblname};";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            return (CMD.ExecuteScalar()).ToString();
        }

        private void guna2TextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            guna2TextBox2.Text = "";
        }

        private void guna2TextBox2_MouseClick(object sender, MouseEventArgs e)
        {
            guna2TextBox1.Text = "";
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ViewAllRecords();
            ProgressCircle();
        }

        private void ViewAllRecords()
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

            guna2TextBox1.Text = "";
            guna2TextBox2.Text = "";

        }

        private void guna2Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            ResetSlot();
        }

        private void ResetSlot()
        {
            guna2TextBox1.Text = "";
            guna2TextBox2.Text = "";
        }

        private void DataGridView1_MouseHover(object sender, EventArgs e)
        {
            guna2Panel2.BorderColor = Color.FromArgb(94, 148, 255);
        }

        private void DataGridView1_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel2.BorderColor = Color.FromArgb(10, 243, 255);
        }
    }
}
