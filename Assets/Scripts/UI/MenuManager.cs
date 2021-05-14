using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Menu[] menus;

    private void Awake()
    {
        menus = GetComponentsInChildren<Menu>();

        CloseAllMenus();
    }

    public void CloseAllMenus()
    {
        foreach (Menu menu in GetComponentsInChildren<Menu>()) 
            menu.Close();
    }

    public void OpenMenu(Menu menu)
    {
        CloseAllMenus();

        menu.Open();
    }
}
