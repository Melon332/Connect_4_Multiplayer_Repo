using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "ScriptableObjects/BodyPartObject", order = 1)]
public class CharacterBodyPart : ScriptableObject
{
    public EBodyType bodyType;
    public Texture2D bodyTexture;
    public GameObject bodyTypeModel;
}
