using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class OpenMenuScene : MonoBehaviour
{
    private VRManager vrManager; // Añadir referencia al VRManager
    void Start()
    {
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Screen.orientation = ScreenOrientation.Portrait;
        // Encontrar el VRManager en la escena
        vrManager = FindObjectOfType<VRManager>();
        if (vrManager == null)
        {
            Debug.LogError("VRManager not found in the scene.");
            return;
        }
    }
    public void OpenMenu()
    {
        SceneManager.LoadScene("Menú");
    }

    public void OpenDevices()
    {
        SceneManager.LoadScene("Dispositivos");
    }
}
