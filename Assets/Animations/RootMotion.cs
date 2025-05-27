using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class RootMotion : MonoBehaviour
{
    private Animator animator;
    private void Start()
    {
        animator = this.GetComponent<Animator>();
        StartDance();
    }

    async void StartDance()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        if (gameObject.tag != "Clone")
        {
            animator.applyRootMotion = true;
        }
        Destroy(this);
    }
    public void InitializeMotion()
    {
        animator.SetTrigger("StartDance");
    }
}
