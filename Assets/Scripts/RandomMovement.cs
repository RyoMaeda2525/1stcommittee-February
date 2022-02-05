using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private float timeCount;
    [Tooltip("行動範囲の中心点"), SerializeField]
    public Vector3 startPosition = default;
    [Tooltip("行動範囲の半径"), SerializeField]
    public float Actionradius = default;
    [Tooltip("行動の制限距離"), SerializeField]
    public float ActionDistance = 20f;
    private NavMeshAgent navMeshAgent;
    private NavMeshHit navMeshHit;
    [Tooltip("次の地点を選ぶまでの時間"), SerializeField]
    public float selectInterval = 10;
    Animator _anim = default;
    bool _stop = false;
    bool _playerAttack = true; 
    PauseMenuController _pauseMenu = default; //停止するために必要
    Vector3 stopvelo = default; //停止する前の速度
    [Tooltip("攻撃力"), SerializeField]
    public int Atk = 0;　
    float interval = 3f; //攻撃までの時間計測
    [Tooltip("攻撃間隔"), SerializeField]
    float attackInterval = 5;
    [Tooltip("クリティカル値"), SerializeField]
    public float critical = 0f;　

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetDestination();
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        startPosition = this.transform.position;
        _anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!_stop)
        {
            timeCount += Time.deltaTime;
            if (timeCount > selectInterval && !_stop)
            {
                _playerAttack = true;
                SetDestination();
                timeCount = 0;
            }
            var diff = navMeshAgent.destination - transform.position;
            var axis = Vector3.Cross(this.transform.forward, diff).y < 0 ? -1 : 1;
            var angle = Vector3.Angle(this.transform.forward, diff);
            var pov = angle * axis;
            _anim.SetFloat("angle", pov);
            _anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);

            if (Vector3.Distance(this.transform.position, startPosition) > 
ActionDistance && _playerAttack == true)
            {
                Debug.Log("範囲外");
                _playerAttack = false;
                interval = 2;
                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        Vector3 randomPos = new Vector3(Random.Range(startPosition.x - Actionradius, startPosition.x + Actionradius), 0, 
                                                                        Random.Range(startPosition.z - Actionradius, startPosition.z + Actionradius));
        NavMesh.SamplePosition(randomPos, out navMeshHit, 10, 1);
        navMeshAgent.destination = navMeshHit.position;
    }

    private void Awake() // この処理は Start やると遅いので Awake でやっている
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
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
            Pause();
            _stop = true;
        }
        else
        {
            Resum();
            _stop = false;
        }
    }

    void Pause() //停止処理
    {
        stopvelo = navMeshAgent.velocity;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
        _anim.enabled = false;
    }

    void Resum() //再開
    {
        navMeshAgent.velocity = stopvelo;
        navMeshAgent.isStopped = false;
        _anim.enabled = true;
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && _playerAttack)
        {
            navMeshAgent.destination = collision.transform.position;
            transform.LookAt(collision.transform.position);
            interval += Time.deltaTime;
            if (interval > attackInterval)
            {
                interval = 0;
                _anim.Play("Attack");
                collision.GetComponent<Damage>().HitAttack(Atk, critical);          
            }

        }
    }
}
