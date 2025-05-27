using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbSourceStartmanager : MonoBehaviour
{
    public void StartReverbSource()
    {
        var newSource = this.GetComponent<AudioSource>();
        newSource.Play();
    }
}
