using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;

[System.Serializable]
struct Audience
{
    public int2 seatPerBlock;
    public float2 seatPich;
    [Space]
    public int2 blockCount;
    public float2 aisleWidth;
    [Space]
    public float gradient;
    public float radius;
    [Space]
    public float swingFrequency;
    public float swingOffset;

    public static Audience Default()
        => new Audience()
            {
                
            };
    
    public int BlockSeatCount
        => seatPerBlock.x * seatPerBlock.y;

    public int TotalSeatCount
        => seatPerBlock.x * seatPerBlock.y * blockCount.x * blockCount.y;

    public (int2 block, int2 seat) GetCoordinatesFromIndex(int i)
    {
        var si = i / BlockSeatCount;
        var pi = i - BlockSeatCount * si;
        var sy = si / blockCount.x;
        var sx = si - blockCount.x * sy;
        var py = pi / seatPerBlock.x;
        var px = pi - seatPerBlock.x * py;
        return(math.int2(sx, sy), math.int2(px, py));
    }

    public float2 GetPositionOnPlane(int2 block, int2 seat)
        => seatPich * (seat - (float2)(seatPerBlock - 1) * 0.5f) + 
            (seatPich * (seatPerBlock - 1) + aisleWidth) * (block - (float2)(blockCount - 1) * 0.5f);

    public float4x4 GetStickMatrix(float2 pos, float4x4 xform, float time, uint seed)
    {
        var rand = new Random(seed);
        rand.NextUInt4();

        var phase = 2 * math.PI * swingFrequency * time;
        var nr1 = rand.NextFloat(-1000, 1000);
        phase += noise.snoise(math.float2(nr1, time * 0.27f));

        var origin = float3.zero;
        var originXZ = pos + rand.NextFloat2(-0.3f, 0.3f) * seatPich;

        var slope = (xform.c2 - originXZ.y) * gradient * 0.1f;
        origin.y = rand.NextFloat(-0.2f, 0.2f) + slope.z;

        var spreadOrigin = math.float2(originXZ.x * math.abs(xform.c3.z - originXZ.y) * 0.01f, originXZ.y);
        var vector = math.normalize(xform.c3.xz - spreadOrigin);
        var radiusVector = vector * (radius + pos.y);
        origin.xz = xform.c3.xz + radiusVector + rand.NextFloat(-0.2f, 0.2f);

        var angle = math.cos(phase);
        var angle_unsmooth = math.smoothstep(-1, 1, angle) * 2 - 1;
        angle = math.lerp(angle, angle_unsmooth, rand.NextFloat());
        angle *= rand.NextFloat(0.3f, 1.0f);

        var nr2 = rand.NextFloat(-1000, 1000);
        var dx = noise.snoise(math.float2(nr2, time * 0.23f + 100));
        var axis = math.normalize(math.float3(dx, 0, 1));

        var offset = swingOffset * rand.NextFloat(0.75f, 1.25f);

        var m1 = float4x4.Translate(origin); //float3をfloat4x4に変換
        var m2 = float4x4.AxisAngle(axis, angle); //axisを軸としてangleだけ時計回りに回転したfloat4x4を算出
        var m3 = float4x4.Translate(math.float3(0, offset, 0));
        return math.mul(math.mul(math.mul(xform, m1), m2), m3);
    }

    /// <summary>
    /// ペンライトの色を決める関数
    /// </summary>
    public Color GetStickColor(float2 pos, float time, uint seed)
    {
        var rand = new Random(seed);
        rand.NextUInt4();

        var wave = math.distance(pos, math.float2(0, 16));
        wave = math.sin(wave * 0.53f - time * 2.8f) * 0.5f + 0.5f; //全部プラスにするために + 0.5f

        var hue = math.frac(rand.NextFloat() + time * 0.83f);
        var br = wave * wave * 50 + 0.1f;

        return Color.HSVToRGB(hue, 1, br);
    }
}

[BurstCompile]
struct AudienceAnimationJob : IJobParallelFor
{
    public Audience config;
    public Matrix4x4 xform;
    public float time;

    public NativeSlice<Matrix4x4> matrices;
    public NativeSlice<Color> colors;

    public void Execute(int i)
    {
        var (block, seat) = config.GetCoordinatesFromIndex(i);
        var pos = config.GetPositionOnPlane(block, seat);
        var seed = (uint)i * 2u + 123u;
        matrices[i] = config.GetStickMatrix(pos, xform, time, seed++);
        colors[i] = config.GetStickColor(pos, time, seed++);
    }
}
