using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    public static List<GameObject> enemyList = new List<GameObject>();
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemyList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemyList.Remove(other.gameObject);
        }
    }

    internal static List<GameObject> GetEnemy()
    {
        return enemyList;
    }

    public void Destroy(GameObject enemy)
    {
        if(enemyList.Contains(enemy) == true)
        {
            enemyList.Remove(enemy);
        }
    }
}
