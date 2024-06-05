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
            cn = new SQLiteConnection($@"URI=file:{dbPath}");
            try
            {
                cn.Open();
            }
            catch
            {
                MessageBox.Show("Database file not found!");
            }
            

            GetAllRecords();

            label4.Text = ValueFromTable("park_name", "settings", "Id", "1","int");
            label5.Text = ValueFromTable("address", "settings", "Id", "1", "int");
            label13.Text = $"Tel : {ValueFromTable("phone_number", "settings", "Id", "1", "int")}";

            status();
        }
        private void status()
        {
            string parking_slots = ValueFromTable("parking_slots", "settings", "Id", "1", "int");
            string used_slots = CountAllTableRows("ParkingStatus");
            label14.Text = $"Used Parking Slots {used_slots}/{parking_slots}";
            progressBar1.Maximum = Convert.ToInt32(parking_slots);
            progressBar1.Minimum = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Value = Convert.ToInt32(used_slots);

        }
        private void GetAllRecords()
        {
            string query = $"Select * from ParkingStatus";
            SQLiteCommand CMD = new SQLiteCommand(query, cn);
            da = new SQLiteDataAdapter(CMD);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string license_plate_no = TextBoxValidation(textBox1.Text);

            if (license_plate_no != "notValid")
            {
                DateTime localDate = DateTime.Now;
                string entry_time = (localDate.ToString());
                string QUERY = "INSERT INTO ParkingStatus " +
                    "(license_plate_no,entry_time)" +
                    "VALUES (@license_plate_no,@entry_time)";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                CMD.Parameters.AddWithValue("@license_plate_no", license_plate_no);
                CMD.Parameters.AddWithValue("@entry_time", entry_time);

                try
                {
                    CMD.ExecuteNonQuery();
                    textBox1.Text = "";
                    AvailableSlot(license_plate_no); //insert plate numer to available space
                    status();
                    GetAllRecords();
                }
                catch
                {
                    MessageBox.Show(textBox1, "This license plate number already parked!");
                }
            }
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string license_plate_no2 = TextBoxValidation(textBox4.Text);
            string o = ValueFromTable("license_plate_no", "ParkingStatus", "license_plate_no", license_plate_no2, "str");
            if (license_plate_no2 != "notValid")
            {
                if (o == license_plate_no2)
                {
                    DateTime entry_time = Convert.ToDateTime(ValueFromTable("entry_time", "ParkingStatus", "license_plate_no", license_plate_no2, "str"));
                    string currency = ValueFromTable("currency", "settings", "Id", "1", "int");
                    string parking_charge = ValueFromTable("parking_charge", "settings", "Id", "1", "int");

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

                    string QUERY = $"Delete from ParkingStatus where license_plate_no = '{license_plate_no2}';";
                    SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                    CMD.ExecuteNonQuery();

                    QUERY = $"INSERT INTO Parking_History VALUES('{license_plate_no2}', '{entry_time.ToString()}', '{Date.ToString()}','{leaving_time.ToString()}','{parking_timeRounded.ToString()}',{Convert.ToDouble(Math.Round(total, 2))});";
                    CMD = new SQLiteCommand(QUERY, cn);
                    CMD.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("The number you entered is incorect or the vehicle is not in the park!.");
                }
                textBox1.Text = "";

                QUERY = $"UPDATE Slot_Details SET Slot_status = 'Empty' WHERE Slot_status = '{license_plate_no2}';";
                SQLiteCommand CMD2 = new SQLiteCommand(QUERY, cn);
                CMD2.ExecuteNonQuery();
                GetAllRecords();
                status();
            }
            
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
            QUERY = $"SELECT COUNT(*) FROM Slot_Details;";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            int totalSlots = Convert.ToInt32(CMD.ExecuteScalar());

            if (totalSlots == 0) //create parking slots table if its empty!
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
                    //Update parkinng status for slots
                    label15.Text = i.ToString();
                    QUERY = $"UPDATE Slot_Details SET Slot_status = '{licensePlateNo}' WHERE Slot_Address = '{i}';";
                    SQLiteCommand CMD2 = new SQLiteCommand(QUERY, cn);
                    CMD2.ExecuteNonQuery();
                    //Update parkinng status for home
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
        }
        protected string ValueFromTable(string column,string tname,string condi,string con,string contype)
        {
            if (contype=="int")
            {

                string QUERY = $"Select {column} from {tname} where {condi} = {Convert.ToInt32(con)};";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                da = new SQLiteDataAdapter(CMD);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
                
            }
            else if(contype == "str")
            {
                string QUERY = $"Select {column} from {tname} where {condi} = '{con}';";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                da = new SQLiteDataAdapter(CMD);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            }
            else
            {
                return "error";
            }

            
        }
        protected string CountAllTableRows(string tblname)
        {
            string QUERY = $"SELECT COUNT(*) FROM {tblname};";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            return (CMD.ExecuteScalar()).ToString();
        }

        public string TextBoxValidation(string text)
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

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox4.Text = "";
        }

        private void textBox4_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox4.Text = row.Cells[0].Value.ToString();
            }
            textBox1.Text = "";
        }
        private void ResetForm()
        {
            textBox1.Text = "";
            textBox4.Text = "";

            label9.Text = "***";
            label10.Text = "***";
            label11.Text = "***";
            label12.Text = "***";
            label15.Text = "**********";
            GetAllRecords();
            status();
        }

        private void childDesktop_MouseClick(object sender, MouseEventArgs e)
        {
            ResetForm();
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)

            {
                button1_Click_1(sender, e);
            }
        }
    }   
}
