using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PCInputManager : MonoBehaviour
{
    [SerializeField] private GameObject TenkeyButton;
    [SerializeField] private GameObject NonPressingObj;
    private GameObject canvas;
    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    public void InputButtonPressed()
    {
        var newPanel = Instantiate(NonPressingObj, canvas.transform, false);
        var newTenkey = Instantiate(TenkeyButton, canvas.transform, false);
        newTenkey.GetComponent<TenkeyInputManager>().ButtonText = this.transform.GetChild(0).gameObject.GetComponent<Text>();
    }

}
