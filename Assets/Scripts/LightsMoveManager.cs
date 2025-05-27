using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class LightsMoveManager : MonoBehaviour
{
    private Transform target;

    private Transform baseAxis, subAxis;

    private Vector3 rotAxis;

    private float baseAngle, subAngle;

    void Awake()
    {
        baseAxis = transform.GetChild(0);
        subAxis = transform.GetChild(0).GetChild(0);

        subAngle = baseAngle = 0;
    }

    void Update()
    {

    }

    public void SetTarget(Transform lightsTarget)
    {
        if (transform.rotation.x < 1f)
        {
            rotAxis = Vector3.up;
        }
        else
        {
            rotAxis = Vector3.down;
        }

        target = lightsTarget;
        DirectionUpdate();
    }

    public void DirectionUpdate()
    {
        var targetPos = target.position;
        targetPos.y = baseAxis.position.y;
        var relativePos = targetPos - baseAxis.position;
        baseAxis.rotation = Quaternion.LookRotation(relativePos, rotAxis);
    }

    public void RotationUpdate(float angle)
    {
        var localTargetAngle = Quaternion.Euler(angle, 0, 0);
        var targetAngle = baseAxis.rotation * localTargetAngle;
        var complementAngle = Quaternion.Lerp(subAxis.rotation, targetAngle, Time.deltaTime * 3);
        subAxis.rotation = complementAngle;
    }

    public void RandomnessUpdate(float speed, float dynamic, float time, uint seed)
    {
        var rand = new Random(seed);
        rand.NextUInt4();

        var phase = math.PI * speed * time;
        phase *= rand.NextFloat(0.8f, 1.2f);

        baseAngle = math.cos(phase);
        baseAngle *= rand.NextFloat(1f, 2f);

        subAngle = math.sin(phase);
        subAngle *= rand.NextFloat(1f, 1.4f);

        var baseAxisRotation = baseAngle * dynamic;
        var subAxisRotation = subAngle * dynamic;
        var rot = 0;
        if (rotAxis != Vector3.up)
        {
            rot = 180;
        }
        baseAxis.rotation = Quaternion.Euler(rot, baseAxisRotation, 0);
        var localTargetAngle = Quaternion.Euler(subAxisRotation, 0, 0);
        var targetAngle = baseAxis.rotation * localTargetAngle;
        subAxis.rotation = targetAngle;
    }
}
