using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser.Utilities
{
    class Crawler
    {

        private static readonly HttpClient _httpClient = new HttpClient();

        List<String> words = new List<string>();

        /* Esta propiedad es opcional porque no sería imprescindible saber desde el instante 0
         cuántos balanceradores de carga hay en el sistema. Se puede realizar una petición asíncrona
         para averiguarlo y si CURRENT_NUM_LOAD_BALANCERS es nulo en el instante de hacer peticiones POST para
         almacenar las palabras, se puede utilizar en su lugar DEFAULT_NUM_LOAD_BALANCERS */
        private static int? CURRENT_NUM_LOAD_BALANCERS;

        /* Si el patrón por defecto tuviese que modificarse,
        * sería necesario actualizar todas las apps de escritorio.
        * Se supone que debe conocerse este patrón desde el principio y 
        * sin posibilidad de modificación */
        private static readonly string DEFAULT_URI_LOAD_BALANCERS = "https://Xloadbalancerbrowserapi.azurewebsites.net/api/postDataToApi";

        /*Como mínimo el sistema va a contar con 2 balanceadores de carga*/
        private static readonly int DEFAULT_NUM_LOAD_BALANCERS = 2;

        /* Esta lista contiene las URLs de todos los balanceadores de carga disponibles en el sistema.
         La idea sería que se actualizase periódicamente al menos antes de leer un documento y compruebe el 
         estado de CURRENT_NUM_LOAD_BALANCERS*/
        private static List<string> LOAD_BALANCERS_LIST = new List<string>();


        private static readonly string URL_GET_NUMBER_BALANCERS_COMPONENT = "https://browserapinumloadbal.azurewebsites.net/api/GetNumLoadBalancers";


        List<String> thesaurus;

        public Crawler()
        {
            updateLoadBalancersListAsync();
        }


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
                    string[] wordsPerLine = line.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in wordsPerLine)
                    {
                        //Here is where we check if the word is contained in the thesaurus or not, and if yes we update 
                        //the database 

                        //TODO
                        foreach (string value in thesaurus)
                        {
                            if (value.Equals(word))
                            {
                                //Insert term into list
                                words.Add(word);
                            }
                        }

                    }

                    //Send terms to balancer via POST
                    postWordsToLoadBalancers();
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


        // Este método debe ser llamado antes de indexar un documento.
        private void updateLoadBalancersListAsync()
        {
            try
            {
                CURRENT_NUM_LOAD_BALANCERS = GetNumLoadBalancersAsync().Result;
            }
            catch
            {
                CURRENT_NUM_LOAD_BALANCERS = DEFAULT_NUM_LOAD_BALANCERS;
            }

            for (int i = 1; i <= CURRENT_NUM_LOAD_BALANCERS; i++)
            {
                string url = DEFAULT_URI_LOAD_BALANCERS.Replace("X", i.ToString());
                LOAD_BALANCERS_LIST.Add(url);
            }
        }

        /*Este es un método asíncrono de tipo Task que devolverá el número 
         * actual de balanceadores del sistema mediante una petición GET a 
         una FunctionApp desplegada en Microsft Azure*/
        private async Task<int> GetNumLoadBalancersAsync()
        {
            var _httpClient = new HttpClient();

            using (var result = await _httpClient.GetAsync(URL_GET_NUMBER_BALANCERS_COMPONENT).ConfigureAwait(false))
            {
                string content = await result.Content.ReadAsStringAsync();
                return int.Parse(content);
            }
        }

        private void postWordsToLoadBalancers()
        {
            for (int i = 0; i < words.Count; i++)
            {
                Post(LOAD_BALANCERS_LIST.ElementAt(i % CURRENT_NUM_LOAD_BALANCERS.Value), words.ElementAt(i)).ConfigureAwait(false);

            }

        }

        public static async Task Post(string urlBalancer, string postData)
        {

            string str = "{\"term\": \"" + postData + "\"}";

            _httpClient.DefaultRequestHeaders
             .Accept
             .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Put method with error handling
            using (var content = new StringContent(str, Encoding.UTF8, "application/json"))
            {
                var result = await _httpClient.PostAsync($"{urlBalancer}", content).ConfigureAwait(false);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    // Something wrong happened
                    string resultContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    // ... post to Monitor
                }
            }
        }
    }
}