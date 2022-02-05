using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenuController : MonoBehaviour
{
    static bool _commandPause = false;
    [SerializeField] GameObject commandPanel = default;//コマンドを出すパネル
    public event Action<bool> onCommandMenu;
    public event Action<bool> offCommandMenu;
    [SerializeField] GameObject attackCommandButton = default;
    [SerializeField] GameObject attackCommandPanel = default;
    ForceSelector commandForce = default;
    [Tooltip("プレイヤーキャラを取得するため"),SerializeField]
    ChangePlayer cp = default;
    List<GameObject> enemyList = new List<GameObject>();
    AttackButton attackButtonScript = default;
    bool _gameBack = true;
    //public int enemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        commandForce = commandPanel.GetComponent<ForceSelector>();
    }

    // Update is called once per frame
    void Update()
    {
        // 左クリックしたら一時停止・再開しつつコマンドメニューを切り替える
        if (Input.GetButtonDown("Fire1"))
        {
            if (!_commandPause && _gameBack)
                OnCommandMenu();
                _gameBack = false;
        }
        
        if (Input.GetButtonDown("Fire2"))
        {
            if (attackCommandPanel.activeSelf)
            {
                attackCommandPanel.SetActive(false);
                commandPanel.SetActive(true);
                commandForce.ForceSelect();
                foreach (Transform t in attackCommandPanel.transform)
                {
                    Destroy(t.gameObject);
                }
            }
           else if (_commandPause)
                OnCommandMenu();
        }
    }
    void OnCommandMenu()
    {
        if (!_commandPause)
        {
            commandPanel.SetActive(true);
            _commandPause = true;
            commandForce.ForceSelect();
            onCommandMenu(_commandPause);  // これで変数に代入した関数を全て呼び出せる
        }
        else
        {
            if (commandPanel)
            {
                commandPanel.SetActive(false);
                _commandPause = false;
                onCommandMenu(_commandPause);  
            }          
        }
    }

    public void AttackCommand()
    {
        GameObject nowchara = cp.charaList[cp.CharaNumber()] ;
        enemyList =  nowchara.transform.Find("EnemyChecker").GetComponent<EnemyChecker>().GetEnemy();
        commandPanel.SetActive(false);
        attackCommandPanel.SetActive(true);
        // 敵の数だけButtonををパネルの子オブジェクトとして生成する
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject go =  Instantiate(attackCommandButton);
            attackButtonScript = go.GetComponent<AttackButton>();
            attackButtonScript.EnemySet(enemyList[i]);
            go.transform.SetParent(attackCommandPanel.transform);
        }
    }

    public void OnAttack() 
    {
        attackCommandPanel.SetActive(false);
        foreach (Transform t in attackCommandPanel.transform)
        {
            Destroy(t.gameObject);
        }
        commandPanel.SetActive(false);
        _commandPause = false;
        onCommandMenu(_commandPause);
    }

    public void GameBack() 
    {
       _gameBack = true;
    }
}
