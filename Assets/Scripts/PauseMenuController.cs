using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenuController : MonoBehaviour
{
    static bool _commandFlg = false;
    public event Action<bool> OnCommandMenu;

    // Start is called before the first frame update
    void Start()
    {
        // 左クリックしたら一時停止・再開しつつコマンドメニューを切り替える
        if (Input.GetButtonDown("Fire1"))
        {
            CommandMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CommandMenu()
    {
        _commandFlg = !_commandFlg;
        OnCommandMenu(_commandFlg);  // これで変数に代入した関数を（全て）呼び出せる
    }
}
