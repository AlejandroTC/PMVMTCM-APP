using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using UnityEngine.UI;
public class VRManager : MonoBehaviour
{
    private bool isVRActive = true;
    public Image VRMode;
    public void OffVR()
    {
        StartCoroutine(StopXR());
    }
    public void ToggleVR()
    {
        if (isVRActive)
        {
            StartCoroutine(StopXR());
        }
        else
        {
            StartCoroutine(StartXR());
        }
    }

    IEnumerator StartXR()
    {
        Debug.Log("Starting XR...");

        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            isVRActive = true;
        }
    }

    IEnumerator StopXR()
    {
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        isVRActive = false;

        yield return null;
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("SampleScene");
    }
}