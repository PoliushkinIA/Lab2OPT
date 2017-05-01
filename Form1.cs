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
            if (parser.Parse())
                MessageBox.Show("The program is correct", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                switch (parser.parseError)
                {
                    case Parser.ParseError.NoError:
                        MessageBox.Show("Undentified error", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Program:
                        MessageBox.Show("Error in program statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Var:
                        MessageBox.Show("Error in var statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Begin:
                        MessageBox.Show("Error: BEGIN expected", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.End:
                        MessageBox.Show("Error: END expected", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Dec:
                        MessageBox.Show("Error in variable declaration", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.PrcdCall:
                        MessageBox.Show("Error in procedure call", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Read:
                        MessageBox.Show("Error in READ statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Write:
                        MessageBox.Show("Error in WRITE statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.UEOF:
                        MessageBox.Show("Error: unexpected end of file", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Assign:
                        MessageBox.Show("Error in assign statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Expr:
                        MessageBox.Show("Error in arithmetic expression", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.If:
                        MessageBox.Show("Error in IF statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Repeat:
                        MessageBox.Show("Error in REPEAT statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Cond:
                        MessageBox.Show("Error in boolean expression", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case Parser.ParseError.Procedure:
                        MessageBox.Show("Error in procedure statement", "Parser output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        break;
                }
            }
        }

        private void abotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }
}
