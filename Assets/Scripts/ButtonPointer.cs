﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPointer : MonoBehaviour
{
    [SerializeField]
    int eventX = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Vector3 eventPosition = EventSystem.current.currentSelectedGameObject.transform.position;
            eventPosition.x -= eventX;
            this.transform.position = eventPosition;
        }

    }
}
