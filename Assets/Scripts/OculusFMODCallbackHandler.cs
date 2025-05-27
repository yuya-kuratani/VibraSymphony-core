using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Oculus FMOD Callback Handler")]
public class OculusFMODCallbackHandler : FMODUnity.PlatformCallbackHandler
{
    public override void PreInitialize(FMOD.Studio.System studioSystem, Action<FMOD.RESULT, string> reportResult)
    {
        string riftId = OVRManager.audioOutId;

        FMOD.System coreSystem;
        FMOD.RESULT result = studioSystem.getCoreSystem(out coreSystem);
        reportResult(result, "studioSystem.getCoreSystem");

        int driverCount = 0;
        result = coreSystem.getNumDrivers(out driverCount);
        reportResult(result, "coreSystem.getNumDrivers");

        for (int i = 0; i < driverCount; i++)
        {
            int rate;
            int channels;
            System.Guid guid;
            FMOD.SPEAKERMODE mode;

            result = coreSystem.getDriverInfo(i, out guid, out rate, out mode, out channels);
            reportResult(result, "coreSystem.getDriverInfo");

            if (guid.ToString() == riftId)
            {
                result = coreSystem.setDriver(i);
                reportResult(result, "coreSystem.setDriver");

                break;
            }
        }
    }
}