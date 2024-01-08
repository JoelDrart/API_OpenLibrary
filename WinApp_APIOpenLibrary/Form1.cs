using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WinApp_APIOpenLibrary
{
    public partial class Form1 : Form
    {
        const string ApiBaseUrl = "https://openlibrary.org/search.json";
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                buscar(textBox1.Text);

            }
        }

        private async void buscar(string libro)
        {
            string libroBuscado = libro;

            if (!string.IsNullOrWhiteSpace(libroBuscado))
            {
                try
                {
                    string apiUrl = $"{ApiBaseUrl}?q={libroBuscado.Replace(" ", "+")}";
                    string jsonResponse = await GetApiResponse(apiUrl);

                    JObject result = JObject.Parse(jsonResponse);

                    if (result.ContainsKey("docs"))
                    {
                        JArray docs = (JArray)result["docs"];
                        if (docs.Count > 0)
                        {
                            HashSet<string> autoresSet = new HashSet<string>(); // Usamos un HashSet para evitar duplicados

                            for (int i = 0; i < docs.Count - 1; i++)
                            {
                                if (docs[i]["author_name"] != null && docs[i]["author_name"].Any())
                                {
                                    string autor = string.Join(", ", docs[i]["author_name"]);
                                    autoresSet.Add(autor );
                                }
                            }

                            // Agregar los autores al listBox1
                            foreach (string autor in autoresSet)
                            {
                                listBox1.Items.Add(autor);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron autores.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Respuesta no presenta 'docs', es decir no se ha devuelto ningún libro ni autor.");
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Ingrese el nombre del libro.");
            }
        }

        private async Task<string> GetApiResponse(string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
