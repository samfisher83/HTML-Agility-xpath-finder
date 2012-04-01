using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Html_Agil_Xpath
{
    class Agil_Xpath
    {
        HtmlDocument m_document = new HtmlDocument();
        List<string> m_xmlPaths = new List<string>();
        List<int> m_foundItems = new List<int>();
        private Agil_Xpath()
        {            
        }

        /// <summary>
        /// Returns a single code
        /// </summary>
        /// <param name="node">xpath of node</param>
        /// <returns>the html node</returns>
        public HtmlNode this[string node]
        {
            get
            {
                try
                {
                    return m_document.DocumentNode.SelectSingleNode(node);
                }
                catch (Exception err)
                {
                    System.Windows.Forms.MessageBox.Show(err.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns all html from web document
        /// </summary>
        /// <returns></returns>
        public string getHtml()
        {
            return m_document.DocumentNode.OuterHtml;
        }

        public static Agil_Xpath LoadFromURL(string url){
            Agil_Xpath item = new Agil_Xpath();
            HtmlWeb webget = new HtmlWeb();
            item.m_document = webget.Load(url);
            return item;
        }

        public static Agil_Xpath LoadFromFile(string path)
        {
            Agil_Xpath item = new Agil_Xpath();
            item.m_document.Load(path);
            return item;
        }

        public static Agil_Xpath LoadFromString(string html)
        {
            Agil_Xpath item = new Agil_Xpath();
            item.m_document.LoadHtml(html);
            return item;
        }
        void findNode(HtmlNode node, string search,int count)
        {   // in case we have two cases down the tree
            int countofterm = Regex.Matches(m_document.DocumentNode.OuterHtml, search).Cast<Match>().Count();
            if (node.OuterHtml.IndexOf(search,StringComparison.InvariantCultureIgnoreCase)!=-1)
            {
                if (!node.XPath.Contains("#"))
                    m_xmlPaths.Add(node.XPath);

                Debug.WriteLine(node.XPath);

                if (node.ChildNodes.Count == 0)
                {
                    m_foundItems.Add(m_xmlPaths.Count - 1);
                    Debug.WriteLine(node.OuterHtml);
                }
                else if (countofterm < count)
                {
                    m_foundItems.Add(m_xmlPaths.Count - 2);
                    Debug.WriteLine("Previous Search term is one of the matches");


                }
            }
            else if (countofterm < count)
            {
                m_foundItems.Add(m_xmlPaths.Count - 1);
            }
            {
            }
            foreach (HtmlNode child in node.ChildNodes)
            {
                findNode(child, search, countofterm);

            }
        }

        public void findText(string term)
        {
            m_xmlPaths.Clear();
            m_foundItems.Clear();

            if (term == null)
                return;
            //Regex.Matches(m_document.DocumentNode.OuterHtml, "And").Cast<Match>().Count() this counts occurances of the term
            findNode(m_document.DocumentNode, term, Regex.Matches(m_document.DocumentNode.OuterHtml, term).Cast<Match>().Count());
            //m_document.DocumentNode.OuterHtml.
                
                //int j = test.Split(' ').Count(x => x.Contains("And"));
                

        }

        public List<string> Xpaths
        {
            get
            {
                return new List<string>(m_xmlPaths);
            }
        }
        public List<int> FoundNodes
        {
            get
            {
                return new List<int>(m_foundItems);
            } 
        }

        //public int count;



    }
}
