using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider = default;
    [SerializeField]
    Image backGround = default;
    GameObject enemySave = default;

    PauseMenuController _pauseMenu = default;
    Tweener tweener = default;
    PlayerStatus plst = default;
    Animator _ani = default;

    [SerializeField] 
    float _changeValueIntervalSave = 0;
    [SerializeField]
    float _changeValueInterval = 6f;
    int eventSave = 0;

    bool _stop = false;
    bool _backgroundAlpha = false;

    private void Awake() // この処理は Start やると遅いので Awake でやっている
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
        plst = GetComponent<PlayerStatus>();
        _ani = GetComponent<Animator>();
    }

    void Start()
    {
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
                backGround.color = Color.Lerp(backGround.color, new Color(0f, 0f, 0f, 255f), 2f * Time.deltaTime);
            if (!_backgroundAlpha)
                backGround.color = Color.Lerp(backGround.color, new Color(0f, 0f, 0f, 0f), 2f * Time.deltaTime);
        }
    }

    /// <summary>
    /// ゲージを満タンにする
    /// </summary>
    public void Reset()
    {
        if (enemySave != null) 
        {
            ChangeValue(enemySave , _changeValueIntervalSave , eventSave);
            enemySave = null;
            _changeValueIntervalSave = 0;
            eventSave = 10;
            return;
        }
        slider.value = 0f;
        _backgroundAlpha = false;
    }

    /// <summary>
    /// 指定された値までゲージを滑らかに変化させる
    /// </summary>
    /// <param name="value"></param>
    public void ChangeValue(GameObject enemy , float _changeValueInterval, int any)
    {
        if (slider.value > 0f || slider.value < 1f)
        {
            Kill();  
        }
        else if (slider.value == 1f)
        {
            enemySave = enemy;
            _changeValueIntervalSave = _changeValueInterval;
            eventSave = any;
            return;
        }
        if (backGround.color.a != 255)
        {
            _backgroundAlpha = true;
        }
            tweener = DOTween.To(() => slider.value, // 連続的に変化させる対象の値
                x => slider.value = x, // 変化させた値 x をどう処理するかを書く
                1f, // x をどの値まで変化させるか指示する
                _changeValueInterval)// 何秒かけて変化させるか指示す
                .OnComplete(() => Do(enemy , any));
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
            Pause();
        }
        else
        {
            Resum();
        }
    }

    public void Pause() 
    {
        tweener.Pause();
        _stop = true;
    }

    void Resum() 
    {
        tweener.Play();
        _stop = false;
    }

    private void Do(GameObject enemy , int any)
    {
        plst.EnemySet(enemy, any);
    }

    public void Kill() 
    {
        tweener.Kill();
        if (_ani.GetBool("magic"))
        {
            _ani.SetBool("magic", false);
        }
        Reset();
    }
}
