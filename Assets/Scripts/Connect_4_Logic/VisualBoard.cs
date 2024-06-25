using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBoard : MonoBehaviour
{
    [SerializeField] private List<Transform> rowTransforms = new List<Transform>();

    public Transform GetColumnTransform(int row)
    {
        return rowTransforms[row];
    }
}
