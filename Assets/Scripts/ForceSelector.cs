using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ForceSelector : MonoBehaviour
{
    //自動選択されるボタン
    public GameObject firstSelected;
    private int i = 0; //0固定だと2回目から反映されないため。

    public void ForceSelect()
    {
        firstSelected = transform.GetChild(i).gameObject;//初めに選択するボタン
        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }
}
