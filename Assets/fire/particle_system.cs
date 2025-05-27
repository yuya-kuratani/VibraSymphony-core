using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float launchForce = 10f;
    public GameObject particleEffect; // ここに最高点で起動させたいパーティクルエフェクトをアサインします
    private Rigidbody rb;
    private bool isLaunched = false;
    
    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        // 上方向に力を加える
        rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
        isLaunched = true;
    }

    void Update()
    {
        // 打ち上げた後で速度が0に近くなったら最高点とみなす
        if (isLaunched && rb.velocity.magnitude < 0.1f)
        {
            // 最高点に達したので、パーティクルエフェクトを起動
            Instantiate(particleEffect, transform.position, Quaternion.identity);
            isLaunched = false; // 一度だけ起動させるためにフラグを下ろす
        }
    }
}

