using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GalleryController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public Sprite[] modelImages;
    public string[] modelNames;
    public string[] modelAuthors;
    public string[] modelDescriptions;
    private VRManager vrManager;

    void Start()
    {
        // Encontrar el VRManager en la escena
        vrManager = FindObjectOfType<VRManager>();
        if (vrManager == null)
        {
            Debug.LogError("VRManager not found in the scene.");
            return;
        }

        if (modelNames == null || modelImages == null || modelDescriptions == null)
        {
            Debug.LogError("Model arrays are not assigned.");
            return;
        }

        if (cardPrefab == null || cardParent == null)
        {
            Debug.LogError("Card prefab or card parent is not assigned.");
            return;
        }

        for (int i = 0; i < modelNames.Length; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);

            TMP_Text practiceTitle = card.transform.Find("PracticeTitle")?.GetComponent<TMP_Text>();
            if (practiceTitle != null)
            {
                practiceTitle.text = modelNames[i];
            }
            else
            {
                Debug.LogError("PracticeTitle not found in card prefab.");
                continue;
            }

            Transform auxiliarCard = card.transform.Find("AuxiliarCardPrefab");
            if (auxiliarCard == null)
            {
                Debug.LogError("AuxiliarCardPrefab not found in card prefab.");
                continue;
            }

            Image practiceImage = auxiliarCard.Find("PracticeImage")?.GetComponent<Image>();
            if (practiceImage != null)
            {
                practiceImage.sprite = modelImages[i];
            }
            else
            {
                Debug.LogError("PracticeImage not found in AuxiliarCardPrefab.");
                continue;
            }

            Transform auxiliarTextCard = auxiliarCard.transform.Find("AuxiliarTextPrefab");
            if (auxiliarTextCard == null)
            {
                Debug.LogError("AuxiliarTextCard not found in card prefab.");
                continue;
            }

            TMP_Text practiceDescription = auxiliarTextCard.Find("PracticeDescription")?.GetComponent<TMP_Text>();
            if (practiceDescription != null)
            {
                practiceDescription.text = modelDescriptions[i];
            }
            else
            {
                Debug.LogError("PracticeDescription not found in AuxiliarTextCard.");
                continue;
            }

            TMP_Text author = auxiliarTextCard.Find("Author")?.GetComponent<TMP_Text>();
            if (author != null)
            {
                author.text = modelAuthors[i];
            }
            else
            {
                Debug.LogError("Author not found in AuxiliarTextCard.");
                continue;
            }

            int index = i;
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => { StartCoroutine(StartVRAndLoadScene(modelNames[index])); });
            }
            else
            {
                Debug.LogError("Button component not found in card prefab.");
            }
        }
    }

    private IEnumerator StartVRAndLoadScene(string sceneName)
    {
        vrManager.ToggleVR(); // Activar VR

        // Espera un momento para asegurarse de que VR se ha activado
        yield return new WaitForSecondsRealtime(1);

        SceneManager.LoadScene(sceneName);
    }
}
