//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
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

        public float animSpeed = 1.5f;              // アニメーション再生速度設定
        public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
        public bool useCurves = true;               // Mecanimでカーブ調整を使うか設定する
                                                    // このスイッチが入っていないとカーブは使われない
        public float useCurvesHeight = 0.5f;        // カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）
        PauseMenuController _pauseMenu = default;   //一時停止の命令を取得する
        Vector3 stopvelo = default;                 //停止する直前の速度を取得する         
        public bool pauseresum = false;             //停止した際に動かないようにするため
        public List<GameObject> enemyList = new List<GameObject>();

        // 以下キャラクターコントローラ用パラメタ
        // 前進速度
        public float forwardSpeed = 7.0f;
        // 後退速度
        public float backwardSpeed = 2.0f;
        // 旋回速度
        public float rotateSpeed = 2.0f;
        // ジャンプ威力
        public float jumpPower = 3.0f;
        // キャラクターコントローラ（カプセルコライダ）の参照
        private CapsuleCollider col;
        private Rigidbody _rb;
        // キャラクターコントローラ（カプセルコライダ）の移動量
        private Vector3 velocity;
        // CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
        private float orgColHight;
        private Vector3 orgVectColCenter;
        private Animator _anim;                          // キャラにアタッチされるアニメーターへの参照
        private AnimatorStateInfo currentBaseState;         // base layerで使われる、アニメーターの現在の状態の参照

        private GameObject cameraObject;    // メインカメラへの参照

        // アニメーター各ステートへの参照
        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int restState = Animator.StringToHash("Base Layer.Rest");

        // 初期化
        void Start()
        {
            // Animatorコンポーネントを取得する
            _anim = GetComponent<Animator>();
            // CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
            col = GetComponent<CapsuleCollider>();
            _rb = GetComponent<Rigidbody>();
            //メインカメラを取得する
            cameraObject = GameObject.FindWithTag("MainCamera");
            // CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
            orgColHight = col.height;
            orgVectColCenter = col.center;
        }


        // 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
        void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
            float v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義
            //anim.SetFloat("Speed", v);                          // Animator側で設定している"Speed"パラメタにvを渡す
            //anim.SetFloat("Direction", h);                      // Animator側で設定している"Direction"パラメタにhを渡す
            _anim.speed = animSpeed;                             // Animatorのモーション再生速度に animSpeedを設定する
            currentBaseState = _anim.GetCurrentAnimatorStateInfo(0); // 参照用のステート変数にBase Layer (0)の現在のステートを設定する
            _rb.useGravity = true;//ジャンプ中に重力を切るので、それ以外は重力の影響を受けるようにする
            Vector3 walkSpeed = _rb.velocity;
            walkSpeed.y = 0;
            _anim.SetFloat("Speed", walkSpeed.magnitude);

            if (!pauseresum)
            {
                //// 以下、キャラクターの移動処理
                //velocity = new Vector3(0, 0, v);        // 上下のキー入力からZ軸方向の移動量を取得
                //                                        // キャラクターのローカル空間での方向に変換
                //velocity = transform.TransformDirection(velocity);
                ////以下のvの閾値は、Mecanim側のトランジションと一緒に調整する
                //if (v > 0.1)
                //{
                //    velocity *= forwardSpeed;       // 移動速度を掛ける
                //}
                //else if (v < -0.1)
                //{
                //    velocity *= backwardSpeed;  // 移動速度を掛ける
                //}
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
                    velo.y = _rb.velocity.y;   // ジャンプした時の y 軸方向の速度を保持する
                    _rb.velocity = velo;   // 計算した速度ベクトルをセットする
                }

                //if (Input.GetButtonDown("Jump"))
                //{   // スペースキーを入力したら

                //    //アニメーションのステートがLocomotionの最中のみジャンプできる
                //    if (currentBaseState.nameHash == locoState)
                //    {
                //        //ステート遷移中でなかったらジャンプできる
                //        if (!anim.IsInTransition(0))
                //        {
                //            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                //            anim.SetBool("Jump", true);     // Animatorにジャンプに切り替えるフラグを送る
                //        }
                //    }
                //}


                //// 上下のキー入力でキャラクターを移動させる
                //transform.localPosition += velocity * Time.fixedDeltaTime;

                //// 左右のキー入力でキャラクタをY軸で旋回させる
                //transform.Rotate(0, h * rotateSpeed, 0);


                // 以下、Animatorの各ステート中での処理
                // Locomotion中
                // 現在のベースレイヤーがlocoStateの時
                if (currentBaseState.nameHash == locoState)
                {
                    //カーブでコライダ調整をしている時は、念のためにリセットする
                    if (useCurves)
                    {
                        resetCollider();
                    }
                }
                // JUMP中の処理
                // 現在のベースレイヤーがjumpStateの時
                else if (currentBaseState.nameHash == jumpState)
                {
                    cameraObject.SendMessage("setCameraPositionJumpView");  // ジャンプ中のカメラに変更
                                                                            // ステートがトランジション中でない場合
                    if (!_anim.IsInTransition(0))
                    {

                        // 以下、カーブ調整をする場合の処理
                        if (useCurves)
                        {
                            // 以下JUMP00アニメーションについているカーブJumpHeightとGravityControl
                            // JumpHeight:JUMP00でのジャンプの高さ（0〜1）
                            // GravityControl:1⇒ジャンプ中（重力無効）、0⇒重力有効
                            float jumpHeight = _anim.GetFloat("JumpHeight");
                            float gravityControl = _anim.GetFloat("GravityControl");
                            if (gravityControl > 0)
                                _rb.useGravity = false;  //ジャンプ中の重力の影響を切る

                            // レイキャストをキャラクターのセンターから落とす
                            Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                            RaycastHit hitInfo = new RaycastHit();
                            // 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
                            if (Physics.Raycast(ray, out hitInfo))
                            {
                                if (hitInfo.distance > useCurvesHeight)
                                {
                                    col.height = orgColHight - jumpHeight;          // 調整されたコライダーの高さ
                                    float adjCenterY = orgVectColCenter.y + jumpHeight;
                                    col.center = new Vector3(0, adjCenterY, 0); // 調整されたコライダーのセンター
                                }
                                else
                                {
                                    // 閾値よりも低い時には初期値に戻す（念のため）					
                                    resetCollider();
                                }
                            }
                        }
                        // Jump bool値をリセットする（ループしないようにする）				
                        _anim.SetBool("Jump", false);
                    }

                }

                // IDLE中の処理
                // 現在のベースレイヤーがidleStateの時
                else if (currentBaseState.nameHash == idleState)
                {
                    //カーブでコライダ調整をしている時は、念のためにリセットする
                    if (useCurves)
                    {
                        resetCollider();
                    }
                    // スペースキーを入力したらRest状態になる
                    if (Input.GetButtonDown("Jump"))
                    {
                        _anim.SetBool("Rest", true);
                    }
                }
                // REST中の処理
                // 現在のベースレイヤーがrestStateの時
                else if (currentBaseState.nameHash == restState)
                {
                    //cameraObject.SendMessage("setCameraPositionFrontView");		// カメラを正面に切り替える
                    // ステートが遷移中でない場合、Rest bool値をリセットする（ループしないようにする）
                    if (!_anim.IsInTransition(0))
                    {
                        _anim.SetBool("Rest", false);
                    }
                }
            }
        }

        //void OnGUI()
        //{
        //    GUI.Box(new Rect(Screen.width - 260, 10, 250, 150), "Interaction");
        //    GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
        //    GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
        //    GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
        //    GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "Hit Spase key while Stopping : Rest");
        //    GUI.Label(new Rect(Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
        //    GUI.Label(new Rect(Screen.width - 245, 130, 250, 30), "Alt : LookAt Camera");
        //}


        // キャラクターのコライダーサイズのリセット関数
        void resetCollider()
        {
            // コンポーネントのHeight、Centerの初期値を戻す
            col.height = orgColHight;
            col.center = orgVectColCenter;
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
            stopvelo = _rb.velocity;
            _rb.velocity = Vector3.zero;
            _anim.enabled = false;

        }

        void Resum() //再開
        {
            _rb.velocity = stopvelo;
            _anim.enabled = true;
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.tag == "Enemy")
        //    {
        //        enemyList.Add(other.gameObject);
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    enemyList.Remove(other.gameObject);
        //}

        //internal static List<GameObject> GetEnemy()
        //{
        //    return enemyList;
        //}

    }
}