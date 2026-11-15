using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectSO", menuName = "Scriptable Objects/ObjectSO")]
public class ObjectSO : ScriptableObject
{
    public string type;
    public Sprite sprite;
    public Vector3 position;
    public float localScale;
}
