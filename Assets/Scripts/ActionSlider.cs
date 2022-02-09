using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionSlider : MonoBehaviour
{
    [SerializeField] float _changeValueInterval = 6f;
    Slider slider = default;
    bool _stop = false;
    PauseMenuController _pauseMenu = default;
    Tweener tweener = default;
    PlayerStatus plst = default;
    Image background = default;
    GameObject enemySave = default;
    bool _backgroundAlpha = false;

    private void Awake() // この処理は Start やると遅いので Awake でやっている
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
        plst = GameObject.FindObjectOfType<PlayerStatus>();
        background = transform.Find("Background").GetComponent<Image>();
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        Reset();
    }

    /// <summary>
    /// ゲージを減らす
    /// </summary>
    /// <param name="value">増減させる量（割合）</param>
    public void FixedUpdate()
    {
        if (!_stop)
        {
            if (_backgroundAlpha)
                background.color = Color.Lerp(background.color, new Color(0f, 0f, 0f, 255f), 2f * Time.deltaTime);
            if (!_backgroundAlpha)
                background.color = Color.Lerp(background.color, new Color(0f, 0f, 0f, 0f), 2f * Time.deltaTime);
        }
    }

    /// <summary>
    /// ゲージを満タンにする
    /// </summary>
    public void Reset()
    {
        if (enemySave != null) 
        {
            ChangeValue(enemySave);
            return;
        }
        slider.value = 0f;
        _backgroundAlpha = false;
    }

    /// <summary>
    /// 指定された値までゲージを滑らかに変化させる
    /// </summary>
    /// <param name="value"></param>
    public void ChangeValue(GameObject enemy)
    {
        if (slider.value > 0f || slider.value < 1f)
        {
            tweener.Kill();
            Reset();
        }
        else if (slider.value == 1f) 
        {
            enemySave = enemy;
        }
        if (background.color.a != 255)
        {
            _backgroundAlpha = true;
        }
        tweener = DOTween.To(() => slider.value, // 連続的に変化させる対象の値
            x => slider.value = x, // 変化させた値 x をどう処理するかを書く
            1f, // x をどの値まで変化させるか指示する
            _changeValueInterval)// 何秒かけて変化させるか指示す
            .OnComplete(() => plst.Attack(enemy));
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

    void Attack(GameObject enemy)
    {
        plst.Attack(enemy);
    }

}
