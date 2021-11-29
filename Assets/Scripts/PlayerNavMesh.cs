using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    GameObject enemyT = default;
    bool _enemytarget = false;
    Animator _anim = default;
    PauseMenuController _pauseMenu = default;
    bool _stop = false;
    Vector3 stopvelo = Vector3.zero;

    private void Awake()
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        EnemyCancel();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            navMeshAgent.isStopped = true;
        }
        else if (_enemytarget && !_stop)
        {
            EnemyResum();
        }

        if (!navMeshAgent.isStopped)
        {
            navMeshAgent.destination = enemyT.transform.position ;
            transform.LookAt(enemyT.transform.Find("Pivot").transform.position);
        }
        _anim.SetFloat("NavSpeed", navMeshAgent.velocity.magnitude);
    }

    public void Enemydiscover(GameObject enemy)
    {
        enemyT = enemy;
        _enemytarget = true;
        EnemyResum();
    }

    private void OnEnable() //ゲームに入ると加わる
    {
        _pauseMenu.onCommandMenu += PauseCommand;
    }

    private void OnDisable() //消えると抜ける
    {
        _pauseMenu.onCommandMenu -= PauseCommand;
    }

    void PauseCommand(bool onPause)
    {
        if (onPause)
        {
            stopvelo = navMeshAgent.velocity;
            navMeshAgent.velocity = Vector3.zero;
            _stop = true;
            navMeshAgent.isStopped = true;
        }
        else
        {
            navMeshAgent.velocity = stopvelo;
            _stop = false;
            EnemyResum();
        }
    }

    public void EnemyCancel()
    {
        navMeshAgent.isStopped = true;
        _enemytarget = false;
    }

    private void EnemyResum()
    {
        navMeshAgent.isStopped = false;
    }

}
