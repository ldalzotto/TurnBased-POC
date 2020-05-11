using _EventQueue;
using _Functional;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace _GameLoop
{
    public class GameLoop : MonoBehaviour
    {

        private static Dictionary<GameLoopHook, List<GameLoopCallback>> GameSequencer = new Dictionary<GameLoopHook, List<GameLoopCallback>>();

        public static void AddGameLoopCallback(GameLoopHook p_hook, GameLoopCallback p_gameLoopCallback)
        {
            if (!GameSequencer.ContainsKey(p_hook)) { GameSequencer[p_hook] = new List<GameLoopCallback>(); }

            List<GameLoopCallback> p_involvedHookCallback = GameSequencer[p_hook];
            p_involvedHookCallback.Add(p_gameLoopCallback);
            p_involvedHookCallback.Sort((GameLoopCallback p1, GameLoopCallback p2) => { return p1.GameLoopPriority.CompareTo(p2.GameLoopPriority); });
        }

        public static void RemoveGameLoopCallback(GameLoopHook p_hook, GameLoopCallback p_gameLoopCallback)
        {
            GameSequencer[p_hook].Remove(p_gameLoopCallback);
        }

        private void Awake()
        {
            Physics.autoSimulation = false;

            ExternalHooks.LogDebug = (string s) => { Debug.Log(Time.frameCount + " : " + s); };
            ExternalHooks.Profiler_BeginSample = (string name) => { Profiler.BeginSample(name); };
            ExternalHooks.Profiler_EndSample = () => { Profiler.EndSample(); };

            ExternalHooks.Transform_get_worldPosition = (object p_transform) => { return ((Transform)p_transform).position; };
            ExternalHooks.Transform_set_worldPosition = (object p_transform, ref float3 p_worldPosition) => { ((Transform)p_transform).position = p_worldPosition; };


            if (!GameSequencer.ContainsKey(GameLoopHook.BeforePhysics)) { GameSequencer[GameLoopHook.BeforePhysics] = new List<GameLoopCallback>(); }
            if (!GameSequencer.ContainsKey(GameLoopHook.Tick)) { GameSequencer[GameLoopHook.Tick] = new List<GameLoopCallback>(); }
            if (!GameSequencer.ContainsKey(GameLoopHook.LateTick)) { GameSequencer[GameLoopHook.LateTick] = new List<GameLoopCallback>(); }
        }
        
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            List<GameLoopCallback> l_tickCallbacks = GameSequencer[GameLoopHook.BeforePhysics];

            for (int i = 0; i < l_tickCallbacks.Count; i++)
            {
                l_tickCallbacks[i].Callback(delta);
            }

        }

        private void Update()
        {
            float delta = Time.deltaTime;

            MyEvent.broadcast(ref ExternalHooks.OnTickStartEvent);

            EventQueueContainer.iterate();

            List<GameLoopCallback> l_tickCallbacks = GameSequencer[GameLoopHook.Tick];

            for (int i = 0; i < l_tickCallbacks.Count; i++)
            {
                l_tickCallbacks[i].Callback(delta);
            }

            EventQueueContainer.iterate();
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;

            List<GameLoopCallback> l_tickCallbacks = GameSequencer[GameLoopHook.LateTick];

            for (int i = 0; i < l_tickCallbacks.Count; i++)
            {
                l_tickCallbacks[i].Callback(delta);
            }

        }
    }

    public enum GameLoopHook
    {
        BeforePhysics,
        Tick,
        LateTick
    }

    public struct GameLoopCallback : IEquatable<GameLoopCallback>
    {
        public float GameLoopPriority;
        public Action<float> Callback;

        public override bool Equals(object obj)
        {
            return obj is GameLoopCallback callback && Equals(callback);
        }

        public bool Equals(GameLoopCallback other)
        {
            return GameLoopPriority == other.GameLoopPriority &&
                   EqualityComparer<Action<float>>.Default.Equals(Callback, other.Callback);
        }

        public override int GetHashCode()
        {
            var hashCode = 1770082687;
            hashCode = hashCode * -1521134295 + GameLoopPriority.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Action<float>>.Default.GetHashCode(Callback);
            return hashCode;
        }
    }
}
