﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Damage : MonoBehaviour
{
    public int hitpointMax = 100;
    int hitPoint = 100;
    public int defense = 40;
    int hit = 0;
    Tweener tweener = default;
    bool _stop = false;
    bool _Alpha = false;
    int tempHp = 0;
    PauseMenuController _pauseMenu = default;
    float hpChangeInterval = 1.5f;
    [SerializeField] public Text maxhpText = default;
    [SerializeField] public Text hpText = default;
    [SerializeField] private Text damageText = default;
    [SerializeField] private GameObject canvas;//親にするキャンバスを格納
    [SerializeField] GameObject pivot = default;
    [SerializeField] Slider hpSlider = default;
    [SerializeField] Image background = default;
    [SerializeField] Image fill = default;

    private void Awake()
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
        hitPoint = hitpointMax;
        tempHp = hitpointMax;
    }

    private void Start()
    {
        hpSlider.maxValue = hitpointMax;
        ChangeValue();
    }

    public void Hit(int damege, float critical) //攻撃を受け取る
    {
        if (Random.Range(0.09f, 100f) < critical) //クリティカル判定
        {
            hit = damege;
            hitPoint -= hit;
        }
        else //通常攻撃
        {
            hit = damege / 2 - defense / 4;
            hitPoint -= hit;
        }
        if (this.gameObject.tag == "Enemy" && hitPoint <= 0)
        {
            Destroy(this.gameObject);
        }
        ChangeValue();
        Text _text = Instantiate(damageText, pivot.transform.position - Camera.main.transform.forward * 0.2f, Quaternion.identity);
        _text.text = hit.ToString();
        _text.transform.SetParent(canvas.transform);
    }

    // Update is called once per frame
    void Heal(int heal) //回復処理
    {
        hitPoint += heal;
        hitPoint = System.Math.Min(hitPoint, hitpointMax);
    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<EnemyChecker>().Destroy(this.gameObject);
    }

    public void FixedUpdate()
    {
        if(gameObject.tag != "Player") 
        {
            hpSlider.transform.LookAt(Camera.main.transform.position);

            //if (!_stop)
            //{
            //    if (_Alpha)
            //    {
            //        background.color = new Color(132f, 0f, 0f, 255f);
            //        fill.color = new Color(255f, 100f, 47f, 255f);
            //    }
            //    else
            //    {
            //        background.color = new Color(132f, 0f, 0f, 0f);
            //        fill.color = new Color(255f, 100f, 47f, 0f);
            //    }
            //}

        }
    }

    public void ChangeValue()
    {
       if(this.gameObject.tag == "Player" )    
        {
            DOTween.To(() => tempHp, // 連続的に変化させる対象の値
            x => tempHp = x, // 変化させた値 x をどう処理するかを書く
            hitPoint, // x をどの値まで変化させるか指示する
            hpChangeInterval)   // 何秒かけて変化させるか指示する
            .OnUpdate(() => hpText.text = tempHp.ToString("000"));   // 数値が変化する度に実行する処理を書く

        }
        else 
        {
            tweener = DOTween.To(() => hpSlider.value, // 連続的に変化させる対象の値
            x => hpSlider.value = x, // 変化させた値 x をどう処理するかを書く
            hitPoint, // x をどの値まで変化させるか指示する
            hpChangeInterval);// 何秒かけて変化させるか指示す
        }
    }

    private void OnEnable() //ゲームに入ると加わる
    {
        _pauseMenu.onCommandMenu += PauseCommand;
    }

    private void OnDisable() //消えると抜ける
    {
        _pauseMenu.onCommandMenu -= PauseCommand;
    }

    void PauseCommand(bool onPause)
    {
        if (onPause)
        {
            tweener.Pause();
            _stop = true;
        }
        else
        {
            tweener.Play();
            _stop = false;
        }
    }

    public void Alpha()
    {
        _Alpha = true;
    }

    public void NonAlpha()
    {
        _Alpha = false;
    }
}