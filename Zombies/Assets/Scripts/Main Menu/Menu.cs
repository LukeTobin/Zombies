using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool menuOpen;

    public void Open(bool openBool = true)
    {
        menuOpen = openBool;
        gameObject.SetActive(openBool);
    }
}
