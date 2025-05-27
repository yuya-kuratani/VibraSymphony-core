using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(RectTransform))]
public class FindFolderManager : MonoBehaviour
{
    [SerializeField] private GameObject MusicBox;
    [SerializeField] private GameObject MusicsListPanel;
    [SerializeField] private GameObject PermissionErrors;
    [SerializeField] private Sprite AdmittedImage;

    [SerializeField] private GameObject NonMusicExsitingObj;
    private bool hasPermissionButtonPressed;
    private bool permissionAdmitted;
    private List<string> FilesPath = new List<string>();
    private List<string> FilesName = new List<string>();

    // Start is called before the first frame update
    async void Start()
    {
        permissionAdmitted = SetPermission();
        if (!permissionAdmitted)
        {
            PermissionErrors.SetActive(true);
        }
        await UniTask.WaitUntil(() => permissionAdmitted);
        FindFolders();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // SetPermission メソッドを Android プラットフォームでのみ実行
    private bool SetPermission()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            return false;
        }
        return true;
#else
        return true;  // Android 以外では許可済みとして扱う
#endif
    }

    public static void Request_SettingsIntent()
    {
#if PLATFORM_ANDROID
        using (var activity = GetActivity())
        {
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.MANAGE_ALL_FILES_ACCESS_PERMISSION"))
            {
                activity.Call("startActivity", intentObject);
            }
        }
#endif
    }

    // OculusのMusicフォルダからMP3ファイルを取得
    private async void FindFolders()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        string musicPath = GetAndroidMusicPath(); // Musicフォルダのパスを取得
        if (string.IsNullOrEmpty(musicPath)) return;

        DirectoryInfo dataDir = new DirectoryInfo(musicPath);
        var files = dataDir.GetFiles("*.mp3"); // MP3ファイルのみを取得
        Debug.Log(musicPath + "   num:" + files.Length);

        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].ToString().Contains(".trashed"))  // ゴミ箱ファイルは除外
            {
                FilesName.Add(files[i].ToString().Split('/').Last());
                FilesPath.Add(files[i].ToString());
                Debug.Log(files[i] + " this is file");
            }
        }
        if (FilesName.Count == 0)
        {
            MusicsListPanel.SetActive(false);
            NonMusicExsitingObj.SetActive(true);
        }
        else
        {
            MusicsListPanel.SetActive(true);
            NonMusicExsitingObj.SetActive(false);
        }
        InstantiateMusicBoxes();
    }

    // Musicフォルダのパスを取得
    string GetAndroidMusicPath()
    {
#if PLATFORM_ANDROID
        using (var envClass = new AndroidJavaClass("android.os.Environment"))
        {
            using (var directoryMusic = envClass.GetStatic<AndroidJavaObject>("DIRECTORY_MUSIC"))
            {
                using (var file = envClass.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", directoryMusic))
                {
                    return file.Call<string>("getAbsolutePath");
                }
            }
        }
#else
        Debug.LogWarning("Not running on an Android device.");
        return null;
#endif
    }

    private void InstantiateMusicBoxes()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        Vector2 SDPanel = MusicsListPanel.GetComponent<RectTransform>().sizeDelta;
        SDPanel.x = Mathf.Clamp(SDPanel.x * FilesPath.Count, 0, 1600);
        MusicsListPanel.GetComponent<RectTransform>().sizeDelta = SDPanel;

        GameObject child = MusicsListPanel.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        Vector2 SDLayOut = child.GetComponent<RectTransform>().sizeDelta;
        SDLayOut.x *= FilesPath.Count;
        child.GetComponent<RectTransform>().sizeDelta = SDLayOut;

        for (int i = 0; i < FilesPath.Count; i++)
        {
            var NewBox = Instantiate(MusicBox, child.transform, false);
            var MBM = NewBox.GetComponent<MusicBoxManager>();
            MBM.SetThisBox(FilesName[i], i + 1, FilesPath[i]);
        }
    }

    private static AndroidJavaObject GetActivity()
    {
#if PLATFORM_ANDROID
        using (var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
#else
        return null;
#endif
    }

    /// <summary>
    /// min or max or size
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    private Vector2 GetSafeAreaAnchor(string kind)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Rect deviceSafeArea = Screen.safeArea;
        if (kind == "min")
        {
            return deviceSafeArea.position;
        }
        else if (kind == "max")
        {
            return deviceSafeArea.position + deviceSafeArea.size;
        }
        else if (kind == "size")
        {
            return deviceSafeArea.size;
        }
        else
        {
            Debug.Log("\"GetSafeAreaAnchor\":this string is Exception.");
            return Vector2.zero;
        }
    }

    // SetPermissionAdmitAgain メソッドも Android の場合のみ動作させる
    public void SetPermissionAdmitAgain()
    {
#if PLATFORM_ANDROID
        if (!hasPermissionButtonPressed)
        {
            hasPermissionButtonPressed = true;
            if (!permissionAdmitted)
            {
                var buttonImage = PermissionErrors.transform.GetChild(2).GetComponent<Image>();
                buttonImage.sprite = AdmittedImage;
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            return;
        }
        else
        {
            permissionAdmitted = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
            if (!permissionAdmitted)
            {
                var permissionText = PermissionErrors.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
                permissionText.text = "設定>アプリ>\"この作品の名前\"から\n\"音楽とオーディオの権限\"を\n許可してください";
                PermissionErrors.SetActive(true);
            }
            else
            {
                PermissionErrors.SetActive(false);
            }
        }
#endif
    }
}
