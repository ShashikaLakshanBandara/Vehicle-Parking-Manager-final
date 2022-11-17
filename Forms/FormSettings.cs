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

            cmd = new SQLiteCommand("Select park_name from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string park_name = string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox1.Text = park_name;

            cmd = new SQLiteCommand("Select parking_charge from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt2 = new DataTable();
            da.Fill(dt2);
            string parking_charges = string.Join(Environment.NewLine, dt2.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox4.Text = parking_charges;

            cmd = new SQLiteCommand("Select address from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt3 = new DataTable();
            da.Fill(dt3);
            string address = string.Join(Environment.NewLine, dt3.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox2.Text = address;

            cmd = new SQLiteCommand("Select phone_number from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt4 = new DataTable();
            da.Fill(dt4);
            string phone_number = string.Join(Environment.NewLine, dt4.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox3.Text = phone_number;

            cmd = new SQLiteCommand("Select parking_slots from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt5 = new DataTable();
            da.Fill(dt5);
            parking_slots = string.Join(Environment.NewLine, dt5.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox5.Text = parking_slots;

            cmd = new SQLiteCommand("Select currency from settings where Id = " + 1, cn);
            da = new SQLiteDataAdapter(cmd);
            DataTable dt6 = new DataTable();
            da.Fill(dt6);
            string currency = string.Join(Environment.NewLine, dt6.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            textBox6.Text = currency;
            cn.Close();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string park_name = textBox1.Text;
            string address = textBox2.Text;
            string phone_number = textBox3.Text;
            string parking_charge = textBox4.Text;
            string currency = textBox6.Text;

            if (parking_slots == textBox5.Text)
            {
                
            }
            else
            {
                string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
                cn = new SQLiteConnection($@"URI=file:{dbPath}");
                cn.Open();

                string QUERY2 = $"DELETE FROM Slot_Details;";
                SQLiteCommand CMD2 = new SQLiteCommand(QUERY2, cn);
                CMD2.ExecuteNonQuery();

                for (int i =1; i < (Convert.ToInt32(textBox5.Text))+1; i++)
                {
                    string QUERY3 = $"INSERT INTO Slot_Details VALUES('{i}','Empty');";
                    SQLiteCommand CMD3 = new SQLiteCommand(QUERY3, cn);
                    CMD3.ExecuteNonQuery();
                }
                cn.Close();


            }
            cn.Open();
            parking_slots = textBox5.Text;
            string QUERY = "UPDATE settings " +
                "SET Id = @Id,parking_charge = @parking_charge,address = @address,phone_number = @phone_number," +
                "park_name = @park_name,parking_slots = @parking_slots,currency = @currency " +
                "WHERE Id = " + 1;
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.Parameters.AddWithValue("@Id", 1);
            CMD.Parameters.AddWithValue("@parking_charge", parking_charge);
            CMD.Parameters.AddWithValue("@address", address);
            CMD.Parameters.AddWithValue("@phone_number", phone_number);
            CMD.Parameters.AddWithValue("@park_name", park_name);
            CMD.Parameters.AddWithValue("@parking_slots", parking_slots);
            CMD.Parameters.AddWithValue("@currency", currency);
            CMD.ExecuteNonQuery();


            MessageBox.Show("saved! Program is restarting now.");
            //this.Close();
            cn.Close();
            Application.Restart();
        }
    }
}
