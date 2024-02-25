using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deslab.Utils
{
    public static class TransformMaster
    {
        public static void CopyTransformData(Transform sourceTransform, Transform targetTransform)
        {
            if (sourceTransform.childCount != targetTransform.childCount)
            {
                Debug.LogWarning("Invalid transform copy, they need to match transform hierarchies");
                Debug.Log("Childs don't correspond");
                return;
            }

            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                var from = sourceTransform.GetChild(i);
                var to = targetTransform.GetChild(i);
                to.position = from.position;
                to.rotation = from.rotation;
                to.localScale = from.localScale;

                CopyTransformData(from, to);
            }
        }

        public static void Clear(this Transform transform)
        {
            if (transform.parent != null)
            {
                foreach (Transform child in transform.parent)
                {
                    Object.Destroy(child.gameObject);
                }

                Object.Destroy(transform.parent.gameObject);
            }
            else if (transform.childCount != 0)
            {
                foreach (Transform child in transform)
                {
                    Object.Destroy(child.gameObject);
                }

                Object.Destroy(transform.gameObject);
            }
            else
            {
                Object.Destroy(transform.gameObject);
            }
        }
    }
}