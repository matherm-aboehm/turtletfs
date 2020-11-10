namespace Ankh.TfsProvider.Extension.IssueTracker.Forms
{
	partial class ConfigurationPage
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.Label urlLabel;
			System.Windows.Forms.Label userNameLabel;
			System.Windows.Forms.Label passwordLabel;
			this._url = new System.Windows.Forms.TextBox();
			this._user = new System.Windows.Forms.TextBox();
			this._password = new System.Windows.Forms.TextBox();
			urlLabel = new System.Windows.Forms.Label();
			userNameLabel = new System.Windows.Forms.Label();
			passwordLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// urlLabel
			// 
			urlLabel.Location = new System.Drawing.Point(11, 12);
			urlLabel.Name = "urlLabel";
			urlLabel.Size = new System.Drawing.Size(150, 20);
			urlLabel.TabIndex = 0;
			urlLabel.Text = "Issue Repository URL";
			urlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userNameLabel
			// 
			userNameLabel.Location = new System.Drawing.Point(11, 43);
			userNameLabel.Name = "userNameLabel";
			userNameLabel.Size = new System.Drawing.Size(150, 20);
			userNameLabel.TabIndex = 2;
			userNameLabel.Text = "User Name";
			userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// passwordLabel
			// 
			passwordLabel.Location = new System.Drawing.Point(11, 74);
			passwordLabel.Name = "passwordLabel";
			passwordLabel.Size = new System.Drawing.Size(150, 20);
			passwordLabel.TabIndex = 4;
			passwordLabel.Text = "Password";
			passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _url
			// 
			this._url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._url.Location = new System.Drawing.Point(172, 12);
			this._url.Name = "_url";
			this._url.Size = new System.Drawing.Size(281, 20);
			this._url.TabIndex = 1;
			this._url.TextChanged += new System.EventHandler(UrlModified);
			// 
			// _user
			// 
			this._user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._user.Location = new System.Drawing.Point(172, 43);
			this._user.Name = "_user";
			this._user.Size = new System.Drawing.Size(281, 20);
			this._user.TabIndex = 3;
			// 
			// _password
			// 
			this._password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._password.Location = new System.Drawing.Point(172, 74);
			this._password.Name = "_password";
			this._password.PasswordChar = '*';
			this._password.Size = new System.Drawing.Size(281, 20);
			this._password.TabIndex = 5;
			// 
			// ConfigurationPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._password);
			this.Controls.Add(passwordLabel);
			this.Controls.Add(this._user);
			this.Controls.Add(userNameLabel);
			this.Controls.Add(this._url);
			this.Controls.Add(urlLabel);
			this.Name = "ConfigurationPage";
			this.Size = new System.Drawing.Size(465, 109);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _url;
		private System.Windows.Forms.TextBox _user;
		private System.Windows.Forms.TextBox _password;
	}
}
