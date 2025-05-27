using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonSystemManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Button button;
    [SerializeField] private PostManager postmanager;
    private void Start()
    {
        button = this.GetComponent<Button>();
    }
    public async void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable == false) return;
        if (gameObject.name == "StartButton")
        {
            postmanager.startButtonEffect = true;
        }
        OVRInput.SetControllerVibration(0.1f, 0.5f, OVRInput.Controller.RTouch);
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        OVRInput.SetControllerVibration(0, 0);


    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.name == "StartButton")
        {
            postmanager.startButtonEffect = false;
        }
    }
    public async void OnPointerClick(PointerEventData eventData)
    {
        OVRInput.SetControllerVibration(0.1f, 1f, OVRInput.Controller.RTouch);
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        OVRInput.SetControllerVibration(0, 0);
    }
}
