using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace Vehicle_Parking_Manager_final_
{
    public partial class Form1 : Form
    {
        private IconButton currentbutton;
        private Panel leftborderbtn;
        private Form currentChildForm;
        public Form1()
        {
            InitializeComponent();
            leftborderbtn = new Panel();
            leftborderbtn.Size = new Size(7, 70);
            Panel1Menu.Controls.Add(leftborderbtn);
            //Form
            this.Text = String.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                //button
                currentbutton = (IconButton)senderBtn;
                currentbutton.BackColor = Color.FromArgb(37, 36, 81);
                currentbutton.ForeColor = color;
                currentbutton.TextAlign = ContentAlignment.MiddleCenter;
                currentbutton.IconColor = color;
                currentbutton.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentbutton.ImageAlign = ContentAlignment.MiddleRight;

                leftborderbtn.BackColor = color;
                leftborderbtn.Location = new Point(0, currentbutton.Location.Y);
                leftborderbtn.Visible = true;
                leftborderbtn.BringToFront();

                //icon current child form
                iconCurrentChildForm.IconChar = currentbutton.IconChar;
                iconCurrentChildForm.IconColor = color;
                
            }

        }
        private void DisableButton()
        {
            if (currentbutton != null)
            {
                currentbutton.BackColor = Color.FromArgb(23, 23, 22);
                currentbutton.ForeColor = Color.WhiteSmoke;
                currentbutton.TextAlign = ContentAlignment.MiddleLeft;
                currentbutton.IconColor = Color.WhiteSmoke;
                currentbutton.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentbutton.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                //open only form
                currentChildForm.Close();

            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitleChildForm.Text = childForm.Text;
        }
        
        private struct RGBColors
        {
            public static Color color1 = Color.FromArgb(10, 243, 255);
            public static Color color2 = Color.FromArgb(10, 243, 255);
            public static Color color3 = Color.FromArgb(10, 243, 255);
            public static Color color4 = Color.FromArgb(10, 243, 255);
            public static Color color5 = Color.FromArgb(10, 243, 255);
            public static Color color6 = Color.FromArgb(10, 243, 255);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            OpenChildForm(new Forms.FormHome());
        }

        private void btnSlots_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            OpenChildForm(new Forms.FormSlots());
        }

        private void btnIncome_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            OpenChildForm(new Forms.FormIncome());
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color4);
            OpenChildForm(new Forms.FormSettings());
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
            OpenChildForm(new Forms.FormHelp());
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color6);
            OpenChildForm(new Forms.FormAbout());
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            Reset();
            pictureBox1.BringToFront();
        }

        private void Reset()
        {
            DisableButton();
            leftborderbtn.Visible = false;

            //icon current child form
            iconCurrentChildForm.IconChar = IconChar.Cab;
            iconCurrentChildForm.IconColor = Color.WhiteSmoke;
            lblTitleChildForm.Text = "Vehicle Parking Manager";
        }

        //Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int IParam);

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pictureBoxLogo_MouseHover(object sender, EventArgs e)
        {
            pictureBoxLogo.Size = new System.Drawing.Size(125, 125);
        }

        private void pictureBoxLogo_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxLogo.Size = new System.Drawing.Size(115, 115);
        }
    }
}
