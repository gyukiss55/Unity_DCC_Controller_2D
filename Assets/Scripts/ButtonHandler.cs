
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
        public GameObject m_Functions;
        public GameObject m_Group1;
        public GameObject m_Group2;
        public Button m_DirectionButton1;
        public Button m_DirectionButton2;
        public Slider m_SpeedSlider;
        public TMP_Dropdown m_FunctionDropdown;

        static int timeStamp = 0;

        bool m_Inicialized = false;

        class ControlTransformation
        {
            public Vector3 position;
            public Vector3 scale;
        };

        class ViewParams
        {
            public List<ControlTransformation> functionControl;
            public List<ControlTransformation> group1Control;
            public List<ControlTransformation> group2Control;

        };

        ViewParams viewParams = new ViewParams();

        private void Start()
        {
            viewParams.functionControl = new List<ControlTransformation>();
            viewParams.group1Control = new List<ControlTransformation>();
            viewParams.group2Control = new List<ControlTransformation>();
        }


        private void Update()
        {
            if (!m_Inicialized)
            {
                m_Inicialized = true;
                DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
                CrossCaller.GetUtils().UpdateButtonColor(m_DirectionButton1, deviceInfo.m_devices[deviceInfo.m_channel].backward);
                CrossCaller.GetUtils().UpdateButtonColor(m_DirectionButton2, deviceInfo.m_devices[deviceInfo.m_channel].backward);
            }
        }

        void SetView (int viewMode)
        {
            Debug.Log($"SetView {viewMode}");

            if (viewParams.functionControl.Count == 0)
            {
                const float offsetX = 800;

                ControlTransformation cf1 = new ControlTransformation(); // function buttons group
                cf1.position = m_Functions.transform.position;
                viewParams.functionControl.Add(cf1);
                ControlTransformation cf2 = new ControlTransformation();
                cf2.position = m_Functions.transform.position + new Vector3(1000f,0,0);
                viewParams.functionControl.Add(cf2);

                ControlTransformation cg11 = new ControlTransformation(); // group 1 buttons
                cg11.position = m_Group1.transform.position;
                viewParams.group1Control.Add(cg11);
                ControlTransformation cg12 = new ControlTransformation();
                cg12.position = m_Group1.transform.position + new Vector3(offsetX, 0, 0);
                viewParams.group1Control.Add(cg12);

                ControlTransformation cg21 = new ControlTransformation(); // group 1 buttons
                cg21.position = m_Group2.transform.position;
                viewParams.group2Control.Add(cg21);
                ControlTransformation cg22 = new ControlTransformation();
                cg22.position = m_Group2.transform.position + new Vector3(700, 0, 0);
                viewParams.group2Control.Add(cg22);

             }

            if (viewParams.functionControl.Count > viewMode)
            {
                m_Functions.transform.position = viewParams.functionControl[viewMode].position;
                Debug.Log($"m_Functions position {viewParams.functionControl[viewMode].position}");
            }

            if (viewParams.group1Control.Count > viewMode)
            {
                m_Group1.transform.position = viewParams.group1Control[viewMode].position;
                Debug.Log($"m_Group1 position {viewParams.group1Control[viewMode].position}");
            }

            if (viewParams.group2Control.Count > viewMode)
            {
                m_Group2.transform.position = viewParams.group2Control[viewMode].position;
                Debug.Log($"m_Group2 position {viewParams.group2Control[viewMode].position}");
            }


        }


        static void RequestResultCallback(string result)
        {
            string log = "Result: " + result;
            CrossCaller.GetInputFieldHandler().SetStatus(log);
        }

        static void SendHTMLCommand(string ipAddress, int channel, string commandIn)
        {
            ++timeStamp;
            string command = commandIn + timeStamp.ToString("X4");
            Utils.DebugLog("SendHTMLCommand " + ipAddress + $", {channel}, " + command);
            Utils utils = CrossCaller.GetUtils();
            if (utils != null)
            {
                string log = "WebCommand " + ipAddress + " " + command;
                CrossCaller.GetInputFieldHandler ().SetStatus(log);
                utils.SendDeviceCommand(ipAddress, 0, command, RequestResultCallback);
            }

        }

        void SendSpeedCommand(int channel, DeviceInfo.DeviceInfoData data)
        {
            bool backward = data.backward;
            Debug.Log($"SendSpeedCommand data.speed:{data.speed}");
            int speedTmp = 0;
            if (data.speed > 0)
                speedTmp = (((data.speed + 1) / 2) + 1);
            if (data.speed > 0 && ((data.speed % 2) == 0))
                speedTmp += 0x10;
            Debug.Log($"SendSpeedCommand speedTmp:{speedTmp}");
            if (backward)
                Debug.Log($"SendSpeedCommand backward");
            else
                Debug.Log($"SendSpeedCommand forward");

            string command = "";

            int speedPrefix = 0x60;
            if (backward)
                speedPrefix = 0x40;
            command = data.deviceID.ToString("X2") + (speedPrefix + speedTmp).ToString("X2"); 

            Debug.Log($"SendSpeedCommand {command}");

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

        public void UpdateButton(DeviceInfo deviceInfo)
        {
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];

            string nameButton = "Forward";
            if (data.backward)
                nameButton = "Backward";
            m_DirectionButton1.GetComponentInChildren<TextMeshProUGUI>().text = nameButton;
            m_DirectionButton2.GetComponentInChildren<TextMeshProUGUI>().text = nameButton;

            CrossCaller.GetUtils().UpdateButtonColor(m_DirectionButton1, data.backward);
            CrossCaller.GetUtils().UpdateButtonColor(m_DirectionButton2, data.backward);

        }


        public void OnClickMode()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            if (deviceInfo.m_mode < 1)
                deviceInfo.m_mode++;
            else
                deviceInfo.m_mode = 0;
            SetView(deviceInfo.m_mode);
        }

        public void OnClickChDown()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            if (deviceInfo.m_channel > 1)
            {
                deviceInfo.m_channel--;
                deviceInfo.m_changed = true;
            }
        }

        public void OnClickChUp()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            if (deviceInfo.m_channel < 4)
            {
                deviceInfo.m_channel++;
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickSpeedUp()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            Debug.Log($"OnClickSpeedUp maxSpeed {data.maxSpeed}");
//            if (data.speed < data.maxSpeed)
            if (data.speed < 28)
            {
                data.speed++;
                SendSpeedCommand(deviceInfo.m_channel, data);
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickSpeedDown()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            Debug.Log($"OnClickSpeedDown maxSpeed {data.maxSpeed}");
            if (data.speed > 0)
            {
                data.speed--;
                SendSpeedCommand(deviceInfo.m_channel, data);
                deviceInfo.m_changed = true;

            }
        }
        public void OnClickDirection()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.speed = 0;
            data.backward = !data.backward;
            SendSpeedCommand(deviceInfo.m_channel, data);
            UpdateButton(deviceInfo);
            deviceInfo.m_changed = true;
        }
        public void OnClickMax()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            //data.speed = data.maxSpeed;
            data.speed = 28;
            SendSpeedCommand(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickStop()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.speed = 0;
            SendSpeedCommand(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }

        public void OnClickEmergencyStop()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            foreach (DeviceInfoData data1 in deviceInfo.m_devices) { 
                data1.speed = 0;
                data1.backward = false;
            }
            string command = "0061";
            SendHTMLCommand(data.ipAddress, 0, command);
            deviceInfo.m_changed = true;
        }

        bool IsHackCommand (string cmd)
        {
            Debug.Log($"hack: {cmd}");
            if (cmd.Length == 4 && cmd[0] == 'F' && cmd[1] == 'F')
            {
                if (cmd[2] >= '0' && cmd[2] <= '9' && cmd[3] >= '0' && cmd[3] <= '9')
                {
                    float value = (cmd[2] - '0') * 10 + (cmd[3] - '0');
                    value = value / 10f;
                    if (value > 0.3f)
                    {
                        GameObject mainPanel = GameObject.Find("MainPanel");
                        if (mainPanel != null)
                        {
                            Debug.Log($"hack scale: {value}");
                            mainPanel.transform.transform.localScale = new Vector3(value, value, value);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void OnClickExplicitCommand()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            if (IsHackCommand(data.explicitCommand))
                return;
            string command = (data.deviceID).ToString("X2") + data.explicitCommand;
            SendHTMLCommand(data.ipAddress, deviceInfo.m_channel, command);
        }

        public void OnValueChangeSlider()
        {
            float v = m_SpeedSlider.value;
            Debug.Log($"OnValueChangeSlider:{v}");
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            //data.speed = data.maxSpeed;
            data.speed = (int)(28f * v);
            SendSpeedCommand(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }

        public void OnValueChangeDropDown()
        {
            string s = m_FunctionDropdown.value.ToString ();
            Debug.Log($"OnValueChangeDropDown:{s}");
        }

        public void OnClickF01()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x10;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF02()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x01;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF03()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x02;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;

        }
        public void OnClickF04()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x04;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF05()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x08;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF06()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x01;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF07()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x02;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF08()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x04;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF09()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x08;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF10()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x01;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF11()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x02;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF12()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x04;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF13()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x08;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF14()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x01;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF15()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x02;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF16()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x04;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF17()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x08;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF18()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x10;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF19()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x20;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF20()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x40;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF21()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x80;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF22()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x01;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF23()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x02;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF24()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x04;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF25()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x08;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF26()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x10;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF27()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x20;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF28()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x40;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
        public void OnClickF29()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x80;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
         public void OnClickF30()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f29_36status ^= 0x01;
            SendF29_36Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
        }
    }

}
