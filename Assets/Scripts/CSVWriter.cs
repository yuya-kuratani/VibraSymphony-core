using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        // CSVファイルのパスを設定 (アプリの保存可能なディレクトリを使用)
    }

    public async void WriteCsv(List<string> Devicedata, string DelayData)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Path.Combine(Application.persistentDataPath, "PreData.csv");
        }
        else
        {
            filePath = Path.Combine(Application.dataPath, "PreData.csv");
        }
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            await writer.WriteLineAsync(string.Join(",", Devicedata));
            await writer.WriteLineAsync(DelayData);
        }

    }
}
