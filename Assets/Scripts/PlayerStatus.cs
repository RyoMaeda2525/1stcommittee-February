using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int atk = 20;
    public float critical = 0.5f;
    public bool _enemyT = false;
    Damage enemyDamage = default;
    ActionSlider acs = default;
    List<GameObject> enemyList = new List<GameObject>();
    GameObject[] enemyTarget = new GameObject[1];
    

    // Update is called once per frame
    private void Start()
    {
        acs = GameObject.FindObjectOfType<ActionSlider>();
    }

    public void Attack(GameObject enemy)
    {
        //enemyDamage = enemy.GetComponent<Damage>();
        enemyTarget[0] = enemy;
        this.GetComponent<Animator>().Play("Attack");
    }

    public void Hit()
    {
        enemyList = EnemyChecker.GetEnemy();
        this.GetComponent<PlayerNavMesh>().EnemyCancel();
        enemyTarget[0].GetComponent<Damage>().Hit(atk ,critical);
    }

    public void SliderReset() 
    {
        acs.Reset();
        if (_enemyT)
        {
            if (enemyList.Contains(enemyTarget[0]) == true)
            {
                acs.ChangeValue(enemyTarget[0]);
            }
        }
    }
}
