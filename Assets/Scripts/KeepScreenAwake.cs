using UnityEngine;

public class KeepScreenAwake : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnDestroy()
    {
        // Restore default sleep behavior when exiting the scene
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}
