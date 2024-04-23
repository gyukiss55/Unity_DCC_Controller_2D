

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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
            public int f1_5status = 0;
            public int f6_10status = 0;
            public string explicitCommand = "";
        }

        public string m_serverAddress;

        public int m_channel = 1;

        public List<DeviceInfoData> m_devices = new List<DeviceInfoData>();

        public bool m_changed = false;

        public string m_version = "1.01.01";

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
                dev.ipAddress = "192.168.50.97";
            }
            m_changed = true;

            LoadData();
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
                Debug.LogError("File not found");
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            DCCData data = (DCCData)bf.Deserialize(file);
            file.Close();
            if (data.version == m_version)
            {
               m_devices = data.devices;
               m_changed = true;
            }
            else
            {
                string log = "Old version " + data.version + " - " + m_version;
                Debug.LogError(log);
                return;

            }
        }

    }

}
