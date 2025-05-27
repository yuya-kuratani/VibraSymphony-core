using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;//Volumeを使うのに必要
using UnityEngine.Rendering.Universal;//Bloomを使うのに必要
using static OVRPlugin;
using UnityEngine.UI;

public class PostManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    float fadeValue,effectValue;
    Volume volume;
    private ColorAdjustments ColAdj;
    private ChromaticAberration ChAb;
    private bool startFade;
    public bool startButtonEffect;
    private bool startButtonPressed;
    private EventSystem eventSystem;
    private void Start()
    {
        volume = this.GetComponent<Volume>();
        volume.profile.TryGet<ColorAdjustments>(out ColAdj);
        volume.profile.TryGet<ChromaticAberration>(out ChAb);
        UniTask.Delay(TimeSpan.FromSeconds(0.5));
        eventSystem = FindObjectOfType<EventSystem>();

    }
    public void StartButtonPressed_Volume()
    {
        volume.profile.TryGet<ColorAdjustments>(out var newProcess);
        //StartFadeOut();
        //startButtonPressed = true;
    }
    public void VolumeChange()
    {
        volume.profile.TryGet<ColorAdjustments>(out var newProcess);
        StartFadeOut();
        startButtonPressed = true;

    }
    public void StartFadeOut()
    {
        startFade = true;
    }
    private void Update()
    {
        if (startFade)
        {
            fadeValue -= Time.deltaTime / 0.3f;
            ColAdj.postExposure.value = fadeValue;
        }

        if (ChAb == null) return;

        if (startButtonEffect)
        {
            effectValue += Time.deltaTime / 0.2f;
        }
        else if(!startButtonPressed)
        {
            effectValue -= Time.deltaTime / 0.2f;
        }
        
        effectValue = Mathf.Clamp(effectValue, 0, 1);
        ChAb.intensity.value = effectValue;


    }

}
