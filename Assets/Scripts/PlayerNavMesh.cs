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
    public GameObject enemyT = default;
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
        Debug.Log(gameObject.name);
        enemyT = chp.charaList[0];
        Stop();
    }

    //void Start()
    //{
    //    _anim = GetComponent<Animator>();
    //    navMeshAgent = GetComponent<NavMeshAgent>();
    //    Debug.Log(gameObject.name);
    //    enemyT = chp.charaList[0];
    //    Stop();
    //}

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
                }
                else if (!_stop)
                {
                    Resum();
                }
            }
            navMeshAgent.destination = enemyT.transform.position;
            navSpeed = navMeshAgent.velocity.magnitude;
            _anim.SetFloat("NavSpeed", navSpeed);
        }
    }

    public void Enemydiscover(GameObject enemy)
    {
        navMeshAgent.enabled = true;
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
        Debug.Log(this.gameObject.name);
        enemyT = chp.charaList[chp.nowChara];
    }
}
