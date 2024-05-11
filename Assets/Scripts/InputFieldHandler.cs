
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor.PackageManager;

using TMPro;


using Utils_NS;

using DCC_Controller_NS;


namespace DCC_Controller_NS
{
    public class InputFieldHandler : MonoBehaviour
    {
        public TMP_InputField m_ipAddress;
        public TMP_InputField m_DeviceID;
        public TMP_InputField m_ExpComm;
        public TextMeshProUGUI m_Status1;
        public TextMeshProUGUI m_Status2;
        public TextMeshProUGUI m_Status3;
        public TextMeshProUGUI m_Error1;
        public TextMeshProUGUI m_Error2;
        
        private void Start()
        {
            m_Status1.text = "";
            m_Status2.text = "";
            m_Status3.text = "";
            m_Error1.text = "";
            m_Error2.text = "";
        }

        void AddStringToLog(string text, bool errorSt)
        {
            if (text.Length > 0)
            {
                if (errorSt)
                {
                    m_Error1.text = m_Error2.text;
                    m_Error2.text = text;

                } else
                {
                    m_Status1.text = m_Status2.text;
                    m_Status2.text = m_Status3.text;
                    m_Status3.text = text;

                }
            }
        }

        public void SplitTextAndSend (string text, bool errorSt)
        {
            int splitLength = 50;
            string[] splitedText = text.Split(char.Parse(" "));
            string partOfString = "";
            foreach (string s in splitedText)
            {
                if (partOfString.Length + s.Length < splitLength)
                    partOfString += " " + s;
                else if (partOfString.Length == 0)
                {
                    AddStringToLog(s, errorSt);
                }
                else
                {
                    AddStringToLog(partOfString, errorSt);
                    partOfString = " " + s;
                }
            }
            AddStringToLog(partOfString, errorSt);
        }

        public void SetStatus (string st)
        {
            Debug.Log(st);
            SplitTextAndSend(st, false);
        }

        public void SetError (string er)
        {
            Debug.Log(er);
            SplitTextAndSend(er, true);
        }

        public void OnChangeIPAddress()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            deviceInfo.m_serverAddress = m_ipAddress.text;
            data.ipAddress = m_ipAddress.text;
        }
        public void OnChangeDeviceID()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            data.deviceID = Int32.Parse(m_DeviceID.text);
        }
        public void OnChangeExplicitCommand()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            DeviceInfo.DeviceInfoData data = deviceInfo.m_devices[deviceInfo.m_channel - 1];
            deviceInfo.m_serverAddress = m_ipAddress.text;
            data.explicitCommand = m_ExpComm.text;
        }
    }
}
