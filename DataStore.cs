using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiceaWindowsFormsApp
{
    public class PairedDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime paired_since { get; set; }
    }

    public class ValWrapper<T> where T : struct
    {
        public T? val { get; set; }
    }

    public class OkWrapper<T> where T : struct
    {
        public ValWrapper<T> ok { get; set; }
    }

    public class PiceaData
    {
        public OkWrapper<double> autarky { get; set; }
        public OkWrapper<double> battery_input_power { get; set; }
        public OkWrapper<double> battery_output_power { get; set; }
        public OkWrapper<double> battery_power { get; set; }
        public OkWrapper<double> battery_state_of_charge { get; set; }
        public OkWrapper<double> co2_avoidance { get; set; }
        public OkWrapper<double> electrolyser_efficiency_electrical { get; set; }
        public OkWrapper<double> electrolyser_efficiency_total { get; set; }
        public OkWrapper<double> electrolyser_heating_power { get; set; }
        public OkWrapper<double> electrolyser_output_power { get; set; }
        public OkWrapper<double> extbattery_input_power { get; set; }
        public OkWrapper<double> extbattery_output_power { get; set; }
        public OkWrapper<double> extbattery_power { get; set; }
        public OkWrapper<double> fuelcell_efficiency_electrical { get; set; }
        public OkWrapper<double> fuelcell_efficiency_total { get; set; }
        public OkWrapper<double> fuelcell_heating_power { get; set; }
        public OkWrapper<double> fuelcell_input_power { get; set; }
        public OkWrapper<double> grid_export_power { get; set; }
        public OkWrapper<double> grid_import_power { get; set; }
        public OkWrapper<double> grid_power { get; set; }
        public OkWrapper<double> heat_contribution_power { get; set; }
        public OkWrapper<double> hot_water_tempearture { get; set; }
        public OkWrapper<double> house_to_picea_air_humidity { get; set; }
        public OkWrapper<double> house_to_picea_air_temperature { get; set; }
        public OkWrapper<double> household_input_power { get; set; }
        public OkWrapper<double> household_input_power_from_battery_picea { get; set; }
        public OkWrapper<double> household_input_power_from_extbattery { get; set; }
        public OkWrapper<double> household_input_power_from_grid { get; set; }
        public OkWrapper<double> household_input_power_from_hydrogen { get; set; }
        public OkWrapper<double> household_input_power_from_picea { get; set; }
        public OkWrapper<double> hydrogen_input_power_electrical { get; set; }
        public OkWrapper<double> hydrogen_output_power_electrical { get; set; }
        public OkWrapper<double> hydrogen_power_electrical { get; set; }
        public OkWrapper<double> hydrogen_state_of_charge { get; set; }
        public OkWrapper<bool> is_efficiencymode { get; set; }
        public OkWrapper<bool> is_equal_charge_from_net { get; set; }
        public OkWrapper<bool> is_equal_charge_ongoing { get; set; }
        public OkWrapper<bool> is_grid_offline { get; set; }
        public OkWrapper<bool> is_heater_alarm { get; set; }
        public OkWrapper<bool> is_heater_error { get; set; }
        public OkWrapper<bool> is_hydrogen_used_except_reserve { get; set; }
        public OkWrapper<bool> is_in_ten_percent_grid_feedin_mode { get; set; }
        public OkWrapper<bool> is_launchphase { get; set; }
        public OkWrapper<bool> is_solar_error { get; set; }
        public OkWrapper<bool> is_ventilation_calibration_now { get; set; }
        public OkWrapper<bool> is_ventilation_differential_pressure_alarm { get; set; }
        public OkWrapper<bool> is_ventilation_filter_full_alarm { get; set; }
        public OkWrapper<bool> is_ventilation_filter_full_warning { get; set; }
        public OkWrapper<bool> is_water_error { get; set; }
        public OkWrapper<int> max_compressor_blockage_duration { get; set; }
        public OkWrapper<double> outdoor_to_picea_air_temperature { get; set; }
        public OkWrapper<double> picea_to_house_air_temperature { get; set; }
        public OkWrapper<double> solar_output_power { get; set; }
        public OkWrapper<double> solar_output_power_ac { get; set; }
        public OkWrapper<double> solar_output_power_dc_total { get; set; }
        public OkWrapper<double> solar_output_power_to_battery { get; set; }
        public OkWrapper<double> solar_output_power_to_extbattery { get; set; }
        public OkWrapper<double> solar_output_power_to_grid { get; set; }
        public OkWrapper<double> solar_output_power_to_household { get; set; }
        public OkWrapper<double> solar_output_power_to_hydrogen { get; set; }
        public OkWrapper<int> ventilation_stage_real { get; set; }
    }

    // Wrapper für Werttypen
    public class ValueWrapperValueType<T> where T : struct
    {
        public T? value { get; set; }

        public ValueWrapperValueType()
        {
            value = null;
        }
    }

    // Wrapper für Referenztypen
    public class ValueWrapperReferenceType<T>
    {
        public T value { get; set; }

        public ValueWrapperReferenceType()
        {
            value = default;  // Standardwert (null für Referenztypen)
        }
    }

    public enum PiceaGeneration
    {
        P1 = 0,
        P2 = 1,
        P3 = 2
    }

    public enum FilterExchangeState
    {
        init = 0,
        filterchange_currently_possible = 1,
        filterchange_preparing = 2,
        ready_for_filterchange = 3,
        filterchange_verifying = 4,
        filterchange_verification_failed = 5,
        filterchange_currently_NOT_possible = 6
    }

    public enum ExtBatterySetup
    {
        none = 0,
        Ext_basic = 1,
        Ext_hybrid = 2
    }

    public class PiceaSettingData
    {
        public ValueWrapperReferenceType<DateTime> ventilation_night_time_end { get; set; } = new ValueWrapperReferenceType<DateTime>();
        public ValueWrapperReferenceType<DateTime> ventilation_night_time_start { get; set; } = new ValueWrapperReferenceType<DateTime>();
        public ValueWrapperReferenceType<string> picea_serial { get; set; } = new ValueWrapperReferenceType<string>();
        public ValueWrapperValueType<bool> has_solar_dc { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_ventilation_integrated { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> vacation_mode_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_no_hot_water_integrated { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> party_mode_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> is_surpluspower_grid_export_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> is_surpluspower_immersionheater_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_sgready { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_solar_ac_and_dc { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> ventilation_night_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> is_grid_connected_system { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_solar_ac { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_immersion_heater { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_no_heat_contribution { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> is_surpluspower_heatpump_enabled { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_differential_pressure_gauge { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperValueType<bool> has_grid_tax_feedin { get; set; } = new ValueWrapperValueType<bool>();
        public ValueWrapperReferenceType<ExtBatterySetup> ext_battery_setup { get; set; } = new ValueWrapperReferenceType<ExtBatterySetup>();
        public ValueWrapperReferenceType<PiceaGeneration> picea_generation { get; set; } = new ValueWrapperReferenceType<PiceaGeneration>();
        public ValueWrapperReferenceType<FilterExchangeState> filter_exchange_state { get; set; } = new ValueWrapperReferenceType<FilterExchangeState>();
        public ValueWrapperValueType<double> picea_count { get; set; } = new ValueWrapperValueType<double>();
        public ValueWrapperValueType<double> ventilation_temperature_target { get; set; } = new ValueWrapperValueType<double>();
        public ValueWrapperValueType<double> hydrogen_reserve { get; set; } = new ValueWrapperValueType<double>();
        public ValueWrapperValueType<int> ventilation_night_stage { get; set; } = new ValueWrapperValueType<int>();
        public ValueWrapperValueType<int> ventilation_stage_user { get; set; } = new ValueWrapperValueType<int>();
        public ValueWrapperValueType<int> compressor_blockage_duration { get; set; } = new ValueWrapperValueType<int>();
    }
}
