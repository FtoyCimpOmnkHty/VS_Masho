﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの見た目について
public class PlayerEffectContloller : MonoBehaviour
{
    public bool isFrip;
    [SerializeField] const int effectSize = 3;
    [SerializeField] Color damageColor = Color.white;
    [SerializeField] Transform[] effectPos = new Transform[effectSize];
    [SerializeField] GameObject[] effects = new GameObject[effectSize];
    [SerializeField] AudioClip[] SEs = new AudioClip[effectSize];
    [SerializeField] GameObject Hahen = null;
    [SerializeField] Sprite[] deathImage = null;

    float beforHP;

    Animator anm;
    Rigidbody2D rb;
    SpriteRenderer armSR;
    SpriteRenderer[] bodyRenderer;
    AudioSource asc;

    Transform arm, body;
    PlayerContloller pc;

    private void RotateBody()
    {
        body.Rotate(body.forward, pc.armRot.eulerAngles.z);
    }
    private void ResetRotateBody()
    {
        body.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
    private void ParticleShot(int i)
    {
        if (i < effectSize)
        {
            asc.PlayOneShot(SEs[i]);
            Instantiate(effects[i], effectPos[i].position, body.rotation);
        }
    }
    private void ChangeBodyColor()
    {
        foreach (var b in bodyRenderer)
        {
            b.color = damageColor;
        }
    }
    private void FlipBody()
    {
        float z = Quaternion.FromToRotation(Vector3.up, pc.bodyVec).eulerAngles.z;
        if (!isFrip) return;
        //体の反転
        if (pc.bodyVec.x < 0f)
        {
            //左向き
            if (rb.velocity.x > 0) anm.SetBool("isLeft", true);
            else anm.SetBool("isLeft", false);
            body.localRotation = Quaternion.Euler(0f, 180f, 0f);
            arm.rotation = Quaternion.Euler(0,0, z) * Quaternion.Euler(0f, 180f, 90f);
            /* armSR.flipY = true;
            //銃の反転
            foreach(var s in pc.gunObj.GetComponentsInChildren<SpriteRenderer>())
            {
                s.flipY = true;
            }*/
        }
        else
        {
            if (rb.velocity.x < 0) anm.SetBool("isLeft", true);
            else anm.SetBool("isLeft", false);
            body.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            arm.rotation = Quaternion.Euler(0, 0, z) * Quaternion.Euler(0f, 0f, 90f);
            /*armSR.flipY = false;
            foreach (var s in pc.gunObj.GetComponentsInChildren<SpriteRenderer>())
            {
                s.flipY = false;
            }*/
        }
    }
    private void HahenSetup()
    {
        GameObject g = Instantiate(Hahen, transform);
        HahenParticle gh = g.GetComponent<HahenParticle>();
        g.transform.parent = null;
        if (gh != null)
        {
            gh.Sprites = deathImage;
            gh.layername = gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        anm = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        arm = transform.Find("体/左腕"); armSR = arm.GetComponent<SpriteRenderer>();
        bodyRenderer = GetComponentsInChildren<SpriteRenderer>();
        body = transform.Find("体");
        pc = GetComponent<PlayerContloller>();
        asc = GetComponent<AudioSource>();

        beforHP = pc.PlayerHp;
    }

    // Update is called once per frame
    void Update()
    {
        //無敵判定の点滅
        anm.SetBool("immortal", !pc.isDamage);

        foreach (var b in bodyRenderer)
        {
            b.material.SetFloat("_NoiseTh", pc.PlayerHp * 3 / pc.MaxPlayerHp);
        }

        //ノックバック
        if (beforHP > pc.PlayerHp)
        {
            beforHP = pc.PlayerHp;
            anm.SetTrigger("damage");
            if (pc.PlayerHp <= 0)
            {
                HahenSetup();
                Destroy(pc.gunObj);
                Destroy(gameObject);
            }
        }
        else if (beforHP < pc.PlayerHp)
        {
            beforHP = pc.PlayerHp;
            
        }

        //腕の画像切り替え
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) anm.SetFloat("armState", 1);
        else if (Input.GetMouseButtonUp(0)) anm.SetFloat("armState", 0);
        else anm.SetFloat("armState", 0);
        
        anm.SetFloat("speed", Mathf.Abs(rb.velocity.x) / pc.speed);
        FlipBody();
        ChangeBodyColor();

        
    } 
}
