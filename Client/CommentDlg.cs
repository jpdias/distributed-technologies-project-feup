using System;
using System.Windows.Forms;

public partial class CommentDlg : Form {
  public string comment;

  public CommentDlg() {
    InitializeComponent();
  }

  private void okButton_Click(object sender, EventArgs e) {
    comment = commTB.Text;
  }
}
