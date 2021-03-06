﻿using System.Collections;
using UnityEngine;

public class BossHeadController : MonoBehaviour
{
    [Header("重力ボール関係")]
    public bool isLaunch = false;
    [SerializeField] float launchPow = 200f, LaunchLate = 5f;
    [SerializeField] float range = 800f;
    [SerializeField] GameObject gravitiBall = null, launchPoint = null,faceCover = null;
    [SerializeField] AudioClip SE = null;

    [Header("レーザー関係")]
    [SerializeField] float Xrange = 600f;
    [SerializeField] float launchTime = 3f;
    [SerializeField] float[] shootProbably = null;
    [SerializeField] Transform body = null;

    [Header("全滅時")]
    [SerializeField] Color deadColor = Color.gray;
    [SerializeField] LauncherController missile = null;

    bool changedLaserProperty = false,deadAll = false;
    Transform target;
    AudioSource ass;
    LaserController[] lasers;
    EnemyContloller Glauncher;

    bool JudgeLaser(float probably)
    {

        bool isLaunchLaser = false;
        float x = 0;
        if (target != null)
        {
            x = Mathf.Abs(target.position.x - transform.position.x);
        }
        if ( x < Xrange)
        {
            float r = Random.Range(0f, 1f);
            isLaunchLaser = r < probably ? true : false;
        }
        return isLaunchLaser;
    }

    IEnumerator LaunchGravity()
    {
        while (Glauncher != null)
        {
            if (target != null && (target.position - transform.position).magnitude < range)
            {


                GameObject gb = null;
                GravityBall b = null;
                if (isLaunch)
                {
                    gb = Instantiate(gravitiBall, launchPoint.transform);
                    b = gb.GetComponent<GravityBall>();
                    b.enabled = false;
                    Destroy(gb, 11.3f);
                }

                yield return new WaitForSeconds(2f);

                if (gb != null)
                {
                    ass.PlayOneShot(SE);
                    b.enabled = true;
                    Vector2 vec = Random.Range(-1, 1f) * Vector2.right;
                    vec += Random.Range(-1, 1f) * Vector2.up;
                    gb.GetComponent<Rigidbody2D>().AddForce(vec * launchPow, ForceMode2D.Impulse);
                }
            }
            yield return new WaitForSeconds(LaunchLate);
        }
    }

    IEnumerator LaserSettings()
    {
        while (Glauncher != null)
        {
            for (int i = 0; i < lasers.Length; i++)
            {
                bool a = JudgeLaser(shootProbably[i]);
                lasers[i].isLaunch = a;
                //角速度を計算　一周するまで待機 
                yield return new WaitForSeconds(launchTime);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ass = GetComponent<AudioSource>();
        lasers = GetComponentsInChildren<LaserController>();
        Glauncher = launchPoint.GetComponentInParent<EnemyContloller>();
        Glauncher.enabled = false;
        Glauncher.GetComponent<Collider2D>().enabled = false;
        var player = GameObject.Find("Player");
        if (player != null) target = player.transform;
        StartCoroutine(LaunchGravity());
        StartCoroutine(LaserSettings());
    }

    private void Update()
    {
        if (faceCover == null && !changedLaserProperty)
        {
            
            isLaunch = true;
            Glauncher.enabled = true;
            changedLaserProperty = true;
            Glauncher.GetComponent<Collider2D>().enabled = true;

            for (int i = 0; i < lasers.Length; i++)
            {
                shootProbably[i] *= 0.5f;
            }
        }
        if (Glauncher == null && !deadAll)
        {
            deadAll = true;
            GetComponent<SpriteRenderer>().color = deadColor;
            EnemyContloller missileE = missile.GetComponent<EnemyContloller>();
            missileE.Damage(missileE.HP);
            foreach(var l in lasers)
            {
                l.isLaunch = false;
            }
        }
    }
}
