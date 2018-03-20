using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    public class FileToSend {
        /// <summary>
        /// The following class contains information about a file transfer
        /// including visual element for the user (i.e. progress bar) 
        /// and a button to cancel the transfer
        /// The panel with all the visual elements can be added to another form
        /// </summary>
        public string path;                         // The path (or the name) of the file
        public string ip;                           // IP address of the remote host
        public long fileSize;                       // The size of the file
        public ProgressBar pbar;                    // A progress bar for the trasfer
        public Label label;                         // A label that show the name of the file above the progress bar
        public Button button;                       // A buton to cancel the transfer
        public FlowLayoutPanel container;           // The container for all the visual elements.
        volatile public bool cancel;                // This boolean is set to true when the user wants to cancel the transfer. It has to be check to stop the transfer

        public FileToSend(string path, string ip, int PBarStep, long fileSize = 0) {
            this.path = path;
            this.ip = ip;
            /* If the file size is not set, the constructor will try to retrieve it from the FS.
             * The purpose of the argment is to make this class not only compatible with outgoing transfer
             * but also for incoming, for which the fise size is provided by the remote host
             */
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
                /* The size of the visual elements are adapted to the size of the panel to fit all the width. */
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
                if (pbar.Value == pbar.Maximum) {
                    /* The transfer has ended. The button will be still active to hide the 
                     * the transfer from the panel but the icon will change for a better visualization.
                     */
                    button.BackgroundImage = Image.FromFile(@"icons\done.png");
                }
            }
        }
        private void ProgressBarButtonClick(object sender, EventArgs e) {
            cancel = true; // If the transfer is still not completed, this will tell the method that is managing the transfer to stop it.
            FlowLayoutPanel panel = (FlowLayoutPanel)((Control)sender).Parent;
            int i = panel.Controls.IndexOf((Button)sender);
            panel.Controls.RemoveAt(i);
            panel.Controls.RemoveAt(i - 1);
            panel.Controls.RemoveAt(i - 2);
            if (panel.Parent != null) {
                /* Remove the visual elements from the panel in wiìhich they have been inserted by the method "AddToPanel" */
                panel.Parent.Controls.Remove(container);
            }
        }

    }
}
