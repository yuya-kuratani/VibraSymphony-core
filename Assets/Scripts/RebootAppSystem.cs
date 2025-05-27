using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RebootApp
{
    public static class RebootAppSystem
    {
        public static void RestartApp()
        {
            // AndroidのIntentを使用してアプリを再起動
            AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", activity.Call<string>("getPackageName"));

            intent.Call<AndroidJavaObject>("addFlags", new AndroidJavaObject("android.content.Intent").GetStatic<int>("FLAG_ACTIVITY_CLEAR_TOP"));
            intent.Call<AndroidJavaObject>("addFlags", new AndroidJavaObject("android.content.Intent").GetStatic<int>("FLAG_ACTIVITY_NEW_TASK"));
            activity.Call("startActivity", intent);
            activity.Call("finish");
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // Ensure the process is killed
        }
    }

}
