using LedCSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_Dota2
{
    public enum PercentEffectType
    {
        AllAtOnce = 0,
        Progressive = 1,
        Progressive_Gradual = 2
    }

    public class Configuration
    {
        public Configuration()
        {
            //Effects
            //// Background
            bg_team_enabled = true;
            radiant_color = Color.FromArgb(0, 140, 30);
            dire_color = Color.FromArgb(200, 0, 0);
            ambient_color = Color.FromArgb(140, 190, 230);
            bg_enable_dimming = true;
            bg_dim_after = 15; //seconds
            bg_respawn_glow = true;
            bg_respawn_glow_color = Color.FromArgb(255, 255, 255);
            bg_display_killstreaks = true;
            bg_killstreakcolors = new List<Color>() {Color.FromArgb(0, 0, 0), //No Streak
                                            Color.FromArgb(0, 0, 0), //First kill
                                            Color.FromArgb(255, 255, 255), //Double Kill
                                            Color.FromArgb(0, 255, 0), //Killing Spree
                                            Color.FromArgb(128, 0, 255),  //Dominating
                                            Color.FromArgb(255, 100, 100),  //Mega Kill
                                            Color.FromArgb(255, 80, 0),  //Unstoppable
                                            Color.FromArgb(130, 180, 130),  //Wicked Sick
                                            Color.FromArgb(255, 0, 255),  //Monster Kill
                                            Color.FromArgb(255, 0, 0),  //Godlike
                                            Color.FromArgb(255, 80, 0)  //Godlike+
                                            };
            bg_peripheral_use = true;


            //// Health
            health_enabled = true;
            healthy_color = Color.FromArgb(0, 255, 0);
            hurt_color = Color.FromArgb(0, 60, 0);
            health_effect_type = PercentEffectType.Progressive_Gradual;
            healthKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.F1, Devices.DeviceKeys.F2, Devices.DeviceKeys.F3, Devices.DeviceKeys.F4, Devices.DeviceKeys.F5, Devices.DeviceKeys.F6, Devices.DeviceKeys.F7, Devices.DeviceKeys.F8, Devices.DeviceKeys.F9, Devices.DeviceKeys.F10, Devices.DeviceKeys.F11, Devices.DeviceKeys.F12 };

            //// Mana
            mana_enabled = true;
            mana_color = Color.FromArgb(0, 125, 255);
            nomana_color = Color.FromArgb(0, 0, 60);
            mana_effect_type = PercentEffectType.Progressive_Gradual;
            manaKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.ONE, Devices.DeviceKeys.TWO, Devices.DeviceKeys.THREE, Devices.DeviceKeys.FOUR, Devices.DeviceKeys.FIVE, Devices.DeviceKeys.SIX, Devices.DeviceKeys.SEVEN, Devices.DeviceKeys.EIGHT, Devices.DeviceKeys.NINE, Devices.DeviceKeys.ZERO, Devices.DeviceKeys.MINUS, Devices.DeviceKeys.EQUALS };

            mimic_respawn_timer = true;
            mimic_respawn_timer_color = Color.FromArgb(255, 0, 0);
            mimic_respawn_timer_respawning_color = Color.FromArgb(255, 170, 0);

            //// Abilities
            abilitykeys_enabled = true;
            ability_can_use_color = Color.FromArgb(0, 255, 0);
            ability_can_not_use_color = Color.FromArgb(255, 0, 0);
            ability_keys =  new List<Devices.DeviceKeys>() { Devices.DeviceKeys.Q, Devices.DeviceKeys.W, Devices.DeviceKeys.E, Devices.DeviceKeys.D, Devices.DeviceKeys.F, Devices.DeviceKeys.R };

            //// Items
            items_enabled = true;
            items_empty_color = Color.FromArgb(0, 0, 0);
            items_on_cooldown_color = Color.FromArgb(0, 0, 0);
            items_no_charges_color = Color.FromArgb(150, 150, 150);
            items_color = Color.FromArgb(255, 255, 255);
            items_use_item_color = true;
            items_keys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.INSERT, Devices.DeviceKeys.HOME, Devices.DeviceKeys.PAGE_UP, Devices.DeviceKeys.DELETE, Devices.DeviceKeys.END, Devices.DeviceKeys.PAGE_DOWN, Devices.DeviceKeys.NUM_SEVEN, Devices.DeviceKeys.NUM_EIGHT, Devices.DeviceKeys.NUM_NINE, Devices.DeviceKeys.NUM_FOUR, Devices.DeviceKeys.NUM_FIVE, Devices.DeviceKeys.NUM_SIX };

            //// Static Keys
            statickeys_enabled = true;
            statickeys_color = Color.FromArgb(255, 220, 0);
            staticKeys = new List<Devices.DeviceKeys>();
            statickeys_2_color = Color.FromArgb(255, 0, 255);
            staticKeys_2 = new List<Devices.DeviceKeys>();

            //// Typing Keys
            typing_enabled = true;
            typing_color = Color.FromArgb(0, 255, 0);
            typingKeys = new List<Devices.DeviceKeys>() { Devices.DeviceKeys.TILDE, Devices.DeviceKeys.ONE, Devices.DeviceKeys.TWO, Devices.DeviceKeys.THREE, Devices.DeviceKeys.FOUR, Devices.DeviceKeys.FIVE, Devices.DeviceKeys.SIX, Devices.DeviceKeys.SEVEN, Devices.DeviceKeys.EIGHT, Devices.DeviceKeys.NINE, Devices.DeviceKeys.ZERO, Devices.DeviceKeys.MINUS, Devices.DeviceKeys.EQUALS, Devices.DeviceKeys.BACKSPACE, 
                                                    Devices.DeviceKeys.TAB, Devices.DeviceKeys.Q, Devices.DeviceKeys.W, Devices.DeviceKeys.E, Devices.DeviceKeys.R, Devices.DeviceKeys.T, Devices.DeviceKeys.Y, Devices.DeviceKeys.U, Devices.DeviceKeys.I, Devices.DeviceKeys.O, Devices.DeviceKeys.P, Devices.DeviceKeys.CLOSE_BRACKET, Devices.DeviceKeys.OPEN_BRACKET, Devices.DeviceKeys.BACKSLASH, 
                                                    Devices.DeviceKeys.CAPS_LOCK, Devices.DeviceKeys.A, Devices.DeviceKeys.S, Devices.DeviceKeys.D, Devices.DeviceKeys.F, Devices.DeviceKeys.G, Devices.DeviceKeys.H, Devices.DeviceKeys.J, Devices.DeviceKeys.K, Devices.DeviceKeys.L, Devices.DeviceKeys.SEMICOLON, Devices.DeviceKeys.APOSTROPHE, Devices.DeviceKeys.HASHTAG, Devices.DeviceKeys.ENTER, 
                                                    Devices.DeviceKeys.LEFT_SHIFT, Devices.DeviceKeys.BACKSLASH_UK, Devices.DeviceKeys.Z, Devices.DeviceKeys.X, Devices.DeviceKeys.C, Devices.DeviceKeys.V, Devices.DeviceKeys.B, Devices.DeviceKeys.N, Devices.DeviceKeys.M, Devices.DeviceKeys.COMMA, Devices.DeviceKeys.PERIOD, Devices.DeviceKeys.FORWARD_SLASH, Devices.DeviceKeys.RIGHT_SHIFT,
                                                    Devices.DeviceKeys.LEFT_CONTROL, Devices.DeviceKeys.LEFT_WINDOWS, Devices.DeviceKeys.LEFT_ALT, Devices.DeviceKeys.SPACE, Devices.DeviceKeys.RIGHT_ALT, Devices.DeviceKeys.RIGHT_WINDOWS, Devices.DeviceKeys.APPLICATION_SELECT, Devices.DeviceKeys.RIGHT_CONTROL,
                                                    Devices.DeviceKeys.ARROW_UP, Devices.DeviceKeys.ARROW_LEFT, Devices.DeviceKeys.ARROW_DOWN, Devices.DeviceKeys.ARROW_RIGHT, Devices.DeviceKeys.ESC
                                                  };
            
        }
        //Effects
        //// Background
        public bool bg_team_enabled;
        public Color radiant_color;
        public Color dire_color;
        public Color ambient_color;
        public bool bg_enable_dimming;
        public int bg_dim_after;
        public bool bg_respawn_glow;
        public Color bg_respawn_glow_color;
        public bool bg_display_killstreaks;
        public List<Color> bg_killstreakcolors;
        public bool bg_peripheral_use;

        //// Health
        public bool health_enabled;
        public Color healthy_color;
        public Color hurt_color;
        public PercentEffectType health_effect_type;
        public List<Devices.DeviceKeys> healthKeys { get; set; }

        //// Mana
        public bool mana_enabled;
        public Color mana_color;
        public Color nomana_color;
        public PercentEffectType mana_effect_type;
        public List<Devices.DeviceKeys> manaKeys { get; set; }

        public bool mimic_respawn_timer;
        public Color mimic_respawn_timer_color;
        public Color mimic_respawn_timer_respawning_color;

        ////Abilities
        public bool abilitykeys_enabled;
        public Color ability_can_use_color;
        public Color ability_can_not_use_color;
        public List<Devices.DeviceKeys> ability_keys { get; set; }

        ////Items
        public bool items_enabled;
        public Color items_empty_color;
        public Color items_on_cooldown_color;
        public Color items_no_charges_color;
        public Color items_color;
        public bool items_use_item_color;
        public List<Devices.DeviceKeys> items_keys { get; set; }

        //// Static Keys
        public bool statickeys_enabled;
        public Color statickeys_color;
        public List<Devices.DeviceKeys> staticKeys { get; set; }
        public Color statickeys_2_color;
        public List<Devices.DeviceKeys> staticKeys_2 { get; set; }

        ////Typing Keys
        public bool typing_enabled;
        public Color typing_color;
        public List<Devices.DeviceKeys> typingKeys { get; set; }
    }

    public class ConfigManager
    {
        private const string ConfigExtension = ".json";

        public static Configuration Load(string fileNameWithoutExtension)
        {
            var configPath = fileNameWithoutExtension + ConfigExtension;

            if (!File.Exists(configPath))
                return CreateDefaultConfigurationFile(fileNameWithoutExtension);

            string content = File.ReadAllText(configPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<Configuration>(content, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public static void Save(string fileNameWithoutExtension, Configuration configuration)
        {
            var configPath = fileNameWithoutExtension + ConfigExtension;
            string content = JsonConvert.SerializeObject(configuration, Formatting.Indented);

            File.WriteAllText(configPath, content, Encoding.UTF8);
        }

        private static Configuration CreateDefaultConfigurationFile(string fileNameWithoutExtension)
        {
            Configuration config = new Configuration();
            var configData = JsonConvert.SerializeObject(config, Formatting.Indented);
            var configPath = fileNameWithoutExtension + ConfigExtension;

            File.WriteAllText(configPath, configData, Encoding.UTF8);

            return config;
        }
    }
}
