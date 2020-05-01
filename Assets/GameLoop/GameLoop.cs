using _Functional;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            ExternalHooks.LogDebug = Debug.Log;

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
            MyEvent.broadcast(ref ExternalHooks.OnTickStartEvent); 

            float delta = Time.deltaTime;

            List<GameLoopCallback> l_tickCallbacks = GameSequencer[GameLoopHook.Tick];

            for (int i = 0; i < l_tickCallbacks.Count; i++)
            {
                l_tickCallbacks[i].Callback(delta);
            }
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

        private static float Max(float[] p_floatArray)
        {
            if (p_floatArray != null && p_floatArray.Length > 0)
            {
                float l_returnMax = p_floatArray[0];
                for (int i = 1; i < p_floatArray.Length; i++)
                {
                    if (l_returnMax < p_floatArray[i])
                    {
                        l_returnMax = p_floatArray[i];
                    }
                }
                return l_returnMax;
            }
            else
            {
                return 0.0f;
            }

        }
        private static float Min(float[] p_floatArray)
        {
            if (p_floatArray != null && p_floatArray.Length > 0)
            {
                float l_returnMin = p_floatArray[0];
                for (int i = 1; i < p_floatArray.Length; i++)
                {
                    if (l_returnMin > p_floatArray[i])
                    {
                        l_returnMin = p_floatArray[i];
                    }
                }
                return l_returnMin;
            }
            else
            {
                return 0.0f;
            }

        }

        public static float EvaluatePriority(float[] p_before, float[] p_after)
        {
            bool l_beforeCaluclation = (p_before != null && p_before.Length > 0);
            bool l_afterCaluclation = (p_after != null && p_after.Length > 0);

            if (l_beforeCaluclation && l_afterCaluclation)
            {
                return (Min(p_before) + Max(p_after)) * 0.5f;
            }
            else if (l_beforeCaluclation)
            {
                return Min(p_before) - 1.0f;
            }
            else if (l_afterCaluclation)
            {
                return Max(p_after) + 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

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
