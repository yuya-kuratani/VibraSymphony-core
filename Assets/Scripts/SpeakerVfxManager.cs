using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpeakerVfxManager : MonoBehaviour
{
    VisualEffect effect;

    void Start()
    {
        effect = this.GetComponent<VisualEffect>();
        if (!effect) Destroy(this);
    }
    public void StartEffect()
    {
        effect.SendEvent("OnPlay");
        effect.SetVector4("Color", new Vector4(Random.Range(0f, 1f), 0.9f, 1f, 1f));
    }
}
