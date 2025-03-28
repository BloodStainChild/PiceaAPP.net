using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text.RegularExpressions;

namespace PiceaWindowsFormsApp
{
    public partial class Config: Form
    {
        public Config()
        {
            InitializeComponent();
        }

        private void b_pairing_Click(object sender, EventArgs e)
        {
            string pairingcode = tb_pairingcode.Text;
            string PiceaIP = tb_ip.Text;
            if (pairingcode.Contains("code="))
            {
                string code = pairingcode;
                Match match = Regex.Match(pairingcode, @"code=([^&]+)");
                if (match.Success)
                {
                    pairingcode = match.Groups[1].Value.Trim();
                }
                if (code.Contains("site_id="))
                {
                    Match match2 = Regex.Match(code, @"site_id=([^&]+)");
                    if (match2.Success)
                    {
                        PiceaIP = match2.Groups[1].Value.Trim();
                        string newIP = $"picea-{PiceaIP}.local";
                        PiceaIP = newIP;
                    }
                }
            }

            RegisterDevice(b_pairing, this, PiceaIP, tb_port.Text, pairingcode);
        }

        public static async Task RegisterDevice(Button b_pairing, Form currentForm, string PiceaIP, string PiceaPort, string PairingCode)
        {
            b_pairing.Invoke((Action)(() => b_pairing.Enabled = false));

            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/paired_devices?site_id=";
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Falls ein selbstsigniertes Zertifikat verwendet wird
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                var requestData = new
                {
                    name = Environment.MachineName,
                    pairing_code = PairingCode
                };
                string json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    using (HttpClient client = new HttpClient(handler))
                    {
                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                            string jwt = responseObject?.jwt;
                            string deviceid = responseObject?.device_id;

                            if (string.IsNullOrEmpty(jwt))
                            {
                                MessageBox.Show("Fehler: Kein JWT im Response!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (!ConfigManager.SaveConfig(PiceaIP, PiceaPort, jwt, deviceid))
                            {
                                MessageBox.Show("Failed to save Config File!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            MessageBox.Show("Erfolgreiche Authentifizierung! JWT: " + jwt, "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Form1.PiceaIP = PiceaIP;
                            Form1.PiceaPort = PiceaPort;
                            Form1.jwt = jwt;
                            Form1.deviceid = deviceid;

                            // Form schließen mit DialogResult.OK
                            currentForm.Invoke((Action)(() => currentForm.DialogResult = DialogResult.OK));
                        }
                        else
                        {
                            MessageBox.Show($"Fehlgeschlagen: {response.StatusCode}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Verbindungsfehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unerwarteter Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Button wieder aktivieren (im UI-Thread)
                    b_pairing.Invoke((Action)(() => b_pairing.Enabled = true));
                }
            }
        }
        public static async Task<bool> GetPairedDevices(string PiceaIP, string PiceaPort, string jwt)
        {
            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/paired_devices?site_id=";
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Falls ein selbstsigniertes Zertifikat verwendet wird
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                string responseBody = await response.Content.ReadAsStringAsync();
                                return ParseResponse(responseBody); // Verarbeitung der Antwort
                            case HttpStatusCode.BadRequest:
                                MessageBox.Show("Invalid request 400", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case HttpStatusCode.Forbidden:
                                MessageBox.Show("Authorization errors 403", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case (HttpStatusCode)432:
                                MessageBox.Show("Unpaired JWT: Jf the JWT that was used in the request was unpaired. 432", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case HttpStatusCode.NotFound:
                                MessageBox.Show("Not found, check IP. 404", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case HttpStatusCode.InternalServerError:
                                MessageBox.Show("Internal Error: Something in the API is broken. 500", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case HttpStatusCode.ServiceUnavailable:
                                MessageBox.Show("Site is offline. 503", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case (HttpStatusCode)514:
                                MessageBox.Show("Accessing a picea that was not yet fully provisioned. 514", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            case (HttpStatusCode)529:
                                MessageBox.Show("Server is overloaded and can not handle your request right now. 529", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            default:
                                MessageBox.Show($"Code {response.StatusCode.ToString()}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }
        private static bool ParseResponse(string responseBody)
        {
            try
            {
                Form1.PairedDevices = JsonConvert.DeserializeObject<List<PairedDevice>>(responseBody);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public static async Task<bool> RemoveDevice(string PiceaIP, string PiceaPort, string jwt, string deviceid)
        {
            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/paired_devices/{deviceid}?site_id=";
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // Falls ein selbstsigniertes Zertifikat verwendet wird
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                    try
                    {
                        HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                string responseBody = await response.Content.ReadAsStringAsync();
                                return true;

                            default:
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }

        private void tb_pairingcode_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
