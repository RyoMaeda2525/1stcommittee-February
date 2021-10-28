using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ForceSelector : MonoBehaviour
{
    //自動選択されるボタン
    public GameObject firstSelected;
    int i = 0; //0固定だと2回目から反映されないため。

    void Start()
    {
        //if (firstSelected == null)
        //{
        //    firstSelected = transform.GetChild(0).gameObject;

        //}
    }

    //void OnEnable()
    //{
    //    ForceSelect();
    //}

    public void ForceSelect()
    {
        firstSelected = transform.GetChild(i).gameObject;
        Debug.Log("a");
        //firstSelectedを選択
        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        //if (this.gameObject.name == "AttackCommandPanel")
        //{
        //    List<GameObject> enemys = new List<GameObject>();
        //    enemys = EnemyChecker.GetEnemy();
        //    i = enemys.Count + 1;
        //}
    }
}
