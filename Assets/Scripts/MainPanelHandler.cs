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
        public Slider m_speedSlider;
        public TMP_Dropdown m_functionDropDown;
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

        //Create a List of new Dropdown options
        List<string> m_DropOptions = new List<string> 
        {   "F0",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12",
            "F13",
            "F14",
            "F15",
            "F16",
            "F17",
            "F18",
            "F19",
            "F20",
            "F21",
            "F22",
            "F23",
            "F24",
            "F25",
            "F26",
            "F27",
            "F28",
            "F29"
        };

        // Start is called before the first frame update
        void Start()
        {
            Utils.DebugLog("MainPanelHandler start...");


            if (Application.platform == RuntimePlatform.Android)
            {
                GameObject mainPanel = GameObject.Find("MainPanel");
                if (mainPanel != null)
                {
//                    mainPanel.transform.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
            GameObject func = GameObject.Find("Functions");
            Debug.Log($"func:{func}");
            if (func != null)
            {
                Button[] buttons = func.GetComponentsInChildren<Button>();
                int i = 0;
                Debug.Log($"buttons:{buttons}");
                foreach (Button button in buttons)
                {
                    string newText = new string ($"F{i}");
                    i++;
                    if (button.tag == "FunctionButton")
                    {
                        Debug.Log($"Taged name: {button.name} new name:{newText}");
                    }
                    button.GetComponentInChildren<TextMeshProUGUI>().text = newText;
                }
            }
            m_functionDropDown.ClearOptions();
            //Add the options created in the List above
            m_functionDropDown.AddOptions(m_DropOptions);

        }

        // Update is called once per frame
        void Update()
        {
            DeviceInfo info = CrossCaller.GetDeviceInfo();
            if (info.m_changed)
            {
                UpdateInputFields();
                CrossCaller.GetButtonHandler ().UpdateButton(info);
                info.m_changed = false;

                UpdateFunctionsOption(info);
            }
        }

        void UpdateFunctionsOption (DeviceInfo info)
        {
            Debug.Log("UpdateFunctionsOption");
            if (info.m_functionConfig.Count > 0)
            {
                int index = info.m_channel - 1;
                int id = info.m_devices[index].deviceID;
                int funcIndex = -1;
                Debug.Log($"deviceID {id}");
                for (int i = 0; i < info.m_functionConfig.Count; i++)
                {
                    string s = info.m_functionConfig[i];
                    int idFunc = -1;
                    if (s.Length > 1 && s[0] == '#')
                    {
                        idFunc = 0;
                        for(int j = 1; j < s.Length; j++)
                        {
                            if (s[j] >= '0' && s[j] <= '9')
                            {
                                idFunc = idFunc * 10 + (s[j] - '0');
                                Debug.Log($"idFunc {idFunc}");
                            }
                            else
                                break;
                        }
                        Debug.Log($"idFunc {idFunc}");
                        if (idFunc == 0)
                        {
                            funcIndex = i;
                            Debug.Log($"0/id {id}");
                            Debug.Log($"idFunc {idFunc}");
                            Debug.Log($"funcIndex {funcIndex}");

                        }
                        else
                            if (id == idFunc)
                        {
                            funcIndex = i;
                            Debug.Log($"1/id {id}");
                            Debug.Log($"idFunc {idFunc}");
                            Debug.Log($"funcIndex {funcIndex}");
                            break;
                        }
                    }
                }
                Debug.Log($"funcIndex {funcIndex}");
                if (funcIndex > 1)
                {
                    List<string> dropOptions = new List<string>();
                    for (int i = funcIndex + 1; i < info.m_functionConfig.Count; i++)
                    {
                        if (info.m_functionConfig[i].Length > 1 && info.m_functionConfig[i][0] == 'F')
                        {
                            dropOptions.Add(info.m_functionConfig[i]);
                            Debug.Log($"add {info.m_functionConfig[i]}");
                        }
                        else
                            break;
                    }
                    if (dropOptions.Count > 0) {
                        Debug.Log($"dropOptions {dropOptions.Count}");
                        m_functionDropDown.ClearOptions();
                        m_functionDropDown.AddOptions(dropOptions);
                    } else
                    {
                        m_functionDropDown.ClearOptions();
                        m_functionDropDown.AddOptions(m_DropOptions);
                    }
                }
                else
                {
                    m_functionDropDown.ClearOptions();
                    m_functionDropDown.AddOptions(m_DropOptions);
                }
            }
        }

        void UpdateTMP_InputField(Button tmp_button, int value, int mask)
        {
            CrossCaller.GetUtils().UpdateButtonColor(tmp_button, (value & mask) != 0);
        }


        public void UpdateInputFields()
        {
            DeviceInfo deviceInfo = CrossCaller.GetDeviceInfo();
            int ch = deviceInfo.m_channel;
            DeviceInfo.DeviceInfoData deviceInfoData = deviceInfo.m_devices[ch - 1];

            m_ChannelNr.text = string.Format("{0}", ch);
            m_IPAddress.text = deviceInfoData.ipAddress;
            m_DeviceID.text = string.Format("{0}", deviceInfoData.deviceID);
            m_speed.text = string.Format("{0}", deviceInfoData.speed);
            m_speedSlider.value = deviceInfoData.speed / 28f;

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