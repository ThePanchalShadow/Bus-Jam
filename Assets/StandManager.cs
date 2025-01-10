using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandManager : MonoBehaviour
{
[SerializeField] List<Transform> stands = new ();
Transform spawnPoint;
float offset = 0;

private void Start()
{
    float centerX = stands[0].position.x/2;
    Debug.Log(centerX);
    foreach (var stand in stands)
    {
        stand.SetParent(null);
    }
    transform.position = new Vector3(centerX, 0, 0);
    foreach (var stand in stands)
    {
        stand.SetParent(transform);
    }
    transform.position = new Vector3(0, 0, 0);
}
}
