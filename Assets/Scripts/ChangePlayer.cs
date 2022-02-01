using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChangePlayer : MonoBehaviour
{
	//　現在どのキャラクターを操作しているか
	private int nowChara;
	//　操作可能なゲームキャラクター
	[SerializeField]
	private List<GameObject> charaList;
	[SerializeField]
	private List<GameObject> cameraList;

	void Start()
	{
		//　最初の操作キャラクターを0番目のキャラクターにする
		charaList[0].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().enabled = true;
		cameraList[0].GetComponent<CinemachineFreeLook>().Priority = 12;
	}

	void Update()
	{
        //　Qキーが押されたら操作キャラクターを次のキャラクターに変更する
        if (Input.GetKeyDown("q"))
        {
            ChangeCharacter(nowChara);
        }
    }

	//　操作キャラクター変更メソッド
	void ChangeCharacter(int tempNowChara)
	{
		//　現在操作しているキャラクターを動かなくする
		charaList[tempNowChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().enabled = false;
		//　次のキャラクターの番号を設定
		var nextChara = tempNowChara + 1;
		if (nextChara >= charaList.Count)
		{
			nextChara = 0;
		}
		//　次のキャラクターを動かせるようにする
		charaList[nextChara].GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().enabled = true;
		cameraList[nextChara].GetComponent<CinemachineFreeLook>().Priority = 12;
		cameraList[tempNowChara].GetComponent<CinemachineFreeLook>().Priority = 11;
		//　現在のキャラクター番号を保持する
		nowChara = nextChara;
	}
}
