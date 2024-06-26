
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
using System;
using System.Linq;
using System.Threading;
//using UnityEngine.UIElements;

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
        List<Func<int>> m_FunctionList = new List<Func<int>>();

        static int timeStamp = 0;

        bool m_Inicialized = false;

        float m_lastTime = 0;
        int m_lastFunc = -1;
        // WaitForSend coroutine
        static float speedPrev = 0;
        static float timePrev = 0;
        static int  callNumber = 0;
        private static Mutex mutexSpeed = new Mutex();

        class ControlTransformation
        {
            public Vector3 position;
//            public Vector3 scale;
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
            m_FunctionList.Add(OnClickF01Ref);
            m_FunctionList.Add(OnClickF02Ref);
            m_FunctionList.Add(OnClickF03Ref);
            m_FunctionList.Add(OnClickF04Ref);
            m_FunctionList.Add(OnClickF05Ref);
            m_FunctionList.Add(OnClickF06Ref);
            m_FunctionList.Add(OnClickF07Ref);
            m_FunctionList.Add(OnClickF08Ref);
            m_FunctionList.Add(OnClickF09Ref);
            m_FunctionList.Add(OnClickF10Ref);
            m_FunctionList.Add(OnClickF11Ref);
            m_FunctionList.Add(OnClickF12Ref);
            m_FunctionList.Add(OnClickF13Ref);
            m_FunctionList.Add(OnClickF14Ref);
            m_FunctionList.Add(OnClickF15Ref);
            m_FunctionList.Add(OnClickF16Ref);
            m_FunctionList.Add(OnClickF17Ref);
            m_FunctionList.Add(OnClickF18Ref);
            m_FunctionList.Add(OnClickF19Ref);
            m_FunctionList.Add(OnClickF20Ref);
            m_FunctionList.Add(OnClickF21Ref);
            m_FunctionList.Add(OnClickF22Ref);
            m_FunctionList.Add(OnClickF23Ref);
            m_FunctionList.Add(OnClickF24Ref);
            m_FunctionList.Add(OnClickF25Ref);
            m_FunctionList.Add(OnClickF26Ref);
            m_FunctionList.Add(OnClickF27Ref);
            m_FunctionList.Add(OnClickF28Ref);
            m_FunctionList.Add(OnClickF29Ref);
            m_FunctionList.Add(OnClickF30Ref);
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
            }

            if (viewParams.group1Control.Count > viewMode)
            {
                m_Group1.transform.position = viewParams.group1Control[viewMode].position;
            }

            if (viewParams.group2Control.Count > viewMode)
            {
                m_Group2.transform.position = viewParams.group2Control[viewMode].position;
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
                utils.SendDeviceCommand(ipAddress, channel - 1, command, RequestResultCallback);
            }

        }

        void SendSpeedCommand(int channel, DeviceInfo.DeviceInfoData data)
        {
            bool backward = data.backward;
            int speedTmp = 0;
            if (data.speed > 0)
                speedTmp = (((data.speed + 1) / 2) + 1);
            if (data.speed > 0 && ((data.speed % 2) == 0))
                speedTmp += 0x10;

            string command = "";

            int speedPrefix = 0x60;
            if (backward)
                speedPrefix = 0x40;
            command = data.deviceID.ToString("X2") + (speedPrefix + speedTmp).ToString("X2"); 
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

        private IEnumerator WaitForSend(float speed)
        {

            bool execute = false;
            float timeNow = Time.deltaTime * 1000f;
            float deltaTime = timeNow - timePrev;
            int callNumberNow = 0; 


            mutexSpeed.WaitOne();
            speedPrev = speed;
            timePrev = timeNow;
            callNumber++;
            callNumberNow = callNumber;
            mutexSpeed.ReleaseMutex();

            if (deltaTime < 100)
            {
                yield return new WaitForSeconds(0.1f);

            }
            if (callNumberNow == callNumber)
                execute = true;

            if (execute)
            {
                DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
                DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
                data.speed = (int)(28f * speed);
                SendSpeedCommand(deviceInfo.m_channel, data);
                deviceInfo.m_changed = true;
            }
 
        }

        public void OnValueChangeSlider()
        {
            float v = m_SpeedSlider.value;
            IEnumerator coroutine = WaitForSend(v);
            StartCoroutine(coroutine);
         }

        public void OnValueChangeDropDown()
        {
            float millis = Time.realtimeSinceStartup * 1000f;
            int index = m_FunctionDropdown.value;
            if ((millis - m_lastTime) < 100f && m_lastFunc == index) {
                return;
            }
            m_lastFunc = index;
            m_lastTime = millis;

            if (index >= 0 && index < m_FunctionList.Count)
                m_FunctionList[index]();

            string s = m_FunctionDropdown.value.ToString ();
        }

        int  OnClickF01Ref()
        {

            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x10;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }

        public int OnClickF02Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x01;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;

        }
        public int OnClickF03Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x02;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;

        }
        public int OnClickF04Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x04;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
       public int OnClickF05Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f1_4status ^= 0x08;
            SendF0_4Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF06Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x01;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF07Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x02;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF08Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x04;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF09Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f5_8status ^= 0x08;
            SendF5_8Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF10Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x01;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF11Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x02;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF12Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x04;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF13Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f9_12status ^= 0x08;
            SendF9_12Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF14Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x01;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF15Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x02;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF16Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x04;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF17Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x08;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF18Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x10;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF19Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x20;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF20Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x40;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF21Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f13_20status ^= 0x80;
            SendF13_20Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF22Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x01;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF23Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x02;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF24Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x04;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF25Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x08;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF26Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x10;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF27Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x20;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF28Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x40;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF29Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f21_28status ^= 0x80;
            SendF21_28Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public int OnClickF30Ref()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.f29_36status ^= 0x01;
            SendF29_36Command(deviceInfo.m_channel, data);
            deviceInfo.m_changed = true;
            return 0;
        }
        public void OnClickF01() => OnClickF01Ref();
        public void OnClickF02() => OnClickF02Ref();
        public void OnClickF03() => OnClickF03Ref();
        public void OnClickF04() => OnClickF04Ref();
        public void OnClickF05() => OnClickF05Ref();
        public void OnClickF06() => OnClickF06Ref();
        public void OnClickF07() => OnClickF07Ref();
        public void OnClickF08() => OnClickF08Ref();
        public void OnClickF09() => OnClickF09Ref();
        public void OnClickF10() => OnClickF10Ref();
        public void OnClickF11() => OnClickF11Ref();
        public void OnClickF12() => OnClickF12Ref();
        public void OnClickF13() => OnClickF13Ref();
        public void OnClickF14() => OnClickF14Ref();
        public void OnClickF15() => OnClickF15Ref();
        public void OnClickF16() => OnClickF16Ref();
        public void OnClickF17() => OnClickF17Ref();
        public void OnClickF18() => OnClickF18Ref();
        public void OnClickF19() => OnClickF19Ref();
        public void OnClickF20() => OnClickF20Ref();
        public void OnClickF21() => OnClickF21Ref();
        public void OnClickF22() => OnClickF22Ref();
        public void OnClickF23() => OnClickF23Ref();
        public void OnClickF24() => OnClickF24Ref();
        public void OnClickF25() => OnClickF25Ref();
        public void OnClickF26() => OnClickF26Ref();
        public void OnClickF27() => OnClickF27Ref();
        public void OnClickF28() => OnClickF28Ref();
        public void OnClickF29() => OnClickF29Ref();
        public void OnClickF30() => OnClickF30Ref();
    }

}
