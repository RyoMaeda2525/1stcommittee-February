using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
    //現在どのキャラクターを操作しているか
    [SerializeField]
    internal int nowChara = 0;
    internal int nextChara = 0;
    internal int notChara = 0;
    //　操作可能なゲームキャラクター
    [Tooltip("操作するキャラリスト"), SerializeField]
    internal List<GameObject> charaList;
    [Tooltip("操作キャラに合わせたカメラ"), SerializeField]
    private List<GameObject> cameraList;
    private PauseMenuController _pauseMenu = default; //停止した際に動かないようにするため

    private bool _stop = false;

    void Start()
    {
        //　最初の操作キャラクターを0番目のキャラクターにする
        charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
        cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
        if (charaList.Count > 1)
        {
            nextChara = nowChara + 1;
            if (nextChara >= charaList.Count)
            {
                nextChara = 0;
            }
            //他のキャラクターが操作キャラを追うようにする
            NotPlay(nextChara);
        }
        if (charaList.Count > 2)
        {
            notChara = nextChara + 1;
            if (notChara >= charaList.Count)
            {
                notChara = 0;
            }
            //他のキャラクターが操作キャラを追うようにする
            NotPlay(notChara);
        }
    }

    void FixedUpdate()
    {
        if (!_stop)
        {
            if (Input.GetKeyDown("q")) //Qキーが押されたら操作キャラクターを次のキャラクターに変更する
            {
                ChangeCharacter("Change");
            }
        }
    }

    //　操作キャラクター変更メソッド
    internal void ChangeCharacter(string any)
    {
        if (any == "Change")
        {
            if (charaList[notChara].activeSelf && charaList[nextChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = nextChara;
                nextChara = notChara;
                notChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
                //　現在操作しているキャラクターを動かなくする
                NotPlay(nextChara);
                NotPlay(notChara);
            }
            else if (charaList[notChara].activeSelf && !charaList[nextChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = notChara;
                notChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
                //　現在操作しているキャラクターを動かなくする
                NotPlay(notChara);
            }
            else if (charaList[nextChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = nextChara;
                nextChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
                //　現在操作しているキャラクターを動かなくする
                NotPlay(nextChara);
            }
        }
        else if (any == "Deth")
        {
            if (!charaList[nowChara].activeSelf && !charaList[nextChara].activeSelf && !charaList[notChara].activeSelf)
            {
                FindObjectOfType<GameManager>().GameOver();
            }
            else if (!charaList[nowChara].activeSelf && !cameraList[nextChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = notChara;
                notChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;            
            }
            else if (!charaList[nowChara].activeSelf && !cameraList[notChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = nextChara;
                nextChara = notChara;
                notChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
            }
            else if (!charaList[nowChara].activeSelf)
            {
                int tempNowChara = nowChara;
                nowChara = nextChara;
                nextChara = notChara;
                notChara = tempNowChara;
                //　次のキャラクターを動かせるようにする
                charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
                cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
                //　現在操作しているキャラクターを動かなくする
                NotPlay(nextChara);
            }
        }

    }

    internal int CharaNumber()
    {
        return nowChara;
    }

    private void NotPlay(int x)
    {
        charaList[x].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().NoPlay();
        cameraList[x].GetComponent<CinemachineFreeLook>().Priority = 11;
    }

    private void Awake() // この処理は Start やると遅いので Awake でやっている
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
    }

    private void OnEnable() //ゲームに入ると加わる
    {
        _pauseMenu.onCommandMenu += PauseCommand;
    }

    private void OnDisable() //消えると抜ける
    {
        _pauseMenu.onCommandMenu -= PauseCommand;
    }

    private void PauseCommand(bool onPause)
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

    private void Pause()
    {
        _stop = true;
    }

    private void Resum()
    {
        _stop = false;
    }
}
