using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMesh : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    [Tooltip("プレイキャラの確認"), SerializeField]
    ChangePlayer chp = default;
    float navSpeed = 0; //navmeshのスピードを入れる
    private GameObject enemyT = default;
    Animator _anim = default;
    PauseMenuController _pauseMenu = default;
    bool _stop = false;
    bool _nonPlay = true;
    Vector3 stopvelo = Vector3.zero;

    private void Awake()
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
        _anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyT = chp.charaList[0];
        Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (navMeshAgent.enabled)
        {
            if (!_nonPlay)
            {
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && !navMeshAgent.isStopped)
                {
                    navMeshAgent.isStopped = true;
                    if(enemyT != null)
                      transform.LookAt(null);
                }
                else if (!_stop)
                {
                    Resum();
                    transform.LookAt(enemyT.transform.position);
                }
            }
            else 
            { 
                transform.LookAt(enemyT.transform.position); 
            }
            navMeshAgent.destination = enemyT.transform.position;
            navSpeed = navMeshAgent.velocity.magnitude;
            _anim.SetFloat("NavSpeed", navSpeed);
        }
    }

    public void Attack(GameObject enemy)
    {
        navMeshAgent.enabled = true;
        navMeshAgent.stoppingDistance = 3f;
        enemyT = enemy;
        if (navMeshAgent.isStopped)
            Resum();
    }

    public void Magic(GameObject enemy) 
    {
        navMeshAgent.enabled = true;
        navMeshAgent.stoppingDistance = 8f;
        enemyT = enemy;
        if (navMeshAgent.isStopped)
            Resum();
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
            Stop();
        }
        else
        {
            navMeshAgent.velocity = stopvelo;
            _stop = false;
            Resum();
        }
    }

    private void Stop()
    {
        if (navMeshAgent.enabled)
            navMeshAgent.isStopped = true;
    }

    private void Resum()
    {
        if (navMeshAgent.enabled)
            navMeshAgent.isStopped = false;
    }

    public void TargetCancel()
    {
        if (_nonPlay) 
        {
            enemyT = chp.charaList[chp.nowChara];
        }
        else
        navMeshAgent.enabled = false;
    }

    public void PlayNow() 
    {
        _nonPlay = false;
        navMeshAgent.enabled = false;
        navSpeed = 0;
        _anim.SetFloat("NavSpeed", navSpeed);
    }

    public void NOPlay()
    {
        _nonPlay = true;
        navMeshAgent.enabled = true;
        navMeshAgent.stoppingDistance = 3f;
        enemyT = chp.charaList[chp.nowChara];
    }
}
