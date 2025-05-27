using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TenkeyInputManager : MonoBehaviour
{
    [HideInInspector]public Text ButtonText;
    private EventSystem eventSystem;
    private GameObject buttonObj;
    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();        
    }

    public void TenkeyButtonPressed()
    {
        buttonObj = eventSystem.currentSelectedGameObject;
        if (buttonObj.name == "ButtonBack")
        {
            if (ButtonText.text != "") ButtonText.text = ButtonText.text[..^1];
        }
        else if (buttonObj.name == "ButtonEnd")
        {
            Destroy(GameObject.Find("NonPressingPanel(Clone)"));
            Destroy(this.gameObject);
        }
        else
        {
            string addingStr = buttonObj.name.Replace("Button", "");
            ButtonText.text += addingStr;
        }
    }

}
