using Hardcodet.Wpf.TaskbarNotification;
using Logitech_Dota2.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Logitech_Dota2
{
    enum KeyboardRecordingType
    {
        None,
        HealthKeys,
        ManaKeys,
        BombKeys,
        StaticKeysGroup1,
        StaticKeysGroup2,
        TypingKeys,
        Ability1,
        Ability2,
        Ability3,
        Ability4,
        Ability5,
        AbilityUltimate,
        ItemSlot1,
        ItemSlot2,
        ItemSlot3,
        ItemSlot4,
        ItemSlot5,
        ItemSlot6,
        StashSlot1,
        StashSlot2,
        StashSlot3,
        StashSlot4,
        StashSlot5,
        StashSlot6
    }
    
    /// <summary>
    /// Interaction logic for ConfigUI.xaml
    /// </summary>
    public partial class ConfigUI : Window
    {
        private bool settingsloaded = false;
        private bool shownHiddenMessage = false;

        private Timer virtual_keyboard_timer;
        private TextBlock last_selected_key;
        private KeyboardRecordingType recordingKeystrokes = KeyboardRecordingType.None;
        private Stopwatch recording_stopwatch = new Stopwatch();
        private List<DeviceKeys> recordedKeys = new List<DeviceKeys>();

        private int respawn_time = 15;
        private Timer preview_respawn_timer;
        private int killstreak = 0;

        public ConfigUI()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!settingsloaded)
            {
                this.preview_team.Items.Add(Dota2GSI.Nodes.PlayerTeam.None);
                this.preview_team.Items.Add(Dota2GSI.Nodes.PlayerTeam.Dire);
                this.preview_team.Items.Add(Dota2GSI.Nodes.PlayerTeam.Radiant);
                
                this.background_enabled.IsChecked = Global.Configuration.bg_team_enabled;
                this.t_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.dire_color);
                this.ct_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.radiant_color);
                this.ambient_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.ambient_color);
                this.background_peripheral_use.IsChecked = Global.Configuration.bg_peripheral_use;
                this.background_dim_enabled.IsChecked = Global.Configuration.bg_enable_dimming;
                this.background_dim_value.Text = Global.Configuration.bg_dim_after + "s";
                this.background_dim_aftertime.Value = Global.Configuration.bg_dim_after;
                this.background_enable_respawn_glow.IsChecked = Global.Configuration.bg_respawn_glow;
                this.respawn_glow_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_respawn_glow_color);
                this.background_killstreaks_enabled.IsChecked = Global.Configuration.bg_display_killstreaks;
                this.bg_killstreak_doublekill_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[2]);
                this.bg_killstreak_killingspree_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[3]);
                this.bg_killstreak_dominating_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[4]);
                this.bg_killstreak_megakill_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[5]);
                this.bg_killstreak_unstoppable_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[6]);
                this.bg_killstreak_wickedsick_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[7]);
                this.bg_killstreak_monsterkill_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[8]);
                this.bg_killstreak_godlike_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[9]);
                this.bg_killstreak_godlikeandon_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.bg_killstreakcolors[10]);

                this.health_enabled.IsChecked = Global.Configuration.health_enabled;
                this.health_healthy_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.healthy_color);
                this.health_hurt_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.hurt_color);
                this.health_effect_type.Items.Add("All At Once");
                this.health_effect_type.Items.Add("Progressive");
                this.health_effect_type.Items.Add("Progressive (Gradual)");
                this.health_effect_type.SelectedIndex = (int)Global.Configuration.health_effect_type;
                foreach (var key in Global.Configuration.healthKeys)
                {
                    this.health_keysequence.Items.Add(key);
                }

                this.mana_enabled.IsChecked = Global.Configuration.mana_enabled;
                this.mana_hasmana_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.mana_color);
                this.mana_nomana_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.nomana_color);
                this.mana_effect_type.Items.Add("All At Once");
                this.mana_effect_type.Items.Add("Progressive");
                this.mana_effect_type.Items.Add("Progressive (Gradual)");
                this.mana_effect_type.SelectedIndex = (int)Global.Configuration.mana_effect_type;
                foreach (var key in Global.Configuration.manaKeys)
                {
                    this.mana_keysequence.Items.Add(key);
                }

                this.mimic_respawn_timer_checkbox.IsChecked = Global.Configuration.mimic_respawn_timer;
                this.mimic_respawn_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.mimic_respawn_timer_color);
                this.mimic_respawn_respawning_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.mimic_respawn_timer_respawning_color);


                this.abilities_enabled.IsChecked = Global.Configuration.abilitykeys_enabled;
                this.abilities_canuse_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.ability_can_use_color);
                this.abilities_cannotuse_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.ability_can_not_use_color);
                SetSingleKey(this.ability_key1_textblock, Global.Configuration.ability_keys, 0);
                SetSingleKey(this.ability_key2_textblock, Global.Configuration.ability_keys, 1);
                SetSingleKey(this.ability_key3_textblock, Global.Configuration.ability_keys, 2);
                SetSingleKey(this.ability_key4_textblock, Global.Configuration.ability_keys, 3);
                SetSingleKey(this.ability_key5_textblock, Global.Configuration.ability_keys, 4);
                SetSingleKey(this.ability_key6_textblock, Global.Configuration.ability_keys, 5);

                this.items_enabled.IsChecked = Global.Configuration.items_enabled;
                this.items_use_item_color.IsChecked = Global.Configuration.items_use_item_color;
                this.item_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.items_color);
                this.item_empty_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.items_empty_color);
                this.item_no_charges_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.items_no_charges_color);
                this.item_on_cooldown_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.items_on_cooldown_color);
                SetSingleKey(this.item_slot1_textblock, Global.Configuration.items_keys, 0);
                SetSingleKey(this.item_slot2_textblock, Global.Configuration.items_keys, 1);
                SetSingleKey(this.item_slot3_textblock, Global.Configuration.items_keys, 2);
                SetSingleKey(this.item_slot4_textblock, Global.Configuration.items_keys, 3);
                SetSingleKey(this.item_slot5_textblock, Global.Configuration.items_keys, 4);
                SetSingleKey(this.item_slot6_textblock, Global.Configuration.items_keys, 5);
                SetSingleKey(this.stash_slot1_textblock, Global.Configuration.items_keys, 6);
                SetSingleKey(this.stash_slot2_textblock, Global.Configuration.items_keys, 7);
                SetSingleKey(this.stash_slot3_textblock, Global.Configuration.items_keys, 8);
                SetSingleKey(this.stash_slot4_textblock, Global.Configuration.items_keys, 9);
                SetSingleKey(this.stash_slot5_textblock, Global.Configuration.items_keys, 10);
                SetSingleKey(this.stash_slot6_textblock, Global.Configuration.items_keys, 11);

                
                this.statickeys_enabled.IsChecked = Global.Configuration.statickeys_enabled;
                this.statickeys_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.statickeys_color);
                foreach (var key in Global.Configuration.staticKeys)
                {
                    this.statickeys_keysequence.Items.Add(key);
                }
                this.statickeys_2_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.statickeys_2_color);
                foreach (var key in Global.Configuration.staticKeys_2)
                {
                    this.statickeys_2_keysequence.Items.Add(key);
                }

                this.typing_enabled.IsChecked = Global.Configuration.typing_enabled;
                this.typing_color_colorpicker.SelectedColor = DrawingColorToMediaColor(Global.Configuration.typing_color);
                foreach (var key in Global.Configuration.typingKeys)
                {
                    this.typingkeys_keysequence.Items.Add(key);
                }

                virtual_keyboard_timer = new Timer(100);
                virtual_keyboard_timer.Elapsed += new ElapsedEventHandler(virtual_keyboard_timer_Tick);
                virtual_keyboard_timer.Start();

                preview_respawn_timer = new Timer(1000);
                preview_respawn_timer.Elapsed += new ElapsedEventHandler(preview_respawn_timer_Tick);

                settingsloaded = true;
            }

            List<KeyboardKey> layout = Global.kbLayout.GetLayout();
            double layout_height = 0;
            double layout_width = 0;

            double max_height = this.keyboard_grid.Height;
            double max_width = this.keyboard_grid.Width;
            double cornerRadius = 5;
            double current_height = 0;
            double current_width = 0;
            bool isFirstInRow = true;

            foreach (KeyboardKey key in layout)
            {
                double keyMargin_Left = key.margin_left;
                double keyMargin_Top = (isFirstInRow ? 0 : key.margin_top);

                Border keyBorder = new Border();
                keyBorder.CornerRadius = new CornerRadius(cornerRadius);
                //keyBorder.Width = key.width;
                //keyBorder.Height = key.height;
                keyBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                keyBorder.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                keyBorder.Margin = new Thickness(current_width + keyMargin_Left, current_height + keyMargin_Top, 0, 0);
                keyBorder.Visibility = System.Windows.Visibility.Visible;
                keyBorder.BorderThickness = new Thickness(1);
                keyBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
                keyBorder.IsEnabled = key.enabled;
                if (!key.enabled)
                {
                    ToolTipService.SetShowOnDisabled(keyBorder, true);
                    keyBorder.ToolTip = new ToolTip { Content = "Changes to this key are not supported" };
                }

                TextBlock keyCap = new TextBlock();
                keyCap.Text = key.visualName;
                keyCap.Tag = key.tag;
                keyCap.FontSize = key.font_size;
                keyCap.FontWeight = FontWeights.Bold;
                keyCap.FontFamily = new FontFamily("Segoe UI");
                keyCap.Width = key.width;
                keyCap.Height = key.height;
                keyCap.TextAlignment = TextAlignment.Center;
                keyCap.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                keyCap.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                keyCap.Margin = new Thickness(0);
                keyCap.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                keyCap.Visibility = System.Windows.Visibility.Visible;
                keyCap.MouseDown += keyboard_grid_pressed;
                keyCap.MouseMove += keyboard_grid_moved;
                keyCap.IsHitTestVisible = true;

                keyBorder.Child = keyCap;

                this.keyboard_grid.Children.Add(keyBorder);
                isFirstInRow = false;

                current_width += key.width + keyMargin_Left;

                if (layout_width < current_width)
                    layout_width = current_width;

                if (key.line_break)
                {
                    current_height += 37;
                    current_width = 0;
                    isFirstInRow = true;
                }

                if (layout_height < current_height)
                    layout_height = current_height;
            }

            //Update size
            if (max_width < layout_width)
            {
                this.keyboard_grid.Width = layout_width;
                this.MaxWidth += layout_width - max_width;
                this.Width = this.MaxWidth;
                this.MinWidth = this.MaxWidth;
            }

            if (max_height < layout_height)
            {
                this.keyboard_grid.Height = layout_height;
                this.MaxHeight += layout_height - max_height;
                this.Height = this.MaxHeight;
                this.MinHeight = this.MaxHeight;
            }

            this.UpdateLayout();
            //this.keyboard_grid.UpdateLayout();


        }

        private void SetSingleKey(TextBlock key_destination, List<DeviceKeys> keyslist, int position)
        {
            if (keyslist.Count > position)
                key_destination.Text = Enum.GetName(typeof(DeviceKeys), keyslist[position]);
            else
                key_destination.Text = Enum.GetName(typeof(DeviceKeys), DeviceKeys.NONE);
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private void virtual_keyboard_timer_Tick(object sender, EventArgs e)
        {
            if (!ApplicationIsActivated())
                return;
            
            Dispatcher.Invoke(
                        () =>
                        {
                            Dictionary<Devices.DeviceKeys, System.Drawing.Color> keylights = new Dictionary<Devices.DeviceKeys, System.Drawing.Color>();

                            if ((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false)
                                keylights = Global.geh.GetKeyboardLights();

                                foreach (var child in this.keyboard_grid.Children)
                                {
                                    if (child is TextBlock &&
                                        (child as TextBlock).Tag is Devices.DeviceKeys
                                        )
                                    {
                                        if(keylights.ContainsKey((Devices.DeviceKeys)(child as TextBlock).Tag))
                                        {
                                            System.Drawing.Color keycolor = keylights[(Devices.DeviceKeys)(child as TextBlock).Tag];

                                            (child as TextBlock).Foreground = new SolidColorBrush(DrawingColorToMediaColor(keycolor));
                                        }
                                        
                                        if (this.recordedKeys.Contains((Devices.DeviceKeys)(child as TextBlock).Tag))
                                            (child as TextBlock).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)0, (byte)255, (byte)0));
                                        else
                                        {
                                            if ((child as TextBlock).IsEnabled)
                                                (child as TextBlock).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                            else
                                                (child as TextBlock).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 128, 128, 128));
                                        }
                                    }
                                    else if (child is Border &&
                                        (child as Border).Child is TextBlock &&
                                        ((child as Border).Child as TextBlock).Tag is Devices.DeviceKeys
                                        )
                                    {
                                        if (keylights.ContainsKey((Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag))
                                        {
                                            System.Drawing.Color keycolor = keylights[(Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag];

                                            ((child as Border).Child as TextBlock).Foreground = new SolidColorBrush(DrawingColorToMediaColor(keycolor));
                                        }
                                        
                                        if (this.recordedKeys.Contains((Devices.DeviceKeys)((child as Border).Child as TextBlock).Tag))
                                            (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)0, (byte)255, (byte)0));
                                        else
                                        {
                                            if ((child as Border).IsEnabled)
                                                (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                            else
                                                (child as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 128, 128, 128));
                                        }
                                    }

                                }

                            //Update single keys
                            switch(recordingKeystrokes)
                            {
                                case(KeyboardRecordingType.Ability1):
                                    (this.ability_key1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.Ability2):
                                    (this.ability_key2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.Ability3):
                                    (this.ability_key3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.Ability4):
                                    (this.ability_key4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.Ability5):
                                    (this.ability_key5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.AbilityUltimate):
                                    (this.ability_key6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;

                                case (KeyboardRecordingType.ItemSlot1):
                                    (this.item_slot1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.ItemSlot2):
                                    (this.item_slot2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.ItemSlot3):
                                    (this.item_slot3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.ItemSlot4):
                                    (this.item_slot4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.ItemSlot5):
                                    (this.item_slot5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.ItemSlot6):
                                    (this.item_slot6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot1):
                                    (this.stash_slot1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot2):
                                    (this.stash_slot2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot3):
                                    (this.stash_slot3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot4):
                                    (this.stash_slot4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot5):
                                    (this.stash_slot5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;
                                case (KeyboardRecordingType.StashSlot6):
                                    (this.stash_slot6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(Math.Min(Math.Pow(Math.Sin((double)recording_stopwatch.ElapsedMilliseconds / 1000.0) + 0.05, 2.0), 1.0) * 255), (byte)255, (byte)0, (byte)0));
                                    break;


                                default:
                                    (this.ability_key1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.ability_key2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.ability_key3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.ability_key4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.ability_key5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.ability_key6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));

                                    (this.item_slot1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.item_slot2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.item_slot3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.item_slot4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.item_slot5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.item_slot6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot1_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot2_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot3_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot4_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot5_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    (this.stash_slot6_textblock.Parent as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
                                    break;
                            }

                        });
        }

        private void preview_respawn_timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                        () =>
                        {
                            if (this.respawn_time < 0)
                            {
                                Global.geh.SetAlive(true);
                                this.preview_killplayer.IsEnabled = true;

                                preview_respawn_timer.Stop();
                            }
                            else
                            {
                                this.preview_respawn_time.Content = "Seconds to respawn: " + this.respawn_time;
                                Global.geh.SetRespawnTime(this.respawn_time);

                                this.respawn_time--;
                            }
                        });
        }


        private System.Drawing.Color MediaColorToDrawingColor(System.Windows.Media.Color in_color)
        {
            return System.Drawing.Color.FromArgb(in_color.A, in_color.R, in_color.G, in_color.B);
        }

        private System.Windows.Media.Color DrawingColorToMediaColor(System.Drawing.Color in_color)
        {
            return System.Windows.Media.Color.FromArgb(in_color.A, in_color.R, in_color.G, in_color.B);
        }

        private List<Devices.DeviceKeys> SequenceToList(ItemCollection items)
        {
            List<Devices.DeviceKeys> newsequence = new List<Devices.DeviceKeys>();

            foreach (Devices.DeviceKeys key in items)
            {
                newsequence.Add(key);
            }

            return newsequence;
        }

        private bool ListBoxMoveSelectedUp(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex > 0)
            {
                int selected_index = list.SelectedIndex;
                var saved = list.Items[selected_index];
                list.Items[selected_index] = list.Items[selected_index - 1];
                list.Items[selected_index - 1] = saved;
                list.SelectedIndex = selected_index - 1;

                list.ScrollIntoView(list.Items[selected_index - 1]);
                return true;
            }

            return false;
        }

        private bool ListBoxMoveSelectedDown(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex < (list.Items.Count - 1))
            {
                int selected_index = list.SelectedIndex;
                var saved = list.Items[selected_index];
                list.Items[selected_index] = list.Items[selected_index + 1];
                list.Items[selected_index + 1] = saved;
                list.SelectedIndex = selected_index + 1;

                list.ScrollIntoView(list.Items[selected_index + 1]);
                return true;
            }

            return false;
        }

        private bool ListBoxRemoveSelected(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex >= 0)
            {
                int selected = list.SelectedIndex;
                list.Items.RemoveAt(selected);

                if (list.Items.Count > selected)
                    list.SelectedIndex = selected;
                else
                    list.SelectedIndex = (list.Items.Count - 1);

                if (list.SelectedIndex > -1)
                    list.ScrollIntoView(list.Items[list.SelectedIndex]);

                return true;
            }

            return false;
        }

        private void RecordKeySequence(KeyboardRecordingType whoisrecording, Button button, ListBox sequence_listbox)
        {

            if (recordingKeystrokes == KeyboardRecordingType.None)
            {
                this.recordedKeys = new List<Devices.DeviceKeys>();

                button.Content = "Stop Recording";
                recording_stopwatch.Restart();
                recordingKeystrokes = whoisrecording;
            }
            else if (recordingKeystrokes == whoisrecording)
            {
                if (sequence_listbox.SelectedIndex > 0 && sequence_listbox.SelectedIndex < (sequence_listbox.Items.Count - 1))
                {
                    int insertpos = sequence_listbox.SelectedIndex;
                    foreach (var key in this.recordedKeys)
                    {
                        sequence_listbox.Items.Insert(insertpos, key);
                        insertpos++;
                    }
                }
                else
                {
                    foreach (var key in this.recordedKeys)
                        sequence_listbox.Items.Add(key);
                }

                switch(whoisrecording)
                {
                    case(KeyboardRecordingType.HealthKeys):
                        Global.Configuration.healthKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.ManaKeys):
                        Global.Configuration.manaKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.StaticKeysGroup1):
                        Global.Configuration.staticKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.StaticKeysGroup2):
                        Global.Configuration.staticKeys_2 = SequenceToList(sequence_listbox.Items);
                        break;
                    case (KeyboardRecordingType.TypingKeys):
                        Global.Configuration.typingKeys = SequenceToList(sequence_listbox.Items);
                        break;
                    default:
                        break;
                }

                this.recordedKeys = new List<Devices.DeviceKeys>();
                button.Content = "Add/Record";
                recording_stopwatch.Stop();
                recordingKeystrokes = KeyboardRecordingType.None;
            }
            else
            {
                System.Windows.MessageBox.Show("You are already recording a key sequence for " + recordingKeystrokes.ToString());
            }
        }

        ////Preview

        private void preview_team_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((Dota2GSI.Nodes.PlayerTeam)this.preview_team.Items[this.preview_team.SelectedIndex])
            {
                case (Dota2GSI.Nodes.PlayerTeam.None):
                    Global.geh.SetTeam(Dota2GSI.Nodes.PlayerTeam.None);
                    break;
                case (Dota2GSI.Nodes.PlayerTeam.Radiant):
                    Global.geh.SetTeam(Dota2GSI.Nodes.PlayerTeam.Radiant);
                    break;
                default:
                    Global.geh.SetTeam(Dota2GSI.Nodes.PlayerTeam.Dire);
                    break;
            }
        }

        private void preview_health_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int hp_val = (int)this.preview_health_slider.Value;
            if (this.preview_health_amount is Label)
            {
                this.preview_health_amount.Content = hp_val + "%";
                Global.geh.SetHealth(hp_val);
                Global.geh.SetHealthMax(100);
            }
        }

        private void preview_mana_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int mana_val = (int)this.preview_mana_slider.Value;
            if (this.preview_mana_amount is Label)
            {
                this.preview_mana_amount.Content = mana_val + "%";
                Global.geh.SetMana(mana_val);
                Global.geh.SetManaMax(100);
            }
        }

        private void preview_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.geh.SetPreview((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false);
            Global.geh.SetForcedUpdate((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false);
        }

        private void preview_killplayer_Click(object sender, RoutedEventArgs e)
        {
            Global.geh.SetAlive(false);
            respawn_time = 15;
            Global.geh.SetRespawnTime(this.respawn_time);
            this.preview_killplayer.IsEnabled = false;
            Global.geh.SetKillStreak(killstreak = 0);
            this.preview_killstreak_label.Content = "Killstreak: " + this.killstreak;

            preview_respawn_timer.Start();
        }

        private void preview_addkill_Click(object sender, RoutedEventArgs e)
        {
            Global.geh.SetKillStreak(killstreak++);
            Global.geh.GotAKill();
            this.preview_killstreak_label.Content = "Killstreak: " + this.killstreak;
        }

        ////Background

        private void background_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_team_enabled = (this.background_enabled.IsChecked.HasValue) ? this.background_enabled.IsChecked.Value : false;
        }

        private void background_peripheral_use_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_peripheral_use = (this.background_peripheral_use.IsChecked.HasValue) ? this.background_peripheral_use.IsChecked.Value : false;
        }

        private void t_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.t_colorpicker.SelectedColor.HasValue)
                Global.Configuration.dire_color = MediaColorToDrawingColor(this.t_colorpicker.SelectedColor.Value);
        }

        private void ct_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.ct_colorpicker.SelectedColor.HasValue)
                Global.Configuration.radiant_color = MediaColorToDrawingColor(this.ct_colorpicker.SelectedColor.Value);
        }

        private void ambient_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.ambient_colorpicker.SelectedColor.HasValue)
                Global.Configuration.ambient_color = MediaColorToDrawingColor(this.ambient_colorpicker.SelectedColor.Value);
        }

        private void background_dim_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_enable_dimming = (this.background_dim_enabled.IsChecked.HasValue) ? this.background_dim_enabled.IsChecked.Value : false;
        }

        private void background_dim_aftertime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = (int)this.background_dim_aftertime.Value;
            if (this.background_dim_value is TextBlock)
            {
                this.background_dim_value.Text = val + "s";
                Global.Configuration.bg_dim_after = val;
            }
        }

        private void background_killstreaks_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_display_killstreaks = (this.background_killstreaks_enabled.IsChecked.HasValue) ? this.background_killstreaks_enabled.IsChecked.Value : false;
        }

        private void bg_killstreak_doublekill_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_doublekill_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[2] = MediaColorToDrawingColor(this.bg_killstreak_doublekill_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_killingspree_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_killingspree_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[3] = MediaColorToDrawingColor(this.bg_killstreak_killingspree_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_dominating_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_dominating_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[4] = MediaColorToDrawingColor(this.bg_killstreak_dominating_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_megakill_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_megakill_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[5] = MediaColorToDrawingColor(this.bg_killstreak_megakill_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_unstoppable_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_unstoppable_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[6] = MediaColorToDrawingColor(this.bg_killstreak_unstoppable_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_wickedsick_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_wickedsick_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[7] = MediaColorToDrawingColor(this.bg_killstreak_wickedsick_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_monsterkill_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_monsterkill_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[8] = MediaColorToDrawingColor(this.bg_killstreak_monsterkill_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_godlike_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_godlike_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[9] = MediaColorToDrawingColor(this.bg_killstreak_godlike_colorpicker.SelectedColor.Value);
        }

        private void bg_killstreak_godlikeandon_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.bg_killstreak_godlikeandon_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_killstreakcolors[10] = MediaColorToDrawingColor(this.bg_killstreak_godlikeandon_colorpicker.SelectedColor.Value);
        }

        ////Player Health

        private void health_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.health_enabled = (this.health_enabled.IsChecked.HasValue) ? this.health_enabled.IsChecked.Value : false;
        }

        private void health_healthy_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.health_healthy_colorpicker.SelectedColor.HasValue)
                Global.Configuration.healthy_color = MediaColorToDrawingColor(this.health_healthy_colorpicker.SelectedColor.Value);
        }

        private void health_hurt_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.health_hurt_colorpicker.SelectedColor.HasValue)
                Global.Configuration.hurt_color = MediaColorToDrawingColor(this.health_hurt_colorpicker.SelectedColor.Value);
        }

        private void sequence_remove_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void sequence_up_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void sequence_down_health_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.health_keysequence))
                Global.Configuration.healthKeys = SequenceToList(this.health_keysequence.Items);
        }

        private void health_effect_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.Configuration.health_effect_type = (PercentEffectType)Enum.Parse(typeof(PercentEffectType), this.health_effect_type.SelectedIndex.ToString());
        }

        private void sequence_record_health_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.HealthKeys, (sender as Button), this.health_keysequence);
        }

        ////Player Mana

        private void mana_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.mana_enabled = (this.mana_enabled.IsChecked.HasValue) ? this.mana_enabled.IsChecked.Value : false;
        }

        private void mana_hasmana_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.mana_hasmana_colorpicker.SelectedColor.HasValue)
                Global.Configuration.mana_color = MediaColorToDrawingColor(this.mana_hasmana_colorpicker.SelectedColor.Value);
        }

        private void mana_nomana_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.mana_nomana_colorpicker.SelectedColor.HasValue)
                Global.Configuration.nomana_color = MediaColorToDrawingColor(this.mana_nomana_colorpicker.SelectedColor.Value);
        }

        private void mana_effect_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.Configuration.mana_effect_type = (PercentEffectType)Enum.Parse(typeof(PercentEffectType), this.mana_effect_type.SelectedIndex.ToString());
        }

        private void sequence_record_mana_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.ManaKeys, (sender as Button), this.mana_keysequence);
        }

        private void sequence_remove_mana_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.mana_keysequence))
                Global.Configuration.manaKeys = SequenceToList(this.mana_keysequence.Items);
        }

        private void sequence_up_mana_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.mana_keysequence))
                Global.Configuration.manaKeys = SequenceToList(this.mana_keysequence.Items);
        }

        private void sequence_down_ammo_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.mana_keysequence))
                Global.Configuration.manaKeys = SequenceToList(this.mana_keysequence.Items);
        }

        ////Static Keys

        private void sequence_remove_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void sequence_up_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void sequence_down_statickeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.statickeys_keysequence))
                Global.Configuration.staticKeys = SequenceToList(this.statickeys_keysequence.Items);
        }

        private void statickeys_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.statickeys_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.statickeys_color = MediaColorToDrawingColor(this.statickeys_color_colorpicker.SelectedColor.Value);
        }

        private void statickeys_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.statickeys_enabled = (this.statickeys_enabled.IsChecked.HasValue) ? this.statickeys_enabled.IsChecked.Value : false;
        }

        private void sequence_record_statickeys_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.StaticKeysGroup1, (sender as Button), this.statickeys_keysequence);
        }

        private void statickeys_2_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.statickeys_2_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.statickeys_2_color = MediaColorToDrawingColor(this.statickeys_2_color_colorpicker.SelectedColor.Value);
        }

        private void sequence_record_statickeys_2_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.StaticKeysGroup2, (sender as Button), this.statickeys_2_keysequence);
        }

        private void sequence_remove_statickeys_2_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.statickeys_2_keysequence))
                Global.Configuration.staticKeys_2 = SequenceToList(this.statickeys_2_keysequence.Items);
        }

        private void sequence_up_statickeys_2_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.statickeys_2_keysequence))
                Global.Configuration.staticKeys_2 = SequenceToList(this.statickeys_2_keysequence.Items);
        }

        private void sequence_down_statickeys_2_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.statickeys_2_keysequence))
                Global.Configuration.staticKeys_2 = SequenceToList(this.statickeys_2_keysequence.Items);
        }

        ////Typing Keys

        private void typing_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.typing_enabled = (this.typing_enabled.IsChecked.HasValue) ? this.typing_enabled.IsChecked.Value : false;
        }

        private void typing_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.typing_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.typing_color = MediaColorToDrawingColor(this.typing_color_colorpicker.SelectedColor.Value);
        }

        private void sequence_record_typingkeys_Click(object sender, RoutedEventArgs e)
        {
            RecordKeySequence(KeyboardRecordingType.TypingKeys, (sender as Button), this.typingkeys_keysequence);
        }

        private void sequence_remove_typingkeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxRemoveSelected(this.typingkeys_keysequence))
                Global.Configuration.typingKeys = SequenceToList(this.typingkeys_keysequence.Items);
        }

        private void sequence_up_typingkeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedUp(this.typingkeys_keysequence))
                Global.Configuration.typingKeys = SequenceToList(this.typingkeys_keysequence.Items);
        }

        private void sequence_down_typingkeys_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMoveSelectedDown(this.typingkeys_keysequence))
                Global.Configuration.typingKeys = SequenceToList(this.typingkeys_keysequence.Items);
        }

        ////Misc

        private void virtualkeyboard_key_selected(TextBlock key)
        {
            if(key.Tag is Devices.DeviceKeys)
            {
                if (recordingKeystrokes == KeyboardRecordingType.Ability1 || recordingKeystrokes == KeyboardRecordingType.Ability2 || 
                    recordingKeystrokes == KeyboardRecordingType.Ability3 || recordingKeystrokes == KeyboardRecordingType.Ability4 || 
                    recordingKeystrokes == KeyboardRecordingType.Ability5 || recordingKeystrokes == KeyboardRecordingType.AbilityUltimate ||

                    recordingKeystrokes == KeyboardRecordingType.ItemSlot1 || recordingKeystrokes == KeyboardRecordingType.ItemSlot2 ||
                    recordingKeystrokes == KeyboardRecordingType.ItemSlot3 || recordingKeystrokes == KeyboardRecordingType.ItemSlot4 ||
                    recordingKeystrokes == KeyboardRecordingType.ItemSlot5 || recordingKeystrokes == KeyboardRecordingType.ItemSlot6 ||
                    
                    recordingKeystrokes == KeyboardRecordingType.StashSlot1 || recordingKeystrokes == KeyboardRecordingType.StashSlot2 ||
                    recordingKeystrokes == KeyboardRecordingType.StashSlot3 || recordingKeystrokes == KeyboardRecordingType.StashSlot4 ||
                    recordingKeystrokes == KeyboardRecordingType.StashSlot5 || recordingKeystrokes == KeyboardRecordingType.StashSlot6
                    )
                {
                    //Single key

                    switch(recordingKeystrokes)
                    {
                        case (KeyboardRecordingType.Ability1):
                            this.ability_key1_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[0] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.Ability2):
                            this.ability_key2_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[1] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.Ability3):
                            this.ability_key3_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[2] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.Ability4):
                            this.ability_key4_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[3] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.Ability5):
                            this.ability_key5_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[4] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.AbilityUltimate):
                            this.ability_key6_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.ability_keys[5] = (Devices.DeviceKeys)key.Tag;
                            break;

                        case (KeyboardRecordingType.ItemSlot1):
                            this.item_slot1_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[0] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.ItemSlot2):
                            this.item_slot2_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[1] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.ItemSlot3):
                            this.item_slot3_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[2] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.ItemSlot4):
                            this.item_slot4_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[3] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.ItemSlot5):
                            this.item_slot5_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[4] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.ItemSlot6):
                            this.item_slot6_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[5] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot1):
                            this.stash_slot1_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[6] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot2):
                            this.stash_slot2_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[7] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot3):
                            this.stash_slot3_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[8] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot4):
                            this.stash_slot4_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[9] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot5):
                            this.stash_slot5_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[10] = (Devices.DeviceKeys)key.Tag;
                            break;
                        case (KeyboardRecordingType.StashSlot6):
                            this.stash_slot6_textblock.Text = ((Devices.DeviceKeys)(key.Tag)).ToString();
                            Global.Configuration.items_keys[11] = (Devices.DeviceKeys)key.Tag;
                            break;
                    }

                    recordingKeystrokes = KeyboardRecordingType.None;
                }
                else if (recordingKeystrokes != KeyboardRecordingType.None)
                {
                    //Multi key
                    if (recordedKeys.Contains((Devices.DeviceKeys)(key.Tag)))
                        recordedKeys.Remove((Devices.DeviceKeys)(key.Tag));
                    else
                        recordedKeys.Add((Devices.DeviceKeys)(key.Tag));
                    last_selected_key = key;
                }
            }
        }

        private void keyboard_grid_pressed(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock)
            {
                virtualkeyboard_key_selected(sender as TextBlock);
            }
        }

        private void keyboard_grid_moved(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TextBlock && last_selected_key != sender as TextBlock)
            {
                virtualkeyboard_key_selected(sender as TextBlock);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void trayicon_menu_quit_Click(object sender, RoutedEventArgs e)
        {
            virtual_keyboard_timer.Stop();
            trayicon.Visibility = System.Windows.Visibility.Hidden;

            Application.Current.Shutdown();
        }

        private void trayicon_menu_settings_Click(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Show();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Program.isSilent)
            {
                this.Visibility = System.Windows.Visibility.Hidden;
                this.WindowStyle = WindowStyle.None;
                this.ShowInTaskbar = false;
                Hide();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!shownHiddenMessage)
            {
                trayicon.ShowBalloonTip("Logitech - Dota 2 Integration", "This program is now hidden in the tray.", BalloonIcon.None);
                shownHiddenMessage = true;
            }

            Global.geh.SetPreview(false);

            //Hide Window
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (System.Windows.Threading.DispatcherOperationCallback)delegate(object o)
            {
                WindowStyle = WindowStyle.None;
                Hide();
                return null;
            }, null);
            //Do not close application
            e.Cancel = true;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Global.geh.SetPreview((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false);
            Global.geh.SetForcedUpdate((this.preview_enabled.IsChecked.HasValue) ? this.preview_enabled.IsChecked.Value : false);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Global.geh.SetPreview(false);
        }

        private void mimic_respawn_timer_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.mimic_respawn_timer = (this.mimic_respawn_timer_checkbox.IsChecked.HasValue) ? this.mimic_respawn_timer_checkbox.IsChecked.Value : false;
        }

        private void mimic_respawn_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.mimic_respawn_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.mimic_respawn_timer_color = MediaColorToDrawingColor(this.mimic_respawn_color_colorpicker.SelectedColor.Value);
        }

        private void mimic_respawn_respawning_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.mimic_respawn_respawning_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.mimic_respawn_timer_respawning_color = MediaColorToDrawingColor(this.mimic_respawn_respawning_color_colorpicker.SelectedColor.Value);
        }

        private void background_enable_respawn_glow_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.bg_respawn_glow = (this.background_enable_respawn_glow.IsChecked.HasValue) ? this.background_enable_respawn_glow.IsChecked.Value : false;
        }

        private void respawn_glow_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.respawn_glow_colorpicker.SelectedColor.HasValue)
                Global.Configuration.bg_respawn_glow_color = MediaColorToDrawingColor(this.respawn_glow_colorpicker.SelectedColor.Value);
        }

        private void abilities_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.abilitykeys_enabled = (this.abilities_enabled.IsChecked.HasValue) ? this.abilities_enabled.IsChecked.Value : false;
        }

        private void abilities_canuse_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.abilities_canuse_colorpicker.SelectedColor.HasValue)
                Global.Configuration.ability_can_use_color = MediaColorToDrawingColor(this.abilities_canuse_colorpicker.SelectedColor.Value);
        }

        private void abilities_cannotuse_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.abilities_cannotuse_colorpicker.SelectedColor.HasValue)
                Global.Configuration.ability_can_not_use_color = MediaColorToDrawingColor(this.abilities_cannotuse_colorpicker.SelectedColor.Value);
        }

        private void ability_key1_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.Ability1);
        }

        private void ability_key2_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.Ability2);
        }

        private void ability_key3_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.Ability3);
        }

        private void ability_key4_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.Ability4);
        }

        private void ability_key5_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.Ability5);
        }

        private void ability_key6_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.AbilityUltimate);
        }

        private void item_slot1_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot1);
        }

        private void item_slot2_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot2);
        }

        private void item_slot3_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot3);
        }

        private void item_slot4_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot4);
        }

        private void item_slot5_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot5);
        }

        private void item_slot6_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.ItemSlot6);
        }

        private void stash_slot1_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot1);
        }

        private void stash_slot2_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot2);
        }

        private void stash_slot3_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot3);
        }

        private void stash_slot4_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot4);
        }

        private void stash_slot5_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot5);
        }

        private void stash_slot6_textblock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RecordSingleKey(KeyboardRecordingType.StashSlot6);
        }

        private void item_empty_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.item_empty_colorpicker.SelectedColor.HasValue)
                Global.Configuration.items_empty_color = MediaColorToDrawingColor(this.item_empty_colorpicker.SelectedColor.Value);
        }

        private void items_enabled_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.items_enabled = (this.items_enabled.IsChecked.HasValue) ? this.items_enabled.IsChecked.Value : false;
        }

        private void item_on_cooldown_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.item_on_cooldown_colorpicker.SelectedColor.HasValue)
                Global.Configuration.items_on_cooldown_color = MediaColorToDrawingColor(this.item_on_cooldown_colorpicker.SelectedColor.Value);
        }

        private void item_no_charges_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.item_no_charges_colorpicker.SelectedColor.HasValue)
                Global.Configuration.items_no_charges_color = MediaColorToDrawingColor(this.item_no_charges_colorpicker.SelectedColor.Value);
        }

        private void item_color_colorpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (this.item_color_colorpicker.SelectedColor.HasValue)
                Global.Configuration.items_color = MediaColorToDrawingColor(this.item_color_colorpicker.SelectedColor.Value);
        }

        private void items_use_item_color_Checked(object sender, RoutedEventArgs e)
        {
            Global.Configuration.items_use_item_color = (this.items_use_item_color.IsChecked.HasValue) ? this.items_use_item_color.IsChecked.Value : false;
        }

        private void RecordSingleKey(KeyboardRecordingType key)
        {
            if (recordingKeystrokes == key)
            {
                recordingKeystrokes = KeyboardRecordingType.None;
                recording_stopwatch.Stop();
            }
            else if (recordingKeystrokes == KeyboardRecordingType.None)
            {
                recordingKeystrokes = key;
                recording_stopwatch.Restart();
            }
            else
                System.Windows.MessageBox.Show("You are already recording a key sequence for " + recordingKeystrokes.ToString());
        }
    }
}
