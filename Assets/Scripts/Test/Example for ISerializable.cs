using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestingSaveData
{
    public int number = 1;
}

public class ExampleForISerializabe : MonoBehaviour, ISerializable<TestingSaveData>
{
    public TestingSaveData SaveData { get; set; }

    private void Awake()
    {
        SaveData = new TestingSaveData();
        this.Load("testing save data");
    }

    private void Update()
    {
        this.Save("testing save data");
    }
}
