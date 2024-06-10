using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public enum Theme { Dark, Light }
    public Theme currentTheme = Theme.Dark;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadTheme(Theme theme)
    {
        currentTheme = theme;

        // Notify all theme listeners to update their themes
        ThemeListener[] listeners = FindObjectsOfType<ThemeListener>();
        foreach (ThemeListener listener in listeners)
        {
            listener.UpdateTheme(theme);
        }
    }

    public void ChangeTheme()
    {
        currentTheme = (currentTheme == Theme.Dark) ? Theme.Light : Theme.Dark;
        LoadTheme(currentTheme);
    }
}
