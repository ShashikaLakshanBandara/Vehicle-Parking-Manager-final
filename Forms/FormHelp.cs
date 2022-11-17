using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;

namespace Vehicle_Parking_Manager_final_.Forms
{
    public partial class FormHelp : Form
    {
        PdfiumViewer.PdfViewer pdf;
        public FormHelp()
        {
            InitializeComponent();
            pdf = new PdfViewer();
            this.Controls.Add(pdf);
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {
            string filePath = (Application.StartupPath +"\\Help"+"\\Help.pdf");
            //openfile(filePath);
            axAcroPDF1.LoadFile(filePath);

        }
        /*
        public void openfile(string filepath)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filepath);
            var stream = new MemoryStream(bytes);
            PdfDocument pdfDocument = PdfDocument.Load(stream);
            pdf.Document = pdfDocument;
        }
        */
    }
}
