using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceIDManager : MonoBehaviour
{
    private int DeviceNum;
    private List<string> PreDatas = new List<string>();

    [SerializeField] GameObject DeviceInfoPanel;
    [SerializeField] private Text Delaytext;
    private string Delaytime = "";
    [SerializeField] Button minusButton, plusButton;
    [SerializeField] Sprite[] sprites = new Sprite[3];
    [SerializeField] int deviceMaxNum = 3;


    private void Start()
    {
        ReadCsv();
        for (int i = 0; i < PreDatas.Count; i++)
        {
            AddDevice(PreDatas[i]);
        }
        if (Delaytime != "")
            Delaytext.text = Delaytime;
    }
    public void PlusButtonPressed()
    {
        DeviceNum++;
        var newInfoPanel = Instantiate(DeviceInfoPanel, this.transform, false);
        newInfoPanel.GetComponent<Text>().text = $"Device{DeviceNum}";
        var PanelText = newInfoPanel.transform.Find("DeviceInputButton/DeviceText").GetComponent<Text>();
        PanelText.text = "";

        if (DeviceNum == 1)
        {
            minusButton.interactable = true;

        }
        if (DeviceNum == deviceMaxNum)
        {
            plusButton.interactable = false;
        }
    }
    public void MinusButtonPressed()
    {
        GameObject child = transform.GetChild(DeviceNum - 1).gameObject;
        Destroy(child);
        DeviceNum--;
        if (DeviceNum == 0)
        {
            minusButton.interactable = false;
        }
        if (DeviceNum == deviceMaxNum - 1)
        {
            plusButton.interactable = true;
        }
    }

    private void AddDevice(string _data)
    {
        DeviceNum++;
        var newInfoPanel = Instantiate(DeviceInfoPanel, this.transform, false);
        newInfoPanel.GetComponent<Text>().text = $"Device{DeviceNum}";
        var PanelText = newInfoPanel.transform.Find("DeviceInputButton/DeviceText").GetComponent<Text>();
        PanelText.text = _data;
        if (DeviceNum == 1)
        {
            minusButton.interactable = true;

        }
        if (DeviceNum == deviceMaxNum)
        {
            plusButton.interactable = false;
        }
    }
    void ReadCsv()
    {
        var filePath = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Path.Combine(Application.persistentDataPath, "PreData.csv");
        }
        else
        {
            filePath = Path.Combine(Application.dataPath, "PreData.csv");
        }

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    switch (i)
                    {
                        case 0:
                            foreach (string value in values)
                            {
                                PreDatas.Add(value);
                            }
                            break;
                        case 1:
                            Delaytime = values[0];
                            break;
                        default:
                            break;
                    }
                    i++;

                }
            }
        }
        else
        {
            Debug.LogError("CSVファイルが存在しません: " + filePath);
        }
    }
}
