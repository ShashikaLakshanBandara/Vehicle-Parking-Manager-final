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
    public partial class FormIncome : Form
    {
        SQLiteConnection cn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        SQLiteDataAdapter da;

        public FormIncome()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (comboBox.Text == "")
            {
                MessageBox.Show("You must select a date!");
            }
            else
            {
                string totalVehicles;
                double totalIncome = 0.0;

                string date = comboBox.Text;
                string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
                cn = new SQLiteConnection($@"URI=file:{dbPath}");
                cn.Open();
                string QUERY = $"SELECT license_plate_no, Entry_Time, Leaving_Time, Parking_Duration, Total_Charge FROM Parking_History WHERE Date = '{date}';";
                SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
                CMD.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SQLiteDataAdapter da = new SQLiteDataAdapter(CMD);
                da.Fill(dt);
                DataGridView1.DataSource = dt;

                QUERY = $"SELECT COUNT(Date) FROM Parking_History WHERE Date='{date}';";
                CMD = new SQLiteCommand(QUERY, cn);
                CMD.ExecuteNonQuery();
                totalVehicles = (CMD.ExecuteScalar()).ToString();
                label4.Text = totalVehicles;

                cmd = new SQLiteCommand("Select currency from settings where Id = " + 1, cn);
                da = new SQLiteDataAdapter(cmd);
                DataTable dt2 = new DataTable();
                da.Fill(dt2);
                string currency = string.Join(Environment.NewLine, dt2.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));

                QUERY = $"SELECT SUM(Total_Charge) FROM Parking_History WHERE Date='{date}';";
                CMD = new SQLiteCommand(QUERY, cn);
                CMD.ExecuteNonQuery();
                totalIncome = Convert.ToDouble(CMD.ExecuteScalar());
                label5.Text = $"{currency} {totalIncome.ToString()}";
            }
            
        }

        private void FormIncome_Load(object sender, EventArgs e)
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dbPath = Path.Combine(dbPath, "Vehicle Parking Manager", "Database1.db");
            cn = new SQLiteConnection($@"URI=file:{dbPath}");
            cn.Open();
            string QUERY = $"SELECT Date FROM Parking_History;";
            SQLiteCommand CMD = new SQLiteCommand(QUERY, cn);
            CMD.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(CMD);
            da.Fill(dt);
            

            foreach (DataRow dr in dt.Rows)
            {
                var Date = (dr["Date"]);
                if (comboBox.Items.Contains(Date))
                {
                    continue;
                }
                else
                {
                    comboBox.Items.Add(Date);
                }
                
            }
            cn.Close();
        }
    }
}
