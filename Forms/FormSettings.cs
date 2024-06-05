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
using System.Data.SQLite;

namespace Vehicle_Parking_Manager_final_.Forms
{
    public partial class FormSettings : Form
    {
        public static FormSettings instance;

        SQLiteConnection cn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;
        string parking_slots;
        public FormSettings()
        {
            InitializeComponent();
            instance = this;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
            //MessageBox.Show(dbPath);

            //cn = new SQLiteConnection (@"URI=file:"+Application.StartupPath+"\\Database1.db");
            cn = new SQLiteConnection($@"URI=file:{dbPath}");
            cn.Open();

            textBox1.Text = ValueFromTable("park_name", "settings", "Id", "1", "int");
            textBox4.Text = ValueFromTable("parking_charge", "settings", "Id", "1", "int");
            textBox2.Text = ValueFromTable("address", "settings", "Id", "1", "int");
            textBox3.Text = ValueFromTable("phone_number", "settings", "Id", "1", "int");
            textBox5.Text = ValueFromTable("parking_slots", "settings", "Id", "1", "int");
            parking_slots = textBox5.Text;
            textBox6.Text = ValueFromTable("currency", "settings", "Id", "1", "int");
            //cn.Close();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            string park_name = TextBoxValidation(textBox1.Text, "name");
            string address = TextBoxValidation(textBox2.Text, "add");
            string phone_number = TextBoxValidation(textBox3.Text, "pno");
            string parking_charge = TextBoxValidation(textBox4.Text, "pcharge");
            string currency = TextBoxValidation(textBox6.Text, "cr");
            string slots = TextBoxValidation(textBox5.Text, "slo");

            if (park_name== "notValid" || address == "notValid" || phone_number== "notValid" || parking_charge == "notValid" || currency== "currency" || slots== "notValid")
            {
                MessageBox.Show("Fix errors first!");
            }
            else
            {
                if (parking_slots != textBox5.Text)
                {
                    //string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
                    //cn = new SQLiteConnection($@"URI=file:{dbPath}");
                    DeleteAllRecords("Slot_Details");

                    for (int i = 1; i < (Convert.ToInt32(textBox5.Text)) + 1; i++)
                    {
                        string QUERY3 = $"INSERT INTO Slot_Details VALUES('{i}','Empty');";
                        SQLiteCommand CMD3 = new SQLiteCommand(QUERY3, cn);
                        CMD3.ExecuteNonQuery();
                    }
                    //cn.Close();
                }
                parking_slots = textBox5.Text;
                string QUERY = $"UPDATE settings SET Id={1},parking_charge='{parking_charge}',address = '{address}',phone_number='{phone_number}',park_name='{park_name}',parking_slots='{parking_slots}',currency ='{currency}' WHERE Id = {1}";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                CMD.ExecuteNonQuery();


                MessageBox.Show("Changes saved! Restarting now.");
                Application.Restart();
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure? This can't be undo!","Confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if (res== DialogResult.Yes)
            {

                DeleteAllRecords("ParkingStatus");
                DeleteAllRecords("Parking_History");
                DeleteAllRecords("Slot_Details");
                MessageBox.Show("Reset completed!");
            }
            

        }

        private void DeleteAllRecords(string tblName)
        {
            string QUERY2 = $"DELETE FROM '{tblName}';";
            SQLiteCommand CMD2 = new SQLiteCommand(QUERY2, cn);
            CMD2.ExecuteNonQuery();
        }

        protected string ValueFromTable(string column, string tname, string condi, string con, string contype)
        {
            if (contype == "int")
            {

                string QUERY = $"Select {column} from {tname} where {condi} = {Convert.ToInt32(con)};";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                da = new SQLiteDataAdapter(CMD);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

            }
            else if (contype == "str")
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

        private void iconButton2_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure? This can't be undo!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {

                textBox1.Text = ("Park Name");
                textBox4.Text = ("0");
                textBox2.Text = ("Address");
                textBox3.Text = ("0000000000");
                textBox5.Text = ("00");
                textBox6.Text = ("LKR");

                button1_Click_1(sender, e);
                iconButton1_Click(sender, e);

            }
            


        }

        private void textBox5_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Don't update this while vehicles parked!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
        }
        protected bool ValidateOnlyNumbers(string number)
        {
            bool x = false;
            try
            {
                Int64 i = Convert.ToInt64(number);
            }
            catch
            {
                x = true;
            }
            return x;
        }

        
        public string TextBoxValidation(string text, string type)
        {


            if (type == "name")
            {
                
                if (text == "")
                {
                    errorProvider1.SetError(textBox1,"The input field couldn't be empty!");
                    text = "notValid";

                }
                else
                {
                    errorProvider1.SetError(textBox1, String.Empty);
                }
                return text;
            }
            else if (type == "pno")
            {

                if (text == "")
                {
                    errorProvider1.SetError(textBox3, "The input field couldn't be empty!");
                    text = "notValid";

                }
                else if (ValidateOnlyNumbers(text))
                {
                    errorProvider1.SetError(textBox3, "The Phone number cannot include Characters or special characters!");
                    text = "notValid";
                }
                else if (10 != text.Length)
                {
                    errorProvider1.SetError(textBox3, "The input field not in the right length!");
                    text = "notValid";
                }
                else if (ValidateSpecialCharacter(text))
                {
                    errorProvider1.SetError(textBox3, "Phone Number cannot include Special Charaters!");
                    text = "notValid";
                }
                else
                {
                    errorProvider1.SetError(textBox3, String.Empty);
                }
                return text;
            }
            else if (type == "add")
            {
                if (text == "")
                {
                    errorProvider1.SetError(textBox2, "The input field couldn't be empty!");
                    text = "notValid";

                }
                else
                {
                    errorProvider1.SetError(textBox2, String.Empty);
                }
                return text;
            }
            else if (type == "pcharge")
            {
                if (text == "")
                {
                    errorProvider1.SetError(textBox4, "The input field couldn't be empty!");
                    text = "notValid";

                }
                else if (ValidateSpecialCharacter(text))
                {
                    errorProvider1.SetError(textBox4, "Parking charges cannot include Special Charaters!");
                    text = "notValid";
                }
                else if (ValidateOnlyNumbers(text))
                {
                    errorProvider1.SetError(textBox4, "The Slot Address cannot include Characters!");
                    text = "notValid";
                }
                else
                {
                    errorProvider1.SetError(textBox4, String.Empty);
                }
                return text;
            }
            else if(type == "slo")
            {
                if (text == "")
                {
                    errorProvider1.SetError(textBox5, "The input field couldn't be empty!");
                    text = "notValid";

                }
                else if (ValidateSpecialCharacter(text))
                {
                    errorProvider1.SetError(textBox5, "Parking charges cannot include Special Charaters!");
                    text = "notValid";
                }
                else if (ValidateOnlyNumbers(text))
                {
                    errorProvider1.SetError(textBox5, "The Slot Address cannot include Characters!");
                    text = "notValid";
                }
                else
                {
                    errorProvider1.SetError(textBox5, String.Empty);
                }
                return text;
            }
            else
            {
                if (text == "")
                {
                    errorProvider1.SetError(textBox6, "The input field couldn't be empty!");
                    text = "notValid";

                }
                else
                {
                    errorProvider1.SetError(textBox6, String.Empty);
                }
                return text;
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox3.Text, "pno");
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox1.Text, "name");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox2.Text, "add");
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox4.Text, "pcharge");
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox5.Text, "slo");
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            TextBoxValidation(textBox5.Text, "cr");
        }
    }
}
