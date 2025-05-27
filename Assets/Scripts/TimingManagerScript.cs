using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Cysharp.Threading.Tasks;
using System;

public class TimingManagerScript : MonoBehaviour
{
    [SerializeField] GameObject BaseObj;
    [SerializeField] GameObject MusicObj;

    private async void Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        BaseObj.SetActive(true);
        MusicObj.SetActive(true);
    }
}
