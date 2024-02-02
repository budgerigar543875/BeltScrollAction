using System;
using UnityEngine;

public class RecoveryItem : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float recoveryRate;

    public float RecoveryRate
    {
        get { return recoveryRate; }
        set
        {
            recoveryRate = Math.Min(1, Math.Max(0, value));
        }
    }
}
