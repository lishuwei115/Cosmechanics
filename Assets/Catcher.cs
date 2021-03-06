﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catcher : MonoBehaviour {

    public Transform respawnPos;
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.position = respawnPos.position;
        collision.transform.eulerAngles = Vector3.zero;
    }
}
