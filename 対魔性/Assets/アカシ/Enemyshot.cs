﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyshot : MonoBehaviour
{
    const int MAX_ENEMY = 5;
    [SerializeField] float distance_ani = 400f;
    [SerializeField] float shotPow = 1000;
    private Transform targetObj;
    private Transform enemyList;
    private Animator shoter = null;
    private EnemyContloller ec;
    public GameObject tumura;

    private int childPartNum = 1;

    int EnemyCount()
    {
        int count;
        //子のトランスフォームの数を取得、、、
        //しかし、子の子のトランスフォームまで取ってくるので、子のトランスフォームの数で最後に割る。
        Transform[] childs = enemyList.GetComponentsInChildren<Transform>();
        count = childs.Length / childPartNum;
        return count;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetObj = GameObject.Find("Player").transform;
        enemyList = transform.Find("EnemyList");
        ec = GetComponent<EnemyContloller>();
        shoter = GetComponent<Animator>();
        childPartNum = tumura.GetComponentsInChildren<Transform>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = 0f;
        if(targetObj!=null)distance = Vector3.Distance(transform.position, targetObj.position);
        if (distance <= distance_ani && EnemyCount()<MAX_ENEMY)
        {
            shoter.SetBool("distance", true);
        }
        else if (distance > distance_ani || EnemyCount() > MAX_ENEMY)
        {
            shoter.SetBool("distance", false);
        }
        if (ec.HP <= 0)
        {
            //親が死ぬと子も死ぬので親が死ぬとき子を開放する
            EnemyContloller[] childs = enemyList.GetComponentsInChildren<EnemyContloller>();
            foreach (var child in childs) child.transform.parent = null;
        }
        shoter.SetBool("immortal", !ec.isDamage);
    }
    void animation_event()
    {
        float x = 0f;
        if (targetObj != null)x = transform.position.x - targetObj.position.x;
        GameObject g = Instantiate(tumura, transform);
        g.GetComponent<Rigidbody2D>().AddForce(Vector2.up * shotPow, ForceMode2D.Impulse); //ひよこを上向きに発射
        g.transform.parent = enemyList;
        if (x < 0) g.transform.Rotate(0, 180, 0);
    }
}
