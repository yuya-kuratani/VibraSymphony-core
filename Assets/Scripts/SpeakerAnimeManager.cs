using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerAnimeManager : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = this.GetComponent<Animator>();
        if (!animator) Destroy(this);
    }

    void Update()
    {

    }
    public void StartAnimation(float power)
    {
        animator.SetFloat("Power", power);
        animator.Play("Vibe");
    }
}
