using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSprite : MonoBehaviour
{
    public Sprite[] directionalSprites;
    private int currentSprite;

    public float sortAngle;

    private Renderer render;
    private Camera cam;

    void Start()
    {
        render = GetComponent<Renderer>();

        sortAngle = 360 / directionalSprites.Length;
    }

    void Update()
    {
        cam = Camera.main;

        if (cam == null)
            return;

        Vector3 toOther = (transform.position - cam.transform.position).normalized;

        float lookAngle = Mathf.Atan2(toOther.z, toOther.x) * Mathf.Rad2Deg + 180; //mathmoment
        lookAngle += transform.parent.eulerAngles.y;

        int spriteNum = (int)((lookAngle - sortAngle / 2) / sortAngle);

        if(spriteNum >= directionalSprites.Length)
            spriteNum = (spriteNum - directionalSprites.Length);

        else if (spriteNum < 0)
            spriteNum = (spriteNum + directionalSprites.Length);


        if (currentSprite != spriteNum)
        {

            currentSprite = spriteNum;
            Sprite sprite = directionalSprites[currentSprite];

            var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);

            croppedTexture.filterMode = FilterMode.Point;

            transform.localScale = new Vector3(sprite.textureRect.width / sprite.pixelsPerUnit,
                sprite.textureRect.height / sprite.pixelsPerUnit, 1);

            croppedTexture.Apply();
            render.material.mainTexture = croppedTexture;
        //}
            transform.LookAt(cam.transform);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);

        }
    }
}
