﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WarningDialog : MonoBehaviour
{
    public ProfileSelector selector;
    public Text text;

    public virtual void Call(string message)
    {
        text.text = message;
        this.gameObject.SetActive(true);
    }
    public virtual void OK()
    {
        gameObject.SetActive(false);
        selector.gameObject.SetActive(true);
    }
}
