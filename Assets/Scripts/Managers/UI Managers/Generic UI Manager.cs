using System.Collections.Generic;
using UnityEngine;

public class GenericUIManager : MonoBehaviour
{
    public static GenericUIManager Instance { get; private set; }
    
    protected virtual void Awake()
    {
        if (Instance == null) Instance = this;
    }
}