using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class AttackButton : MonoBehaviour
{
    [SerializeField] Text enemyNameText = default;
    LineRenderer lineR = default;
    EventSystem eventS = default;
    GameObject unityChan = default;
    ActionSlider acs = default;
    string enemyName = "None";
    RandomMovement rm = default;
    PauseMenuController pauseM = default;
    PlayerNavMesh plaNav = default;

    private void Awake()
    {
        lineR = GetComponent<LineRenderer>();
        plaNav = GameObject.FindObjectOfType<PlayerNavMesh>();
        acs = GameObject.FindObjectOfType<ActionSlider>();
        pauseM = GameObject.FindObjectOfType<PauseMenuController>();
        eventS = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        unityChan = GameObject.Find("Unitychan");
        lineR.SetPosition(0, unityChan.transform.position);
    }

    // Start is called before the first frame update
    public void EnemySet(GameObject enemy)
    {
        Button b = GetComponent<Button>();

        enemyName = enemy.name;
        enemyNameText.text = enemyName;
        b?.OnSelectAsObservable()
            .Subscribe(_ => lineR.SetPosition(1, enemy.transform.position));

        b?.OnClickAsObservable()
            .Subscribe(_ => Attack(enemy));
    }

    // Update is called once per frame
    void Update()
    {
        if (eventS.currentSelectedGameObject == this.gameObject)
        {
            lineR.enabled = true;
        }
        else
        {
            lineR.enabled = false;
        }

    }

    void Attack(GameObject enemy) 
    {
        plaNav.Enemydiscover(enemy);
        pauseM.OnAttack();
        acs.ChangeValue(enemy);
    }

}
