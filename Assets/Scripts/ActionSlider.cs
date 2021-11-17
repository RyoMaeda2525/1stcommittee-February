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

    private void Awake() // この処理は Start やると遅いので Awake でやっている
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        Fill();
        Change();
    }

    /// <summary>
    /// ゲージを減らす
    /// </summary>
    /// <param name="value">増減させる量（割合）</param>
    public void Change()
    {
        ChangeValue(1f);
    }

    /// <summary>
    /// ゲージを満タンにする
    /// </summary>
    public void Fill()
    {
        slider.value = 0f;
    }

    /// <summary>
    /// 指定された値までゲージを滑らかに変化させる
    /// </summary>
    /// <param name="value"></param>
    void ChangeValue(float value)
    {
        //Camera.main.DOColor(Color.white, 5f)
        //.SetEase(Ease.InBounce)
        //.OnComplete(() => Debug.Log("色変更完了"));
        // DOTween.To() を使って連続的に変化させる
        tweener = DOTween.To(() => slider.value, // 連続的に変化させる対象の値
            x => slider.value = x, // 変化させた値 x をどう処理するかを書く
            value, // x をどの値まで変化させるか指示する
            _changeValueInterval);   // 何秒かけて変化させるか指示する
        if (slider.value <= 0.999f)
        {
            Fill();
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
        }
        else
        {
            tweener.Play();
        }
    }
}
