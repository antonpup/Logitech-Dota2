using Dota2GSI.Nodes;
using Logitech_Dota2.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Logitech_Dota2
{
    public class GameEventHandler
    {
        private bool isInitialized = false;

        private int lastUpdate = 0;
        private int updateRate = 1; //1 second
        private bool isForced = false;
        private static Dictionary<Devices.DeviceKeys, Color> keyColors = new Dictionary<Devices.DeviceKeys, Color>();
        private static Dictionary<Devices.DeviceKeys, Color> final_keyColors = new Dictionary<Devices.DeviceKeys, Color>();

        private bool keyboard_updated = false;
        private Timer update_timer;

        private Stopwatch general_timer = Stopwatch.StartNew();
        private Random randomizer = new Random();

        private bool preview_mode = false;

        private bool isPlayingKillStreakAnimation = false;
        private double ks_blendamount = 0.0;
        private long ks_duration = 4000;
        private long ks_end_time = 0;

        private bool isDimming = false;
        private double dim_value = 1.0;
        private int dim_bg_at = 15;

        //Game Integration stuff
        int mapTime = 0;
        PlayerTeam current_team = PlayerTeam.Undefined;
        bool isAlive = true;
        int respawnTime = 0;
        int health = 0;
        int health_max = 100;
        int mana = 0;
        int mana_max = 100;
        int kills = 0;
        int player_killstreak = 0;
        Abilities abilities;
        Items items;
        PlayerActivity current_activity = PlayerActivity.Undefined;

        Dictionary<string, Color> item_colors = new Dictionary<string, Color>()
        {
            { "empty", Color.FromArgb(0, 0, 0) },
            { "item_aegis", Color.FromArgb(255, 240, 200) },
            { "item_courier", Color.FromArgb(180, 120, 50) },
            { "item_boots_of_elves", Color.FromArgb(115, 140, 50) },
            { "item_belt_of_strength", Color.FromArgb(160, 130, 75) },
            { "item_blade_of_alacrity", Color.FromArgb(130, 150, 140) },
            { "item_blades_of_attack", Color.FromArgb(160, 115, 40) },
            { "item_blink", Color.FromArgb(100, 200, 200) },
            { "item_boots", Color.FromArgb(130, 95, 60) },
            { "item_bottle", Color.FromArgb(35, 155, 185) },
            { "item_broadsword", Color.FromArgb(190, 190, 180) },
            { "item_chainmail", Color.FromArgb(150, 140, 130) },
            { "item_cheese", Color.FromArgb(235, 240, 70) },
            { "item_circlet", Color.FromArgb(220, 160, 30) },
            { "item_clarity", Color.FromArgb(150, 210, 250) },
            { "item_claymore", Color.FromArgb(190, 190, 180) },
            { "item_cloak", Color.FromArgb(115, 40, 20) },
            { "item_demon_edge", Color.FromArgb(30, 240, 150) },
            { "item_dust", Color.FromArgb(220, 180, 245) },
            { "item_eagle", Color.FromArgb(155, 180, 110) },
            { "item_enchanted_mango", Color.FromArgb(240,155,30)},
            { "item_energy_booster", Color.FromArgb(75, 77, 235) },
            { "item_faerie_fire", Color.FromArgb(190, 255, 100) },
            { "item_flying_courier", Color.FromArgb(180, 120, 50) },
            { "item_gauntlets", Color.FromArgb(190, 186, 177) },
            { "item_gem", Color.FromArgb(86, 212, 83) },
            { "item_ghost", Color.FromArgb(249, 252, 120) },
            { "item_gloves", Color.FromArgb(65, 212, 80) },
            { "item_flask", Color.FromArgb(6, 202, 17) },
            { "item_helm_of_iron_will", Color.FromArgb(238, 200, 53) },
            { "item_hyperstone", Color.FromArgb(56, 238, 188) },
            { "item_branches", Color.FromArgb(226, 184, 24) },
            { "item_javelin", Color.FromArgb(172, 165, 160) },
            { "item_magic_stick", Color.FromArgb(120, 215, 173) },
            { "item_mantle", Color.FromArgb(123, 182, 234) },
            { "item_mithril_hammer", Color.FromArgb(193, 210, 195) },
            { "item_lifesteal", Color.FromArgb(149, 139, 134) },
            { "item_mystic_staff", Color.FromArgb(204, 228, 158) },
            { "item_ward_observer", Color.FromArgb(196, 169, 55) },
            { "item_ogre_axe", Color.FromArgb(161, 62, 54) },
            { "item_orb_of_venom", Color.FromArgb(237, 206, 38) },
            { "item_platemail", Color.FromArgb(119, 33, 10) },
            { "item_point_booster", Color.FromArgb(226 ,97, 234) },
            { "item_quarterstaff", Color.FromArgb(130, 98, 24) },
            { "item_quelling_blade", Color.FromArgb(188, 188, 183) },
            { "item_reaver", Color.FromArgb(171, 142, 35) },
            { "item_ring_of_health", Color.FromArgb(166, 48, 4) },
            { "item_ring_of_protection", Color.FromArgb(208, 104, 6) },
            { "item_ring_of_regen", Color.FromArgb(214, 59, 62) },
            { "item_robe", Color.FromArgb(115, 183, 203) },
            { "item_relic", Color.FromArgb(225, 165, 17) },
            { "item_sobi_mask", Color.FromArgb(126, 114, 217) },
            { "item_ward_sentry", Color.FromArgb(107, 207, 214) },
            { "item_shadow_amulet", Color.FromArgb(201, 120, 191) },
            { "item_slippers", Color.FromArgb(99, 188, 25) },
            { "item_smoke_of_deceit", Color.FromArgb(180, 122, 243) },
            { "item_staff_of_wizardry", Color.FromArgb(34, 147, 207) },
            { "item_stout_shield", Color.FromArgb(182, 75, 19) },
            { "item_talisman_of_evasion", Color.FromArgb(181, 250, 238) },
            { "item_tango", Color.FromArgb(80, 243, 194) },
            { "item_tango_single", Color.FromArgb(23, 201, 87) },
            { "item_tpscroll", Color.FromArgb(212, 182, 139) },
            { "item_ultimate_orb", Color.FromArgb(238, 240, 229) },
            { "item_vitality_booster", Color.FromArgb(219, 55, 64) },
            { "item_void_stone", Color.FromArgb(40, 242, 253) },
            { "item_abyssal_blade", Color.FromArgb(123, 94, 99) },
            { "item_recipe_abyssal_blade", Color.FromArgb(218, 177, 103) },
            { "item_aether_lens", Color.FromArgb(97, 241, 221) },
            { "item_recipe_aether_lens", Color.FromArgb(218, 177, 103) },
            { "item_ultimate_scepter", Color.FromArgb(70, 142, 244) },
            { "item_recipe_ultimate_scepter", Color.FromArgb(218, 177, 103) },
            { "item_arcane_boots", Color.FromArgb(67, 170, 218) },
            { "item_recipe_arcane_boots", Color.FromArgb(218, 177, 103) },
            { "item_armlet", Color.FromArgb(229, 165, 172) },
            { "item_recipe_armlet", Color.FromArgb(218, 177, 103) },
            { "item_assault", Color.FromArgb(56, 196, 202) },
            { "item_recipe_assault", Color.FromArgb(218, 177, 103) },
            { "item_bfury", Color.FromArgb(231, 165, 89) },
            { "item_recipe_bfury", Color.FromArgb(218, 177, 103) },
            { "item_black_king_bar", Color.FromArgb(200, 155, 13) },
            { "item_recipe_black_king_bar", Color.FromArgb(218, 177, 103) },
            { "item_blade_mail", Color.FromArgb(53, 33, 55) },
            { "item_recipe_blade_mail", Color.FromArgb(218, 177, 103) },
            { "item_bloodstone", Color.FromArgb(227, 33, 46) },
            { "item_recipe_bloodstone", Color.FromArgb(218, 177, 103) },
            { "item_travel_boots", Color.FromArgb(188, 140, 82) },
            { "item_travel_boots_2", Color.FromArgb(226, 189, 123) },
            { "item_recipe_travel_boots", Color.FromArgb(218, 177, 103) },
            { "item_bracer", Color.FromArgb(189, 95, 19) },
            { "item_recipe_bracer", Color.FromArgb(218, 177, 103) },
            { "item_buckler", Color.FromArgb(213, 177, 75) },
            { "item_recipe_buckler", Color.FromArgb(218, 177, 103) },
            { "item_butterfly", Color.FromArgb(226, 243, 142) },
            { "item_recipe_butterfly", Color.FromArgb(218, 177, 103) },
            { "item_crimson_guard", Color.FromArgb(189, 113, 2) },
            { "item_recipe_crimson_guard", Color.FromArgb(218, 177, 103) },
            { "item_lesser_crit", Color.FromArgb(234, 50, 51) },
            { "item_recipe_lesser_crit", Color.FromArgb(218, 177, 103) },
            { "item_greater_crit", Color.FromArgb(254, 88, 77) },
            { "item_recipe_greater_crit", Color.FromArgb(218, 177, 103) },
            { "item_dagon", Color.FromArgb(235, 170, 18) },
            { "item_dagon_2", Color.FromArgb(236, 190, 45) },
            { "item_dagon_3", Color.FromArgb(252, 211, 94) },
            { "item_dagon_4", Color.FromArgb(251, 216, 78) },
            { "item_dagon_5", Color.FromArgb(255, 214, 109) },
            { "item_recipe_dagon", Color.FromArgb(218, 177, 103) },
            { "item_desolator", Color.FromArgb(216, 15, 14) },
            { "item_recipe_desolator", Color.FromArgb(218, 177, 103) },
            { "item_diffusal_blade", Color.FromArgb(112, 44, 199) },
            { "item_diffusal_blade_2", Color.FromArgb(189, 39, 170) },
            { "item_recipe_diffusal_blade", Color.FromArgb(218, 177, 103) },
            { "item_dragon_lance", Color.FromArgb(141, 26, 32) },
            { "item_recipe_dragon_lance", Color.FromArgb(218, 177, 103) },
            { "item_ancient_janggo", Color.FromArgb(168, 186, 97) },
            { "item_recipe_ancient_janggo", Color.FromArgb(218, 177, 103) },
            { "item_ethereal_blade", Color.FromArgb(209, 222, 73) },
            { "item_recipe_ethereal_blade", Color.FromArgb(218, 177, 103) },
            { "item_cyclone", Color.FromArgb(231, 229, 223) },
            { "item_recipe_cyclone", Color.FromArgb(218, 177, 103) },
            { "item_skadi", Color.FromArgb(42, 207, 204) },
            { "item_recipe_skadi", Color.FromArgb(218, 177, 103) },
            { "item_force_staff", Color.FromArgb(123, 225, 187) },
            { "item_recipe_force_staff", Color.FromArgb(218, 177, 103) },
            { "item_glimmer_cape", Color.FromArgb(50, 77, 184) },
            { "item_recipe_glimmer_cape", Color.FromArgb(218, 177, 103) },
            { "item_guardian_greaves", Color.FromArgb(73, 169, 128) },
            { "item_recipe_guardian_greaves", Color.FromArgb(218, 177, 103) },
            { "item_hand_of_midas", Color.FromArgb(222, 154, 21) },
            { "item_recipe_hand_of_midas", Color.FromArgb(218, 177, 103) },
            { "item_headdress", Color.FromArgb(53, 110, 83) },
            { "item_recipe_headdress", Color.FromArgb(218, 177, 103) },
            { "item_heart", Color.FromArgb(196, 51, 31) },
            { "item_recipe_heart", Color.FromArgb(218, 177, 103) },
            { "item_heavens_halberd", Color.FromArgb(235, 240, 218) },
            { "item_recipe_heavens_halberd", Color.FromArgb(218, 177, 103) },
            { "item_helm_of_the_dominator", Color.FromArgb(114, 57, 6) },
            { "item_recipe_helm_of_the_dominator", Color.FromArgb(218, 177, 103) },
            { "item_hood_of_defiance", Color.FromArgb(133, 84, 106) },
            { "item_recipe_hood_of_defiance", Color.FromArgb(218, 177, 103) },
            { "item_iron_talon", Color.FromArgb(200, 151, 70) },
            { "item_recipe_iron_talon", Color.FromArgb(218, 177, 103) },
            { "item_sphere", Color.FromArgb(148, 214, 235) },
            { "item_recipe_sphere", Color.FromArgb(218, 177, 103) },
            { "item_lotus_orb", Color.FromArgb(241, 56, 120) },
            { "item_recipe_lotus_orb", Color.FromArgb(218, 177, 103) },
            { "item_maelstrom", Color.FromArgb(76, 191, 216) },
            { "item_recipe_maelstrom", Color.FromArgb(218, 177, 103) },
            { "item_magic_wand", Color.FromArgb(188, 228, 210) },
            { "item_recipe_magic_wand", Color.FromArgb(218, 177, 103) },
            { "item_manta", Color.FromArgb(133, 180, 223) },
            { "item_recipe_manta", Color.FromArgb(218, 177, 103) },
            { "item_mask_of_madness", Color.FromArgb(185, 74, 51) },
            { "item_recipe_mask_of_madness", Color.FromArgb(218, 177, 103) },
            { "item_medallion_of_courage", Color.FromArgb(209, 173, 108) },
            { "item_recipe_medallion_of_courage", Color.FromArgb(218, 177, 103) },
            { "item_mekansm", Color.FromArgb(167, 221, 211) },
            { "item_recipe_mekansm", Color.FromArgb(218, 177, 103) },
            { "item_mjollnir", Color.FromArgb(245, 250, 225) },
            { "item_recipe_mjollnir", Color.FromArgb(218, 177, 103) },
            { "item_monkey_king_bar", Color.FromArgb(203, 146, 29) },
            { "item_recipe_monkey_king_bar", Color.FromArgb(218, 177, 103) },
            { "item_moon_shard", Color.FromArgb(100, 111, 170) },
            { "item_recipe_moon_shard", Color.FromArgb(218, 177, 103) },
            { "item_necronomicon", Color.FromArgb(25, 130, 117) },
            { "item_necronomicon_2", Color.FromArgb(159, 40, 15) },
            { "item_necronomicon_3", Color.FromArgb(97, 143, 3) },
            { "item_recipe_necronomicon", Color.FromArgb(218, 177, 103) },
            { "item_null_talisman", Color.FromArgb(173, 51, 210) },
            { "item_recipe_null_talisman", Color.FromArgb(218, 177, 103) },
            { "item_oblivion_staff", Color.FromArgb(198, 197, 195) },
            { "item_recipe_oblivion_staff", Color.FromArgb(218, 177, 103) },
            { "item_ward_dispenser", Color.FromArgb(174, 174, 119) },
            { "item_recipe_ward_dispenser", Color.FromArgb(218, 177, 103) },
            { "item_octarine_core", Color.FromArgb(243, 165, 250) },
            { "item_recipe_octarine_core", Color.FromArgb(218, 177, 103) },
            { "item_orchid", Color.FromArgb(249, 190, 174) },
            { "item_recipe_orchid", Color.FromArgb(218, 177, 103) },
            { "item_pers", Color.FromArgb(227, 53, 125) },
            { "item_recipe_pers", Color.FromArgb(218, 177, 103) },
            { "item_phase_boots", Color.FromArgb(147, 67, 240) },
            { "item_recipe_phase_boots", Color.FromArgb(218, 177, 103) },
            { "item_pipe", Color.FromArgb(197, 158, 15) },
            { "item_recipe_pipe", Color.FromArgb(218, 177, 103) },
            { "item_poor_mans_shield", Color.FromArgb(146, 142, 136) },
            { "item_recipe_poor_mans_shield", Color.FromArgb(218, 177, 103) },
            { "item_power_treads", Color.FromArgb(163, 122, 94) },
            { "item_recipe_power_treads", Color.FromArgb(218, 177, 103) },
            { "item_radiance", Color.FromArgb(249, 235, 138) },
            { "item_recipe_radiance", Color.FromArgb(218, 177, 103) },
            { "item_rapier", Color.FromArgb(229, 217, 120) },
            { "item_recipe_rapier", Color.FromArgb(218, 177, 103) },
            { "item_refresher", Color.FromArgb(225, 252, 193) },
            { "item_recipe_refresher", Color.FromArgb(218, 177, 103) },
            { "item_ring_of_aquila", Color.FromArgb(205, 182, 96) },
            { "item_recipe_ring_of_aquila", Color.FromArgb(218, 177, 103) },
            { "item_ring_of_basilius", Color.FromArgb(106, 145, 210) },
            { "item_recipe_ring_of_basilius", Color.FromArgb(218, 177, 103) },
            { "item_rod_of_atos", Color.FromArgb(247, 99, 92) },
            { "item_recipe_rod_of_atos", Color.FromArgb(218, 177, 103) },
            { "item_sange", Color.FromArgb(172, 7, 7) },
            { "item_recipe_sange", Color.FromArgb(218, 177, 103) },
            { "item_sange_and_yasha", Color.FromArgb(244, 38, 251) },
            { "item_recipe_sange_and_yasha", Color.FromArgb(218, 177, 103) },
            { "item_satanic", Color.FromArgb(194, 79, 8) },
            { "item_recipe_satanic", Color.FromArgb(218, 177, 103) },
            { "item_sheepstick", Color.FromArgb(123, 217, 215) },
            { "item_recipe_sheepstick", Color.FromArgb(218, 177, 103) },
            { "item_invis_sword", Color.FromArgb(196, 74, 220) },
            { "item_recipe_invis_sword", Color.FromArgb(218, 177, 103) },
            { "item_shivas_guard", Color.FromArgb(91, 199, 207) },
            { "item_recipe_shivas_guard", Color.FromArgb(218, 177, 103) },
            { "item_silver_edge", Color.FromArgb(250, 177, 249) },
            { "item_recipe_silver_edge", Color.FromArgb(218, 177, 103) },
            { "item_basher", Color.FromArgb(215, 158, 164) },
            { "item_recipe_basher", Color.FromArgb(218, 177, 103) },
            { "item_solar_crest", Color.FromArgb(255, 150, 84) },
            { "item_recipe_solar_crest", Color.FromArgb(218, 177, 103) },
            { "item_soul_booster", Color.FromArgb(252, 211, 232) },
            { "item_recipe_soul_booster", Color.FromArgb(218, 177, 103) },
            { "item_soul_ring", Color.FromArgb(205, 149, 41) },
            { "item_recipe_soul_ring", Color.FromArgb(218, 177, 103) },
            { "item_tranquil_boots", Color.FromArgb(97, 221, 123) },
            { "item_recipe_tranquil_boots", Color.FromArgb(218, 177, 103) },
            { "item_urn_of_shadows", Color.FromArgb(108, 168, 25) },
            { "item_recipe_urn_of_shadows", Color.FromArgb(218, 177, 103) },
            { "item_vanguard", Color.FromArgb(100, 68, 76) },
            { "item_recipe_vanguard", Color.FromArgb(218, 177, 103) },
            { "item_veil_of_discord", Color.FromArgb(159, 0, 148) },
            { "item_recipe_veil_of_discord", Color.FromArgb(218, 177, 103) },
            { "item_vladmir", Color.FromArgb(157, 157, 129) },
            { "item_recipe_vladmir", Color.FromArgb(218, 177, 103) },
            { "item_wraith_band", Color.FromArgb(134, 174, 101) },
            { "item_recipe_wraith_band", Color.FromArgb(218, 177, 103) },
            { "item_yasha", Color.FromArgb(13, 190, 90) },
            { "item_recipe_yasha", Color.FromArgb(218, 177, 103) },
            { "item_halloween_candy_corn", Color.FromArgb(207, 220, 71) },
            { "item_mystery_hook", Color.FromArgb(218, 177, 103) },
            { "item_mystery_arrow", Color.FromArgb(218, 177, 103) },
            { "item_mystery_missile", Color.FromArgb(218, 177, 103) },
            { "item_mystery_toss", Color.FromArgb(218, 177, 103) },
            { "item_mystery_vacuum", Color.FromArgb(218, 177, 103) },
            { "item_halloween_rapier", Color.FromArgb(172, 202, 12) },
            { "item_greevil_whistle_toggle", Color.FromArgb(62, 121, 100) },
            { "item_winter_stocking", Color.FromArgb(191, 121, 101) },
            { "item_winter_skates", Color.FromArgb(152, 91, 69) },
            { "item_winter_cake", Color.FromArgb(201, 190, 181) },
            { "item_winter_cookie", Color.FromArgb(180, 60, 40) },
            { "item_winter_coco", Color.FromArgb(152, 144, 148) },
            { "item_winter_ham", Color.FromArgb(232, 161, 122) },
            { "item_winter_kringle", Color.FromArgb(197, 124, 56) },
            { "item_winter_mushroom", Color.FromArgb(189, 38, 24) },
            { "item_winter_greevil_treat", Color.FromArgb(150, 114, 106) },
            { "item_winter_greevil_garbage", Color.FromArgb(122, 151, 114) },
            { "item_winter_grevil_chewy", Color.FromArgb(157, 148, 104) },
            { "item_greevil_whistle", Color.FromArgb(62, 121, 100) }
        };

        Dictionary<string, Color> bottle_rune_colors = new Dictionary<string, Color>()
        {
            { "empty", Color.FromArgb(35, 155, 185) },
            { "arcane", Color.FromArgb(244, 92, 246) },
            { "bounty", Color.FromArgb(252, 145, 12) },
            { "double_damage", Color.FromArgb(59, 199, 255) },
            { "haste", Color.FromArgb(228, 46, 20) },
            { "illusion", Color.FromArgb(253, 216, 116) },
            { "invisibility", Color.FromArgb(146, 85, 183) },
            { "regeneration", Color.FromArgb(146, 85, 183) }
        };


        public GameEventHandler()
        {
            Devices.DeviceKeys[] allKeys = Enum.GetValues(typeof(Devices.DeviceKeys)).Cast<Devices.DeviceKeys>().ToArray();
            
            foreach(Devices.DeviceKeys key in allKeys)
            {
                keyColors.Add(key, Color.Black);
            }
        }

        ~GameEventHandler()
        {
            Destroy();
        }
        
        public bool Init()
        {
            bool devices_inited = Global.dev_manager.Initialize();

            if(devices_inited)
            {
                update_timer = new Timer(100);
                update_timer.Elapsed += new ElapsedEventHandler(update_timer_Tick);
                update_timer.Interval = 100; // in miliseconds
                update_timer.Start();

                general_timer.Start();
            }

            if (!devices_inited)
                Global.logger.LogLine("No devices initialized.", Logging_Level.Warning);

            return devices_inited;
        }

        public void Destroy()
        {
            update_timer.Stop();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private string GetActiveWindowsProcessname()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindow();
                uint procId = 0;
                GetWindowThreadProcessId(hWnd, out procId);
                var proc = Process.GetProcessById((int)procId);
                return proc.MainModule.FileName;
            }
            catch(Exception exc)
            {
                //Console.WriteLine(exc);
                return "";
            }
        }

        private void update_timer_Tick(object sender, EventArgs e)
        {
            if (GetActiveWindowsProcessname().ToLowerInvariant().EndsWith("\\dota2.exe") || preview_mode)
            {
                UpdateKeyboard();
            }
            else
            {
                if (keyboard_updated)
                {
                    Global.dev_manager.ResetDevices();
                }
            }
           
        }

        private double getDimmingValue()
        {
            if(isDimming && Global.Configuration.bg_enable_dimming)
            {
                dim_value -= 0.02;
                return dim_value = (dim_value < 0.0 ? 0.0 : dim_value);
            }
            else
                return dim_value = 1.0;
        }

        private double getKSEffectValue()
        {
            if (isPlayingKillStreakAnimation)
            {
                ks_blendamount += 0.15;
                return ks_blendamount = (ks_blendamount > 1.0 ? 1.0 : ks_blendamount);
            }
            else
            {
                ks_blendamount -= 0.15;
                return ks_blendamount = (ks_blendamount < 0.0 ? 0.0 : ks_blendamount);
            }
                
        }
        
        public void SetForcedUpdate(bool forced)
        {
            this.isForced = forced;
        }

        public void SetPreview(bool preview)
        {
            this.preview_mode = preview;
        }

        public void SetMapTime(int time)
        {
            this.mapTime = time;
        }

        public void SetTeam(PlayerTeam team)
        {
            this.current_team = team;
        }

        public void SetAlive(bool alive)
        {
            this.isAlive = alive;
        }

        public void SetRespawnTime(int time)
        {
            this.respawnTime = time;
        }

        public void SetHealth(int health)
        {
            this.health = health;
        }

        public void SetHealthMax(int health)
        {
            this.health_max = health;
        }

        public void SetMana(int mana)
        {
            this.mana = mana;
        }

        public void SetManaMax(int mana)
        {
            this.mana_max = mana;
        }

        public void SetKillStreak(int streak)
        {
            this.player_killstreak = streak;
        }

        public void SetKills(int count)
        {
            if (count > this.kills)
                GotAKill();

            this.kills = count;
        }

        public void SetPlayerActivity(PlayerActivity activity)
        {
            this.current_activity = activity;
        }

        public void SetAbilities(Abilities abilities)
        {
            this.abilities = abilities;
        }

        public void SetItems(Items items)
        {
            this.items = items;
        }

        public void Respawned()
        {
            isDimming = false;
            dim_bg_at = mapTime + Global.Configuration.bg_dim_after;
            dim_value = 1.0;
        }

        public void GotAKill()
        {
            isPlayingKillStreakAnimation = true;
            ks_end_time = this.general_timer.ElapsedMilliseconds + this.ks_duration;
        }

        public void UpdateKeyboard()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (isPlayingKillStreakAnimation && general_timer.ElapsedMilliseconds >= ks_end_time)
            {
                isPlayingKillStreakAnimation = false;
            }

            //update background
            if (Global.Configuration.bg_team_enabled)
            {
                Color bg_color = Global.Configuration.ambient_color;
                
                if (this.current_team == PlayerTeam.Dire)
                    bg_color = Global.Configuration.dire_color;
                else if (this.current_team == PlayerTeam.Radiant)
                    bg_color = Global.Configuration.radiant_color;

                if (this.current_team == PlayerTeam.Dire || this.current_team == PlayerTeam.Radiant)
                {
                    if (dim_bg_at <= mapTime || !isAlive)
                    {
                        isDimming = true;
                        bg_color = ColorMultiplication(bg_color, getDimmingValue());
                    }
                    else
                    {
                        isDimming = false;
                        dim_value = 1.0;
                    }

                    if(Global.Configuration.bg_respawn_glow && !isAlive)
                    {
                        bg_color = BlendColors(bg_color, Global.Configuration.bg_respawn_glow_color, (this.respawnTime > 5 ? 0.0 : 1.0 - (this.respawnTime / 5.0)));
                    }
                }

                if (Global.Configuration.bg_display_killstreaks && this.player_killstreak > 0)
                {
                    Color[] killstreakcolors = Global.Configuration.bg_killstreakcolors.ToArray();
                    bg_color = BlendColors(bg_color, killstreakcolors[(this.player_killstreak > 10 ? 10 : this.player_killstreak)], getKSEffectValue());
                }

                SetAllKeys(bg_color);
                if (Global.Configuration.bg_peripheral_use)
                    SetOneKey(Devices.DeviceKeys.Peripheral, bg_color);
            }
            else
            {
                SetAllKeys(Color.Black);
            }

            //Not initialized
            if (this.current_team != PlayerTeam.Undefined && this.current_team != PlayerTeam.None)
            {
                if (Global.Configuration.mimic_respawn_timer && !isAlive)
                {
                    //Update Health
                    if (Global.Configuration.health_enabled)
                    {
                        PercentEffect(Global.Configuration.mimic_respawn_timer_color, Global.Configuration.mimic_respawn_timer_respawning_color, Global.Configuration.healthKeys.ToArray(), (double)(this.respawnTime > 4 ? 5.0 : this.respawnTime), 4.0, PercentEffectType.AllAtOnce);
                    }
                    //Update Mana
                    if (Global.Configuration.mana_enabled)
                    {
                        PercentEffect(Global.Configuration.mimic_respawn_timer_color, Global.Configuration.mimic_respawn_timer_respawning_color, Global.Configuration.manaKeys.ToArray(), (double)(this.respawnTime > 4 ? 5.0 : this.respawnTime), 4.0, PercentEffectType.AllAtOnce);
                    }
                }
                else
                {
                    //Update Health
                    if (Global.Configuration.health_enabled)
                        PercentEffect(Global.Configuration.healthy_color, Global.Configuration.hurt_color, Global.Configuration.healthKeys.ToArray(), (double)this.health, (double)this.health_max, Global.Configuration.health_effect_type);

                    //Update Mana
                    if (Global.Configuration.mana_enabled)
                        PercentEffect(Global.Configuration.mana_color, Global.Configuration.nomana_color, Global.Configuration.manaKeys.ToArray(), (double)this.mana, (double)this.mana_max, Global.Configuration.mana_effect_type);
                }


                //Abilities
                if (Global.Configuration.abilitykeys_enabled && this.abilities != null && Global.Configuration.ability_keys.Count >= 6)
                {
                    for (int index = 0; index < this.abilities.Count; index++)
                    {
                        Ability ability = this.abilities[index];
                        DeviceKeys key = Global.Configuration.ability_keys[index];

                        if (ability.IsUltimate)
                            key = Global.Configuration.ability_keys[5];


                        if (ability.CanCast && ability.Cooldown == 0 && ability.Level > 0)
                        {
                            SetOneKey(key, Global.Configuration.ability_can_use_color);
                        }
                        else if (ability.Cooldown <= 5 && ability.Level > 0)
                        {
                            SetOneKey(key, BlendColors(Global.Configuration.ability_can_use_color, Global.Configuration.ability_can_not_use_color, (double)ability.Cooldown / 5.0));
                        }
                        else
                        {
                            SetOneKey(key, Global.Configuration.ability_can_not_use_color);
                        }
                    }
                }

                //Items
                if (Global.Configuration.items_enabled && this.items != null && Global.Configuration.items_keys.Count >= 6)
                {
                    for (int index = 0; index < this.items.CountInventory; index++)
                    {
                        Item item = this.items.GetInventoryAt(index);
                        DeviceKeys key = Global.Configuration.items_keys[index];

                        if (item.Name.Equals("empty"))
                        {
                            SetOneKey(key, Global.Configuration.items_empty_color);
                        }
                        else
                        {
                            if (Global.Configuration.items_use_item_color && item_colors.ContainsKey(item.Name))
                            {
                                if (!String.IsNullOrWhiteSpace(item.ContainsRune))
                                {
                                    SetOneKey(key, BlendColors(item_colors[item.Name], bottle_rune_colors[item.ContainsRune], 0.8));
                                }
                                else
                                {
                                    SetOneKey(key, item_colors[item.Name]);
                                }
                            }
                            else
                            {
                                SetOneKey(key, Global.Configuration.items_color);
                            }

                            //Cooldown
                            if (item.Cooldown > 5)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_on_cooldown_color, 1.0));
                            }
                            else if (item.Cooldown > 0 && item.Cooldown <= 5)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_on_cooldown_color, item.Cooldown / 5.0));
                            }

                            //Charges
                            if (item.Charges == 0)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_no_charges_color, 0.7));
                            }
                        }
                    }

                    for (int index = 0; index < this.items.CountStash; index++)
                    {
                        Item item = this.items.GetStashAt(index);
                        DeviceKeys key = Global.Configuration.items_keys[6 + index];

                        if (item.Name.Equals("empty"))
                        {
                            SetOneKey(key, Global.Configuration.items_empty_color);
                        }
                        else
                        {
                            if (Global.Configuration.items_use_item_color && item_colors.ContainsKey(item.Name))
                            {
                                if (!String.IsNullOrWhiteSpace(item.ContainsRune))
                                {
                                    SetOneKey(key, BlendColors(item_colors[item.Name], bottle_rune_colors[item.ContainsRune], 0.8));
                                }
                                else
                                {
                                    SetOneKey(key, item_colors[item.Name]);
                                }
                            }
                            else
                            {
                                SetOneKey(key, Global.Configuration.items_color);
                            }

                            //Cooldown
                            if (item.Cooldown > 5)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_on_cooldown_color, 1.0));
                            }
                            else if (item.Cooldown > 0 && item.Cooldown <= 5)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_on_cooldown_color, item.Cooldown / 5.0));
                            }

                            //Charges
                            if (item.Charges == 0)
                            {
                                SetOneKey(key, BlendColors(GetOneKey(key), Global.Configuration.items_no_charges_color, 0.7));
                            }
                        }
                    }
                }

            }

            //Restore Static Keys
            if (Global.Configuration.statickeys_enabled)
            {
                Devices.DeviceKeys[] _statickeys = Global.Configuration.staticKeys.ToArray();
                foreach (Devices.DeviceKeys key in _statickeys)
                    SetOneKey(key, Global.Configuration.statickeys_color);
                Devices.DeviceKeys[] _statickeys_2 = Global.Configuration.staticKeys_2.ToArray();
                foreach (Devices.DeviceKeys key in _statickeys_2)
                    SetOneKey(key, Global.Configuration.statickeys_2_color);
            }


            if (general_timer.Elapsed.Seconds % this.updateRate == 0 && (general_timer.Elapsed.Seconds != this.lastUpdate))
            {
                this.isForced = true;
                this.lastUpdate = general_timer.Elapsed.Seconds;
            }

            keyboard_updated = Global.dev_manager.UpdateDevices(keyColors, this.isForced);
            this.isForced = false;

            final_keyColors = keyColors;

            stopwatch.Stop();
        }

        private byte ColorByteMultiplication(byte color, double value)
        {
            byte returnbyte = color;

            if ((double)returnbyte * value > 255)
                returnbyte = 255;
            else if ((double)returnbyte * value < 0)
                returnbyte = 0;
            else
                returnbyte = (byte)((double)returnbyte * value);

            return returnbyte;
        }

        private Color ColorMultiplication(Color color, double percent)
        {
            byte alpha = ColorByteMultiplication(color.A, percent);
            byte red = ColorByteMultiplication(color.R, percent);
            byte green = ColorByteMultiplication(color.G, percent);
            byte blue = ColorByteMultiplication(color.B, percent);

            return Color.FromArgb(alpha, red, green, blue);
        }

        private void SetAllKeysEffect(Color color, double percent)
        {
            foreach (Devices.DeviceKeys key in keyColors.Keys.ToArray())
            {
                Color keycolor = GetOneKey(key);

                SetOneKey(key, BlendColors(keycolor, color, percent));
            }
        }

        public Dictionary<Devices.DeviceKeys, System.Drawing.Color> GetKeyboardLights()
        {
            return final_keyColors;
        }

        public Color BlendColors(Color background, Color foreground, double percent)
        {
            if (percent < 0.0)
                percent = 0.0;
            else if (percent > 1.0)
                percent = 1.0;
            
            byte r = (byte)Math.Min((Int32)foreground.R * percent + (Int32)background.R * (1.0 - percent), 255);
            byte g = (byte)Math.Min((Int32)foreground.G * percent + (Int32)background.G * (1.0 - percent), 255);
            byte b = (byte)Math.Min((Int32)foreground.B * percent + (Int32)background.B * (1.0 - percent), 255);

            return Color.FromArgb(r, g, b);
        }

        private Color GetOneKey(Devices.DeviceKeys key)
        {
            Color ret = Color.Black;

            if(keyColors.ContainsKey(key))
                ret = keyColors[key];

            return ret;
        }

        private void SetAllKeys(Color color)
        {
            foreach (Devices.DeviceKeys key in keyColors.Keys.ToArray())
            {
                SetOneKey(key, color);
            }
        }

        private void SetOneKey(Devices.DeviceKeys key, Color color)
        {
            keyColors[key] = color;
        }

        private void PercentEffect(Color foregroundColor, Color backgroundColor, Devices.DeviceKeys[] keys, double value, double total, PercentEffectType effectType = PercentEffectType.Progressive)
        {
            double progress_total = value / total;
            if (progress_total < 0.0)
                progress_total = 0.0;
            else if (progress_total > 1.0)
                progress_total = 1.0;

            double progress = progress_total * keys.Count();

            for (int i = 0; i < keys.Count(); i++)
            {
                Devices.DeviceKeys current_key = keys[i];

                switch (effectType)
                {
                    case (PercentEffectType.AllAtOnce):
                        SetOneKey(current_key, Color.FromArgb(
                                (Int32)Math.Min((Int32)foregroundColor.R * progress_total + (Int32)backgroundColor.R * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.G * progress_total + (Int32)backgroundColor.G * (1.0 - progress_total), 255),
                                (Int32)Math.Min((Int32)foregroundColor.B * progress_total + (Int32)backgroundColor.B * (1.0 - progress_total), 255)
                            ));
                        break;
                    case (PercentEffectType.Progressive_Gradual):
                        if (i == (int)progress)
                        {
                            double percent = (double)progress - i;
                            SetOneKey(current_key, Color.FromArgb(
                                (Int32)Math.Min((Int32)foregroundColor.R * percent + (Int32)backgroundColor.R * (1.0 - percent), 255),
                                (Int32)Math.Min((Int32)foregroundColor.G * percent + (Int32)backgroundColor.G * (1.0 - percent), 255),
                                (Int32)Math.Min((Int32)foregroundColor.B * percent + (Int32)backgroundColor.B * (1.0 - percent), 255)
                                ));
                        }
                        else if (i < (int)progress)
                            SetOneKey(current_key, foregroundColor);
                        else
                            SetOneKey(current_key, backgroundColor);
                        break;
                    default:
                        if (i < (int)progress)
                            SetOneKey(current_key, foregroundColor);
                        else
                            SetOneKey(current_key, backgroundColor);
                        break;
                }
            }
        }

    }
}
