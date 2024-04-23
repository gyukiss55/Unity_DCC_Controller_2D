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
        public TMP_InputField m_F01;
        public TMP_InputField m_F02;
        public TMP_InputField m_F03;
        public TMP_InputField m_F04;
        public TMP_InputField m_F05;
        public TMP_InputField m_F06;
        public TMP_InputField m_F07;
        public TMP_InputField m_F08;
        public TMP_InputField m_F09;
        public TMP_InputField m_F10;
        public TMP_InputField m_ExplicitCommand;

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

        void UpdateTMP_InputField(TMP_InputField tmp_if, int value, int mask)
        {
            if ((value & mask) == 0)
            {
                tmp_if.text = "0";
            } else
            {
                tmp_if.text = "1";
            }
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

            UpdateTMP_InputField(m_F01, deviceInfoData.f1_5status, 0x01);
            UpdateTMP_InputField(m_F02, deviceInfoData.f1_5status, 0x02);
            UpdateTMP_InputField(m_F03, deviceInfoData.f1_5status, 0x04);
            UpdateTMP_InputField(m_F04, deviceInfoData.f1_5status, 0x08);
            UpdateTMP_InputField(m_F05, deviceInfoData.f1_5status, 0x10);
            UpdateTMP_InputField(m_F06, deviceInfoData.f6_10status, 0x01);
            UpdateTMP_InputField(m_F07, deviceInfoData.f6_10status, 0x02);
            UpdateTMP_InputField(m_F08, deviceInfoData.f6_10status, 0x04);
            UpdateTMP_InputField(m_F09, deviceInfoData.f6_10status, 0x08);
            UpdateTMP_InputField(m_F10, deviceInfoData.f6_10status, 0x10);

            m_ExplicitCommand.text = deviceInfoData.explicitCommand;
        }

    }

}