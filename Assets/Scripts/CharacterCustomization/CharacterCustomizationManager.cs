using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizationManager : MonoBehaviourSingletonPersistent<CharacterCustomizationManager>
{
    [SerializeField] private List<CharacterContainer> playersCustomizations = new List<CharacterContainer>();
    private CharacterContainer localCharacterContainer;
    
    [SerializeField] private List<CharacterBodyPart> allTorsoPart = new List<CharacterBodyPart>();
    [SerializeField] private List<CharacterBodyPart> allHeadPart = new List<CharacterBodyPart>();
    [SerializeField] private List<CharacterBodyPart> allEyesPart = new List<CharacterBodyPart>();
    [SerializeField] private List<CharacterBodyPart> allEarsPart = new List<CharacterBodyPart>();

    private int selectedTorso = 0;
    private int selectedHead = 0;
    private int selectedEyes = 0;
    private int selectedEars = 0;

    private void Start()
    {
        localCharacterContainer = new CharacterContainer();
    }

    public void ModifyTorso(bool increase)
    {
        selectedTorso += increase ? -1 : 1;
        selectedTorso = Mathf.Clamp(selectedTorso, 0, allTorsoPart.Count);
        UIManager.Instance.SetBodyPartIndexText(EBodyType.Torso, selectedTorso);
    }
    public void ModifyHead(bool increase)
    {
        selectedHead += increase ? -1 : 1;
        selectedHead = Mathf.Clamp(selectedHead, 0, allHeadPart.Count);
        UIManager.Instance.SetBodyPartIndexText(EBodyType.Head, selectedHead);
    }
    public void ModifyEyes(bool increase)
    {
        selectedEyes += increase ? -1 : 1;
        selectedEyes = Mathf.Clamp(selectedEyes, 0, allEyesPart.Count);
        UIManager.Instance.SetBodyPartIndexText(EBodyType.Eyes, selectedEyes);
    }
    public void ModifyEars(bool increase)
    {
        selectedEars += increase ? -1 : 1;
        selectedEars = Mathf.Clamp(selectedEars, 0, allEarsPart.Count);
        UIManager.Instance.SetBodyPartIndexText(EBodyType.Ears, selectedEars);
    }

    public void AddNewCustomization(int torso, int head, int ears, int eyes)
    {
        CharacterContainer container = new CharacterContainer();
        container.SaveData(torso, head, ears, eyes);
        playersCustomizations.Add(container);
        Debug.LogError("Customization Added!");
    }

    public void AddLocalCustomization()
    {
        playersCustomizations.Add(localCharacterContainer);
        Debug.LogError("Customization Added!");
    }

    public void ClearCustomizations()
    {
        playersCustomizations.Clear();
    }

    public void SaveBody()
    {
        localCharacterContainer.SaveData(selectedTorso, selectedHead, selectedEars, selectedEyes);
        localCharacterContainer.PrintData();
    }

    public CharacterContainer GetCharacterContainer()
    {
        return localCharacterContainer;
    }
}

[Serializable]
public class CharacterContainer
{
    public int currentTorso;
    public int currentHead;
    public int currentEars;
    public int currentEyes;
    
    public void SaveData(int torso, int head, int ears, int eyes)
    {
        currentTorso = torso;
        currentHead = head;
        currentEars = ears;
        currentEyes = eyes;
    }

    public void PrintData()
    {
        Debug.Log($"Torso: {currentTorso}\nHead: {currentHead}\nEars: {currentEars}\nEyes: {currentEyes}");
    }
}

public enum EBodyType
{
    None,
    Torso,
    Head,
    Ears,
    Eyes
}
