using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Damage : MonoBehaviour
{
    [Tooltip("プレイヤーのステータスをまとめたシート"), SerializeField]
    private PlayerStatusSheet pss = default;
    [Tooltip("プレイヤーキャラのリスト番号"), SerializeField]
    private int charaNumber = 0;
    [SerializeField]
    private ChangePlayer cP = default;

    [Tooltip("最大体力"), SerializeField]
    private int hitPointMax = 100;
    [Tooltip("現在の体力"), SerializeField]
    internal int hitPoint = 100;
    [Tooltip("物理防御力"), SerializeField]
    private int defense = 40;
    [Tooltip("魔法防御力"), SerializeField]
    private int magicDefense = 40;

    int hit = 0;
    Tweener tweener = default;
    int tempHp = 0;
    PauseMenuController _pauseMenu = default;
    private float hpChangeInterval = 1.5f;
    [SerializeField] public Text maxHpText = default;
    [SerializeField] public Text hpText = default;
    [SerializeField] private Text damageText = default;
    [SerializeField] private GameObject canvas;//親にするキャンバスを格納
    [SerializeField] GameObject pivot = default;
    [SerializeField] Slider hpSlider = default;

    private void Awake()
    {
        _pauseMenu = GameObject.FindObjectOfType<PauseMenuController>();
    }

    private void Start()
    {
        hitPoint = System.Math.Min(hitPoint, hitPointMax);
        HpMaxValue();
        ChangeValue();
    }

    public void HitAttack(int damege, string type ,GameObject player) //攻撃を受け取る
    {
        if (type == "Attack") //通常攻撃
        {
            hit = damege / 2 - defense / 4;
            hitPoint -= hit;
            hitPoint = System.Math.Max(hitPoint, 0);
        }
        else if (type == "Magic")
        {
            hit = damege / 2 - magicDefense / 4;
            hitPoint -= hit;
            hitPoint = System.Math.Max(hitPoint, 0);
        }
        if (hitPoint <= 0)
        {
            if (gameObject.tag != "Player")
            {
                ChangePlayer cP = FindObjectOfType<ChangePlayer>();
                for (int i = 0; i < 3; i++) 
                {
                    if (cP.charaList[i].activeSelf) 
                    {
                        cP.charaList[i].GetComponentInChildren<EnemyChecker>().Destroy(this.gameObject);
                    }
                }     
            }
            else 
            {
                GetComponent<ActionSlider>().Pause();
            }
            GetComponent<Animator>().Play("Deth");
        }
        ChangeValue();
        Text _text = Instantiate(damageText, pivot.transform.position - Camera.main.transform.forward * 0.2f, Quaternion.identity);
        _text.text = hit.ToString();
        _text.transform.SetParent(canvas.transform);
        if(GetComponent<FoxStatus>() && GetComponent<FoxStatus>().playerList[0] == null) 
        {
            GetComponent<FoxStatus>().playerList.Remove(GetComponent<FoxStatus>().playerList[0]);
            GetComponent<FoxStatus>().playerList.Add(GetComponent<FoxStatus>().playerList[0]);
        }
    }

    // Update is called once per frame
    internal void Heal(int heal) //回復処理
    {
        if (hitPoint <= 0)
        {
            hitPoint += heal;
            hitPoint = System.Math.Min(hitPoint, hitPointMax);
            ChangeValue();
        }
    }

    private void Deth()
    {
        if (gameObject.tag != "Player")
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }       
    }

    private void FixedUpdate()
    {
        //体力ゲージなどをカメラに向ける
        canvas.transform.LookAt(Camera.main.transform.position);
    }

    public void ChangeValue()
    {
        if (this.gameObject.tag == "Player")
        {
            DOTween.To(() => tempHp, // 連続的に変化させる対象の値
            x => tempHp = x, // 変化させた値 x をどう処理するかを書く
            hitPoint, // x をどの値まで変化させるか指示する
            hpChangeInterval)  // 何秒かけて変化させるか指示する
            .OnUpdate(() => hpText.text = tempHp.ToString("000"));   // 数値が変化する度に実行する処理を書く
        }
        tweener = DOTween.To(() => hpSlider.value, // 連続的に変化させる対象の値
        x => hpSlider.value = x, // 変化させた値 x をどう処理するかを書く
        hitPoint, // x をどの値まで変化させるか指示する
        hpChangeInterval)/*.OnComplete(() => Debug.Log("完了"))*/;// 何秒かけて変化させるか指示す*/
    }

    private void OnEnable() //ゲームに入ると加わる
    {
        _pauseMenu.onCommandMenu += PauseCommand;
    }

    private void OnDisable() //消えると抜ける
    {
        cP.ChangeCharacter("Deth");
        _pauseMenu.onCommandMenu -= PauseCommand;
    }

    void PauseCommand(bool onPause)
    {
        if (onPause)
        {
            tweener.Pause();
        }
        else
        {
            tweener.Play();
        }
    }

    public void MaxHPText()
    {
        if (gameObject.tag == "Player")
            maxHpText.text = hitPointMax.ToString();
    }

    public void HpMaxValue()
    {
        hpSlider.maxValue = hitPointMax;
    }

    internal void LevelUP(int level)
    {
        hitPointMax = pss.sheets[charaNumber].list[level].Hp;
        defense = pss.sheets[charaNumber].list[level].Defense;
        magicDefense = pss.sheets[charaNumber].list[level].MagicDefense;
        tempHp = hitPointMax;
        MaxHPText();
        HpMaxValue();
    }
}
