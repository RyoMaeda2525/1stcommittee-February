using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MouseCursor : MonoBehaviour
{
    CinemachineFreeLook cinema = default; //シネマシーンを取得
    bool onCursor = false; //マウスカーソルを制御する
    // Start is called before the first frame update
    void Start()
    {
        cinema = GetComponent<CinemachineFreeLook>();
        OnEnable();//ゲーム開始時マウスカーソルを消す
    }

    // Update is called once per frame
    void Update()
    {   
        float y = Input.GetAxis("Mouse ScrollWheel"); //視野角をマウスホイールで変える
        if(y > 0)
        {  if(cinema.m_Lens.FieldOfView <= 99.5f)
            {
                cinema.m_Lens.FieldOfView += 1.5f;
            }
        }
        else if (y < 0)
        {
            if (cinema.m_Lens.FieldOfView >= 59.5f)
            {
                cinema.m_Lens.FieldOfView -= 1.5f;
            }
        }

    }

    void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
