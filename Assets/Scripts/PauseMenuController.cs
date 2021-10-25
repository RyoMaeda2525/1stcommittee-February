using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenuController : MonoBehaviour
{
    static bool _commandPause = false;
    static bool _commandResum = false;
    [SerializeField] GameObject commandPanel = default;//コマンドを出すパネル
    public event Action<bool> onCommandMenu;
    public event Action<bool> offCommandMenu;

    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリックしたら一時停止・再開しつつコマンドメニューを切り替える
        if (Input.GetButtonDown("Fire2"))
        {
            OnCommandMenu();
        }
        //else if (Input.GetButtonDown("Fire2"))
        //{
        //    OffCommandMenu();
        //}
    }

    void OnCommandMenu()
    {
        if (!_commandPause)
        {
            commandPanel.SetActive(true);
            _commandPause = true;
            onCommandMenu(_commandPause);  // これで変数に代入した関数を全て呼び出せる
        }
        else
        {       
            commandPanel.SetActive(false);
            _commandPause = false;
            onCommandMenu(_commandPause);  // これで変数に代入した関数を全て呼び出せる
        }
        
        //_commandResum = false;
        //onCommandMenu(_commandResum);
    }

    //void OffCommandMenu()
    //{
    //    if (_commandPause)
    //    {
    //        _commandPause = false;
    //        _commandResum = true;
    //        onCommandMenu(_commandResum);
    //        offCommandMenu(_commandPause);
    //    }
    //}      
}
