using System;
using System.Collections.Generic;
using Deslab.Utils;
using UnityEngine;

namespace Deslab.Deslytics.Provider
{
    [Serializable]
    public class Providers
    {
        [SerializeField, InterfaceType(typeof(IProvider))]
        private List<MonoBehaviour > ProvidersList = new List<MonoBehaviour >();

        public void CallAction(string method, params object[] parameters)
        {
            for (int i = 0; i < ProvidersList.Count; i++)
            {
                ProvidersList[i].CallMethod(method, parameters);
            }
        }
    }
}