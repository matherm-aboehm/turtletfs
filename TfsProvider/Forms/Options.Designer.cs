namespace TurtleTfs.Forms
{
	partial class Options
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
			this.label1 = new System.Windows.Forms.Label();
			this.tfsAddressTextBox = new System.Windows.Forms.TextBox();
			this.projectComboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.refreshProjectsButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.tfsUsernameTextBox = new System.Windows.Forms.TextBox();
			this.tfsPasswordTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tfsVisualStudioOnline = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "TFS Address:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// tfsAddressTextBox
			// 
			this.tfsAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tfsAddressTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
			this.tfsAddressTextBox.Location = new System.Drawing.Point(91, 13);
			this.tfsAddressTextBox.Name = "tfsAddressTextBox";
			this.tfsAddressTextBox.Size = new System.Drawing.Size(488, 20);
			this.tfsAddressTextBox.TabIndex = 0;
			this.tfsAddressTextBox.Text = "http://lit-department:8080/";
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
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(42, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Project:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(532, 132);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(76, 23);
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(451, 132);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
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
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(30, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Username";
			// 
			// tfsUsernameTextBox
			// 
			this.tfsUsernameTextBox.Location = new System.Drawing.Point(91, 62);
			this.tfsUsernameTextBox.Name = "tfsUsernameTextBox";
			this.tfsUsernameTextBox.Size = new System.Drawing.Size(200, 20);
			this.tfsUsernameTextBox.TabIndex = 2;
			// 
			// tfsPasswordTextBox
			// 
			this.tfsPasswordTextBox.Location = new System.Drawing.Point(379, 62);
			this.tfsPasswordTextBox.Name = "tfsPasswordTextBox";
			this.tfsPasswordTextBox.PasswordChar = '*';
			this.tfsPasswordTextBox.Size = new System.Drawing.Size(200, 20);
			this.tfsPasswordTextBox.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(320, 65);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Password";
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
			// Options
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(620, 167);
			this.Controls.Add(this.tfsVisualStudioOnline);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tfsPasswordTextBox);
			this.Controls.Add(this.tfsUsernameTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.refreshProjectsButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.projectComboBox);
			this.Controls.Add(this.tfsAddressTextBox);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Options";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.Options_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tfsAddressTextBox;
		private System.Windows.Forms.ComboBox projectComboBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button refreshProjectsButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tfsUsernameTextBox;
		private System.Windows.Forms.TextBox tfsPasswordTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox tfsVisualStudioOnline;
	}
}