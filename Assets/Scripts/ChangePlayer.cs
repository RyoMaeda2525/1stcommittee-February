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
	[SerializeField]
	public List<GameObject> charaList;
	[SerializeField]
	private List<GameObject> cameraList;

	void Start()
	{
		nextChara = nowChara + 1;
		if (nextChara >= charaList.Count)
        {
			nextChara = 0;
        }
		notChara = nextChara + 1;
		if (notChara >= charaList.Count)
		{
			notChara = 0;
		}
		//　最初の操作キャラクターを0番目のキャラクターにする
		charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
		cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
		//他のキャラクターが操作キャラを追うようにする
		NotPlay(notChara);
		NotPlay(nextChara);
	}

	void Update()
	{
        //　Qキーが押されたら操作キャラクターを次のキャラクターに変更する
        if (Input.GetKeyDown("q"))
        {
            ChangeCharacter();
        }
    }

	//　操作キャラクター変更メソッド
	void ChangeCharacter()
	{
		//var nowChara = tempNowChara + 1;
		//if (nowChara >= charaList.Count)
		//{
		//	nowChara = 0;
		//}
		//var notChara = nowChara + 1;
		//if (notChara >= charaList.Count)
		//{
		//	notChara = 0;
		//}
		NextChara();
		//　次のキャラクターを動かせるようにする
		charaList[nowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().PlayCharacterNow();
		cameraList[nowChara].GetComponent<CinemachineFreeLook>().Priority = 12;
		//　現在操作しているキャラクターを動かなくする
		NotPlay(nextChara);
		NotPlay(notChara);
	}

	internal  int CharaNumber() 
	{
		return nowChara; 
	}

    private void NotPlay(int x)
    {
		charaList[x].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().NoPlay();
		cameraList[x].GetComponent<CinemachineFreeLook>().Priority = 11;
	}

    private void NextChara() //　次のキャラクターの番号を設定
	{
		int tempNowChara = nowChara;
		nowChara = nextChara;
		nextChara = notChara;
		notChara = tempNowChara;
	}

}
