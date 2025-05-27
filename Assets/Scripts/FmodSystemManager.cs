using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Cysharp.Threading.Tasks;
using System;

public class FmodSystemManager : MonoBehaviour
{
    FMOD.Sound sound1;
    FMOD.System system;
    FMOD.Channel channel;
    FMOD.ChannelGroup channelGroup;
    FMOD.ChannelGroup cgSys1, cgSys2;
    FMOD.Channel chaSys1, chaSys2;
    FMOD.RESULT playResult;
    // Start is called before the first frame update
    private async void OnEnable()
    {
        FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out int num);
        print(num);
        for (int i = 0; i < num; i++)
        {
            FMODUnity.RuntimeManager.CoreSystem.getDriverInfo(i,
              out string name,
              16,
              out System.Guid guid,
              out int systemrate,
              out SPEAKERMODE speakermode,
              out int speakermodechannels);
            print(name);
        }
        var resGetDrivers_core = FMODUnity.RuntimeManager.CoreSystem.getNumDrivers(out int totalDrivers);
        var sound = FindObjectOfType<IDInfoManager>().musicSound;
        await UniTask.Delay(TimeSpan.FromSeconds(2.13));
        //playResult = FMODUnity.RuntimeManager.CoreSystem.playSound(sound, cgSys2, false, out chaSys2);
        FMODUnity.RuntimeManager.CoreSystem.setDriver(1);
        print(playResult);

    }
    private async void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
