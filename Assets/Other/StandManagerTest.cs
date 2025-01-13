using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

public class StandManagerTest : MonoBehaviour
{
[SerializeField] List<Transform> stands = new ();
Transform spawnPoint;
float offset = 0;

private void Start()
{
    var centerX = stands[0].position.x/2;
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

public bool CheckForAvailablePeopleStand(CustomerAI customerAI, out Transform transform1)
{
    throw new NotImplementedException();
}
}
