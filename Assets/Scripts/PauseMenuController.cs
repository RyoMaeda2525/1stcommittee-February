using System;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    static bool _commandPause = false;
    [SerializeField] GameObject commandPanel = default;//コマンドを出すパネル
    public event Action<bool> onCommandMenu;
    [Tooltip("パネルなどの要素数"), SerializeField]
    int activeCount = 0;
    [SerializeField] GameObject[] attackCommandButtons = new GameObject[3];
    [SerializeField] GameObject attackCommandPanel = default;
    [SerializeField] GameObject[] images = new GameObject[3];
    [Tooltip("選択しているボタンを指すカーソル"), SerializeField]
    GameObject cursorImage = default;
    ForceSelector commandForce = default;
    [Tooltip("現在操作可能なキャラの数字"), SerializeField]
    int activeChara = 0;
    [Tooltip("プレイヤーキャラを取得するため"), SerializeField]
    ChangePlayer cp = default;
    List<GameObject> enemyList = new List<GameObject>();
    AttackButton attackButtonScript = default;
    bool _gameBack = true;
    //public int enemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        activeChara = cp.nowChara;
        commandForce = commandPanel.GetComponent<ForceSelector>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");

        // 左クリックしたら一時停止・再開しつつコマンドメニューを切り替える
        if (Input.GetButtonDown("Fire1"))
        {
            if (!_commandPause && _gameBack)
            {
                activeChara = cp.nowChara;
                OnCommandMenu();
                cursorImage.SetActive(true);
            }
            _gameBack = false;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (attackCommandPanel.activeSelf)
            {
                attackCommandPanel.SetActive(false);
                commandPanel.SetActive(true);
                images[activeChara].SetActive(true);
                commandForce.ForceSelect();
                foreach (Transform t in attackCommandPanel.transform)
                {
                    Destroy(t.gameObject);
                }
            }
            else if (_commandPause)
                OnCommandMenu();
        }


        if (commandPanel.activeSelf)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                images[activeChara].SetActive(false);
                if (h > 0)
                {
                    activeChara++;
                    if (activeChara > activeCount)
                    {
                        activeChara = 0;
                    }
                }
                else
                {
                    if (activeChara == 0)
                    {
                        activeChara = activeCount;
                    }
                    else
                    activeChara--;
                }
                images[activeChara].SetActive(true);
            }
        }
    }

    void OnCommandMenu()
    {
        if (!_commandPause)
        {
            commandPanel.SetActive(true);
            images[activeChara].SetActive(true);
            _commandPause = true;
            commandForce.ForceSelect();
            onCommandMenu(_commandPause);  // これで変数に代入した関数を全て呼び出せる
        }
        else
        {
            if (commandPanel)
            {
                images[activeChara].SetActive(false);
                commandPanel.SetActive(false);
                _commandPause = false;
                cursorImage.SetActive(false);
                onCommandMenu(_commandPause);
            }
        }
    }

    public void AttackCommand()
    {
        GameObject nowchara = cp.charaList[cp.nowChara];
        enemyList = nowchara.transform.Find("EnemyChecker").GetComponent<EnemyChecker>().GetEnemy();
        commandPanel.SetActive(false);
        images[activeChara].SetActive(false);
        attackCommandPanel.SetActive(true);
        // 敵の数だけButtonををパネルの子オブジェクトとして生成する
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject go = Instantiate(attackCommandButtons[activeChara]);
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
        cursorImage.SetActive(false);
        onCommandMenu(_commandPause);
    }

    public void GameBack()
    {
        _gameBack = true;
    }
}
