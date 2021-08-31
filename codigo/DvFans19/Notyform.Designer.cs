namespace DvFans19
{
    partial class Notyform
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notyform));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tinotify = new System.Windows.Forms.Label();
            this.txtnotify = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Blue;
            this.panel1.Location = new System.Drawing.Point(8, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 120);
            this.panel1.TabIndex = 0;
            // 
            // tinotify
            // 
            this.tinotify.AutoSize = true;
            this.tinotify.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tinotify.ForeColor = System.Drawing.Color.White;
            this.tinotify.Location = new System.Drawing.Point(31, 22);
            this.tinotify.Name = "tinotify";
            this.tinotify.Size = new System.Drawing.Size(132, 24);
            this.tinotify.TabIndex = 1;
            this.tinotify.Text = "Calibrar HUD";
            // 
            // txtnotify
            // 
            this.txtnotify.AutoSize = true;
            this.txtnotify.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtnotify.ForeColor = System.Drawing.Color.White;
            this.txtnotify.Location = new System.Drawing.Point(23, 54);
            this.txtnotify.Name = "txtnotify";
            this.txtnotify.Size = new System.Drawing.Size(411, 64);
            this.txtnotify.TabIndex = 2;
            this.txtnotify.Text = resources.GetString("txtnotify.Text");
            // 
            // Notyform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(439, 142);
            this.Controls.Add(this.txtnotify);
            this.Controls.Add(this.tinotify);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Notyform";
            this.Text = "Notyform";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label tinotify;
        private System.Windows.Forms.Label txtnotify;
    }
}