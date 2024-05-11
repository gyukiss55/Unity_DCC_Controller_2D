using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DCC_Controller_NS;
using Utils_NS;

namespace DCC_Controller_NS
{
    public class CrossCaller : MonoBehaviour
    {
        static CrossCaller instance;

        Utils utils = null;
        DeviceInfo deviceInfo = null;
        InputFieldHandler inputFieldHandler = null;
        ButtonHandler buttonHandler = null;


        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        static public Utils GetUtils()
        {
            if (instance.utils == null)
            {
                GameObject mainControl = GameObject.Find("MainControl");
                if (mainControl == null)
                {
                    Debug.Log("MainControl not found!");
                    return instance.utils;
                }
                instance.utils = mainControl.GetComponent<Utils>();
                if (instance.utils == null)
                {
                    Debug.Log("DeviceInfo not found!");
                }
 
            }
            return instance.utils;

        }
        static public DeviceInfo GetDeviceInfo()
        {
            if (instance.deviceInfo == null)
            {
                GameObject mainControl = GameObject.Find("MainControl");
                if (mainControl == null)
                {
                    Debug.Log("MainControl not found!");
                    return instance.deviceInfo;
                }
                instance.deviceInfo = mainControl.GetComponent<DeviceInfo>();
                if (instance.deviceInfo == null)
                {
                    Debug.Log("DeviceInfo not found!");
                }
            }
            return instance.deviceInfo;

        }
        static public InputFieldHandler GetInputFieldHandler()
        {
            if (instance.inputFieldHandler == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.Log("Canvas not found!");
                    return instance.inputFieldHandler;
                }
                instance.inputFieldHandler = canvas.GetComponent<InputFieldHandler>();
                if (instance.inputFieldHandler == null)
                {
                    Debug.Log("InputFieldHandler not found!");
                }

            }
            return instance.inputFieldHandler;

        }
        static public ButtonHandler GetButtonHandler()
        {
            if (instance.buttonHandler == null)
            {
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas == null)
                {
                    Debug.Log("Canvas not found!");
                    return instance.buttonHandler;
                }
                instance.buttonHandler = canvas.GetComponent<ButtonHandler>();
                if (instance.buttonHandler == null)
                {
                    Debug.Log("ButtonHandler not found!");
                }

            }
            return instance.buttonHandler;

        }

    }
}
