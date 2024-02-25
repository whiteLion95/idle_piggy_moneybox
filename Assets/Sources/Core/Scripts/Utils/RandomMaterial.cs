using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomMaterial : MonoBehaviour
{
    [SerializeField] private List<Material> materials;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private bool changeCertainElements;
    [ShowIf("changeCertainElements")] [SerializeField] private List<int> elementsIndexesToChange;

    private void Start()
    {
        SetRandMaterial();
    }

    private void SetRandMaterial()
    {
        Material randMaterial = GetRandMaterial();

        foreach (Renderer renderer in renderers)
        {
            Material[] rendererMats = renderer.materials;

            if (changeCertainElements)
            {
                for (int i = 0; i < elementsIndexesToChange.Count; i++)
                {
                    rendererMats[elementsIndexesToChange[i]] = randMaterial;
                }
            }
            else
            {
                for (int i = 0; i < rendererMats.Length; i++)
                {
                    rendererMats[i] = randMaterial;
                }
            }

            renderer.materials = rendererMats;
        }
    }

    private Material GetRandMaterial()
    {
        if (materials.Count != 0)
        {
            int randIndex = Random.Range(0, materials.Count);
            return materials[randIndex];
        }

        return null;
    }
}
