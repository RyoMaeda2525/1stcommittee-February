using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int hitPoint = 100;
    public int defense = 120;
    int hit = 0;

    public void Hit(int damege, float critical) //攻撃を受け取る
    {
        if (Random.Range(0.09f, 100f) < critical) //クリティカル判定
        {
            hit = damege;
            hitPoint -= hit;
        }
        else //通常攻撃
        {
            hit = damege / 2 - defense / 4;　
            hitPoint -= hit;
        }
        Debug.Log(hit);
        if(this.gameObject.tag == "Enemy" && hitPoint <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Heal(int heal) //回復処理
    {
        hitPoint += heal;
    }

    private void OnDestroy()
    {
       GameObject.FindGameObjectWithTag("EnemyChecker").GetComponent<EnemyChecker>().Destroy(this.gameObject);
    }

}
