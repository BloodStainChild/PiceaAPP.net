using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using PiceaWindowsFormsApp.Properties;

namespace PiceaWindowsFormsApp
{
    public partial class Form1: Form
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        private static readonly HttpClient client = new HttpClient(handler);
        private static System.Windows.Forms.Timer updateTimer;
        private static System.Windows.Forms.Timer updateTimerSettings;
        public static PiceaData PD = new PiceaData();
        public static PiceaSettingData PSD = new PiceaSettingData();

        public static string PiceaIP = "";
        public static string PiceaPort = "8080";
        public static string jwt = "";
        public static string deviceid = "";
        public static List<PairedDevice> PairedDevices = new List<PairedDevice>();
        public static bool bfirstCheck = true;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Picea Info [" + System.Environment.MachineName + "]";
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000; // 1000 ms = 1 Sekunde
            updateTimer.Tick += async (s, ev) => await UpdateUI();

            updateTimerSettings = new System.Windows.Forms.Timer();
            updateTimerSettings.Interval = 1000; // 1000 ms = 1 Sekunde
            updateTimerSettings.Tick += async (s, ev) => await UpdateSettingUI();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (ConfigManager.LoadConfig(out PiceaIP, out PiceaPort, out jwt, out deviceid))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                GetDevices();
            }
            else
            {
                RegisterNewDevice();
            }
        }

        private void RegisterNewDevice()
        {
            using (Config configForm = new Config())
            {
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    GetDevices();
                }
                else
                {
                    MessageBox.Show("Registrierung abgebrochen oder fehlgeschlagen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private async void GetDevices() // Diese Methode muss als async markiert werden
        {
            // Asynchronen Code in einer async-Methode mit await aufrufen
            bool success = await Config.GetPairedDevices(PiceaIP, PiceaPort, jwt);
            if (!success)
            {
                RegisterNewDevice();
                return;
            }

            // Erfolgreiche Verarbeitung
            dgv_devices.DataSource = null;
            dgv_devices.Columns.Clear();  // Löscht eventuell vorhandene Spalten

            // ID-Spalte
            var idColumn = new DataGridViewTextBoxColumn();
            idColumn.Width = 50;
            idColumn.Name = "Id";
            idColumn.HeaderText = "Id";
            dgv_devices.Columns.Add(idColumn);

            // Name-Spalte
            var nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Width = 200;
            nameColumn.Name = "Name";
            nameColumn.HeaderText = "Name";
            dgv_devices.Columns.Add(nameColumn);

            // Connected Since-Spalte als Datumsstempel
            var connectedSinceColumn = new DataGridViewTextBoxColumn();
            connectedSinceColumn.Width = 200;
            connectedSinceColumn.Name = "ConnectedSince";
            connectedSinceColumn.HeaderText = "Connected Since";
            connectedSinceColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; // Datumsformat
            dgv_devices.Columns.Add(connectedSinceColumn);

            // Remove-Spalte mit Button
            var removeButtonColumn = new DataGridViewButtonColumn();
            removeButtonColumn.Width = 100;
            removeButtonColumn.Name = "Remove";
            removeButtonColumn.HeaderText = "Remove";
            removeButtonColumn.Text = "Remove";
            removeButtonColumn.UseColumnTextForButtonValue = true;
            dgv_devices.Columns.Add(removeButtonColumn);

            // Füllen des DataGridView mit Daten
            foreach (var device in PairedDevices)
            {
                dgv_devices.Rows.Add(device.Id, device.Name, device.paired_since);
            }
            // Markiere das aktuelle Gerät visuell (z.B. durch Farbe)
            foreach (DataGridViewRow row in dgv_devices.Rows)
            {
                var device = PairedDevices[row.Index];
                if (deviceid == device.Id.ToString())
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen; // Setze eine Hintergrundfarbe
                    row.DefaultCellStyle.Font = new Font(dgv_devices.Font, FontStyle.Bold); // Optional: Fett formatieren
                }
            }
            // Event-Handler für den "Remove"-Button hinzufügen
            dgv_devices.CellContentClick += async (sender, e) =>
            {
                if (e.ColumnIndex == dgv_devices.Columns["Remove"].Index && e.RowIndex >= 0)
                {
                    // Zeige eine Bestätigungsnachricht an
                    var result = MessageBox.Show("Möchten Sie dieses Gerät wirklich entfernen?", "Bestätigung", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Entferne das Gerät aus der Liste
                        var deviceToRemove = PairedDevices[e.RowIndex];
                        bool br = await Config.RemoveDevice(PiceaIP, PiceaPort, jwt, deviceToRemove.Id.ToString()); // Hier wird deviceToRemove.Id verwendet
                        if (br)
                        {
                            PairedDevices.Remove(deviceToRemove);
                            // Zeige die aktualisierten Daten im Grid
                            dgv_devices.Rows.RemoveAt(e.RowIndex);
                        }
                    }
                }
            };
            updateTimer.Start();
        }

        static async Task FetchData()
        {
            string query = "autarky,";
            query += "battery_input_power,battery_output_power,battery_power,battery_state_of_charge,";
            query += "co2_avoidance,";
            query += "electrolyser_efficiency_electrical,electrolyser_efficiency_total,electrolyser_heating_power,electrolyser_output_power,";
            query += "extbattery_input_power,extbattery_output_power,extbattery_power,";
            query += "fuelcell_efficiency_electrical,fuelcell_efficiency_total,fuelcell_heating_power,fuelcell_input_power,";
            query += "grid_export_power,grid_import_power,grid_power,";
            query += "heat_contribution_power,hot_water_tempearture,";
            query += "house_to_picea_air_humidity,house_to_picea_air_temperature,household_input_power,household_input_power_from_battery_picea,household_input_power_from_extbattery,";
            query += "household_input_power_from_grid,household_input_power_from_hydrogen,household_input_power_from_picea,";
            query += "hydrogen_input_power_electrical,hydrogen_output_power_electrical,hydrogen_power_electrical,hydrogen_state_of_charge,";
            query += "is_efficiencymode,is_equal_charge_from_net,is_equal_charge_ongoing,is_grid_offline,is_heater_alarm,is_heater_error,";
            query += "is_hydrogen_used_except_reserve,is_in_ten_percent_grid_feedin_mode,is_launchphase,is_solar_error,";
            query += "is_ventilation_calibration_now,is_ventilation_differential_pressure_alarm,is_ventilation_filter_full_alarm,is_ventilation_filter_full_warning,is_water_error,";
            query += "max_compressor_blockage_duration,";
            query += "outdoor_to_picea_air_temperature,picea_to_house_air_temperature,";
            query += "solar_output_power,solar_output_power_ac,solar_output_power_dc_total,solar_output_power_to_battery,";
            query += "solar_output_power_to_extbattery,solar_output_power_to_grid,solar_output_power_to_household,solar_output_power_to_hydrogen,";
            query += "ventilation_stage_real";

            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/data/query/{query}?site_id=";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    PD = JsonConvert.DeserializeObject<PiceaData>(responseBody);
                }
                else
                {
                    updateTimer.Stop();
                    MessageBox.Show(response.StatusCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                updateTimer.Stop();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateUI()
        {
            await FetchData(); // Hole die aktuellen Daten
            // UI-Elemente aktualisieren
            if (PD != null) // Falls Daten vorhanden sind
            {
                // Battery
                lbl_battery_soc.Text = (PD.battery_state_of_charge.ok.val ?? 0).ToString("F2") + " %";
                lbl_battery_p.Text = (PD.battery_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_battery_op.Text = (PD.battery_output_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_battery_ip.Text = (PD.battery_input_power.ok.val ?? 0).ToString("F2") + " W";
                // Ext Battery
                lbl_ext_battery_p.Text = (PD.extbattery_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_ext_battery_op.Text = (PD.extbattery_output_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_ext_battery_ip.Text = (PD.extbattery_input_power.ok.val ?? 0).ToString("F2") + " W";
                // Electrolyser
                lbl_electrolyser_ee.Text = (PD.electrolyser_efficiency_electrical.ok.val ?? 0).ToString("F2") + " %";
                lbl_electrolyser_et.Text = (PD.electrolyser_efficiency_total.ok.val ?? 0).ToString("F2") + " %";
                lbl_electrolyser_hp.Text = (PD.electrolyser_heating_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_electrolyser_op.Text = (PD.electrolyser_output_power.ok.val ?? 0).ToString("F2") + " W";
                // Fuelcell
                lbl_fuelcell_ee.Text = (PD.fuelcell_efficiency_electrical.ok.val ?? 0).ToString("F2") + " %";
                lbl_fuelcell_et.Text = (PD.fuelcell_efficiency_total.ok.val ?? 0).ToString("F2") + " %";
                lbl_fuelcell_hp.Text = (PD.fuelcell_heating_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_fuelcell_ip.Text = (PD.fuelcell_input_power.ok.val ?? 0).ToString("F2") + " W";
                // Grid
                lbl_grid_p.Text = (PD.grid_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_grid_ep.Text = (PD.grid_export_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_grid_ip.Text = (PD.grid_import_power.ok.val ?? 0).ToString("F2") + " W";
                // Hydrogen
                lbl_hydrogen_soc.Text = (PD.hydrogen_state_of_charge.ok.val ?? 0).ToString("F2") + " %";
                lbl_hydrogen_pe.Text = (PD.hydrogen_power_electrical.ok.val ?? 0).ToString("F2") + " W";
                lbl_hydrogen_ope.Text = (PD.hydrogen_output_power_electrical.ok.val ?? 0).ToString("F2") + " W";
                lbl_hydrogen_ipe.Text = (PD.hydrogen_input_power_electrical.ok.val ?? 0).ToString("F2") + " W";
                // Solar
                lbl_solar_op.Text = (PD.solar_output_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_opac.Text = (PD.solar_output_power_ac.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_opdct.Text = (PD.solar_output_power_dc_total.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_optb.Text = (PD.solar_output_power_to_battery.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_opteb.Text = (PD.solar_output_power_to_extbattery.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_optg.Text = (PD.solar_output_power_to_grid.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_opthh.Text = (PD.solar_output_power_to_household.ok.val ?? 0).ToString("F2") + " W";
                lbl_solar_opthg.Text = (PD.solar_output_power_to_hydrogen.ok.val ?? 0).ToString("F2") + " W";
                // Household
                lbl_household_ip.Text = (PD.household_input_power.ok.val ?? 0).ToString("F2") + " W";
                lbl_household_ipfb.Text = (PD.household_input_power_from_battery_picea.ok.val ?? 0).ToString("F2") + " W";
                lbl_household_ipfeb.Text = (PD.household_input_power_from_extbattery.ok.val ?? 0).ToString("F2") + " W";
                lbl_household_ipfg.Text = (PD.household_input_power_from_grid.ok.val ?? 0).ToString("F2") + " W";
                lbl_household_ipfh.Text = (PD.household_input_power_from_hydrogen.ok.val ?? 0).ToString("F2") + " W";
                lbl_household_ipfp.Text = (PD.household_input_power_from_picea.ok.val ?? 0).ToString("F2") + " W";
                // is info
                lbl_is_effi.Text = (PD.is_efficiencymode.ok.val ?? false).ToString();
                lbl_is_ecfn.Text = (PD.is_equal_charge_from_net.ok.val ?? false).ToString();
                lbl_is_eco.Text = (PD.is_equal_charge_ongoing.ok.val ?? false).ToString();
                lbl_is_go.Text = (PD.is_grid_offline.ok.val ?? false).ToString();
                lbl_is_ha.Text = (PD.is_heater_alarm.ok.val ?? false).ToString();
                lbl_is_he.Text = (PD.is_heater_error.ok.val ?? false).ToString();
                lbl_is_huer.Text = (PD.is_hydrogen_used_except_reserve.ok.val ?? false).ToString();
                lbl_is_itpgfm.Text = (PD.is_in_ten_percent_grid_feedin_mode.ok.val ?? false).ToString();
                lbl_is_launch.Text = (PD.is_launchphase.ok.val ?? false).ToString();
                lbl_is_se.Text = (PD.is_solar_error.ok.val ?? false).ToString();
                lbl_is_vcn.Text = (PD.is_ventilation_calibration_now.ok.val ?? false).ToString();
                lbl_is_vdpa.Text = (PD.is_ventilation_differential_pressure_alarm.ok.val ?? false).ToString();
                lbl_is_vffa.Text = (PD.is_ventilation_filter_full_alarm.ok.val ?? false).ToString();
                lbl_is_vffw.Text = (PD.is_ventilation_filter_full_warning.ok.val ?? false).ToString();
                lbl_is_we.Text = (PD.is_water_error.ok.val ?? false).ToString();

                lbl_temp_otp.Text = (PD.outdoor_to_picea_air_temperature.ok.val ?? 0).ToString("F2") + " °C";
                lbl_temp_pth.Text = (PD.picea_to_house_air_temperature.ok.val ?? 0).ToString("F2") + " °C";
                lbl_temp_htp.Text = (PD.house_to_picea_air_temperature.ok.val ?? 0).ToString("F2") + " °C";
                lbl_temp_hw.Text = (PD.hot_water_tempearture.ok.val ?? 0).ToString("F2") + " °C";

                lbl_autarky.Text = (PD.autarky.ok.val ?? 0).ToString("F2") + " %";
                lbl_co2_avoidance.Text = (PD.co2_avoidance.ok.val ?? 0).ToString("F2") + " g/h";
                lbl_hcp.Text = (PD.heat_contribution_power.ok.val ?? 0).ToString("F2") + "W";
                lbl_mcbd.Text = (PD.max_compressor_blockage_duration.ok.val ?? 0).ToString("F2") + "m";
                lbl_vsr.Text = (PD.ventilation_stage_real.ok.val ?? 0).ToString();
                lbl_htpah.Text = (PD.house_to_picea_air_humidity.ok.val ?? 0).ToString("F2") + " %";
            }

            if (bfirstCheck)
            {
                bfirstCheck = false;
                updateTimerSettings.Start();
            }
        }

        static async Task GetConfigSettings()
        {
            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/config?site_id=";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    PSD = JsonConvert.DeserializeObject<PiceaSettingData>(responseBody);
                }
                else
                {
                    updateTimerSettings.Stop();
                    MessageBox.Show(response.StatusCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                updateTimerSettings.Stop();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //
        public static async Task SendSettingsData(PiceaSettingData settings)
        {
            // Erstelle ein Dictionary für die zu sendenden Werte
            var dataToSend = new Dictionary<string, object>();

            // Füge die Werte aus PSD hinzu
            if (settings.party_mode_enabled?.value != null)
                dataToSend.Add("party_mode_enabled", new { value = settings.party_mode_enabled.value ?? false });
            if (settings.vacation_mode_enabled?.value != null)
                dataToSend.Add("vacation_mode_enabled", new { value = settings.vacation_mode_enabled.value ?? false });
            if (settings.has_grid_tax_feedin?.value != null)
                dataToSend.Add("has_grid_tax_feedin", new { value = settings.has_grid_tax_feedin.value ?? false });
            if (settings.ventilation_stage_user?.value != null)
                dataToSend.Add("ventilation_stage_user", new { value = settings.ventilation_stage_user.value });
            if (settings.ventilation_temperature_target?.value != null)
                dataToSend.Add("ventilation_temperature_target", new { value = settings.ventilation_temperature_target.value });
            if (settings.ventilation_night_enabled?.value != null)
                dataToSend.Add("ventilation_night_enabled", new { value = settings.ventilation_night_enabled.value ?? false });
            if (settings.ventilation_night_stage?.value != null)
                dataToSend.Add("ventilation_night_stage", new { value = settings.ventilation_night_stage.value ?? 0 });
            if (settings.ventilation_night_time_start?.value != null)
            {
                dataToSend.Add("ventilation_night_time_start", new
                {
                    value = settings.ventilation_night_time_start.value.ToString("HH:mm:ss")
                });
            }
            if (settings.ventilation_night_time_end?.value != null)
            {
                dataToSend.Add("ventilation_night_time_end", new
                {
                    value = settings.ventilation_night_time_end.value.ToString("HH:mm:ss")
                });
            }
            if (settings.is_surpluspower_heatpump_enabled?.value != null)
                dataToSend.Add("is_surpluspower_heatpump_enabled", new { value = settings.is_surpluspower_heatpump_enabled.value ?? false });
            if (settings.is_surpluspower_grid_export_enabled?.value != null)
                dataToSend.Add("is_surpluspower_grid_export_enabled", new { value = settings.is_surpluspower_grid_export_enabled.value ?? false });
            if (settings.is_surpluspower_immersionheater_enabled?.value != null)
                dataToSend.Add("is_surpluspower_immersionheater_enabled", new { value = settings.is_surpluspower_immersionheater_enabled.value ?? false });
            if (settings.is_grid_connected_system?.value != null)
                dataToSend.Add("is_grid_connected_system", new { value = settings.is_grid_connected_system.value ?? false });
            if (settings.has_no_hot_water_integrated?.value != null)
                dataToSend.Add("has_no_hot_water_integrated", new { value = settings.has_no_hot_water_integrated.value ?? false });
            if (settings.has_differential_pressure_gauge?.value != null)
                dataToSend.Add("has_differential_pressure_gauge", new { value = settings.has_differential_pressure_gauge.value ?? false });
            if (settings.hydrogen_reserve?.value != null)
                dataToSend.Add("hydrogen_reserve", new { value = settings.hydrogen_reserve.value });
            if (settings.ext_battery_setup?.value != null)
                dataToSend.Add("ext_battery_setup", new { value = settings.ext_battery_setup.value.ToString() });
            if (settings.filter_exchange_state?.value != null)
                dataToSend.Add("filter_exchange_state", new { value = settings.filter_exchange_state.value.ToString() });
            if (settings.compressor_blockage_duration?.value != null)
                dataToSend.Add("compressor_blockage_duration", new { value = settings.compressor_blockage_duration.value ?? 0 });

            // Sende die aktualisierten Einstellungen an die API
            await UpdateConfigSettings(dataToSend);
        }

        static async Task UpdateConfigSettings(Dictionary<string, object> data)
        {
            string apiUrl = $"https://{PiceaIP}:{PiceaPort}/picea/v1/config?site_id=";

            try
            {
                // Konvertiere das Dictionary in JSON
                string jsonData = JsonConvert.SerializeObject(data);

                // Erstelle den HttpContent (hier als StringContent)
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Führe die POST-Anforderung aus
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Antwort erfolgreich
                    MessageBox.Show("Settings updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Fehlerbehandlung
                    updateTimerSettings.Stop();
                    MessageBox.Show(response.StatusCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                updateTimerSettings.Stop();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveSettings()
        {
            // Hier kannst du das gespeicherte PiceaSettingData-Objekt verwenden
            await SendSettingsData(PSD);
            b_update.Enabled = true;

            if (updateTimerSettings.Enabled) return;
            b_refresh.Enabled = false;
            updateTimerSettings.Start();
        }

        private void b_update_Click(object sender, EventArgs e)
        {
            b_update.Enabled = false;
            SaveSettings();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateTimerSettings.Start();
        }

        private void b_refresh_Click(object sender, EventArgs e)
        {
            if (updateTimerSettings.Enabled) return;
            b_refresh.Enabled = false;
            updateTimerSettings.Start();
        }

        private async Task UpdateSettingUI()
        {
            await GetConfigSettings(); // Hole die aktuellen Daten

            cb_partyMode.Checked = PSD.party_mode_enabled?.value ?? false;
            cb_vacationMode.Checked = PSD.vacation_mode_enabled?.value ?? false;
            ch_hasgridtaxfeedin.Checked = PSD.has_grid_tax_feedin?.value ?? false;
            cb_igcs.Checked = PSD.is_grid_connected_system?.value ?? false;
            cb_hnhwi.Checked = PSD.has_no_hot_water_integrated?.value ?? false;
            cb_hdpg.Checked = PSD.has_differential_pressure_gauge?.value ?? false;
            nud_hr.Value = Convert.ToDecimal(PSD.hydrogen_reserve?.value ?? 0);

            // Ventilation
            cb_ventilation_stage.SelectedIndex = PSD.ventilation_stage_user?.value ?? 0;
            nud_vent_temp_target.Value = Convert.ToDecimal(PSD.ventilation_temperature_target?.value ?? 18);
            cb_vne.Checked = PSD.ventilation_night_enabled.value ?? false;
            cb_ventilation_night_stage.SelectedIndex = PSD.ventilation_night_stage?.value ?? 0;
            dtp_vent_night_start.Value = PSD.ventilation_night_time_start?.value ?? DateTime.Now;
            dtp_vent_night_stop.Value = PSD.ventilation_night_time_end?.value ?? DateTime.Now;

            // Surpluspower
            cb_sh.Checked = PSD.is_surpluspower_heatpump_enabled?.value ?? false;
            cb_sge.Checked = PSD.is_surpluspower_grid_export_enabled?.value ?? false;
            cb_si.Checked = PSD.is_surpluspower_immersionheater_enabled?.value ?? false;
            cb_ext_bat.SelectedItem = PSD.ext_battery_setup.value.ToString().Replace("_", "").ToLower();
            cb_filterexstate.SelectedIndex = ((int)PSD.filter_exchange_state?.value);

            int max_block = PD.max_compressor_blockage_duration?.ok?.val ?? 0;
            int block_dur = PSD.compressor_blockage_duration.value ?? 0;

            b_comp_block_0.BackColor = Color.Transparent;
            b_comp_block_1.BackColor = Color.Transparent;
            b_comp_block_4.BackColor = Color.Transparent;
            b_comp_block_24.BackColor = Color.Transparent;

            //120, 240, 1440
            if (max_block >= 0)
                b_comp_block_0.Enabled = true;
            if (max_block >= 120)
                b_comp_block_1.Enabled = true;
            if (max_block >= 240)
                b_comp_block_4.Enabled = true;
            if (max_block >= 1440)
                b_comp_block_24.Enabled = true;

            if (block_dur == 0)
                b_comp_block_0.BackColor = Color.Green;
            if (block_dur == 120)
                b_comp_block_1.BackColor = Color.Green;
            if (block_dur == 240)
                b_comp_block_4.BackColor = Color.Green;
            if (block_dur == 1440)
                b_comp_block_24.BackColor = Color.Green;

            // editable":false
            tb_pigen.Text = PSD.picea_generation.value.ToString();
            tb_picount.Text = PSD.picea_count.value.ToString();
            tb_picserial.Text = PSD.picea_serial.value.ToString();
            cb_hsdc.Checked = PSD.has_solar_dc.value ?? false;
            cb_hsac.Checked = PSD.has_solar_ac.value ?? false;
            cb_hsacadc.Checked = PSD.has_solar_ac_and_dc.value ?? false;
            cb_hsg.Checked = PSD.has_sgready.value ?? false;
            cb_hnhc.Checked = PSD.has_no_heat_contribution.value ?? false;
            cb_himh.Checked = PSD.has_immersion_heater.value ?? false;
            cb_hvi.Checked = PSD.has_ventilation_integrated.value ?? false;

            updateTimerSettings.Stop();
            b_refresh.Enabled = true;
        }

        private void cb_partyMode_CheckedChanged(object sender, EventArgs e)
        {
            PSD.party_mode_enabled.value = cb_partyMode.Checked;
        }

        private void cb_vacationMode_CheckedChanged(object sender, EventArgs e)
        {
            PSD.vacation_mode_enabled.value = cb_vacationMode.Checked;
        }

        private void ch_hasgridtaxfeedin_CheckedChanged(object sender, EventArgs e)
        {
            PSD.has_grid_tax_feedin.value = ch_hasgridtaxfeedin.Checked;
        }

        private void cb_igcs_CheckedChanged(object sender, EventArgs e)
        {
            PSD.is_grid_connected_system.value = cb_igcs.Checked;
        }

        private void cb_hnhwi_CheckedChanged(object sender, EventArgs e)
        {
            PSD.has_no_hot_water_integrated.value = cb_hnhwi.Checked;
        }

        private void cb_hdpg_CheckedChanged(object sender, EventArgs e)
        {
            PSD.has_differential_pressure_gauge.value = cb_hdpg.Checked;
        }

        private void nud_hr_ValueChanged(object sender, EventArgs e)
        {
            PSD.hydrogen_reserve.value = Convert.ToDouble(nud_hr.Value);
        }

        private void cb_ext_bat_SelectedIndexChanged(object sender, EventArgs e)
        {
            string enumString = cb_ext_bat.SelectedItem.ToString();
            if (Enum.TryParse<ExtBatterySetup>(enumString, out var outValue))
            {
                PSD.ext_battery_setup.value = outValue;
            }
            else
            {
                MessageBox.Show(outValue.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cb_filterexstate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string enumString = cb_filterexstate.SelectedItem.ToString();
            if (Enum.TryParse<FilterExchangeState>(enumString, out var outValue))
            {
                PSD.filter_exchange_state.value = outValue;
            }
            else
            {
                MessageBox.Show(outValue.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cb_sh_CheckedChanged(object sender, EventArgs e)
        {
            PSD.is_surpluspower_heatpump_enabled.value = cb_sh.Checked;
        }

        private void cb_sge_CheckedChanged(object sender, EventArgs e)
        {
            PSD.is_surpluspower_grid_export_enabled.value = cb_sge.Checked;
        }

        private void cb_si_CheckedChanged(object sender, EventArgs e)
        {
            PSD.is_surpluspower_immersionheater_enabled.value = cb_si.Checked;
        }

        private void cb_ventilation_stage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selval = cb_ventilation_stage.SelectedIndex;
            if (selval >= 0 && selval <= 5)
                PSD.ventilation_stage_user.value = selval;
        }

        private void nud_vent_temp_target_ValueChanged(object sender, EventArgs e)
        {
            PSD.ventilation_temperature_target.value = Convert.ToDouble(nud_vent_temp_target.Value);
        }

        private void cb_vne_CheckedChanged(object sender, EventArgs e)
        {
            PSD.ventilation_night_enabled.value  = cb_vne.Checked;
        }

        private void cb_ventilation_night_stage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selval = cb_ventilation_night_stage.SelectedIndex;
            if (selval >= 0 && selval <= 5)
                PSD.ventilation_night_stage.value = selval;
        }

        private void dtp_vent_night_start_ValueChanged(object sender, EventArgs e)
        {
             PSD.ventilation_night_time_start.value = dtp_vent_night_start.Value;
        }

        private void dtp_vent_night_stop_ValueChanged(object sender, EventArgs e)
        {
             PSD.ventilation_night_time_end.value = dtp_vent_night_stop.Value;
        }

        private void b_comp_block_1_Click(object sender, EventArgs e)
        {
            PSD.compressor_blockage_duration.value = 120;
            b_comp_block_1.BackColor = Color.GreenYellow;
        }

        private void b_comp_block_4_Click(object sender, EventArgs e)
        {
            PSD.compressor_blockage_duration.value = 240;
            b_comp_block_4.BackColor = Color.GreenYellow;
        }

        private void b_comp_block_24_Click(object sender, EventArgs e)
        {
            PSD.compressor_blockage_duration.value = 1440;
            b_comp_block_24.BackColor = Color.GreenYellow;
        }

        private void b_comp_block_0_Click(object sender, EventArgs e)
        {
            PSD.compressor_blockage_duration.value = 0;
            b_comp_block_0.BackColor = Color.GreenYellow;
        }

        private void cb_MouseDown_Block(object sender, MouseEventArgs e)
        {
            ((CheckBox)sender).CheckState = ((CheckBox)sender).CheckState;
            ((CheckBox)sender).Enabled = false;
            ((CheckBox)sender).Enabled = true;
        }

    }
}
