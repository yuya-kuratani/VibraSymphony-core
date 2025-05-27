using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDisplayManager : MonoBehaviour
{
    Material material;

    private float vibration = 0f;
    private Color newColor;

    void Start()
    {
        material = this.GetComponent<MeshRenderer>().material;
        if (!material) Destroy(this);
    }

    void Update()
    {
        if (vibration >= 0.02f)
        {
            vibration -= 0.02f;
        }

        material.SetColor("_Color", newColor);
        material.SetFloat("Vibration", vibration);
    }

    public void StartVibration()
    {
        newColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.5f, 1f);
        vibration = 2f;
    }
}
