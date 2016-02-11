using Dota2GSI;
using System;
using System.Windows;

namespace Logitech_Dota2
{
    public static class Global {

        public static Logger logger = new Logger();
        public static GameEventHandler geh = new GameEventHandler();
        public static GameStateListener gsl;
        public static Configuration Configuration { get; set; }
        public static DeviceManager dev_manager = new DeviceManager();
        public static KeyboardLayouts kbLayout;
    }
    
    static class Program
    {
        public static Application WinApp { get; private set; }
        public static Window MainWindow;

        public static bool isSilent = false;

        [STAThread]
        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                switch (arg)
                {
                    case("-silent"):
                        Global.logger.LogLine("Program started with '-silent' parameter", Logging_Level.Info);
                        isSilent = true;
                        break;
                }
            }
            
            //Load config
            try
            {
                Global.Configuration = ConfigManager.Load("Config");
            }
            catch (Exception e)
            {
                Global.logger.LogLine("Exception during ConfigManager.Load(). Error: " + e, Logging_Level.Error);
            }

            if (!Global.geh.Init())
                return;

            Global.gsl = new GameStateListener("http://127.0.0.1:30742/");
            Global.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);

            if (!Global.gsl.Start())
            {
                Global.logger.LogLine("GameStateListener could not start", Logging_Level.Error);
                System.Windows.MessageBox.Show("GameStateListener could not start. Try running this program as Administrator.\r\nExiting.");
                Environment.Exit(0);
            }

            Global.logger.LogLine("Listening for game integration calls...", Logging_Level.None);

            Global.kbLayout = new KeyboardLayouts();

            foreach (var device in Global.dev_manager.GetInitializedDevices())
            {
                if (!device.Key.Equals("Logitech") && device.Value)
                {
                    switch (device.Key)
                    {
                        case ("Corsair"):
                            Global.kbLayout = new KeyboardLayouts(KeyboardBrand.Corsair);
                            break;
                        default:
                            continue;
                    }

                    break;
                }
            }

            MainWindow = new ConfigUI();
            WinApp = new Application();
            WinApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            WinApp.MainWindow = MainWindow;
            WinApp.Run(MainWindow);

            ConfigManager.Save("Config", Global.Configuration);

            Global.geh.Destroy();
            Global.gsl.Stop();

            Environment.Exit(0);
        }

        static void OnNewGameState(GameState gs)
        {
            try
            {
                Global.geh.SetMapTime(gs.Map.GameTime);
                Global.geh.SetTeam(gs.Player.Team);
                Global.geh.SetAlive(gs.Hero.IsAlive);
                Global.geh.SetRespawnTime(gs.Hero.SecondsToRespawn);
                Global.geh.SetHealth(gs.Hero.Health);
                Global.geh.SetHealthMax(gs.Hero.MaxHealth);
                Global.geh.SetMana(gs.Hero.Mana);
                Global.geh.SetManaMax(gs.Hero.MaxMana);
                Global.geh.SetPlayerActivity(gs.Player.Activity);
                Global.geh.SetAbilities(gs.Abilities);
                Global.geh.SetItems(gs.Items);
                Global.geh.SetKillStreak(gs.Player.KillStreak);
                Global.geh.SetKills(gs.Player.Kills);


                if (gs.Previously.Hero.HealthPercent == 0 && gs.Hero.HealthPercent == 100 && !gs.Previously.Hero.IsAlive && gs.Hero.IsAlive)
                {
                    Global.geh.Respawned();
                }

                if (gs.Previously.Player.Kills != -1 && gs.Previously.Player.Kills < gs.Player.Kills)
                {
                    Global.geh.GotAKill();
                }
            }
            catch(Exception e)
            {
                Global.logger.LogLine("Exception during OnNewGameState. Error: " + e, Logging_Level.Error);
            
            }
        }
    }
}
