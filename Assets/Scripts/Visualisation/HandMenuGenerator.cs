using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;
using UnityEngine.Events;

public class HandMenuGenerator : MonoBehaviour
{
    [Tooltip("Prefab položky menu (HandMenuCodeBoxShowPrefab Variant).")]
    public GameObject menuItemPrefab;

    [Tooltip("Rodičovský transform, pod ktorý sa budú položky menu umiestňovať.")]
    public Transform menuItemsParent;

    private Dictionary<string, PressableButton> menuItemButtons = new Dictionary<string, PressableButton>();

    void OnEnable()
    {
        CodeboxManager.OnCodeboxesCreated += HandleCodeboxesCreated;
    }

    void OnDisable()
    {
        CodeboxManager.OnCodeboxesCreated -= HandleCodeboxesCreated;
    }

    void Start()
    {
        Debug.Log("HandMenuGenerator sa spustil!");
    }

    public void HandleCodeboxesCreated(Dictionary<string, GameObject> codeboxes)
    {
        Debug.Log($"HandMenuGenerator zachytil udalosť o vytvorení Codeboxov. Počet: {codeboxes.Count}");
        CreateMenuItems(codeboxes);
    }

    // Nová metóda na obnovenie menu
    public void RefreshHandMenu(Dictionary<string, GameObject> codeboxes)
    {
        Debug.Log("Obnovujem hand menu...");
        CreateMenuItems(codeboxes);
    }

    private void CreateMenuItems(Dictionary<string, GameObject> codeboxes)
    {
        if (menuItemPrefab == null || menuItemsParent == null)
        {
            Debug.LogError("Nie je nastavený prefab položky menu alebo rodičovský transform!");
            return;
        }

        // Vyčisti existujúce položky v menu
        foreach (Transform child in menuItemsParent)
        {
            Destroy(child.gameObject);
        }
        menuItemButtons.Clear();

        foreach (var entry in codeboxes)
        {
            string className = entry.Key;
            GameObject codebox = entry.Value;

            // Vytvor inštanciu prefabu položky menu
            GameObject newMenuItem = Instantiate(menuItemPrefab, menuItemsParent);

            // Nájdaj komponent TextMeshProUGUI pre Label
            TextMeshProUGUI labelText = newMenuItem.GetComponentInChildren<TextMeshProUGUI>();
            if (labelText != null)
            {
                labelText.text = className;
            }
            else
            {
                Debug.LogWarning($"TextMeshPro komponent nebol nájdený v prefabe položky menu.");
            }

            // Nájdaj PressableButton komponent
            PressableButton pressableButton = newMenuItem.GetComponent<PressableButton>();
            if (pressableButton != null)
            {
                // Ulož referenciu na tlačidlo pre neskoršiu aktualizáciu
                menuItemButtons[className] = pressableButton;

                // Vytvor UnityAction pre zobrazenie/skrytie Codeboxu
                UnityAction buttonAction = () =>
                {
                    if (CodeboxManager.Instance != null)
                    {
                        // Prepínaj stav aktivity Codeboxu
                        if (codebox.activeSelf)
                        {
                            CodeboxManager.Instance.HideCodebox(className);
                        }
                        else
                        {
                            CodeboxManager.Instance.DisplayCodebox(className);
                        }
                        // Aktualizuj stav tlačidla po zmene aktivity Codeboxu
                        if (menuItemButtons.TryGetValue(className, out PressableButton button))
                        {
                            button.ForceSetToggled(codebox.activeSelf);
                        }
                    }
                };
                pressableButton.OnClicked.AddListener(buttonAction);

                // Nastav počiatočný stav tlačidla
                pressableButton.ForceSetToggled(codebox.activeSelf);
            }
            else
            {
                Debug.LogWarning($"PressableButton komponent nebol nájdený v prefabe položky menu.");
            }
        }
    }

    // Metóda na nastavenie stavu prepínača pre daný Codebox
    public void SetMenuItemToggleState(string className, bool isVisible)
    {
        if (menuItemButtons.TryGetValue(className, out PressableButton button))
        {
            button.ForceSetToggled(isVisible);
            Debug.Log($"Nastavujem stav tlačidla pre {className} na ForceSetToggled: {isVisible}");
        }
    }
}