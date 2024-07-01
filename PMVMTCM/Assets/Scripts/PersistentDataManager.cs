using UnityEngine;
using System;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    private string receivedData;

    // Evento para notificar cambios en los datos recibidos
    public event Action<string> OnDataReceived;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener este objeto entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruir la instancia duplicada
        }
    }

    public void SetReceivedData(string data)
    {
        receivedData = data;
        Debug.Log("Datos recibidos: " + data);
        OnDataReceived?.Invoke(data); // Notificar a todos los suscriptores
    }

    public string GetReceivedData()
    {
        return receivedData;
    }
}
