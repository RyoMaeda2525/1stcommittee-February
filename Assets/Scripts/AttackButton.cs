using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class AttackButton : MonoBehaviour
{
    [SerializeField] Text enemyNameText = default;
    LineRenderer lineR = default;
    EventSystem eventS = default;
    GameObject unityChan = default;
    string enemyName = "None";

    private void Awake()
    {
        lineR = GetComponent<LineRenderer>();
        eventS = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        unityChan = GameObject.Find("Unitychan");
        lineR.SetPosition(0, unityChan.transform.position);
    }

    // Start is called before the first frame update
    public void EnemySet(GameObject enemy)
    {
        //Debug.Log(enemy.name+"A");
        Button b = GetComponent<Button>();
        //if(enemy != null)
        //{
        enemyName = enemy.name;
        enemyNameText.text = enemyName;
        b?.OnSelectAsObservable()
            .Subscribe(_ => lineR.SetPosition(1, enemy.transform.position));

        b?.OnClickAsObservable()
            .Subscribe(_ => enemy.GetComponent<Damage>().Hit(100,0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (eventS.currentSelectedGameObject == this.gameObject)
        {
            lineR.enabled = true;
        }
        else
        {
            lineR.enabled = false;
        }

    }
}
