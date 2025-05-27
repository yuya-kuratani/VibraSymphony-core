using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsTargetManager : MonoBehaviour
{
    private Transform lightsTarget;
    private List<LightsMoveManager> stageLights = new();

    public Vector3 Offset;
    public float Angle;
    public bool Random;
    public float Speed, Dynamic;

    private Vector3 localOffset;

    void Start()
    {
        lightsTarget = transform.GetChild(0);

        var stageLightObjects = GameObject.FindGameObjectsWithTag("StageLights");
        foreach (var light in stageLightObjects)
        {
            stageLights.Add(light.GetComponent<LightsMoveManager>());
        }

        foreach (var light in stageLights)
        {
            light.SetTarget(lightsTarget);
        }
    }

    void Update()
    {
        localOffset = Offset + transform.position;

        var distance = (localOffset - lightsTarget.position).sqrMagnitude;
        

        if (!Random)
        {
            if (distance > 0.1f)
            {
                DirectionManager();
            }

            RotationManager();
        }
        else
        {
            RandomnessManager();
        }
    }

    private void DirectionManager()
    {
        lightsTarget.position = Vector3.Lerp(lightsTarget.position, localOffset, Time.deltaTime * 3);
        foreach (var light in stageLights)
        {
            light.DirectionUpdate();
        }
    }

    private void RotationManager()
    {
        foreach (var light in stageLights)
        {
            light.RotationUpdate(Angle);
        }
    }

    private void RandomnessManager()
    {
        foreach (var light in stageLights)
        {
            var seed = (uint)stageLights.IndexOf(light) * 2u + 123u;

            light.RandomnessUpdate(Speed, Dynamic, Time.time, seed++);
        }
    }
}
