namespace Ankh.TfsProvider.Extension.IssueTracker.Forms
{
	partial class IssuesListView
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
			"id001",
			"This is where the title of the issue would display"}, -1);
			System.Windows.Forms.ColumnHeader namecol;
			System.Windows.Forms.ColumnHeader idcol;
			this._list = new System.Windows.Forms.ListView();
			namecol = new System.Windows.Forms.ColumnHeader();
			idcol = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// _list
			// 
			this._list.CheckBoxes = true;
			this._list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			idcol,
			namecol});
			this._list.Dock = System.Windows.Forms.DockStyle.Fill;
			listViewItem1.StateImageIndex = 0;
			this._list.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem1});
			this._list.Location = new System.Drawing.Point(0, 0);
			this._list.Name = "_list";
			this._list.Size = new System.Drawing.Size(581, 141);
			this._list.TabIndex = 0;
			this._list.UseCompatibleStateImageBehavior = false;
			this._list.View = System.Windows.Forms.View.Details;
			// 
			// namecol
			// 
			namecol.Text = "Name";
			namecol.Width = 500;
			// 
			// idcol
			// 
			idcol.Text = "ID";
			// 
			// IssuesListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._list);
			this.Name = "IssuesListView";
			this.Size = new System.Drawing.Size(581, 141);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView _list;
	}
}
