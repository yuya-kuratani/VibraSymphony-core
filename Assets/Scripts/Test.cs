using Cysharp.Threading.Tasks;
using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Net.Mail;
using System.Text;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject particle;
    [SerializeField]
    Text _text;
    // Start is called before the first frame update
    public string[] songs;
    public string currentSong;
    public WebClient webClient;
    public float Size = 0.0f;
    private int xRand;
    WWW newRequest;
    byte[] fileData;
    FMOD.System sys;

    private TcpListener mListener;
    private TcpClient mClient;

    IEnumerator loadAndPlayAudioClip(string _path)
    {

        newRequest = new WWW(_path);

        yield return newRequest;
    }

    async void Start()
    {
        var audioname = OVRManager.audioOutId;
        FMODUnity.RuntimeManager.CoreSystem.getDriver(out int a);
        FMODUnity.RuntimeManager.StudioSystem.getCoreSystem(out sys);
        //sys.setOutput);
        //var setOutPut = sys.setOutput(OUTPUTTYPE.AUDIO3D);
        int driverCount = 0;
        FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out driverCount);
        string riftId = OVRManager.audioOutId;
        var result = FMODUnity.RuntimeManager.CoreSystem.setDriver(0);
        _text.text += $"result:{result} ? {audioname}:: ";
        for (int i = 0; i < driverCount; i++)
        {

            sys.getDriverInfo(i,
              out string name,
              16,
              out System.Guid guid,
              out int systemrate,
              out SPEAKERMODE speakermode,
              out int speakermodechannels);
            _text.text += $"{i}:{guid} ";
        }
        var path = Path.Combine(Application.streamingAssetsPath, "TokyoFlash3D.mp3");
        //bool newBool = System.IO.File.Exists(path);
        //bool isURI = Uri.IsWellFormedUriString(path, UriKind.Absolute);
        //_text.text += path + $"\nURI:{isURI} Exists:{newBool}";
        //var newParticle = Instantiate(particle, this.transform, false);
        //UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path);
        //await request.SendWebRequest();

        //var audiofile_path = System.IO.Path.Combine(Application.persistentDataPath, "TokyoFlash3D.mp3");
        //System.IO.File.WriteAllBytes(audiofile_path, request.downloadHandler.data);
        //await loadAndPlayAudioClip(path);
        //Destroy(newParticle);
        //_text.text += $"\n{newRequest}\n{audiofile_path}";
        //var a =  await ReadStreamingAssetsWithWebRequestAsync(Application.streamingAssetsPath + "/TokyoFlash3D.mp3");
        //var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(audiofile_path, FMOD.MODE.DEFAULT, out var sound);
        //_text.text += $"\ncreateSound{soundResult}";
        var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(path, FMOD.MODE.DEFAULT, out var sound);
        FindAnyObjectByType<IDInfoManager>().musicSound = sound;
        print(result);

        var ip = IPAddress.Parse("127.0.0.1");
        mListener = new TcpListener(ip, 2000);
        mListener.Start();
        //コールバック設定　第二引数はコールバック関数に渡される
        mListener.BeginAcceptSocket(DoAcceptTcpClientCallback, mListener);
    }

    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        //渡されたものを取り出す
        var listener = (TcpListener)ar.AsyncState;
        mClient = listener.EndAcceptTcpClient(ar);
        print("Connect: " + mClient.Client.RemoteEndPoint);

        // 接続した人とのネットワークストリームを取得
        var stream = mClient.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);

        // 接続が切れるまで送受信を繰り返す
        while (mClient.Connected)
        {
            while (!reader.EndOfStream)
            {
                // 一行分の文字列を受け取る
                print(reader.ReadLine());
            }

            // クライアントの接続が切れたら
            if (mClient.Client.Poll(1000, SelectMode.SelectRead) && (mClient.Client.Available == 0))
            {
                print("Disconnect: " + mClient.Client.RemoteEndPoint);
                mClient.Close();
                break;
            }
        }
    }
    async UniTask<string> ReadStreamingAssetsWithWebRequestAsync(string path)
    {
        var request = UnityEngine.Networking.UnityWebRequest.Get(path);
        // UniTaskの機能で非同期に読み込む
        await request.SendWebRequest();

        return request.downloadHandler.text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}