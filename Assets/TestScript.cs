using _Entity._Turn;
using _ExecutionTree;
using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public event Action myev;
    struct Value
    {
        public float X;
        public float Y;
    }

    void Start()
    {
        myev += Test;
        myev.Invoke();

        RefDictionary<int, Value> test = new RefDictionary<int, Value>();
        test.Add(76, new Value());
        test.Add(12, new Value());
        test.Add(78, new Value());


        for (int i = 0; i < test.Count; i++)
        {
            test.entries[i].value.X = 50;
        }

    }

    public void Test()
    {
        myev -= Test;
    }

    private void Update()
    {

    }



}