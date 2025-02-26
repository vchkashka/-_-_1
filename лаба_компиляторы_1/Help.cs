using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace лаба_компиляторы_1
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            try
            {
                AppSettings.ApplyFontSizeToControls(this.Controls);

                string resourcesFolder = Path.Combine(Application.StartupPath, "..", "..", "Properties", "resources");
                string filePath = null;

                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru")
                    filePath = Path.Combine(resourcesFolder, "ru.html");
                else
                    filePath = Path.Combine(resourcesFolder, "en.html");


                if (File.Exists(filePath))
                {
                    webBrowser1.Navigate(filePath);
                }
                else
                {
                    throw new Exception("Файл html не найден. Убедитесь, что он находится в папке Resources.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
