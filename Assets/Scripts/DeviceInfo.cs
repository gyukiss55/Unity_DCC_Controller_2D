

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using DCC_Controller_NS;

namespace DCC_Controller_NS
{

    public class DeviceInfo : MonoBehaviour
    {

        [System.Serializable]
        public class DeviceInfoData
        {
            public string ipAddress = "";
            public int deviceID = 0;
            public int speed = 0;
            public bool backward = false;
            public int maxSpeed = 24;
            public int f1_4status = 0;      // 0x80 + 04321 - 1byte command
            public int f5_8status = 0;      // 0xB0 + 3210 - 1byte command
            public int f9_12status = 0;     // 0xA0 + 3210 - 1byte command

            public int f13_20status = 0;     // 0xDE + 76543210 - 2bytes command
            public int f21_28status = 0;     // 0xDF + 76543210 - 2bytes command
            public int f29_36status = 0;     // 0xD8 + 76543210 - 2bytes command
            public string explicitCommand = "";
        }

        public string m_serverAddress;

        public int m_channel = 1;

        public List<DeviceInfoData> m_devices = new List<DeviceInfoData>();

        public int m_mode = 0;


        public bool m_changed = false;

        public List<string> m_functionConfig = new List<string>();



        /// <summary>
        /// "1.01.02" F1 - F30
        /// </summary>

        public string m_version = "1.01.05";

        [System.Serializable]
        public class DCCData
        {
            public string version;
            public List<DeviceInfoData> devices;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_devices = new List<DeviceInfoData>();
            m_devices.Add(new DeviceInfoData());
            m_devices.Add(new DeviceInfoData());
            m_devices.Add(new DeviceInfoData());
            m_devices.Add(new DeviceInfoData());
            int deviceID = 3;
            foreach (DeviceInfoData dev in m_devices)
            {
                dev.deviceID = deviceID++;
                dev.ipAddress = "192.168.2.97";
                dev.maxSpeed = 24;
            }
            m_changed = true;

            LoadData();
            LoadConfigData();
        }


        public void SaveData()
        {
            string destination = Application.persistentDataPath + "/save.dat";
            FileStream file;

            if (File.Exists(destination)) file = File.OpenWrite(destination);
            else file = File.Create(destination);

            DCCData data = new DCCData();
            data.version = m_version;
            data.devices = m_devices;
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();

        }

        public void LoadData()
        {
            string destination = Application.persistentDataPath + "/save.dat";
            FileStream file;

            if (File.Exists(destination)) file = File.OpenRead(destination);
            else
            {
                string log = "File not found";
                Debug.LogError(log);
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            DCCData data = (DCCData)bf.Deserialize(file);
            file.Close();
            if (data.version == m_version)
            {
                m_devices = data.devices;
                m_changed = true;
                Debug.Log($"Load data: data.version{data.version}");
                Debug.Log($" max spped: dev 0 {m_devices[0].maxSpeed}");
            }
            else
            {
                string log = "Old version " + data.version + " - " + m_version;
                Debug.LogError(log);
                CrossCaller.GetInputFieldHandler().SetError(log);
                return;

            }
        }

        public void LoadConfigData()
        {
            string filePath = Application.persistentDataPath + "/ConfigFunctions.txt";

            if (File.Exists(filePath))
            {
                StreamReader reader = new StreamReader(filePath);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    m_functionConfig.Add(new string(line));
                }
                int index = 1;
                foreach(string s in m_functionConfig)
                {
                    Debug.Log($"{index++}.{s}");
                }
                reader.Close();
            }
            else
            {
                string log = "File not found";
                Debug.LogError(log);
                CrossCaller.GetInputFieldHandler().SetError(log);
                return;
            }

        }
    }

}
