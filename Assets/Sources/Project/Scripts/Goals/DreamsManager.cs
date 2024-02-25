using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mamboo.Internal.Scripts;
using UnityEngine;

namespace Deslab
{
    public class DreamsManager : Singleton<DreamsManager>
    {
        [System.Serializable]
        public struct Dream
        {
            public int Id;
            public GameObject DreamObject;
        }

        [SerializeField] private List<Dream> _dreams;

        [SerializeField] private GameObject _container;

        [SerializeField] private Animator _animator;

        public void UpdateStateDreams(List<Goal> purchasedGoals)
        {
            foreach (Goal goal in purchasedGoals)
            {
                _dreams.Find((d)=>d.Id==goal.id).DreamObject.SetActive(true);
            }

            if (purchasedGoals.Count == 1)
            {
                _animator.Play("VictoryIdle");
            }
            
            _container.SetActive(purchasedGoals.Count<4); 
        }
    }
}