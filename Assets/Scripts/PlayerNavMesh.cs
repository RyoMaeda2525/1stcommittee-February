﻿using System.Collections;
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
        Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (navMeshAgent.enabled)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 && !navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = true;
            }
            else if (!_stop)
            {
                Resum();
            }
                
            navMeshAgent.destination = enemyT.transform.position;
            Debug.Log(enemyT);
        }
            _anim.SetFloat("NavSpeed", navMeshAgent.velocity.magnitude);
    }

    public void Enemydiscover(GameObject enemy)
    {
        navMeshAgent.enabled = true;
        enemyT = enemy;
        if(navMeshAgent.isStopped)
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
       if(navMeshAgent.enabled)
        navMeshAgent.isStopped = false;
    }

    public void TargetCancel()
    {
        navMeshAgent.enabled = false;
    }
}
