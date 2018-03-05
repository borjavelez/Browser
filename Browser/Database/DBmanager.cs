using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

namespace Browser.Database
{
    class DBmanager
    {
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataReader dr;

        public DBmanager()
        {
            if (!connection())
            {
                MessageBox.Show("Error! Database not found");
            } 
        }

        public bool connection()
        {
            try
            {
                cn = new SqlConnection();
                String dbFileName = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Database\\BrowserDB.mdf";
                cn.ConnectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = '" + dbFileName + "'; Integrated Security = True; MultipleActiveResultSets=true;";
                cn.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void close()
        {
            try
            {
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool insertIntoTerm(String term)
        {
            try
            {
                String query = "INSERT INTO Term (value) VALUES ('" + term + "');";
                SqlCommand cmd2 = new SqlCommand(query, cn);
                int i = cmd2.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool insertIntoDoc(String url, String type)
        {
            try
            {
                String query = "INSERT INTO Doc (url, type) VALUES ('" + url + "', '" + type + "');";
                cmd = new SqlCommand(query, cn);
                int i = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool insertIntoTerm_Doc(int term_id, int doc_id)
        {
            if (checkTermId(term_id) && checkDocId(doc_id))
            {
                try
                {
                    String query = "INSERT INTO Term_Doc (term_id, doc_id) VALUES ('" + term_id + "', '" + doc_id + "');";
                    cmd = new SqlCommand(query, cn);
                    int i = cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            return false;
        }

        public bool checkTermId(int term_id)
        {
            try
            {
                String query = "SELECT * FROM Term where Id=" + term_id;
                cmd = new SqlCommand(query, cn);
                dr = cmd.ExecuteReader();
                bool result = dr.Read();
                dr.Close();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dr.Close();
                return false;
            }
        }



        public bool checkDocId(int doc_id)
        {
            try
            {
                String query = "SELECT * FROM Doc where Id=" + doc_id;
                cmd = new SqlCommand(query, cn);
                dr = cmd.ExecuteReader();
                bool result = dr.Read();
                dr.Close();
                return result;
            }
            catch (Exception ex)
            {
                dr.Close();
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public List<String> searchTerm(String input)
        {
            List<String> result = new List<String>();

            int id = getTermId(input);
            if (id == 0)
            {
                result.Add("No results found");
                return result;
            }
            else
            {
                List<int> result2 = new List<int>();
                result2 = getDocsFromTermId(id);
                return getDocsFromDocId(result2);
            }
        }

        public int getTermId(String input)
        {
            try
            {
                //Get the id of the received term
                int id;
                String query = "SELECT Id FROM Term WHERE value='" + input + "'";
                cmd = new SqlCommand(query, cn);
                id = (int)cmd.ExecuteScalar();
                return id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<int> getDocsFromTermId(int id)
        {
            List<int> result = new List<int>();
            try
            {
                //Get the docs linked to the received term
                String query = "SELECT doc_id FROM Term_Doc WHERE term_id=" + id;
                cmd = new SqlCommand(query, cn);
                using (dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        result.Add(dr.GetInt32(0));
                    }
                    //dr.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                //dr.Close();
                result.Add(0);
                return result;
            }
        }

        public List<String> getDocsFromDocId(List<int> doc_ids)
        {
            List<String> result = new List<String>();
            try
            {
                foreach (int i in doc_ids)
                {
                    //Get the docs linked to the received id
                    String query = "SELECT url FROM Doc WHERE Id=" + i;
                    cmd = new SqlCommand(query, cn);
                    using (dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(dr.GetString(0));
                        }
                        //dr.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                //dr.Close();
                result.Add("No results found");
                return result;
            }
        }


        public int checkTermValue(String value)
        {
            try
            {
                String query = "SELECT Id FROM Term where value='" + value + "'";
                cmd = new SqlCommand(query, cn);
                dr = cmd.ExecuteReader();
                int result = 0;
                if (dr.Read())
                {
                    result = int.Parse(dr["Id"].ToString());
                }
                dr.Close();
                return result;
            }
            catch (Exception ex)
            {
                dr.Close();
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public int checkDocUrl(String url)
        {
            try
            {
                String query = "SELECT Id FROM Doc where url='" + url + "'";
                cmd = new SqlCommand(query, cn);
                dr = cmd.ExecuteReader();
                int result = 0;
                if (dr.Read())
                {
                    result = int.Parse(dr["Id"].ToString());
                }
                dr.Close();
                return result;
            }
            catch (Exception ex)
            {
                dr.Close();
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

    }
}
