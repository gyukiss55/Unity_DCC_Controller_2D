using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Utils_NS;

using DCC_Controller_NS;
using System;

namespace DCC_Controller_NS
{
    public class InputFieldHandler : MonoBehaviour
    {
        public TMP_InputField m_ipAddress;
        public TMP_InputField m_DeviceID;
        public TMP_InputField m_ExpComm;
        public TextMeshProUGUI m_Status;
        public TextMeshProUGUI m_Error;

        public void SetStatus (string st)
        {
            m_Status.text = st;
        }

        public void SetError (string er)
        {
            m_Error.text = er;
        }

        public void OnChangeIPAddress()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            deviceInfo.m_serverAddress = m_ipAddress.text;
            data.ipAddress = m_ipAddress.text;
        }
        public void OnChangeDeviceID()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.deviceID = Int32.Parse(m_DeviceID.text);
        }
        public void OnChangeExplicitCommand()
        {
            DeviceInfo deviceInfo = MainPanelHandler.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            deviceInfo.m_serverAddress = m_ipAddress.text;
            data.explicitCommand = m_ExpComm.text;
        }
    }
}
