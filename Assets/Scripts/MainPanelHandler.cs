using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Utils_NS;

namespace DCC_Controller_NS
{


    public class MainPanelHandler : MonoBehaviour
    {

        public TextMeshProUGUI m_TextMeshProUGUI;
        public TMP_InputField m_ChannelNr;
        public TMP_InputField m_IPAddress;
        public TMP_InputField m_DeviceID;
        public TMP_InputField m_speed;
        public TMP_InputField m_ExplicitCommand;
        public Button m_F01;
        public Button m_F02;
        public Button m_F03;
        public Button m_F04;
        public Button m_F05;
        public Button m_F06;
        public Button m_F07;
        public Button m_F08;
        public Button m_F09;
        public Button m_F10;
        public Button m_F11;
        public Button m_F12;
        public Button m_F13;
        public Button m_F14;
        public Button m_F15;
        public Button m_F16;
        public Button m_F17;
        public Button m_F18;
        public Button m_F19;
        public Button m_F20;
        public Button m_F21;
        public Button m_F22;
        public Button m_F23;
        public Button m_F24;
        public Button m_F25;
        public Button m_F26;
        public Button m_F27;
        public Button m_F28;
        public Button m_F29;
        public Button m_F30;

        Color colorOn = new Color(1, 0.92f, 0.016f, 1); // yellow
        Color colorOff = new Color(0, 1, 1, 1); // cyan
        Color colorHighOn = new Color(1, 1, 0.03f, 1); // yellow
        Color colorHighOff = new Color(0.2f, 1, 1, 1); // cyan
        Color colorDarkOn = new Color(0.8f, 0.92f, 0.016f, 1); // yellow
        Color colorDarkOff = new Color(0, 0.8f, 0.8f, 1); // cyan

        // Start is called before the first frame update
        void Start()
        {
            Utils.DebugLog("MainPanelHandler start...");

        }

        // Update is called once per frame
        void Update()
        {
            DeviceInfo info = GetDeviceInfo();
            if (info.m_changed)
            {
                UpdateInputFields();
                info.m_changed = false;
            }
        }

        static public DeviceInfo GetDeviceInfo()
        {
            DeviceInfo deviceInfo = null;
            GameObject mainControl = GameObject.Find("MainControl");
            if (mainControl == null)
            {
                Utils.DebugLog("MainControl not found!");
                return deviceInfo;
            }
            deviceInfo = mainControl.GetComponent<DeviceInfo>();
            if (deviceInfo == null)
            {
                Utils.DebugLog("DeviceInfo not found!");
            }
            return deviceInfo;

        }

        void UpdateTMP_InputField(Button tmp_button, int value, int mask)
        {
            ColorBlock cb = tmp_button.colors;
            if ((value & mask) == 0)
            {
                Color c = colorOff;
                cb.normalColor = c;
                cb.selectedColor = c;
                cb.highlightedColor = colorHighOff;
                cb.pressedColor = colorDarkOff;
            } else
            {
                Color c = colorOn;
                cb.normalColor = c;
                cb.selectedColor = c;
                cb.highlightedColor = colorHighOn;
                cb.pressedColor = colorDarkOn;
            }
            tmp_button.colors = cb;
        }


        public void UpdateInputFields()
        {
            DeviceInfo deviceInfo = GetDeviceInfo();
            int ch = deviceInfo.m_channel;
            DeviceInfo.DeviceInfoData deviceInfoData = deviceInfo.m_devices[ch - 1];

            m_ChannelNr.text = string.Format("{0}", ch);
            m_IPAddress.text = deviceInfoData.ipAddress;
            m_DeviceID.text = string.Format("{0}", deviceInfoData.deviceID);
            m_speed.text = string.Format("{0}", deviceInfoData.speed);

            m_ExplicitCommand.text = deviceInfoData.explicitCommand;

            UpdateTMP_InputField(m_F01, deviceInfoData.f1_4status, 0x10);
            UpdateTMP_InputField(m_F02, deviceInfoData.f1_4status, 0x01);
            UpdateTMP_InputField(m_F03, deviceInfoData.f1_4status, 0x02);
            UpdateTMP_InputField(m_F04, deviceInfoData.f1_4status, 0x04);
            UpdateTMP_InputField(m_F05, deviceInfoData.f1_4status, 0x08);

            UpdateTMP_InputField(m_F06, deviceInfoData.f5_8status, 0x01);
            UpdateTMP_InputField(m_F07, deviceInfoData.f5_8status, 0x02);
            UpdateTMP_InputField(m_F08, deviceInfoData.f5_8status, 0x04);
            UpdateTMP_InputField(m_F09, deviceInfoData.f5_8status, 0x08);

            UpdateTMP_InputField(m_F10, deviceInfoData.f9_12status, 0x01);
            UpdateTMP_InputField(m_F11, deviceInfoData.f9_12status, 0x02);
            UpdateTMP_InputField(m_F12, deviceInfoData.f9_12status, 0x04);
            UpdateTMP_InputField(m_F13, deviceInfoData.f9_12status, 0x08);

            UpdateTMP_InputField(m_F14, deviceInfoData.f13_20status, 0x01);
            UpdateTMP_InputField(m_F15, deviceInfoData.f13_20status, 0x02);
            UpdateTMP_InputField(m_F16, deviceInfoData.f13_20status, 0x04);
            UpdateTMP_InputField(m_F17, deviceInfoData.f13_20status, 0x08);
            UpdateTMP_InputField(m_F18, deviceInfoData.f13_20status, 0x10);
            UpdateTMP_InputField(m_F19, deviceInfoData.f13_20status, 0x20);
            UpdateTMP_InputField(m_F20, deviceInfoData.f13_20status, 0x40);
            UpdateTMP_InputField(m_F21, deviceInfoData.f13_20status, 0x80);

            UpdateTMP_InputField(m_F22, deviceInfoData.f21_28status, 0x01);
            UpdateTMP_InputField(m_F23, deviceInfoData.f21_28status, 0x02);
            UpdateTMP_InputField(m_F24, deviceInfoData.f21_28status, 0x04);
            UpdateTMP_InputField(m_F25, deviceInfoData.f21_28status, 0x08);
            UpdateTMP_InputField(m_F26, deviceInfoData.f21_28status, 0x10);
            UpdateTMP_InputField(m_F27, deviceInfoData.f21_28status, 0x20);
            UpdateTMP_InputField(m_F28, deviceInfoData.f21_28status, 0x40);
            UpdateTMP_InputField(m_F29, deviceInfoData.f21_28status, 0x80); 

            UpdateTMP_InputField(m_F30, deviceInfoData.f29_36status, 0x01);
            //UpdateTMP_InputField(m_F31, deviceInfoData.f29_36status, 0x02);
            //UpdateTMP_InputField(m_F32, deviceInfoData.f29_36status, 0x04);
            //UpdateTMP_InputField(m_F33, deviceInfoData.f29_36status, 0x08);
            //UpdateTMP_InputField(m_F34, deviceInfoData.f29_36status, 0x10);
            //UpdateTMP_InputField(m_F35, deviceInfoData.f29_36status, 0x20);
            //UpdateTMP_InputField(m_F36, deviceInfoData.f29_36status, 0x40);
            //UpdateTMP_InputField(m_F37, deviceInfoData.f29_36status, 0x80);

            m_ExplicitCommand.text = deviceInfoData.explicitCommand;    
        }                                                               

    }

}