using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using HtmlAgilityPack;
using mshtml;
using System.Diagnostics;

namespace Html_Agil_Xpath
{
    public partial class Form1 : Form
    {
        bool dont_update_browser = false;

        public Form1()
        {
            InitializeComponent();

            //dataGridView1.DoubleClick += new EventHandler(dataGridView1_DoubleClick);
            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView1_CellDoubleClick);
            dataGridView1.AutoGenerateColumns = false;
            webBrowser1.ScriptErrorsSuppressed = true;


        }

        void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dont_update_browser = true;
            string path = dataGridView1[0, e.RowIndex].Value.ToString();

            HtmlNode node = m_html[path];

            if (node == null)
                return;

            webBrowser1.DocumentText = node.OuterHtml;
            textBox1.Text = node.OuterHtml;
        }

        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //DataGridViewCellMouseEventArgs eventToHandle = e as DataGridViewCellMouseEventArgs;
            try
            {
                DataGridView temp = (DataGridView)sender;



                string path = dataGridView1[0, 0].Value.ToString();

                HtmlNode node = m_html[path];

                if (node == null)
                {
                    MessageBox.Show("can't select node");
                    return;
                }

                webBrowser1.DocumentText = node.OuterHtml;
                textBox1.Text = node.OuterHtml;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        Agil_Xpath m_html;
        private void openUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string url = Interaction.InputBox("Enter Url", "Url", "http://google.com");

            if (url == string.Empty)
                return;
            
            //m_html = Agil_Xpath.LoadFromURL(url);
            webBrowser1.Navigate(url);

            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
            //m_html = Agil_Xpath.LoadFromString(webBrowser1.DocumentText);
            //textBox1.Text = webBrowser1.DocumentText;
            
            //SetBrowser();

        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            Debug.WriteLine("Entering Browser Complete");
            if (dont_update_browser)
            {
                dont_update_browser = false;
                Debug.WriteLine("Not Leaving");
                return;
            }

            //throw new NotImplementedException();
            //textBox1.Text = webBrowser1.DocumentText;
            //webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
            string html = webBrowser1.Document.Body.Parent.OuterHtml;
            textBox1.Text = html;
            Debug.WriteLine("Leaving Browser Complete");
        }

        private void SetBrowser()
        {
            textBox1.Text = m_html.getHtml();
            webBrowser1.DocumentText = m_html.getHtml();
            
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();

            if (openFile.FileName == string.Empty)
                return;

            m_html = Agil_Xpath.LoadFromFile(openFile.FileName);
            webBrowser1.DocumentText = m_html.getHtml();
            textBox1.Text = m_html.getHtml();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void find_btn_Click(object sender, EventArgs e)
        {
            doFind(find_box.Text);
        }

        private void doFind(string term)
        {
            
            loadIntoAgilityToolStripMenuItem_Click(null, null);
            
            
            
            m_html.findText(term);
            dataGridView1.DataSource = m_html.Xpaths.Select(x => new { Value = x }).ToList();
            dataGridView1.Columns["Xpath"].DataPropertyName = "Value";

            foreach (int i in m_html.FoundNodes)
            {
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
            }
        }

        private void findSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IHTMLDocument2 htmlDocument = webBrowser1.Document.DomDocument as IHTMLDocument2;

            IHTMLSelectionObject currentSelection = htmlDocument.selection;
            
            string string_find;

            if (currentSelection != null && currentSelection.type == "Text")
            {
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;

                if (range != null)
                {
                    string_find = range.text;
                    doFind(string_find);
                }
            }
            else
            {
                MessageBox.Show(currentSelection.type);
            }

        }

        private void loadIntoAgilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string html = webBrowser1.Document.Body.Parent.OuterHtml;
            //m_html = Agil_Xpath.LoadFromString(webBrowser1.DocumentText);
            m_html = Agil_Xpath.LoadFromString(html);
        }
    }
}
