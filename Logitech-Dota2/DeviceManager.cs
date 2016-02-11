using Logitech_Dota2.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitech_Dota2
{
    public class DeviceManager
    {
        private List<Device> devices = new List<Device>();

        public DeviceManager()
        {
            devices.Add(new LogitechDevice());
            devices.Add(new CorsairDevice());
        }

        public bool Initialize()
        {
            bool anyInitialized = false;

            foreach (Device device in devices)
            {
                if (device.Initialize())
                    anyInitialized = true;

                Global.logger.LogLine("Device, " + device.GetDeviceName() + ", was" + (device.IsInitialized() ? "" : " not") + " initialized", Logging_Level.Info);
            }

            return anyInitialized;
        }

        public Dictionary<string, bool> GetInitializedDevices()
        {
            Dictionary<string, bool> ret = new Dictionary<string, bool>();

            foreach (Device device in devices)
            {
                ret.Add(device.GetDeviceName(), device.IsInitialized());
            }

            return ret;
        }

        public void ResetDevices()
        {
            foreach (Device device in devices)
            {
                if (device.IsInitialized())
                {
                    device.Reset();
                }
            }
        }

        public bool UpdateDevices(Dictionary<DeviceKeys, System.Drawing.Color> keyColors, bool forced = false)
        {
            bool anyUpdated = false;
            Dictionary<DeviceKeys, System.Drawing.Color> _keyColors = new Dictionary<DeviceKeys, System.Drawing.Color>(keyColors);

            foreach (Device device in devices)
            {
                if (device.IsInitialized())
                {
                    if (device.UpdateDevice(_keyColors, forced))
                        anyUpdated = true;
                }
            }

            return anyUpdated;
        }

    }
}
