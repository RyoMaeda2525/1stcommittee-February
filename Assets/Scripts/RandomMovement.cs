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
    [Tooltip("プレイヤーが取得する経験値"), SerializeField]
    int exp = 0;
    [SerializeField]
    Golem go = default;

    [Tooltip("振り向くスピード"), SerializeField]
    float speed = 0;

    Animator _anim = default;
    [SerializeField]
    PauseMenuController _pauseMenu = default; //停止するために必要
    Vector3 stopvelo = default; //停止する前の速度

    bool _stop = false;
    internal bool _attack = true;
    bool _start = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetDestination();
        startPosition = this.transform.position;
        _anim = GetComponent<Animator>();
        if(gameObject.tag == "Boss") 
        {
            navMeshAgent.isStopped = true;
            _stop = true;
        }
    }

    private void FixedUpdate()
    {
        if (!_stop)
        {
            var diff = navMeshAgent.destination - transform.position;
            timeCount += Time.deltaTime;
            if (timeCount > selectInterval)
            {
                _attack = true;
                timeCount = 0;
                if (gameObject.tag == "Enemy")
                SetDestination();
            }
            if (gameObject.tag == "Enemy") 
            {
                var axis = Vector3.Cross(this.transform.forward, diff).y < 0 ? -1 : 1;
                var angle = Vector3.Angle(this.transform.forward, diff);
                var pov = angle * axis;
                _anim.SetFloat("angle", pov);
                _anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            }
            else 
            {
                // Quaternion(回転値)を取得
                Quaternion quaternion = Quaternion.LookRotation(diff);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, quaternion, Time.deltaTime * speed);
                _anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
                var axis = Vector3.Cross(this.transform.forward, diff).y < 0 ? -1 : 1;
                var angle = Vector3.Angle(this.transform.forward, diff);
                var pov = angle * axis;
                _anim.SetFloat("angle", pov);
            }

            if (Vector3.Distance(this.transform.position, startPosition) > ActionDistance)
            {
                _attack = false;
                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        Vector3 randomPos = default;
        if (gameObject.tag != "Boss")
        {
            randomPos = new Vector3(Random.Range(startPosition.x - Actionradius, startPosition.x + Actionradius), 0,
                                                            Random.Range(startPosition.z - Actionradius, startPosition.z + Actionradius));
            NavMesh.SamplePosition(randomPos, out navMeshHit, 10, 1);
            navMeshAgent.destination = navMeshHit.position;
        }
        else 
        {
            navMeshAgent.destination = new Vector3(startPosition.x, 0, startPosition.z);
        }
        
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
        }
        else
        {
            Resum();
        }
    }

    void Pause() //停止処理
    {
        stopvelo = navMeshAgent.velocity;
        navMeshAgent.velocity = Vector3.zero;
        _anim.enabled = false;
        if (gameObject.tag == "Enemy")
        {
            navMeshAgent.isStopped = true;
            _stop = true;
        }
        else if (_start)
        {
            navMeshAgent.isStopped = true;
            _stop = false;
        }
    }

    void Resum() //再開
    {
        navMeshAgent.velocity = stopvelo;
        _anim.enabled = true;
        if(gameObject.tag == "Enemy") 
        {
            navMeshAgent.isStopped = false;
            _stop = false;
        }
        else if (_start)
        {
            navMeshAgent.isStopped = false;
            _stop = false;
        }
    }

    public void StartBoss() 
    {
    
    }

    public void Dead() 
    {
        _stop = true;
    }

    private void OnDestroy()
    {
        FindObjectOfType<GameManager>().GetExp(exp);
    }
}
