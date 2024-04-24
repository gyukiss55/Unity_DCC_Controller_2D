
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

using Utils_NS;

using DCC_Controller_NS;
using static DCC_Controller_NS.DeviceInfo;


namespace DCC_Controller_NS
{

    public class ButtonHandler : MonoBehaviour
    {
        public Button m_F01_Button;

        static int timeStamp = 0;

        static public Utils GetUtils()
        {
            Utils utils = null;
            GameObject mainControl = GameObject.Find("MainControl");
            if (mainControl == null)
            {
                Utils.DebugLog("MainControl not found!");
                return utils;
            }
            utils = mainControl.GetComponent<Utils>();
            if (utils == null)
            {
                Utils.DebugLog("DeviceInfo not found!");
            }
            return utils;

        }

        static public InputFieldHandler GetInputFieldHandler()
        {
            InputFieldHandler inputFieldHandler = null;
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Utils.DebugLog("Canvas not found!");
                return inputFieldHandler;
            }
            inputFieldHandler = canvas.GetComponent<InputFieldHandler>();
            if (inputFieldHandler == null)
            {
                Utils.DebugLog("InputFieldHandler not found!");
            }
            return inputFieldHandler;

        }

 
        static void RequestResultCallback(string result)
        {
            string log = "Result: " + result;
            GetInputFieldHandler ().SetStatus(log);
        }

        static void SendHTMLCommand(string ipAddress, int channel, string commandIn)
        {
            ++timeStamp;
            string command = commandIn + timeStamp.ToString("X4");
            Utils.DebugLog("SendHTMLCommand " + ipAddress + $", {channel}, " + command);
            Utils utils = GetUtils();
            if (utils != null)
            {
                string log = "WebCommand " + ipAddress + " " + command;
                GetInputFieldHandler ().SetStatus(log);
                utils.SendDeviceCommand(ipAddress, 0, command, RequestResultCallback);
            }

        }

        void SendSpeedCommand(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + 0x60.ToString("X2");
            if (data.speed > 0) {
                command = data.deviceID.ToString ("X2") + (0x61 + data.speed).ToString("X2");
            } else if (data.speed < 0) {
                command = data.deviceID.ToString("X2") + (0x41 - data.speed).ToString("X2"); ;
            }
            SendHTMLCommand(data.ipAddress, channel, command);
        }

        void SendF0_4Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + (data.f1_4status + 0x80).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }

        void SendF5_8Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + (data.f5_8status + 0xB0).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }
        void SendF9_12Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + (data.f9_12status + 0xA0).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }

        void SendF13_20Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + "DE" + (data.f13_20status).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }
        void SendF21_28Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + "DF" + (data.f21_28status).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }
        void SendF29_36Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + "D8" + (data.f29_36status).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }

        public void OnClickChDown()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            if (deviceInfo.m_channel > 1)
            {
                deviceInfo.m_channel--;
                deviceInfo.m_changed = true;
            }
        }

        public void OnClickChUp()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            if (deviceInfo.m_channel < 4)
            {
                deviceInfo.m_channel++;
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickSpeedUp()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            if (data.speed < 14)
            {
                data.speed++;
                SendSpeedCommand(deviceInfo.m_channel, data);
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickSpeedDown()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            if (data.speed > -14)
            {
                data.speed--;
                SendSpeedCommand(deviceInfo.m_channel, data);
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickStop()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.speed = 0;
            SendSpeedCommand(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }

        public void OnClickEmergencyStop()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            foreach (DeviceInfoData data1 in deviceInfo.m_devices) { 
                data1.speed = 0;
            }
            string command = "0061";
            SendHTMLCommand(data.ipAddress, 0, command);
            deviceInfo.m_changed = true;
        }

        public void OnClickExplicitCommand()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            string command = (data.deviceID).ToString("X2") + data.explicitCommand;
            SendHTMLCommand(data.ipAddress, deviceInfo.m_channel, command);
        }
        public void OnClickF01()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x10;
            ColorBlock cb = m_F01_Button.colors;
            if ((data.f1_4status & 0x10) == 0)
            {
                cb.normalColor = new Color(0.9f, 0, 0);
            } else
            {
                cb.normalColor = new Color(0, 0.9f, 0);
            }
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF02()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x01;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF03()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x02;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF04()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x04;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF05()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x08;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF06()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x01;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF07()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x02;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF08()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x04;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF09()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x08;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF10()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x01;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF11()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x02;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF12()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x04;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF13()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x08;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF14()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x01;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF15()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x02;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF16()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x04;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF17()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x08;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF18()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x10;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF19()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x20;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF20()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x40;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF21()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x80;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF22()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x01;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF23()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x02;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF24()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x04;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF25()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x08;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF26()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x10;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF27()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x20;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF28()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x40;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF29()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x80;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
         public void OnClickF30()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f29_36status ^= 0x01;
            SendF29_36Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
    }

}
