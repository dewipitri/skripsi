using System;
using UnityEngine;

public class StartStage : MonoBehaviour
{
    public static event Action OnStartStage;

    public static void StartStageNow()
    {
        OnStartStage?.Invoke();
    }
}
