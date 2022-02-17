using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    static bool _commandPause = false;
    [SerializeField] GameObject commandPanel = default;//コマンドを出すパネル
    public event Action<bool> onCommandMenu;
    [Tooltip("操作プレイヤーの数"), SerializeField]
    int activeCount = 0;


    [Tooltip("一時停止メニュー"), SerializeField]
    GameObject pauseMenuPanel = default;
    [Tooltip("攻撃するボタンを表示するパネル"), SerializeField]
    GameObject attackCommandPanel = default;
    [Tooltip("攻撃ボタン"), SerializeField]
    GameObject attackCommandButton = default;
    [Tooltip("魔法を使うボタンを表示するパネル"), SerializeField]
    GameObject magicCommandPanel = default; //
    [Tooltip("味方を選択するためのパネル"), SerializeField]
    GameObject PartyCommandPanel = default;
    [Tooltip("味方を選択するためのボタン"), SerializeField]
    GameObject[] PartyCommandButtons = default;
    [Tooltip("どのプレイヤーのパネルなのか判別する絵"), SerializeField]
    GameObject[] images = new GameObject[3];
    [Tooltip("選択しているボタンを指すカーソル"), SerializeField]
    GameObject cursorImage = default;
    [Tooltip("どのコマンドか判別するための名前"), SerializeField]
    Text comanndText = default;


    ForceSelector commandForce = default;
    [Tooltip("現在操作しているキャラの数字"), SerializeField]
    int activeChara = 0;
    [Tooltip("プレイヤーキャラを取得するため"), SerializeField]
    ChangePlayer cp = default;
    List<GameObject> enemyList = new List<GameObject>();
    DoButton attackButtonScript = default;
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
            if (!_commandPause && _gameBack && !pauseMenuPanel.activeSelf)
            {
                activeChara = cp.nowChara;
                OnCommandMenu();
                cursorImage.SetActive(true);
            }
            _gameBack = false;
        }

        if (Input.GetButtonDown("Cancel")) 
        {
            if (!_commandPause && _gameBack && !pauseMenuPanel.activeSelf) 
            {
                pauseMenuPanel.SetActive(true);
                _commandPause = true;
                pauseMenuPanel.GetComponent<ForceSelector>().ForceSelect();
                onCommandMenu(_commandPause);
                cursorImage.SetActive(true);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (magicCommandPanel.activeSelf)
            {
                magicCommandPanel.SetActive(false);
                commandPanel.SetActive(true);
                commandForce.ForceSelect();
                comanndText.gameObject.SetActive(false);
            }
            else if (attackCommandPanel.activeSelf)
            {
                if (comanndText.text == "たたかう")
                {
                    attackCommandPanel.SetActive(false);
                    commandPanel.SetActive(true);
                    commandForce.ForceSelect();
                    comanndText.gameObject.SetActive(false);
                    foreach (Transform t in attackCommandPanel.transform)
                    {
                        Destroy(t.gameObject);
                    }
                }
                else
                {
                    attackCommandPanel.SetActive(false);
                    magicCommandPanel.SetActive(true);
                    magicCommandPanel.GetComponent<ForceSelector>().ForceSelect();
                    comanndText.text = "まほう";
                    foreach (Transform t in attackCommandPanel.transform)
                    {
                        Destroy(t.gameObject);
                    }
                }

            }
            else if (_commandPause && !pauseMenuPanel.activeSelf)
                OnCommandMenu();
        }


        if (commandPanel.activeSelf)
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                images[activeChara].SetActive(false);
                if (h > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        activeChara++;
                        if (activeChara > activeCount)
                        {
                            activeChara = 0;
                        }
                        if (cp.charaList[activeChara].activeSelf)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (activeChara == 0)
                        {
                            activeChara = activeCount;
                        }
                        else
                            activeChara--;
                        if (cp.charaList[activeChara].activeSelf)
                        {
                            break;
                        }
                    }
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
        attackCommandPanel.SetActive(true);
        comanndText.gameObject.SetActive(true);
        comanndText.text = "たたかう";
        // 敵の数だけButtonををパネルの子オブジェクトとして生成する
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject go = Instantiate(attackCommandButton);
            attackButtonScript = go.GetComponent<DoButton>();
            attackButtonScript.AttackSet(enemyList[i], cp.charaList[activeChara]);
            go.transform.SetParent(attackCommandPanel.transform);
        }
    }

    public void MagicCommand()
    {
        commandPanel.SetActive(false);
        magicCommandPanel.SetActive(true);
        comanndText.gameObject.SetActive(true);
        comanndText.text = "まほう";
    }

    public void FlameCommand()
    {
        if (cp.charaList[activeChara].GetComponent<PlayerStatus>().magicCost[0] <= cp.charaList[activeChara].GetComponent<PlayerStatus>().magicPoint)
        {
            GameObject nowchara = cp.charaList[cp.nowChara];
            enemyList = nowchara.transform.Find("EnemyChecker").GetComponent<EnemyChecker>().GetEnemy();
            magicCommandPanel.SetActive(false);
            attackCommandPanel.SetActive(true);
            comanndText.text = "フレイム";
            // 敵の数だけButtonををパネルの子オブジェクトとして生成する
            for (int i = 0; i < enemyList.Count; i++)
            {
                GameObject go = Instantiate(attackCommandButton);
                attackButtonScript = go.GetComponent<DoButton>();
                attackButtonScript.FlameSet(enemyList[i], cp.charaList[activeChara]);
                go.transform.SetParent(attackCommandPanel.transform);
            }
        }
    }

    public void IceCommand()
    {
        if (cp.charaList[activeChara].GetComponent<PlayerStatus>().magicCost[1] <= cp.charaList[activeChara].GetComponent<PlayerStatus>().magicPoint)
        {
            GameObject nowchara = cp.charaList[cp.nowChara];
            enemyList = nowchara.transform.Find("EnemyChecker").GetComponent<EnemyChecker>().GetEnemy();
            magicCommandPanel.SetActive(false);
            attackCommandPanel.SetActive(true);
            comanndText.text = "アイス";
            // 敵の数だけButtonををパネルの子オブジェクトとして生成する
            for (int i = 0; i < enemyList.Count; i++)
            {
                GameObject go = Instantiate(attackCommandButton);
                attackButtonScript = go.GetComponent<DoButton>();
                attackButtonScript.IceSet(enemyList[i], cp.charaList[activeChara]);
                go.transform.SetParent(attackCommandPanel.transform);
            }
        }
    }

    public void OnAttack()
    {
        attackCommandPanel.SetActive(false);
        images[activeChara].SetActive(false);
        comanndText.gameObject.SetActive(false);
        foreach (Transform t in attackCommandPanel.transform)
        {
            Destroy(t.gameObject);
        }
        _commandPause = false;
        cursorImage.SetActive(false);
        onCommandMenu(_commandPause);
    }

    public void HealCommand()
    {
        if (cp.charaList[activeChara].GetComponent<PlayerStatus>().magicCost[2] <= cp.charaList[activeChara].GetComponent<PlayerStatus>().magicPoint)
        {
            GameObject nowchara = cp.charaList[cp.nowChara];
            magicCommandPanel.SetActive(false);
            attackCommandPanel.SetActive(true);
            PartyCommandPanel.SetActive(true);
            comanndText.text = "ヒール";
            //味方の数だけButtonをsetactivにする
            for (int i = 0; i < cp.charaList.Count; i++)
            {
                if (cp.charaList[i].activeSelf)
                {
                    PartyCommandButtons[i].SetActive(true);
                    PartyCommandButtons[i].GetComponent<DoButton>().HealSet(cp.charaList[i], cp.charaList[activeChara]);
                }
            }
        }
    }

    public void OnBuff()
    {
        PartyCommandPanel.SetActive(false);
        attackCommandPanel.SetActive(false);
        images[activeChara].SetActive(false);
        comanndText.gameObject.SetActive(false);
        for (int i = 0; i < cp.charaList.Count; i++)
        {
            if (cp.charaList[i].activeSelf)
            {
                PartyCommandButtons[i].SetActive(false);
            }
        }
        _commandPause = false;
        cursorImage.SetActive(false);
        onCommandMenu(_commandPause);
    }

    public void GameBack()
    {
        _gameBack = true;
    }

    public void PauseManueCancel() 
    {
        pauseMenuPanel.SetActive(false);
        _commandPause = false;
        cursorImage.SetActive(false);
        onCommandMenu(_commandPause);
    }

    public void Pause()
    {
        _commandPause = true;
        onCommandMenu(_commandPause);
    }

    public void Resum() 
    {
        _commandPause = false;
        onCommandMenu(_commandPause);
    }
}
