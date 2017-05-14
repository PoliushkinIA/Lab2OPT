using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPTLab2
{
    public partial class Form1 : Form
    {
        string fileName;

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = false;
            richTextBox1.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;
                richTextBox1.LoadFile(fileName);
                saveToolStripMenuItem.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SaveFile(fileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fileName = sfd.FileName;
                richTextBox1.SaveFile(fileName);
                saveToolStripMenuItem.Enabled = true;
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser(richTextBox1.Text);
            string errorText;
            if (parser.Parse())
                MessageBox.Show("The program is correct", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                switch (parser.parseError)
                {
                    case Parser.ParseError.NoError:
                        errorText = "Undentified error";
                        break;
                    case Parser.ParseError.Program:
                        errorText = "Error in program statement";
                        break;
                    case Parser.ParseError.Var:
                        errorText = "Error in var statement";
                        break;
                    case Parser.ParseError.Begin:
                        errorText = "Error: BEGIN expected";
                        break;
                    case Parser.ParseError.End:
                        errorText = "Error: END expected";
                        break;
                    case Parser.ParseError.Dec:
                        errorText = "Error in variable declaration";
                        break;
                    case Parser.ParseError.PrcdCall:
                    case Parser.ParseError.Read:
                    case Parser.ParseError.Write:
                    case Parser.ParseError.Assign:
                    case Parser.ParseError.Expr:
                    case Parser.ParseError.If:
                    case Parser.ParseError.Repeat:
                    case Parser.ParseError.Cond:
                        errorText = "Error in program body";
                        break;
                    case Parser.ParseError.Procedure:
                        errorText = "Error in procedure statement";
                        break;
                    case Parser.ParseError.UEOF:
                        errorText = "Error: unexpected end of file";
                        break;
                    default:
                        errorText = "Something went very wrong";
                        break;
                }
                MessageBox.Show(errorText + " at line " + parser.GetLine(),
                                "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void abotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }
}
