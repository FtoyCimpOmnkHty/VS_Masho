﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PlayerContloller pc;
    AudioSource AudioSource;

    public IEnumerator WipeLoadScene(string sceneName)
    {
        //この関数よく使うのでちょっと汚くなたよ
        PostEffect pe = GameObject.Find("Main Camera").GetComponent<PostEffect>();
        for (float wipetime = 1f; wipetime > 0f; wipetime -= 0.01f)
        {
            if (pe == null) break;
            pe.wipeCircle.SetFloat("_Radius", wipetime);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
        StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        GameObject player = GameObject.Find("Player");
        if (player!=null)pc = player.GetComponent<PlayerContloller>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        if (pc!=null && pc.PlayerHp <= 0) StartCoroutine(WipeLoadScene("GameOver"));
    }

    public void SoundEffect(AudioClip audioClip)
    {
        AudioSource.PlayOneShot(audioClip);
    }
}
