using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using OscJack;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using RebootApp;
//スマホ(Android,iPhone)の画面左下に表示されてる3~6桁程度のIDを"IPList"のListに打ち込むことでスマホのポートとIPアドレスを取得できるような仕組み.
//尚,ポートは9000とする(スマホ側でポート解放時のエラーが起こったらインクリメントして再度ポート解放を試す仕組みとした)
// [RequireComponent(typeof(AudioSource))]
public class analysis_audio : MonoBehaviour
{
    enum Sensitivity
    {
        strong = 10,
        normal = 26,
        weak = 30
    }
    [SerializeField] private GameObject SensePanel;
    private Image SenseImage;
    [SerializeField]
    private Sprite[] SenseImages;
    private float SenseTimer;
    private float clipLength;
    //_audioSource.clip = Microphone.Start(Microphone.devices[0], false, 500, AudioSettings.outputSampleRate);
    private Sensitivity nowSense = Sensitivity.normal;
    private Sprite NowSenceImage;
    private int vibeNum;

    [SerializeField] private AudioSource reverb_AudioSource;
    [SerializeField] private AudioSource m_AudioSource;
    private const int RESOLUTION = 1024;
    public float thresholdFrequency = 440.0f;
    [SerializeField] private bool GameMode = false;
    private readonly Vector3[] m_Positions = new Vector3[1024];
    [SerializeField, Range(1, 300)] private float m_AmpGain = 300;
    [SerializeField, Range(0, 1)] private float sharp = 0.5f;
    [SerializeField]
    private float VibeTime = 0.2f;
    [SerializeField] Animator DanceAnimator;
    private bool vibing;
    private FFTWindow fftKind = FFTWindow.BlackmanHarris;
    float[] Power_Sharp = new float[2];
    [SerializeField] private float SoundLength = 26;

    private Queue<float> audioBuffer; // 音声データのバッファ
    private int bufferSampleSize;

    private float delayTime = 0.15f; // 音の再生を遅らせる秒数


    [SerializeField] List<string> IPlist = new List<string>(); //int型のListを定義
    private int MainDeviceNum;
    [SerializeField] private int BeforePort;
    private List<int> Ports = new List<int>();

    OscClient client;
    List<OscClient> clientList = new List<OscClient>();
    private IDInfoManager InfoScript;
    private List<SpeakerAnimeManager> speakeranimemanagers = new List<SpeakerAnimeManager>();
    private List<SpeakerVfxManager> speakervfxmanagers = new List<SpeakerVfxManager>();
    private List<CircleDisplayManager> circledisplaymanagers = new List<CircleDisplayManager>();

    GameObject[] speakerVfxObjects;
    GameObject[] speakerObjects;
    GameObject[] circleDisplayObjects;

    void OnEnable()                                         //アクティブになれば呼び出される。
    {
        InfoScript = FindObjectOfType<IDInfoManager>();     //IDInfoManagerの中の関数にアクセスできるようにする。

        IPlist = InfoScript._IPlist;                        //それぞれの変数に関数の値を入れて初期化を行う。
        clientList = InfoScript._clientList;
        InfoScript._clientList = null;
        Ports = InfoScript._Ports;
        MainDeviceNum = InfoScript.MainDeviceNum;
    }

