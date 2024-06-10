using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeListener : MonoBehaviour
{
    // Scene 1
    public TMP_Text headerText;

    public Button practicesButton;
    public Image practicesIcon;
    public TMP_Text practicesText;

    public Button devicesButton;
    public Image devicesIcon;
    public TMP_Text devicesText;

    public Image themeMode;

    public Image panel;

    // Scene 2
    public Image tipBackground;
    public Image tipIcon;
    public TMP_Text tipHint;

    public Button pairedDevicesButton;
    public TMP_Text pairedDevicesText;

    public Button connectButton;
    public TMP_Text connectButtonText;

    public Button disconnectButton;
    public TMP_Text disconnectButtonText;

    public TMP_Text deviceAddressInputPlaceholder;
    public TMP_Text deviceAddressInputText;

    // Scene 3
    public Image cardPrefabPressed;
    public TMP_Text practiceTitle;
    public Image practiceImage; // Assuming this image doesn't change based on theme
    public TMP_Text practiceDescription;
    public TMP_Text author;

    public TMP_Text dataReceivedText;

    private void Start()
    {
        UpdateTheme(ThemeManager.Instance.currentTheme);
    }

    public void UpdateTheme(ThemeManager.Theme theme)
    {
        string themeFolder = "Images/" + theme.ToString();

        // Scene 1
        if (headerText != null)
            headerText.color = (theme == ThemeManager.Theme.Dark) ? Color.white : Color.black;

        if (practicesButton != null)
            practicesButton.image.sprite = Resources.Load<Sprite>(themeFolder + "/Button-" + theme.ToString());
        if (practicesIcon != null)
            practicesIcon.sprite = Resources.Load<Sprite>(themeFolder + "/Practices_" + theme.ToString());
        if (practicesText != null)
            practicesText.color = (theme == ThemeManager.Theme.Dark) ? new Color(7 / 255f, 0 / 255f, 171 / 255f) : Color.white;

        if (devicesButton != null)
            devicesButton.image.sprite = Resources.Load<Sprite>(themeFolder + "/Button-" + theme.ToString());
        if (devicesIcon != null)
            devicesIcon.sprite = Resources.Load<Sprite>(themeFolder + "/Devices_" + theme.ToString());
        if (devicesText != null)
            devicesText.color = (theme == ThemeManager.Theme.Dark) ? new Color(7 / 255f, 0 / 255f, 171 / 255f) : Color.white;

        if (themeMode != null)
            themeMode.sprite = Resources.Load<Sprite>(themeFolder + "/" + theme.ToString() + "_Enabled");

        if (panel != null)
            panel.color = (theme == ThemeManager.Theme.Dark) ? new Color(43 / 255f, 43 / 255f, 43 / 255f) : Color.white;

        // Scene 2
        if (tipBackground != null)
            tipBackground.sprite = Resources.Load<Sprite>(themeFolder + "/TipBackground_" + theme.ToString());
        if (tipIcon != null)
            tipIcon.sprite = Resources.Load<Sprite>(themeFolder + "/Bulb_" + theme.ToString());
        if (tipHint != null)
            tipHint.color = (theme == ThemeManager.Theme.Dark) ? new Color(94 / 255f, 0 / 255f, 81 / 255f) : Color.white;

        if (pairedDevicesButton != null)
            pairedDevicesButton.image.sprite = Resources.Load<Sprite>(themeFolder + "/Button-" + theme.ToString());
        if (pairedDevicesText != null)
            pairedDevicesText.color = (theme == ThemeManager.Theme.Dark) ? new Color(7 / 255f, 0 / 255f, 171 / 255f) : Color.white;

        if (connectButton != null)
            connectButton.image.sprite = Resources.Load<Sprite>(themeFolder + "/Button-" + theme.ToString());
        if (connectButtonText != null)
            connectButtonText.color = (theme == ThemeManager.Theme.Dark) ? new Color(7 / 255f, 0 / 255f, 171 / 255f) : Color.white;

        if (disconnectButton != null)
            disconnectButton.image.sprite = Resources.Load<Sprite>(themeFolder + "/Button-" + theme.ToString());
        if (disconnectButtonText != null)
            disconnectButtonText.color = (theme == ThemeManager.Theme.Dark) ? new Color(7 / 255f, 0 / 255f, 171 / 255f) : Color.white;

        if (deviceAddressInputPlaceholder != null)
            deviceAddressInputPlaceholder.color = (theme == ThemeManager.Theme.Dark) ? new Color(191 / 255f, 191 / 255f, 191 / 255f) : new Color(43 / 255f, 43 / 255f, 43 / 255f);

        if (deviceAddressInputText != null)
            deviceAddressInputText.color = (theme == ThemeManager.Theme.Dark) ? Color.white : Color.black;

        if (dataReceivedText != null)
            dataReceivedText.color = (theme == ThemeManager.Theme.Dark) ? Color.black : Color.white;


        // Scene 3

        if (cardPrefabPressed != null)
            cardPrefabPressed.sprite = Resources.Load<Sprite>(themeFolder + "/CardBackground_" + theme.ToString());

        if (practiceTitle != null)
            practiceTitle.color = (theme == ThemeManager.Theme.Dark) ? new Color(69 / 255f, 0 / 255f, 134 / 255f) : Color.white;

        if (practiceDescription != null)
            practiceDescription.color = (theme == ThemeManager.Theme.Dark) ? Color.black : Color.white;
        if (author != null)
            author.color = (theme == ThemeManager.Theme.Dark) ? Color.black : Color.white;

    }
}
