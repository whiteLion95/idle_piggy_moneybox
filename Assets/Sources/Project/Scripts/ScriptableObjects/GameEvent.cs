using System.Collections.Generic;
using UnityEngine;

namespace Deslab
{
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "Deslab/Game Event", order = 52)]
    public class GameEvent : ScriptableObject 
    {
        private List<GameEventListener> listeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}