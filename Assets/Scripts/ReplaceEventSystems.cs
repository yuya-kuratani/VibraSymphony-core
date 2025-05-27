#if UNITY_EDITOR

using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEditor;

public class ReplaceEventSystems
{
    const float InitialHeight = 1.6f;

    /// <summary> 初期化メソッド</summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        // VR用シーンでなさそうならreturn
        if (GameObject.FindObjectOfType<OVRCameraRig>() == null) { return; }

        Replace();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary> シーン読み込み完了時に呼び出されるメソッド</summary>
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Replace();
    }

    /// <summary> シーン上のVR向け設定を変換するメソッド</summary>
    static void Replace()
    {
        // カメラ位置の調整
        var ovrManager = GameObject.FindObjectOfType<OVRManager>();
        if (ovrManager != null && ovrManager.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
        {
            Camera.main.transform.position += Vector3.up * InitialHeight;
        }

        // カメラにキーボード操作用コンポーネントを追加(必要なければ削除)
        Camera.main.gameObject.AddComponent<EditorCamera>();

        ReplaceEventSystem();
    }

    /// <summary> EventSystemに関するシーン上のVR向け設定を変換するメソッド</summary>
    static void ReplaceEventSystem()
    {
        // OVRInputModuleを無効にし、StandaloneInputModuleを有効に
        var eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.GetComponent<OVRInputModule>().enabled = false;
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            Debug.Log("変換成功: OVRInputModule -> StandaloneInputModule");
        }
        // Project中の全Canvasコンポーネントの中からAssets以下でないもの(= Hierarchy上のもの)を全て取得
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>()
            .Where(c => AssetDatabase.GetAssetOrScenePath(c).Contains(".unity")).ToArray();

        // GraphicRaycasterがあれば有効に、なければ追加
        foreach (var canvas in canvases)
        {
            var raycasters = canvas.gameObject.GetComponents<GraphicRaycaster>().ToList();
            if (raycasters.Exists(r => r.GetType() == typeof(GraphicRaycaster)))
            {
                raycasters.ForEach(r => r.enabled = true);
            }
            else
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"変換成功: GraphicRaycaster added to {canvas.name}");
            }
        }
    }
}

#endif

