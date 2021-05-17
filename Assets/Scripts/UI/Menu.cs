using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to define the root of the menu
/// </summary>
public class Menu : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
