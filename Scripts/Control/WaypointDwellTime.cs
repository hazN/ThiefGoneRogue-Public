using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointDwellTime : MonoBehaviour
{
    [SerializeField] private float dwellTime = 0f;

    public float DwellTime
    {
        get { return dwellTime; }
    }
}
