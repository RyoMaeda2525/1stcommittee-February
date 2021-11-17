using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
	private float timeCount;
	[SerializeField]
	private float colliderSize = 20;
	private GameObject colliderObject;
	Vector3 course = default;
	public float speed = 3;
	public float wanderRange;
	private NavMeshAgent navMeshAgent;
	private NavMeshHit navMeshHit;
	public float selectInterval = 5;
	Transform targettransform;
	Rigidbody _rb = default;
	Animator _anim = default;
	bool _stop = false;
	PauseMenuController _pauseMenu = default;   //一時停止の命令を取得する
	Vector3 stopvelo = default;

	void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = transform.position;
		SetDestination();
		navMeshAgent.avoidancePriority = Random.Range(0, 100);
        wanderRange = base.transform.position.x;
		_rb = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
	}

	//void RandomWander()
	//{
	//	//指定した目的地に障害物があるかどうか、そもそも到達可能なのかを確認して問題なければセットする。
	//	//pathPending 経路探索の準備できているかどうか
	//	if (!navMeshAgent.pathPending)
	//	{
	//		if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
	//		{
	//			//hasPath エージェントが経路を持っているかどうか
	//			//navMeshAgent.velocity.sqrMagnitudeはスピード
	//			if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
	//			{
	//				SetDestination();
	//			}
	//		}
	//	}
	//}

    private void FixedUpdate()
    {
		timeCount += Time.deltaTime;
		if(timeCount > selectInterval && !_stop)
        {
			SetDestination();
			timeCount = 0;
        }
		var diff = navMeshAgent.destination - transform.position ;
		var axis = Vector3.Cross(this.transform.forward, diff).y < 0 ? -1 : 1;
		var angle = Vector3.Angle(this.transform.forward , diff) ; 
		var pov = angle * axis;
		_anim.SetFloat("angle", pov);
		//targettransform.position = transform.position;
		//Vector3 walkSpeed = _rb.velocity;
		//walkSpeed.y = 0;
		_anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
		Debug.Log(navMeshAgent.velocity.magnitude);
		//_anim.SetFloat("Speedx", x);
		//_anim.SetFloat("Speedy", y);
	}

    private void SetDestination()
	{
		Vector3 randomPos = new Vector3(Random.Range(wanderRange - 20, wanderRange), 0, Random.Range(wanderRange - 20, wanderRange));
		NavMesh.SamplePosition(randomPos, out navMeshHit, 10, 1);
		navMeshAgent.destination = navMeshHit.position;
	}

    public Vector3 Destination()
    {
		Vector3 p = navMeshAgent.destination;
		return p;
    }

	private void Awake() // この処理は Start やると遅いので Awake でやっている
	{
		_pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
	}

	private void OnEnable() //ゲームに入ると加わる
	{
		_pauseMenu.onCommandMenu += PauseCommand;
		//_pauseMenu.offCommandMenu += ResumCommand;
	}

	private void OnDisable() //消えると抜ける
	{
		_pauseMenu.onCommandMenu -= PauseCommand;
		//_pauseMenu.offCommandMenu -= ResumCommand;
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

	//void ResumCommand(bool onResum)
	//{
	//    if (onResum && pauseresum)
	//    {
	//        Debug.Log("a");
	//        Resum();
	//        pauseresum = false;
	//    }
	//}

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

}
