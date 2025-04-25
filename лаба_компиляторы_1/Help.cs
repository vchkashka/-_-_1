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
using лаба_компиляторы_1.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace лаба_компиляторы_1
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private static string ImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string SafeImageToBase64(Image image)
        {
            try
            {
                return ImageToBase64(image);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при конвертации изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        private void pasteImages(string htmlTemplate)
        {
            var imageMap = new Dictionary<string, Image>
            {
                { "add_circle_create_expand_new_plus_icon_123218.png", Resources.add_circle_create_expand_new_plus_icon_123218 },
                { "folder_open_icon_173134.png", Resources.folder_open_icon_173134 },
                { "save_icon_125167.png", Resources.save_icon_125167 },
                { "undo-black-arrow-pointing-to-left_icon-icons.com_56880.png", Resources.undo_black_arrow_pointing_to_left_icon_icons_com_56880 },
                { "redo-arrow-1_icon-icons.com_56892.png", Resources.redo_arrow_1_icon_icons_com_56892 },
                { "copy-file_icon-icons.com_56095.png", Resources.copy_file_icon_icons_com_56095 },
                { "edit_cut_icon_124981.png", Resources.edit_cut_icon_124981 },
                { "paste_120009.png", Resources.paste_120009_png },
                { "start-button_icon-icons.com_53873.png", Resources.start_button_icon_icons_com_53873 },
                { "Help_icon-icons.com_55891.png", Resources.Help_icon_icons_com_55891 },
                { "info_information_icon_178159.png", Resources.info_information_icon_178159 }
            };

            foreach (var pair in imageMap)
            {
                string base64 = SafeImageToBase64(pair.Value);
                htmlTemplate = htmlTemplate.Replace(pair.Key, $"data:image/png;base64,{base64}");
            }

            webBrowser1.DocumentText = htmlTemplate;
        }

        private void Help_Load(object sender, EventArgs e)
        {
            try
            {
                AppSettings.ApplyFontSizeToControls(this.Controls);

                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru")
                    pasteImages(Properties.Resources.ru);
                else
                    pasteImages(Properties.Resources.en);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
