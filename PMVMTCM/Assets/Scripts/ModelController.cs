using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ModelController : MonoBehaviour
{
    public GameObject model3D; // Asigna el modelo 3D en el inspector

    private float scaleFactor = 100.0f;
    private float rotationSpeed = 50.0f;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private float smoothingFactor = 0.1f;

    private Queue<float> scaleValues = new Queue<float>();
    private int scaleQueueSize = 10; // Número de valores a promediar para el escalado

    private bool canAcceptSceneChange = true;  // Permite aceptar cambios de escena
    private float sceneChangeCooldown = 2.0f;  // Tiempo de espera mínimo entre cambios de escena

    private Dictionary<int, int> sceneMapping = new Dictionary<int, int>()
    {
        { 1, 3 },   // Número 1 corresponde a la escena 3
        { 2, 4 },   // Número 2 corresponde a la escena 4
        { 3, 5 }    // Número 3 corresponde a la escena 5
    };

    void Start()
    {
        // Suscribirse al evento de datos recibidos
        PersistentDataManager.Instance.OnDataReceived += ReceiveData;

        // Obtener los datos iniciales si existen
        string data = PersistentDataManager.Instance.GetReceivedData();
        if (!string.IsNullOrEmpty(data))
        {
            ReceiveData(data);
        }

        currentRotation = model3D.transform.rotation.eulerAngles;
        targetRotation = currentRotation;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento cuando este objeto se destruya
        if (PersistentDataManager.Instance != null)
        {
            PersistentDataManager.Instance.OnDataReceived -= ReceiveData;
        }
    }

    void Update()
    {
        // Suavizar la rotación
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, smoothingFactor);
        model3D.transform.rotation = Quaternion.Euler(currentRotation);
    }

    public void ReceiveData(string data)
    {
        if (!canAcceptSceneChange)
        {
            return;  // No aceptar cambios de escena si no se puede
        }

        string[] values = data.Split(',');


        try
        {
            if (values.Length == 2)
            {
                // Datos de escalamiento: xxxx,xxxx
                float flexValue1 = float.Parse(values[0]);
                float flexValue2 = float.Parse(values[1]);
                ScaleModel(flexValue1, flexValue2);
            }
            else if (values.Length == 6)
            {
                // Datos de rotación: +-xxxx, +-xxxx, +-xxxx, +-xxxx, +-xxxx, +-xxxx
                float gx = float.Parse(values[3]);
                float gy = float.Parse(values[4]);
                float gz = float.Parse(values[5]);
                SetTargetRotation(gx, gy, gz);
            }
            else if (values.Length == 1)
            {
                // Datos de cambio de escena: 1 a 3 (ejemplo)
                int sceneAction = int.Parse(values[0]);
                if (sceneMapping.ContainsKey(sceneAction))
                {
                    int targetSceneIndex = sceneMapping[sceneAction];
                    LoadScene(targetSceneIndex);
                }
                else
                {
                    Debug.LogWarning("Invalid scene action received");
                }

                // Activar el cooldown para aceptar nuevos cambios de escena
                canAcceptSceneChange = false;
                Invoke("ResetSceneChangeCooldown", sceneChangeCooldown);
            }
            else
            {
                Debug.LogWarning("Data format not recognized");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing data: " + e.Message);
        }
    }

    void ScaleModel(float value1, float value2)
    {
        // Promediar los valores de escalado recientes
        float newScaleValue = Mathf.Clamp((value1 + value2) / 2.0f, 0.1f, 30.0f);
        if (scaleValues.Count >= scaleQueueSize)
        {
            scaleValues.Dequeue();
        }
        scaleValues.Enqueue(newScaleValue);

        scaleFactor = 0;
        foreach (float value in scaleValues)
        {
            scaleFactor += value;
        }
        scaleFactor /= scaleValues.Count;

        model3D.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    void SetTargetRotation(float gx, float gy, float gz)
    {
        targetRotation = new Vector3(gx, gy, gz);
    }

    void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning("Invalid scene index: " + sceneIndex);
        }
    }

    void ResetSceneChangeCooldown()
    {
        canAcceptSceneChange = true;
    }
}
