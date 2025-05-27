using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.IO;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using MP3Loding;

/// <summary>
/// TCP 通信を行うサーバ側のコンポーネント
/// </summary>
public class TCPServer : MonoBehaviour
{

    [SerializeField]
    Text debugtext;

    //================================================================================
    // 変数
    //================================================================================
    // この IP アドレスとポート番号はクライアント側と統一する
    public string m_ipAddress = "127.0.0.1";
    public int m_port = 2001;
    private bool fileSent;
    private bool FileLengthSerch;
    FileInfo fileinfo;
    private string SavePath;
    [SerializeField]
    private AudioSource audiosource;



    private void Awake()
    {
        Task.Run(() => OnProcess());
    }
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SavePath = Application.persistentDataPath + "/PlayingMusic.mp3";
        }
        else
        {
            SavePath = Application.dataPath + "/PlayingMusic.mp3";
        }
    }
    private void Update()
    {
        if (fileSent)
        {
            fileinfo = new FileInfo(SavePath);
            FileLengthSerch = true;
            fileSent = false;
        }
        if (FileLengthSerch)
        {
        }

    }


    private async void PlayMusic()
    {

        try
        {
            debugtext.text = "Changing mp3 file to AudioClip file...";
            FindObjectOfType<IDInfoManager>().newAudioClip = Mp3Loader.LoadMp3(SavePath);
            FindObjectOfType<IDInfoManager>().newAudioClip_forReverb = Mp3Loader.LoadMp3(SavePath);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5));
            debugtext.text = "All process has completed.";
            FindObjectOfType<PostManager>().VolumeChange();
            FindObjectOfType<ButtonManager>().ChangeScene();
        }
        catch (Exception e)
        {
            debugtext.text = $"Error({e})!!\nPlease reboot this app!!";
            print("error");
        }
    }
    private void PlayMusicforAdjust()
    {

        try
        {
            debugtext.text = "Changing mp3 file to AudioClip file...";
            var script = FindObjectOfType<DelayAdjustmanager>();
            script.StartAdjustment(Mp3Loader.LoadMp3(SavePath));
            debugtext.text = "Starting musics's delay adjustment...";

        }
        catch (Exception e)
        {
            debugtext.text = $"Error({e})!!\nPlease reboot this app!!";
            print("error");
        }
    }
    /// <summary>
    /// クライアント側から通信を監視し続けます
    /// </summary>
    private async Task OnProcess()
    {

        var tcpListener = new TcpListener(IPAddress.Any, m_port);
        tcpListener.Start();
        UnityEngine.Debug.Log("待機中:" + m_port);
        // クライアントからの接続を待機
        var tcpClient = tcpListener.AcceptTcpClient();
        UnityEngine.Debug.Log("接続完了");
        using (NetworkStream stream = tcpClient.GetStream())
        {
            using (var cancellationTokenSource = new System.Threading.CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(60000);//60secでタイムアウト

                try
                {
                    using (FileStream fileStream = new FileStream(SavePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(1));
                        debugtext.text = "Copying  mp3 file from phone...";
                        await stream.CopyToAsync(fileStream, cancellationTokenSource.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    tcpClient?.Dispose();
                    tcpListener?.Stop();
                    debugtext.color = new Color(1, 0, 0);
                    debugtext.text = "Error(Time Out)!! \nPlease Reboot this App!!";
                    UnityEngine.Debug.Log("操作がタイムアウトによりキャンセルされました");
                    throw;
                }
                catch (Exception ex)
                {
                    tcpClient?.Dispose();
                    tcpListener?.Stop();
                    debugtext.color = new Color(1, 0, 0);
                    debugtext.text = $"Error({ex})!!\nPlease Reboot this App!!";
                    UnityEngine.Debug.Log($"エラーが発生しました: {ex.Message}");
                    throw;
                }
                debugtext.text = "Finished Copying mp3 file from phone.";
                tcpClient?.Dispose();
                tcpListener?.Stop();
                if (FindObjectOfType<TCPSender>().isAdjustMode)
                {
                    Debug.Log("AdjustMode");
                    PlayMusicforAdjust();
                }
                else
                {
                    PlayMusic();
                }
                fileSent = true;
            }
        }

    }
    /// <summary>
    /// 破棄する時に呼び出されます
    /// </summary>
    private void OnDestroy()
    {
        // 通信に使用したインスタンスを破棄します
        //m_networkStream?.Dispose();
        //m_tcpClient?.Dispose();
        //m_tcpListener?.Stop();
    }
}

//while (true)
//{
//    var buffer = new byte[256];
//    var count = m_networkStream.Read(buffer, 0, buffer.Length);


//    //using (FileStream fileStream = new FileStream(Application.dataPath + "/testMusic.Png", FileMode.Create, FileAccess.ReadWrite))
//    //{
//    //    debugtext.text += "Creating File...";
//    //    m_networkStream.CopyTo(fileStream); //Android側が送信完了したら自動的に切断され次に進む

//    //    UnityEngine.Debug.Log("切断");
//    //    debugtext.text += "End Connection...";
//    //}

//    // クライアントからの接続が切断された場合は
//    if (count == 0)
//    {
//        UnityEngine.Debug.Log("切断");

//        // 通信に使用したインスタンスを破棄して
//        OnDestroy();

//        // 再度クライアントからの接続を待機します
//        //Task.Run(() => OnProcess());

//        break;
//    }

//    // クライアントから文字列を受信した場合は
//    // GUI とログに出力します
//    var message = Encoding.UTF8.GetString(buffer, 0, count);
//    m_message += message + "\n";
//    //var newint = int.Parse(message);
//    UnityEngine.Debug.LogFormat("受信成功：{0}", message);
//    //if (sendedInt != 0 && sendedInt + 1 == newint)
//    //{
//    //    //UnityEngine.Debug.LogFormat("受信成功：{0}", message);
//    //}
//    //else
//    //{
//    //    UnityEngine.Debug.LogError("??");
//    //}
//    //sendedInt = newint;
//}

// クライアントからの接続が完了したので
// クライアントから文字列が送信されるのを待機します
// 通信に使用したインスタンスを破棄
