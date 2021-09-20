using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteToMat : MonoBehaviour
{
    public Sprite sprite;
    public float scale = 1;

    private Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
        ApplySprite(sprite);
    }

    public void ApplySprite(Sprite sprite)
    {
        var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);

        croppedTexture.filterMode = FilterMode.Point;

        transform.localScale = new Vector3(sprite.textureRect.width / sprite.pixelsPerUnit,
            sprite.textureRect.height / sprite.pixelsPerUnit, 1) * scale;

        croppedTexture.Apply();
        render.material.mainTexture = croppedTexture;
    }
}
