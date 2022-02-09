using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityChan
{
    // 必要なコンポーネントの列記
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]

    public class UnityChanControlScriptWithRgidBody : MonoBehaviour
    {
        [Tooltip("プレイヤーのNavmeshを切り替えるスクリプト"), SerializeField]
        private PlayerNavMesh _nav = default;
        PauseMenuController _pauseMenu = default;   //一時停止の命令を取得する
        Vector3 stopvelo = default;                 //停止する直前の速度を取得する         
        public bool pauseresum = false;             //停止した際に動かないようにするため
        public List<GameObject> enemyList = new List<GameObject>();
        public bool _nonPlayerCharacter = true;
        Vector3 walkSpeed = default;

        // 以下キャラクターコントローラ用パラメタ
        // 前進速度
        public float forwardSpeed = 7.0f;
        // 後退速度
        public float backwardSpeed = 2.0f;
        // 旋回速度
        public float rotateSpeed = 2.0f;
        private Rigidbody _rb;
        private Animator _anim;

        void Start()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _nav = GetComponent<PlayerNavMesh>();
        }


        // 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
        void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
            float v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義

            if (!pauseresum && !_nonPlayerCharacter)
            {

                GameObject.FindObjectOfType<PauseMenuController>().GameBack();

                Vector3 dir = Vector3.forward * v + Vector3.right * h;

                if (dir == Vector3.zero)
                {
                    _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);// 方向の入力がニュートラルの時は、y 軸方向の速度を保持する
                }
                else
                {
                    //anim.SetBool("run", true);
                    // カメラを基準に入力が上下=奥/手前, 左右=左右にキャラクターを向ける
                    dir = Camera.main.transform.TransformDirection(dir);    // メインカメラを基準に入力方向のベクトルを変換する
                    dir.y = 0;  // y 軸方向はゼロにして水平方向のベクトルにする
                                // 入力方向に滑らかに回転させる
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

                    Vector3 velo = dir.normalized * forwardSpeed; // 入力した方向に移動する
                    _rb.velocity = velo;   // 計算した速度ベクトルをセットする
                }
                walkSpeed = _rb.velocity;
                walkSpeed.y = 0;
                _anim.SetFloat("Speed", walkSpeed.magnitude);
            }
            else if (!pauseresum) 
            {
                _rb.velocity = Vector3.zero;
            }
        }

        private void Awake()　// この処理は Start やると遅いので Awake でやっている
        {
            _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
        }

        private void OnEnable()　//ゲームに入ると加わる
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
                pauseresum = true;
            }
            else
            {
                Resum();
                pauseresum = false;
            }
        }

        void Pause() //停止処理
        {
            stopvelo = _rb.velocity;
            _rb.velocity = Vector3.zero;
            _anim.speed = 0;

        }

        void Resum() //再開
        {
            _rb.velocity = stopvelo;
            _anim.speed = 1;
        }
        public void PlayCharacterNow()
        {
            _nonPlayerCharacter = false;
            _nav.PlayNow();
        }

        public void NoPlay()
        {
            _nonPlayerCharacter = true;
            walkSpeed = Vector3.zero;
            _anim.SetFloat("Speed", walkSpeed.magnitude);
            _nav.NOPlay();
        }
    }
}