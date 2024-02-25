using UnityEngine;

public class OffsetAnimation : MonoBehaviour
{
    Renderer ownRenderer;
    [Range(-40f, 40f)] public float textureSpeedX, textureSpeedZ;

    float TextureSpeedX
    {
        get { return textureSpeedX * 0.03f; }
    }

    float TextureSpeedZ
    {
        get { return textureSpeedZ * 0.03f; }
    }

    private void Start()
    {
        ownRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        ownRenderer.material.mainTextureOffset = new Vector2(
            Time.time * TextureSpeedX,
            Time.time * TextureSpeedZ);
    }
}