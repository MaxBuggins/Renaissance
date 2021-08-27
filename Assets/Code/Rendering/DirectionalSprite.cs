using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSprite : MonoBehaviour
{
    [Header("Look propertys")]
    public bool constantFollow = false;
    public bool yAxisOnly = true;

    public Vector3 lookOffset = Vector3.zero;
    [HideInInspector] public Vector3 orginalRot = Vector3.zero;

    [Header("Sprite propertys")]
    public bool randomSpirte = false;
    public float scale = 1;
    public List<Sprite> directionalSprites;

    private int currentSprite;

    public float sortAngle;
    private float lookAngle;

    //public float inFrontAmount = 0;

    private Renderer render;
    private Camera cam;

    void Start()
    {
        render = GetComponent<Renderer>();

        if (randomSpirte)
        {
            currentSprite = Random.Range(0, directionalSprites.Count);
            ApplySprite(directionalSprites[currentSprite]);
        }

        else if(directionalSprites.Count > 0)
            SetUp();

        if (constantFollow == false && directionalSprites.Count < 2 || randomSpirte)
            enabled = false; //optermisation

    }

    public void SetUp()
    {
        sortAngle = 360 / directionalSprites.Count;
        ApplySprite(directionalSprites[0]);
    }

    void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            return;
        }

        //Vector3 frontPos = transform.forward * inFrontAmount;

        //transform.localPosition = new Vector3(frontPos.x, transform.localPosition.y, frontPos.z);

        if(randomSpirte || constantFollow)
        {
            faceCamera(yAxisOnly);
            if (randomSpirte)
                return;
        }

        if (directionalSprites.Count > 1) //no need if there is no other sprites to change to
        {
            Vector3 toOther = (transform.position - cam.transform.position).normalized;

            lookAngle = Mathf.Atan2(toOther.z, toOther.x) * Mathf.Rad2Deg + 180; //mathmoment
            float relLookAngle = (lookAngle - (sortAngle / 2)) + transform.parent.eulerAngles.y;


            int spriteNum = (int)(relLookAngle / sortAngle); ;

            if (spriteNum >= directionalSprites.Count)
                spriteNum = (spriteNum - directionalSprites.Count);

            else if (spriteNum < 0)
                spriteNum = (spriteNum + directionalSprites.Count);


            if (currentSprite != spriteNum)
            {
                currentSprite = spriteNum;
                ApplySprite(directionalSprites[currentSprite]);

                if (!constantFollow)
                    faceCamera(yAxisOnly);
            }
        }
    }

    void faceCamera(bool yOnly) //rotates the mesh to face the main.camera
    {
        transform.LookAt(cam.transform.position, -Vector3.up);

        transform.eulerAngles += lookOffset;

        if (yOnly)
            transform.eulerAngles = new Vector3(lookOffset.x, transform.eulerAngles.y, lookOffset.z);
    }

    void ApplySprite(Sprite sprite)
    {
        var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                (int)sprite.textureRect.y,
                                                (int)sprite.textureRect.width,
                                                (int)sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);

        croppedTexture.filterMode = FilterMode.Point;

        transform.localScale = new Vector3(sprite.textureRect.width / sprite.pixelsPerUnit * scale,
            sprite.textureRect.height / sprite.pixelsPerUnit * scale, 1);

        croppedTexture.Apply();
        render.material.mainTexture = croppedTexture;
    }
}
