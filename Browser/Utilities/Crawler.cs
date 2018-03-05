using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Browser.Database;
using System.Windows.Forms;

namespace Browser.Utilities
{
    class Crawler
    {
        List<String> thesaurus;

        public void indexFilesAndDirectories()
        {
            readThesaurus();
            String path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Docs";
            //With this method all the .txt files in a directory are found recursively.
            string[] files = Directory.GetFiles(path, "*.txt*", SearchOption.AllDirectories);
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '?', '!', ';', '-' };
            foreach (string fileName in files)
            {
                string[] readText = File.ReadAllLines(fileName);
                foreach (string line in readText)
                {
                    string[] words = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in words)
                    {
                        //Here is where we check if the word is contained in the thesaurus or not, and if yes we update 
                        //the database 

                        //TODO
                        foreach (string value in thesaurus)
                        {
                            if (value.Equals(word))
                            {
                                addWord(value, fileName);
                            }
                        }

                    }
                }
            }

        }

        public List<string> readThesaurus()
        {
            String thesaurusPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Internal\\Thesaurus.txt";
            string[] readText = File.ReadAllLines(thesaurusPath);
            List<string> list = new List<string>(readText);
            thesaurus = list;
            return list; 
        }

        public bool addWord(String word, String url)
        {
            try
            {
                DBmanager db = new DBmanager();
                if (db.checkTermValue(word) == 0)
                {
                    db.insertIntoTerm(word);
                    if (db.checkDocUrl(url) != 0)
                    {
                        db.insertIntoTerm_Doc(db.checkTermValue(word), db.checkDocUrl(url));
                    } else
                    {
                        db.insertIntoDoc(url, "txt");
                        db.insertIntoTerm_Doc(db.checkTermValue(word), db.checkDocUrl(url));
                    }
                }
                return true;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}