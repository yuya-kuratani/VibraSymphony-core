using System;
using System.Linq;
using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using OscJack;
using UnityEngine;
using UnityEngine.UI;

public class IDInfoManager : MonoBehaviour
{
    private int _BeforePort = 9000;
    public List<int> _Ports = new List<int>();
    public List<OscClient> _clientList = new List<OscClient>();
    public List<string> _IPlist = new List<string>(); //int型のListを定義
    public string PCIP;
    public int PCPort;
    public Sound musicSound;
    public int MainDeviceNum;
    private bool mainDeviceHasSelected;
    public AudioClip newAudioClip;
    public AudioClip newAudioClip_forReverb;

    public float DelayTime;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public async void StartSolvingIDs(bool isDelayAdjust)
    {
        var InputIDs = GameObject.FindGameObjectsWithTag("DeviceIDs");
        foreach (var TextOBj in InputIDs)
        {
            _IPlist.Add(TextOBj.GetComponent<Text>().text);

        }
        //csv入力  
        DelayTime = SetDelayTime();
        FindObjectOfType<CSVWriter>().WriteCsv(_IPlist, (DelayTime * 1000).ToString());
        //------

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        GetDiviceIPs();
        FindObjectOfType<TCPSender>().ConnectToPhone(_IPlist[MainDeviceNum], 8000, isDelayAdjust);

    }
    public string IP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                print(ip.ToString());
                return ip.ToString();
            }
        }
        return "127.0.0.1";
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void GetDiviceIPs()
    {
        string[] SplitThisIP = IP().Split('.');
        int port = _BeforePort;
        for (int i = 0; i < _IPlist.Count; i++)
        {
            //mainデバイスかどうか検知
            if (_IPlist[i][0] == 'B')
            {
                if (mainDeviceHasSelected == false) MainDeviceNum = i;
                mainDeviceHasSelected = true;
                _IPlist[i] = _IPlist[i].Remove(0, 1);
            }
            bool isExceptional = false;

            string[] SplitPortAndIP = _IPlist[i].Split('A');
            switch (SplitPortAndIP.Length)
            {
                case 1:
                    break;
                case 2:
                    port += int.Parse(SplitPortAndIP[0]);
                    _IPlist[i] = SplitPortAndIP[1];
                    break;

            }

            string[] IDSplit = _IPlist[i].Split('B');
            switch (IDSplit.Length)
            {
                case 1:
                    _IPlist[i] = $"{SplitThisIP[0]}.{SplitThisIP[1]}.{SplitThisIP[2]}.{IDSplit[0]}";
                    break;
                case 2:
                    _IPlist[i] = $"{SplitThisIP[0]}.{SplitThisIP[1]}.{SplitThisIP[2]}.{IDSplit[1]}";
                    break;
                case 3:
                    _IPlist[i] = $"{SplitThisIP[0]}.{SplitThisIP[1]}.{IDSplit[1]}.{IDSplit[2]}";
                    break;
                case 4:
                    _IPlist[i] = $"{SplitThisIP[0]}.{IDSplit[1]}.{IDSplit[2]}.{IDSplit[3]}";
                    break;
                case 5:
                    _IPlist[i] = $"{IDSplit[1]}.{IDSplit[2]}.{IDSplit[3]}.{IDSplit[4]}";
                    break;
                default:
                    print("Exceptional ID");
                    _IPlist.RemoveAt(i);
                    isExceptional = true;
                    break;
            }
            if (!isExceptional)
            {
                _Ports.Add(port);
                print(_IPlist[i] + _Ports[i]);
                _clientList.Add(new OscClient(_IPlist[i], _Ports[i]));

            }
        }

    }

    private float SetDelayTime()
    {
        string timeText = GameObject.FindWithTag("DelayTime").GetComponent<Text>().text;
        string digitsOnly = new string(timeText.Where(char.IsDigit).ToArray());
        if (float.TryParse(digitsOnly, out float result))
        {
            print(result / 1000);

            return result / 1000;
        }
        else
        {
            return 0.15f;
        }
    }
}

