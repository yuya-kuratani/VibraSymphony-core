using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using Oculus.Platform.Models;
using OscJack;
using System.Net;
using UnityEngine.UI;
using Unity.VisualScripting;
public class ButtonManager : MonoBehaviour
{

    [SerializeField] private GameObject IDInfoSender;
    [SerializeField] private GameObject LodingParticle;
    [SerializeField] private bool isDebugMode;
    public async void OnStartButtonClicked()
    {
        if (isDebugMode)
        {
            SceneManager.LoadScene("LiveVR");
        }
        else
        {
            Instantiate(LodingParticle, GameObject.Find("Canvas").transform, false);
            var StartButton = GameObject.Find("StartButton");
            StartButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            StartButton.GetComponent<Button>().interactable = false;

            FindObjectOfType<IDInfoManager>().StartSolvingIDs(false);
        }
    }
    public async void ChangeScene()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.4));
        SceneManager.LoadScene("LiveVR");

    }
}
