partial class CommentDlg {
  /// <summary>
  /// Required designer variable.
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary>
  /// Clean up any resources being used.
  /// </summary>
  /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  protected override void Dispose(bool disposing) {
    if (disposing && (components != null)) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  #region Windows Form Designer generated code

  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent() {
    this.commTB = new System.Windows.Forms.RichTextBox();
    this.okButton = new System.Windows.Forms.Button();
    this.button1 = new System.Windows.Forms.Button();
    this.SuspendLayout();
    // 
    // commTB
    // 
    this.commTB.Location = new System.Drawing.Point(12, 14);
    this.commTB.Multiline = false;
    this.commTB.Name = "commTB";
    this.commTB.Size = new System.Drawing.Size(252, 120);
    this.commTB.TabIndex = 0;
    this.commTB.Text = "comment";
    // 
    // okButton
    // 
    this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
    this.okButton.Location = new System.Drawing.Point(42, 152);
    this.okButton.Name = "okButton";
    this.okButton.Size = new System.Drawing.Size(74, 33);
    this.okButton.TabIndex = 1;
    this.okButton.Text = "OK";
    this.okButton.UseVisualStyleBackColor = true;
    this.okButton.Click += new System.EventHandler(this.okButton_Click);
    // 
    // button1
    // 
    this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    this.button1.Location = new System.Drawing.Point(160, 152);
    this.button1.Name = "button1";
    this.button1.Size = new System.Drawing.Size(74, 33);
    this.button1.TabIndex = 2;
    this.button1.Text = "Cancel";
    this.button1.UseVisualStyleBackColor = true;
    // 
    // CommentDlg
    // 
    this.AcceptButton = this.okButton;
    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.CancelButton = this.button1;
    this.ClientSize = new System.Drawing.Size(276, 197);
    this.ControlBox = false;
    this.Controls.Add(this.button1);
    this.Controls.Add(this.okButton);
    this.Controls.Add(this.commTB);
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = "CommentDlg";
    this.ShowInTaskbar = false;
    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
    this.Text = "CommentDlg";
    this.ResumeLayout(false);

  }

  #endregion

  private System.Windows.Forms.RichTextBox commTB;
  private System.Windows.Forms.Button okButton;
  private System.Windows.Forms.Button button1;
}