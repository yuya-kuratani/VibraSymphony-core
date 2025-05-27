using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static IDInfoManager;
using UnityEngine.UI;
public class TCPSender : MonoBehaviour
{
    [SerializeField]
    Text debugtext;
    private byte[] newbits;
    private bool converted;
    public string m_ipAddress = "127.0.0.1";
    public int m_port = 2001;
    private bool start = false;
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    public bool isAdjustMode;
    private bool m_isConnection;
    int errorNum;
    public string path;


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void FixedUpdate()
    {
        if (!start) return;
        if (!converted) return;
    }
    public void SetBytes(byte[] _bytes)
    {
        newbits = _bytes;
        converted = true;
    }

    public async void ConnectToPhone(string ip, int port, bool isDelayAdjust)
    {
        isAdjustMode = isDelayAdjust;
        var thisIP = IP();
        try
        {
            // 指定された IP アドレスとポートでサーバに接続します
            print(ip);
            m_tcpClient = new TcpClient(ip, port);
            m_networkStream = m_tcpClient.GetStream();
            m_isConnection = true;
            UnityEngine.Debug.LogFormat("接続成功");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            var sendingPort = FindObjectOfType<TCPServer>().m_port;

            if (!isDelayAdjust)
            {
                await TrySend("Normal/" + thisIP + "B" + sendingPort.ToString());

            }
            else
            {
                await TrySend("Adjust/" + thisIP + "B" + sendingPort.ToString());
            }
        }
        catch (SocketException e)
        {
            if (errorNum == 4)
            {
                debugtext.color = new Color(1, 0, 0);
                debugtext.text = $"Error({e})!!\nPlease reboot this app!!";
                throw;
            }
            debugtext.text = "Error! retry sending info...";
            errorNum++;
            // サーバが起動しておらず接続に失敗した場合はここに来ます
            UnityEngine.Debug.LogError("接続失敗");
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            ConnectToPhone(ip, port, isDelayAdjust);
        }

    }
    private void OnDestroy()
    {
        m_tcpClient?.Dispose();
        m_networkStream?.Dispose();
        UnityEngine.Debug.Log("TCPSender:切断");

    }
    public async UniTask TrySend(string info)
    {
        try
        {
            if (debugtext) debugtext.text = "Sending this device's info...";
            var buffer = Encoding.UTF8.GetBytes(info);
            await m_networkStream.WriteAsync(buffer, 0, buffer.Length);
            UnityEngine.Debug.LogFormat("送信成功：{0}", info);
            if (isAdjustMode)
            {

            }
            else
            {
                //m_tcpClient?.Dispose();
                //m_networkStream?.Dispose();
                //UnityEngine.Debug.Log("TCPSender:切断");
            }
            if (debugtext) debugtext.text = "Waiting for phone's connection...";
            await UniTask.Delay(TimeSpan.FromSeconds(0.1));
        }
        catch (Exception e)
        {
            if (debugtext)
            {
                debugtext.color = new Color(1, 0, 0);
                debugtext.text = $"Error({e})!!\nPlease reboot this app!!";
            }
            UnityEngine.Debug.LogError("送信失敗");
        }
    }

    public void SendForAdjust(string info)
    {
        try
        {
            debugtext.text = "Sending this device's info...";
            var buffer = Encoding.UTF8.GetBytes(info);
            m_networkStream.Write(buffer, 0, buffer.Length);
            m_networkStream.Flush();
            UnityEngine.Debug.LogFormat("送信成功：{0}", info);
            //m_tcpClient?.Dispose();
            //m_networkStream?.Dispose();
            debugtext.text = "Sent info to phone.";
        }
        catch (Exception e)
        {
            debugtext.color = new Color(1, 0, 0);
            debugtext.text = $"Error({e})!!\nPlease reboot this app!!";
            UnityEngine.Debug.LogError($"送信失敗:{e}");
        }
    }

    private string IP()
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

}
