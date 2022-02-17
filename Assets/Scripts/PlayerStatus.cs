using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatus : MonoBehaviour
{
    [Tooltip("プレイヤーのステータスをまとめたシート"),SerializeField]
    private PlayerStatusSheet pss = default;
    [Tooltip("プレイヤーキャラのリスト番号"), SerializeField]
    private int charaNumber = 0; 

    [Tooltip("現在のレベル"), SerializeField]
    private int level = 1;
    [Tooltip("物理攻撃力"), SerializeField]
    private int atk = 20;
    [Tooltip("魔法攻撃力"), SerializeField]
    private int magicAtk = 20;
    [Tooltip("マジックポイントの最大値"), SerializeField]
    private int magicPointMax = 10;
    [Tooltip("魔法を使うのに必要なマジックポイント"),SerializeField]
    internal int magicPoint = 0;
    [Tooltip("総経験値"), SerializeField]
    internal int exp = 0;
    [Tooltip("次のレベルまでの経験値"),SerializeField]
    private int nextLevel = 10;

    [SerializeField]
    Text levelText = default;

    [Tooltip("魔法"), SerializeField]
    GameObject[] magicParticles = default;
    [Tooltip("マジックポイントテキスト"),SerializeField]
    private Text mpText = default;

    ActionSlider acs = default;
    NavMeshAgent nav = default;
    Rigidbody rb = default;
    IEnumerator _enumerator = default;

    private GameObject[] enemyTarget = new GameObject[1];
    [Tooltip("魔法を発動するためのMp"),SerializeField]
    internal int[] magicCost = new int[3];

    int magicNumber = 0;
    int tempMp = 0;

    bool _attack = false;
    bool _flame = false;
    bool _ice = false;
    bool _heal = false;

    private void Awake()
    {
        atk = pss.sheets[charaNumber].list[level].Atk;
        magicPointMax = pss.sheets[charaNumber].list[level].Mp;
        magicAtk = pss.sheets[charaNumber].list[level].MagicAtk;
        nextLevel = pss.sheets[charaNumber].list[level].NextLevel;
        GetComponent<Damage>().LevelUP(level);
        magicPoint = System.Math.Min(magicPoint, magicPointMax);
        tempMp = magicPoint;
        MptextValue();
    }

    // Update is called once per frame
    private void Start()
    {
        acs = GetComponent<ActionSlider>();
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
    }

    public void EnemySet(GameObject enemy, int any) //受け取った値からどの行動をするのか分ける
    {
        enemyTarget[0] = enemy;
        if (any == 0)
        {
            _flame = true;
            magicNumber = any;
        }
        else if (any == 1)
        {
            _ice = true;
            magicNumber = any;
        }
        else if (any == 2)
        {
            _heal = true;
            magicNumber = any;
        }
        else if (any == 3)
        {
            _attack = true;
            magicNumber = any;
        }
    }

    private void FixedUpdate()
    {
        //levelup時のStatus
        if (pss.sheets[charaNumber].list[level].NextLevel != 0)
        {
            if (nextLevel < exp)
            {
                level = pss.sheets[charaNumber].list[level + 1].level;
                levelText.text = level.ToString();
                magicPointMax = pss.sheets[charaNumber].list[level].Mp;
                atk = pss.sheets[charaNumber].list[level].Atk;
                magicAtk = pss.sheets[charaNumber].list[level].MagicAtk;
                nextLevel = pss.sheets[charaNumber].list[level].NextLevel;
                GetComponent<Damage>().LevelUP(level);
            }
        }
        //行動
        if (_attack)
        {
            _enumerator = Attack();
        }
        else if (_flame) 
        {
            _enumerator = Flame();
        }
        else if (_ice) 
        {
            _enumerator = Ice();
        }
        else if (_heal) 
        {
            _enumerator = Heal();
        }
    }

    IEnumerator Attack()
    {
        if (rb.velocity.magnitude < 0.1 && nav.velocity.magnitude < 0.1)
        {
            this.GetComponent<Animator>().Play("Attack");
        }
          return null;
    }

    IEnumerator Flame()
    {
        if (rb.velocity.magnitude < 0.1 && nav.velocity.magnitude < 0.1)
        {
            GetComponent<Animator>().SetBool("magic", false);
            this.GetComponent<Animator>().Play("Magic");
        }
          return null;
    }

    IEnumerator Ice()
    {
        if (rb.velocity.magnitude < 0.1 && nav.velocity.magnitude < 0.1)
        {
            GetComponent<Animator>().SetBool("magic", false);
            this.GetComponent<Animator>().Play("Magic");
        }
          return null;
    }

    IEnumerator Heal()
    {
        if (rb.velocity.magnitude < 0.1 && nav.velocity.magnitude < 0.1)
        {
            GetComponent<Animator>().SetBool("magic", false);
            this.GetComponent<Animator>().Play("Magic");
        }
            return null;
    }

    public void Hit()
    {
        _attack = false;
        GetComponent<PlayerNavMesh>().TargetCancel();
        enemyTarget[0].GetComponent<Damage>().HitAttack(atk, "Attack" , this.gameObject);
    }

    public void MagicHit()
    {
        if (magicNumber == 0)
        {
            _flame = false;
            GetComponent<PlayerNavMesh>().TargetCancel();
            Instantiate(magicParticles[0], enemyTarget[0].transform.position, Quaternion.identity);
            magicPoint -= magicCost[0];
            enemyTarget[0].GetComponent<Damage>().HitAttack(magicAtk, "Magic" , this.gameObject);
        }
        else if(magicNumber == 1)
        {
            _ice = false;
            GetComponent<PlayerNavMesh>().TargetCancel();
            Instantiate(magicParticles[1], enemyTarget[0].transform.position, Quaternion.identity);
            magicPoint -= magicCost[1];
            enemyTarget[0].GetComponent<Damage>().HitAttack(magicAtk, "Magic" , this.gameObject);
        }
        else 
        {
            _heal = false;
            GetComponent<PlayerNavMesh>().TargetCancel();
            Instantiate(magicParticles[2], enemyTarget[0].transform.position, Quaternion.identity);
            magicPoint -= magicCost[2];
            enemyTarget[0].GetComponent<Damage>().Heal(100 + magicAtk);
        }
    }

    public void SliderReset()
    {
        acs.Reset();
    }

    internal void MptextValue() 
    {
        DOTween.To(() => tempMp, // 連続的に変化させる対象の値
            x => tempMp = x, // 変化させた値 x をどう処理するかを書く
            magicPoint, // x をどの値まで変化させるか指示する
            1.5f)  // 何秒かけて変化させるか指示する
            .OnUpdate(() => mpText.text = tempMp.ToString());   // 数値が変化する度に実行する処理を書く
    }

    internal void MagicHeal(int heal) 
    {
        magicPoint += heal;
        magicPoint = System.Math.Min(magicPoint, magicPointMax);
        tempMp = magicPoint;
        MptextValue();
    }
}
