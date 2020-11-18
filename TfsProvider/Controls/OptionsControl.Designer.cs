namespace TurtleTfs.Controls
{
	partial class OptionsControl
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            this.tfsUsernameTextBox = new System.Windows.Forms.TextBox();
            this.tfsPasswordTextBox = new System.Windows.Forms.TextBox();
            this.tfsAddressTextBox = new System.Windows.Forms.TextBox();
            this.projectComboBox = new System.Windows.Forms.ComboBox();
            this.refreshProjectsButton = new System.Windows.Forms.Button();
            this.tfsVisualStudioOnline = new System.Windows.Forms.CheckBox();
            this.tfsConnector = new TurtleTfs.Controls.TfsConnector(this.components);
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(71, 13);
            label1.TabIndex = 0;
            label1.Text = "TFS Address:";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 92);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 13);
            label2.TabIndex = 3;
            label2.Text = "Project:";
            label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 6);
            label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(55, 13);
            label3.TabIndex = 5;
            label3.Text = "Username";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(281, 6);
            label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 13);
            label4.TabIndex = 8;
            label4.Text = "Password";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label3, 0, 0);
            tableLayoutPanel1.Controls.Add(this.tfsUsernameTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(label4, 2, 0);
            tableLayoutPanel1.Controls.Add(this.tfsPasswordTextBox, 3, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(27, 59);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(555, 26);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // tfsUsernameTextBox
            // 
            this.tfsUsernameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tfsUsernameTextBox.Location = new System.Drawing.Point(64, 3);
            this.tfsUsernameTextBox.Name = "tfsUsernameTextBox";
            this.tfsUsernameTextBox.Size = new System.Drawing.Size(211, 20);
            this.tfsUsernameTextBox.TabIndex = 2;
            // 
            // tfsPasswordTextBox
            // 
            this.tfsPasswordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tfsPasswordTextBox.Location = new System.Drawing.Point(340, 3);
            this.tfsPasswordTextBox.Name = "tfsPasswordTextBox";
            this.tfsPasswordTextBox.PasswordChar = '*';
            this.tfsPasswordTextBox.Size = new System.Drawing.Size(212, 20);
            this.tfsPasswordTextBox.TabIndex = 3;
            // 
            // tfsAddressTextBox
            // 
            this.tfsAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tfsAddressTextBox.Location = new System.Drawing.Point(91, 13);
            this.tfsAddressTextBox.Name = "tfsAddressTextBox";
            this.tfsAddressTextBox.Size = new System.Drawing.Size(488, 20);
            this.tfsAddressTextBox.TabIndex = 0;
            this.tfsAddressTextBox.Text = "http://server:8080/tfs/DefaultCollection";
            this.tfsAddressTextBox.WordWrap = false;
            // 
            // projectComboBox
            // 
            this.projectComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.projectComboBox.FormattingEnabled = true;
            this.projectComboBox.Location = new System.Drawing.Point(91, 89);
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(517, 21);
            this.projectComboBox.TabIndex = 4;
            this.projectComboBox.SelectedIndexChanged += new System.EventHandler(this.projectComboBox_SelectedIndexChanged);
            // 
            // refreshProjectsButton
            // 
            this.refreshProjectsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshProjectsButton.Location = new System.Drawing.Point(585, 60);
            this.refreshProjectsButton.Name = "refreshProjectsButton";
            this.refreshProjectsButton.Size = new System.Drawing.Size(23, 23);
            this.refreshProjectsButton.TabIndex = 1;
            this.refreshProjectsButton.Text = "↻";
            this.refreshProjectsButton.UseVisualStyleBackColor = true;
            this.refreshProjectsButton.Click += new System.EventHandler(this.refreshProjectsButton_Click);
            // 
            // tfsVisualStudioOnline
            // 
            this.tfsVisualStudioOnline.AutoSize = true;
            this.tfsVisualStudioOnline.Location = new System.Drawing.Point(91, 39);
            this.tfsVisualStudioOnline.Name = "tfsVisualStudioOnline";
            this.tfsVisualStudioOnline.Size = new System.Drawing.Size(179, 17);
            this.tfsVisualStudioOnline.TabIndex = 9;
            this.tfsVisualStudioOnline.Text = "Visual Studio Online functionality";
            this.tfsVisualStudioOnline.UseVisualStyleBackColor = true;
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.Controls.Add(tableLayoutPanel1);
            this.Controls.Add(this.tfsVisualStudioOnline);
            this.Controls.Add(this.refreshProjectsButton);
            this.Controls.Add(label2);
            this.Controls.Add(this.projectComboBox);
            this.Controls.Add(this.tfsAddressTextBox);
            this.Controls.Add(label1);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(620, 125);
            this.Load += new System.EventHandler(this.Options_Load);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox tfsAddressTextBox;
		private System.Windows.Forms.ComboBox projectComboBox;
		private System.Windows.Forms.Button refreshProjectsButton;
		private System.Windows.Forms.TextBox tfsUsernameTextBox;
		private System.Windows.Forms.TextBox tfsPasswordTextBox;
		private System.Windows.Forms.CheckBox tfsVisualStudioOnline;
		private TfsConnector tfsConnector;
	}
}
