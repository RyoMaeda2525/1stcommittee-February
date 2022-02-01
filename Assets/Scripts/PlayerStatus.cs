using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int atk = 20;
    public float critical = 0.5f;
    public bool _enemyT = false;
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
        while(Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0) 
        {
        }
        enemyTarget[0] = enemy;
        this.GetComponent<Animator>().Play("Attack");
    }

    public void Hit()
    {
        enemyList = EnemyChecker.GetEnemy();
        GetComponent<PlayerNavMesh>().TargetCancel();
        enemyTarget[0].GetComponent<Damage>().HitAttack(atk ,critical);
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
