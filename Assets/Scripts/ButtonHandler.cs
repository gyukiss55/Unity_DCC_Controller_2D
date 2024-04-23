
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Utils_NS;

using DCC_Controller_NS;
using UnityEditor;
using static DCC_Controller_NS.DeviceInfo;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;

namespace DCC_Controller_NS
{

    public class ButtonHandler : MonoBehaviour
    {
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
            string log = "RequestResultCallback " + result;
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
                string log = "SendDeviceCommand " + ipAddress + "" + command;
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

        void SendF1_5Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + (data.f1_5status + 0x80).ToString("X2"); ;
            SendHTMLCommand(data.ipAddress, channel, command);
        }

        void SendF6_10Command(int channel, DeviceInfo.DeviceInfoData data)
        {
            string command = data.deviceID.ToString("X2") + (data.f6_10status + 0xA0).ToString("X2"); ;
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
            data.f1_5status ^= 0x01;
            SendF1_5Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF02()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_5status ^= 0x02;
            SendF1_5Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF03()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_5status ^= 0x04;
            SendF1_5Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF04()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_5status ^= 0x08;
            SendF1_5Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF05()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_5status ^= 0x10;
            SendF1_5Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF06()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f6_10status ^= 0x01;
            SendF6_10Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF07()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f6_10status ^= 0x02;
            SendF6_10Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF08()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f6_10status ^= 0x04;
            SendF6_10Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF09()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f6_10status ^= 0x08;
            SendF6_10Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF10()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f6_10status ^= 0x10;
            SendF6_10Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
    }

}
