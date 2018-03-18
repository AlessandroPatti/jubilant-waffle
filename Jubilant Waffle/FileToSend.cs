using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    public class FileToSend {
        public string path;
        public string ip;
        public long fileSize;
        public ProgressBar pbar;
        public Label label;
        public Button button;
        public FlowLayoutPanel container;
        volatile public bool cancel;

        public FileToSend(string path, string ip, int PBarStep, long fileSize = 0) {
            this.path = path;
            this.ip = ip;
            this.fileSize = fileSize != 0 ? fileSize : (new System.IO.FileInfo(path)).Length;
            label = new Label();
            label.Text = System.IO.Path.GetFileName(path);
            

            pbar = new ProgressBar();
            pbar.Minimum = 0;
            pbar.Maximum = (int)Math.Ceiling((double)this.fileSize / (1024 * 1024));
            pbar.Step = Math.Max(1, Math.Min(PBarStep, pbar.Maximum));

            button = new Button();
            button.Size = new Size(20, 20);
            button.FlatAppearance.BorderSize = 0;
            button.FlatStyle = FlatStyle.Flat;
            button.BackgroundImage = Image.FromFile(@"icons\cancel.png");
            button.Text = "";
            button.Click += ProgressBarButtonClick;

            container = new FlowLayoutPanel();
            container.Controls.Add(label);
            container.Controls.Add(pbar);
            container.Controls.Add(button);
        }

        public delegate void AddToPanelCallback(Control panel);
        public void AddToPanel(Control panel) {
            if (panel.InvokeRequired) {
                AddToPanelCallback callback = new AddToPanelCallback(AddToPanel);
                panel.Invoke(callback, panel);
            }
            else {
                pbar.Size = new Size(panel.Size.Width - 60, 20);
                label.Size = new Size(panel.Width - 25, 15);
                container.Size = new Size(panel.Size.Width - 25, label.Size.Height + pbar.Size.Height + 5);
                panel.Controls.Add(container);
            }
        }

        public delegate void UpdateProgressCallback();
        public void UpdateProgress() {
            if (pbar.InvokeRequired) {
                UpdateProgressCallback callback = new UpdateProgressCallback(UpdateProgress);
                pbar.Invoke(callback);
            }
            else {
                pbar.PerformStep();
                if (pbar.Value == pbar.Maximum)
                    button.BackgroundImage = Image.FromFile(@"icons\done.png");
            }
        }
        private void ProgressBarButtonClick(object sender, EventArgs e) {
            cancel = true;
            FlowLayoutPanel panel = (FlowLayoutPanel)((Control)sender).Parent;
            int i = panel.Controls.IndexOf((Button)sender);
            panel.Controls.RemoveAt(i);
            panel.Controls.RemoveAt(i - 1);
            panel.Controls.RemoveAt(i - 2);
            panel.Parent.Controls.Remove(container);
        }

    }
}
