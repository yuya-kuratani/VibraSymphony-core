using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DelayAdjustmanager : MonoBehaviour
{
    private float SendingDelayTime;
    private int qSamples = 512; //�z��̃T�C�Y
    private float threshold = 0.04f; //�s�b�`�Ƃ��Č��o����ŏ��̕��z
    private float stopWatch;
    private bool startStopWatch;
    private Vector3[] LinePos = new Vector3[512];
    [SerializeField] private int amp = 20;
    [SerializeField] private Vector3 ofset;

    public AudioClip AdjustClip;
    private AudioSource audiosource;
    private float musicOfset;
    private bool StartFFT;
    [SerializeField] private GameObject AdjustmentPanel;
    // Start is called before the first frame update
    void Start()
    {
        audiosource = this.GetComponent<AudioSource>();
        for (int i = 0; i < 512; i++)
        {
            var x = (i / 512f - 1);
            LinePos[i] = new Vector3(x, 0, 0) + ofset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (StartFFT == false) return;
        if (startStopWatch) stopWatch += Time.deltaTime;
        AnalyzeSound();
    }

    public async void StartAdjustment(AudioClip _clip)
    {
        AdjustClip = _clip;
        audiosource.clip = AdjustClip;
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        DateTime utcNow = DateTime.UtcNow;
        var sec = utcNow.AddSeconds(5).Second;
        Debug.Log($"Playing from{utcNow} ");
        FindObjectOfType<TCPSender>().SendForAdjust("Time/" + sec.ToString());
        await UniTask.WaitUntil(() => DateTime.UtcNow.Second == sec);
        audiosource.Play();
        StartFFT = true;
        AdjustmentPanel.SetActive(true);
    }
    public void DelayAdjustButtonPressed()
    {
        FindObjectOfType<IDInfoManager>().StartSolvingIDs(true);

    }
    void AnalyzeSound()
    {
        float[] spectrum = new float[512];
        audiosource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        for (int i = 0; i < qSamples; i++)
        {

            LinePos[i] = new Vector3((i / 512f - 1), spectrum[i] * amp, 0) + ofset;
        }
        GetComponent<LineRenderer>().SetPositions(LinePos);

    }

    public async void ChangeTheMusicOfset(float time)
    {
        SendingDelayTime += time;
        if (time > 0)
        {
            startStopWatch = true;
            audiosource.Pause();
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            startStopWatch = false;
            print(stopWatch);
            audiosource.Play();
        }
        else
        {
            FindObjectOfType<TCPSender>().SendForAdjust("DelayTime/" + (-time).ToString());
        }

    }

    public void EndAdjustButtonPressed()
    {
        FindObjectOfType<TCPSender>().SendForAdjust("EndAdjust/" + (SendingDelayTime).ToString());
    }
}
