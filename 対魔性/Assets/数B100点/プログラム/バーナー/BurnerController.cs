﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnerController : MonoBehaviour
{
    public bool isLaunch=false;
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var emission = ps.emission; //こうしないとできないらしい
        emission.enabled=isLaunch;
    }
}
