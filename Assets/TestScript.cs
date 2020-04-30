using _ExecutionTree;
using _Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    struct Value
    {
        public float X;
        public float Y;
    }

    void Start()
    {
        RefDictionary<int, Value> test = new RefDictionary<int, Value>();
        test.Add(76, new Value());
        test.ValueRef(76).Y = 99;

        RefList<Value> zd = new RefList<Value>();
        zd.Add(new Value());
        zd.ValueRef(0).X = 10;

    }

    private void Update()
    {

    }



}