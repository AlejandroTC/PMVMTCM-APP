using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string receivedData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener este objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
