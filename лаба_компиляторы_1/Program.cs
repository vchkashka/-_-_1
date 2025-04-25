using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace лаба_компиляторы_1
{
    public static class AppSettings
    {
        private static void ApplyResourcesToMenuStrip(MenuStrip menuStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in menuStrip.Items)
            {
                resources.ApplyResources(item, item.Name);
                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyResourcesToToolStripMenuItem(menuItem, resources);
                }
            }
        }

        private static void ApplyResourcesToToolStripMenuItem(ToolStripMenuItem menuItem, ComponentResourceManager resources)
        {
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                resources.ApplyResources(subItem, subItem.Name);
                if (subItem is ToolStripMenuItem subMenuItem)
                {
                    ApplyResourcesToToolStripMenuItem(subMenuItem, resources);
                }
            }
        }
        private static void ApplyResourcesToDataGridView(DataGridView dataGridView, ComponentResourceManager resources)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                string columnName = column.Name;
                string localizedHeader = resources.GetString($"Column{column.Index+1}.HeaderText");

                if (!string.IsNullOrEmpty(localizedHeader))
                {
                    column.HeaderText = localizedHeader;
                }
            }
        }

        private static void ApplyResourcesToStatusStrip(StatusStrip statusStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in statusStrip.Items)
            {
                resources.ApplyResources(item, item.Name);
            }
        }

        public static void ApplyFontSizeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                control.Font = new Font(control.Font.FontFamily, Properties.Settings.Default.FontSize);
                if (control.HasChildren)
                {
                    ApplyFontSizeToControls(control.Controls);
                }
            }
        }

        private static void ApplyResourcesToToolStrip(ToolStrip toolStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                resources.ApplyResources(item, item.Name);
            }
        }

        public static void UpdateFormLanguage(Form form)
        {
            ComponentResourceManager resources = new ComponentResourceManager(form.GetType());

            ApplyResourcesToControls(form, resources);

            form.Text = resources.GetString("$this.Text");

            foreach (Control control in form.Controls)
            {
                if (control is MenuStrip menuStrip)
                {
                    ApplyResourcesToMenuStrip(menuStrip, resources);
                }
                else if (control is ToolStrip toolStrip)
                {
                    ApplyResourcesToToolStrip(toolStrip, resources);
                }
                else if (control is DataGridView dataGridView)
                {
                    ApplyResourcesToDataGridView(dataGridView, resources);
                }

            }
            form.AutoSize = false;
        }

        private static void ApplyResourcesToControls(Control parent, ComponentResourceManager resources)
        {
            foreach (Control control in parent.Controls)
               if (control is StatusStrip statusStrip)
                    {
                        ApplyResourcesToStatusStrip(statusStrip, resources);
                    }
            else if (control is DataGridView dataGridView)
                {
                    ApplyResourcesToDataGridView(dataGridView, resources);
                }
                else
               {
                   resources.ApplyResources(control, control.Name);
                   if (control.HasChildren)
                   {
                       ApplyResourcesToControls(control, resources);
                   }
               }
        }

        public static string ErrorMessage()
        {
            if (Thread.CurrentThread.CurrentUICulture == new CultureInfo("ru-RU"))
                return "Ошибка";
            else
                return "Error";
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            string culture = Properties.Settings.Default.AppLanguage;
            if (string.IsNullOrEmpty(culture))
                culture = "ru-RU";

            CultureInfo newCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

                if (requestedAssembly.Name.EndsWith(".resources"))
                {
                    var currentAssembly = Assembly.GetExecutingAssembly();
                    var resourceName = requestedAssembly.Name + ".dll";

                    using (var stream = currentAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            byte[] assemblyData = new byte[stream.Length];
                            stream.Read(assemblyData, 0, assemblyData.Length);
                            return Assembly.Load(assemblyData);
                        }
                    }
                }

                return null;
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

}
