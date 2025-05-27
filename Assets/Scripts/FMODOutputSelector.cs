using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FMODUnity;
using System.Diagnostics;

//namespace DoubleShot.Editor
//{
//    [InitializeOnLoad]
//    public class FMODOutputSelector : EditorWindow
//    {
//        #region Variable Definitions
//        private const string settingName = "DoubleShot.FMODAutoSetOutput.Enabled";

//        private const string targetAudioDeviceID = "DoubleShot.FMODOutput";
//        private const string targetAudioDeviceName = "DoubleShot.FMODOutput.Name";
//        private const string numOfAvailableDevices = "DoubleShot.FMODOutput.NumberOfAvailableDevices";
//        private const string availableDevicesConfig = "DoubleShot.FMODAudioDevices";

//        private static bool isAutoFMODOutput
//        {
//            get { return EditorPrefs.GetBool(settingName, false); }
//            set { EditorPrefs.SetBool(settingName, value); }
//        }

//        EditorApplication.CallbackFunction callback;
//        #endregion

//        #region JSON Class
//        [Serializable]
//        public class DeviceConfig
//        {
//            public List<int> ids;
//            public List<string> names;

//            public void Init()
//            {
//                ids = new List<int>();
//                names = new List<string>();
//            }
//        }
//        #endregion

//        #region Helper Function
//        private static bool isCurrentUsedDevice(int deviceID)
//        {
//            if (EditorPrefs.GetInt(targetAudioDeviceID) == deviceID)
//                return true;
//            else return false;
//        }
//        private static void SetAudioDeviceToDefault()
//        {
//            EditorPrefs.SetInt(targetAudioDeviceID, 99);
//            EditorPrefs.SetString(targetAudioDeviceName, "Default System Device");
//        }
//        private static void GetNumAudioDrivers()
//        {
//            int i;
//            RuntimeManager.CoreSystem.getNumDrivers(out i);
//            EditorPrefs.SetInt(numOfAvailableDevices, i);
//        }
//        private static void ListAudioDevices()
//        {
//            DeviceConfig config = new DeviceConfig();
//            config.Init();
//            for (int i = 0; i < EditorPrefs.GetInt(numOfAvailableDevices); i++)
//            {
//                string name;
//                Guid guid;
//                int systemrate;
//                FMOD.SPEAKERMODE AudioSpeakerMode;
//                int speakermodechannel;
//                RuntimeManager.CoreSystem.getDriverInfo(i, out name, 100, out guid, out systemrate, out AudioSpeakerMode, out speakermodechannel);

//                config.ids.Add(i);
//                config.names.Add(name);
//                EditorPrefs.SetString(availableDevicesConfig, JsonUtility.ToJson(config));
//            }
//        }
//        private static void OnAvailableAudioDevicesChanged()
//        {
//            if (EditorPrefs.GetString(targetAudioDeviceName) == "Default System Device")
//                return;
//            else
//            {
//                DeviceConfig config = JsonUtility.FromJson<DeviceConfig>(EditorPrefs.GetString(availableDevicesConfig));
//                if (config.names.Contains(EditorPrefs.GetString(targetAudioDeviceName)))
//                {
//                    int id = config.names.IndexOf(EditorPrefs.GetString(targetAudioDeviceName));
//                    EditorPrefs.SetInt(targetAudioDeviceID, id);
//                }
//                else
//                {
//                    // Debug.LogWarning("Previous audio device not found, reverting back to default!");
//                    SetAudioDeviceToDefault();
//                }
//            }

//        }

//        #endregion

//        #region Method run on entering Play Mode
//        [RuntimeInitializeOnLoadMethod]
//        private static void SetOutputToDriver()
//        {
//            if (isAutoFMODOutput)
//            {
//                if (EditorPrefs.GetInt(targetAudioDeviceID) == 99)
//                    RuntimeManager.CoreSystem.setDriver(0);
//                else
//                    RuntimeManager.CoreSystem.setDriver(EditorPrefs.GetInt(targetAudioDeviceID));
//            }
//        }

//        #endregion

//        #region Window Menu Item and initializer
//        [MenuItem("Double Shot/FMOD/Output Selector", false, 0)]
//        private static void Init()
//        {
//            if (!EditorPrefs.HasKey(targetAudioDeviceID))
//            {
//                SetAudioDeviceToDefault();
//            }

//            GetNumAudioDrivers();
//            ListAudioDevices();
//            OnAvailableAudioDevicesChanged();

//            FMODOutputSelector window = (FMODOutputSelector)GetWindow(typeof(FMODOutputSelector), false, "FMOD Output Selector");
//            window.Show();

//        }
//        #endregion

//        #region OnGUI
//        private void OnGUI()
//        {
//            GUI.skin.button.wordWrap = true;

//            GUILayout.Label("Automatic Output Selection", EditorStyles.boldLabel);
//            isAutoFMODOutput = GUILayout.Toggle(isAutoFMODOutput, "Enable");


//            GUILayout.Space(10);

//            GUILayout.Label("Audio Devices", EditorStyles.boldLabel);
//            PrintAudioDevices();

//            GUILayout.Space(10);


//            if (GUILayout.Button("Refresh Audio Devices List", GUILayout.Height(50)))
//            {
//                GetNumAudioDrivers();
//                ListAudioDevices();
//                OnAvailableAudioDevicesChanged();
//            }

//        }
//        private static void PrintAudioDevices()
//        {
//            // Print Default Device
//            if (GUILayout.Toggle(isCurrentUsedDevice(99), "Default System Device"))
//            {
//                SetAudioDeviceToDefault();
//                if (EditorApplication.isPlaying)
//                {
//                    RuntimeManager.CoreSystem.setDriver(0);
//                }
//            }

//            // Print specific audio devices
//            DeviceConfig config = JsonUtility.FromJson<DeviceConfig>(EditorPrefs.GetString(availableDevicesConfig));
//            for (int i = 0; i < config.ids.Count; i++)
//            {
//                if (GUILayout.Toggle(isCurrentUsedDevice(i), config.names[i]) && EditorPrefs.GetInt(targetAudioDeviceID) != i)
//                {
//                    EditorPrefs.SetInt(targetAudioDeviceID, i);
//                    EditorPrefs.SetString(targetAudioDeviceName, config.names[i]);
//                    if (EditorApplication.isPlaying)
//                    {
//                        if (EditorPrefs.GetInt(targetAudioDeviceID) == 99)
//                        {
//                            RuntimeManager.CoreSystem.setDriver(0);
//                        }
//                        else
//                        {
//                            RuntimeManager.CoreSystem.setDriver(EditorPrefs.GetInt(targetAudioDeviceID));
//                        }


//                    }


//                }
//            }
//        }
//        #endregion
//    }
//}