    void OnDisable()                                        //非アクティブになれば呼ばれる。
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i]?.Dispose();                       //もしnullではなかったら、OSCの接続を遮断する。
            Debug.Log("OSC:切断");
        }
        clientList = null;
    }

    private void Awake()
    {

    }

    void Start()
    {
        SenseImage = SensePanel.GetComponent<Image>();
        var InfoScript = FindObjectOfType<IDInfoManager>();     //IDInfoManagerをInfoScriptに代入。
        delayTime = InfoScript.DelayTime;
        print("delaytime: " + delayTime);
        m_AudioSource.clip = InfoScript.newAudioClip;           //オーディオを設定
        clipLength = InfoScript.newAudioClip.length;
        reverb_AudioSource.clip = InfoScript.newAudioClip_forReverb;
        audioBuffer = new Queue<float>();
        bufferSampleSize = Mathf.CeilToInt(delayTime * m_AudioSource.clip.frequency * m_AudioSource.clip.channels);
        StartMusic();
        if (GameMode)
        {
            this.GetComponent<AudioLowPassFilter>().cutoffFrequency = 1000;     //AudioLowPassFilterのcutoffFrequencyを1000にする。
            var filter = this.gameObject.AddComponent<AudioHighPassFilter>();   //AudiohigtPassFilterのcutoffFrequencyを800にする。
            filter.cutoffFrequency = 800;
            fftKind = FFTWindow.Triangle;                                       //FFTを設定
        }
        speakerObjects = GameObject.FindGameObjectsWithTag("Speakers");         //変数にゲームオブジェクトタグが""のものを探して代入する。
        speakerVfxObjects = GameObject.FindGameObjectsWithTag("SpeakersVFX");
        circleDisplayObjects = GameObject.FindGameObjectsWithTag("CircleDisplays");
        foreach (var obj in speakerObjects)                                     //コンポーネントを取得してリストに追加する。
        {
            speakeranimemanagers.Add(obj.GetComponent<SpeakerAnimeManager>());
        }

        foreach (var obj in speakerVfxObjects)
        {
            speakervfxmanagers.Add(obj.GetComponent<SpeakerVfxManager>());
        }

        foreach (var obj in circleDisplayObjects)
        {
            circledisplaymanagers.Add(obj.GetComponent<CircleDisplayManager>());
        }
        foreach (var script in circledisplaymanagers)
        {
            script.StartVibration();
        }
        for (int i = 0; i < RESOLUTION; i++)
        {
            var x = 10 * (i / 512f - 1);
            m_Positions[i] = new Vector3(x, 0, 0);
        }


        GetComponent<LineRenderer>().SetPositions(m_Positions);     //取得したラインレンダラーの位置情報を"m_Positions"にする。
        if (m_AudioSource.clip == null)
        {
            Debug.LogWarning("AudioClip not set in the AudioSource component.");
        }
        Debug.Log($"Sample rate: {AudioSettings.outputSampleRate}");

    }
    async void StartMusic()
    {

        Debug.Log(bufferSampleSize);
        await FindObjectOfType<TCPSender>().TrySend("StartMusic/" + 1); //「StartMusic/1」を送信する。送信が完了するまで待機。
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        m_AudioSource.Play();
        StartReverbMusic();
        var Rootmotions = FindObjectsOfType<RootMotion>();      //Rootmotionsを代入する。
        foreach (var thisScript in Rootmotions)                 //格納されている分だけ、モーションの初期化を行う。
        {
            thisScript.InitializeMotion();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(clipLength + 5));    //音楽の再生が終わるまで待つ。
        FindObjectOfType<PostManager>().StartFadeOut();             //フェードアウトを行う。
        Debug.Log("Music Finished");
        SendTheMusicsEnd(clientList);                               //音楽が終了したことを通知する。
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));             //0.5秒待機する。
        OnDisable();                                                //オブジェクト無効化
        Destroy(FindObjectOfType<TCPSender>().gameObject);          //TCPSencerにあるゲームオブジェクトを削除する。
        Destroy(InfoScript.gameObject);                             //InfoScriptのgameObjectを削除する。
        await UniTask.Delay(TimeSpan.FromSeconds(2));               //2秒待機する。
        if (Application.platform == RuntimePlatform.Android)    //もしもAndroidなら再起動。
        {
            RebootAppSystem.RestartApp();
        }
        else
        {
            SceneManager.LoadScene("StartScene");
        }
    }
    async void StartReverbMusic()
    {

        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        reverb_AudioSource.gameObject.GetComponent<ReverbSourceStartmanager>().StartReverbSource();
    }
    void Update()
    {
        DrawSpectrum();
        if (OVRInput.GetUp(OVRInput.Button.One) || Input.GetKeyUp(KeyCode.Return))
        {
            SenseTimer = 3;
            switch (nowSense)
            {
                case Sensitivity.normal:
                    nowSense = Sensitivity.strong;
                    NowSenceImage = SenseImages[2];
                    break;
                case Sensitivity.strong:
                    nowSense = Sensitivity.weak;
                    NowSenceImage = SenseImages[0];
                    break;
                case Sensitivity.weak:
                    nowSense = Sensitivity.normal;
                    NowSenceImage = SenseImages[1];
                    break;
            }
            SoundLength = (float)nowSense;
            SetSensitivitySprite(NowSenceImage);
            SensePanel.SetActive(true);
        }
        if (SenseTimer >= 0)
        {
            SenseTimer -= Time.deltaTime;
        }
        else if (SensePanel.activeSelf)
        {
            SensePanel.SetActive(false);
        }
    }

    private void SetSensitivitySprite(Sprite _sprite)
    {
        SenseImage.sprite = _sprite;
    }
    [SerializeField]
    private int SamplingRate = 12000; // サンプリング周波数
    private float currentHertz = 0;
    public float CurrentHertz => currentHertz;
    private float preHerts;

    // RMS正規化関数
    private float[] RMSNormalize(float[] data)
    {
        float rms = 0f;
        foreach (var sample in data)
        {
            rms += sample * sample;
        }
        rms = Mathf.Sqrt(rms / data.Length);

        if (rms > 0)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] /= rms; // RMSを基準に正規化
            }
        }
        return data;
    }

    private void DrawSpectrum()
    {
        if (m_AudioSource.clip == null || !m_AudioSource.isPlaying) return;

        // FFT用のデータを取得
        float[] spectrum = new float[RESOLUTION];
        m_AudioSource.GetSpectrumData(spectrum, 0, fftKind);

        // RMS正規化を行う
        // spectrum = RMSNormalize(spectrum);

        var max = 0;
        float max_volume = 0;
        for (int i = 0; i < RESOLUTION; i++)
        {
            m_Positions[i].y = spectrum[i] * m_AmpGain;

            var current = spectrum[i];
            if (current > spectrum[max])
            {
                max = i;
                max_volume = m_Positions[i].y;
            }
        }

        var nyquistFreq = (float)SamplingRate / 2f;
        currentHertz = nyquistFreq * ((float)max / 1024);       //元の周波数を出している。
        if (max_volume >= SoundLength && currentHertz <= 40 && currentHertz >= 0 && preHerts != currentHertz)
        {
            preHerts = currentHertz;
            var sharpness = 0.4f + (currentHertz - 25) / 50;
            sharpness *= 10;
            sharpness = Mathf.Floor(sharpness) / 10;
            Power_Sharp[1] = sharpness + 0.2f;
            Power_Sharp[1] = 0.4f;
            var power = max_volume / 4f;
            power = Mathf.Floor(power) / 10;
            Power_Sharp[0] = power;

            SendVibeInfo(Power_Sharp, clientList, false);
        }
        else
        {
            Power_Sharp[0] = 0;
            Power_Sharp[1] = 0;
        }

        GetComponent<LineRenderer>().SetPositions(m_Positions);
    }

    private async void SendVibeInfo(float[] powerSharp, List<OscClient> _client, bool isEnd)
    {
        if (isEnd)
        {
            for (int i = 0; i < _client.Count; i++)
            {
                _client[i].Send("/End", 1);
            }
            return;
        }
        if (vibing == false)
        {
            for (int i = 0; i < _client.Count; i++)
            {
                _client[i].Send("/Vibe", 1, powerSharp[1]);
            }
            VibeAnimationStart(powerSharp[0]);
            vibeNum++;
            vibing = true;
            Debug.Log($"{vibeNum}!!!  {powerSharp[0]}   :   {powerSharp[1]}");
            await UniTask.Delay(TimeSpan.FromSeconds(VibeTime));
            vibing = false;
        }
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        print("aa");
        // オーディオデータをバッファに格納
        for (int i = 0; i < data.Length; i++)
        {
            audioBuffer.Enqueue(data[i]);
        }

        // バッファ内のサイズが遅延時間分のサンプル数を超えたら再生
        int samplesToPlay = data.Length;

        if (audioBuffer.Count >= bufferSampleSize)
        {
            // 遅延分のサンプルをバッファから取り出して、再生用のデータにコピー
            for (int i = 0; i < samplesToPlay; i++)
            {
                if (audioBuffer.Count > 0)
                {
                    data[i] = audioBuffer.Dequeue();
                }
                else
                {
                    data[i] = 0; // バッファ不足の場合は無音にする
                }
            }
        }
        else
        {
            // バッファが不足している場合は無音にする
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
        }
    }

    private void SendTheMusicsEnd(List<OscClient> _client)
    {
        for (int i = 0; i < _client.Count; i++)
        {
            _client[i].Send("/End", 1);
        }
    }

    private async void VibeAnimationStart(float power)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.15));
        foreach (var script in speakeranimemanagers)
        {
            script.StartAnimation(power);
        }
        foreach (var script in speakervfxmanagers)
        {
            script.StartEffect();
        }
        foreach (var script in circledisplaymanagers)
        {
            script.StartVibration();
        }
    }
}
