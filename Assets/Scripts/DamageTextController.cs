using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextController : MonoBehaviour
{
    [Tooltip("上に移動するスピードs"),SerializeField] 
    private float moveSpeed = 0.4f;
    [Tooltip("消えるまでの時間"),SerializeField]
    private float fadeOutSpeed = 1f;
    Text damageText = default;
    PauseMenuController _pauseMenu = default;
    bool _stop = false;

    private void Awake()
    {
        _pauseMenu = FindObjectOfType<PauseMenuController>();
    }

    void Start()
    {
        damageText = GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_stop)
        {
            transform.LookAt(Camera.main.transform.position);
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            damageText.color = Color.Lerp(damageText.color, new Color(255f, 0f, 0f, 0f), fadeOutSpeed * Time.deltaTime);

            if (damageText.color.a <= 0.1f)
            {
                Destroy(gameObject);
            }
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
            _stop = true;
        }
        else
        {
            _stop = false;
        }
    }
}
