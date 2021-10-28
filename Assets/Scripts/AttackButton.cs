using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class AttackButton : MonoBehaviour
{
    [SerializeField] Text enemyNameText = default;
    string enemyName = "None";


    // Start is called before the first frame update
    public void EnemySet(GameObject enemy)
    {
        Debug.Log(enemy.name+"A");
        Button b = GetComponent<Button>();
        //if(enemy != null)
        //{
        enemyName = enemy.name;
        enemyNameText.text = enemyName;
        b?.OnClickAsObservable()
            .Subscribe(_ => Debug.Log(enemyName +"を攻撃"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
