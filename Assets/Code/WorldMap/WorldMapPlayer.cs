using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pixelplacement;

public class WorldMapPlayer : MonoBehaviour
{
    public float moveDelay;
    public float moveSpeed;
    public AnimationCurve moveCurve;

    private Vector2 move;
    int moveDir = 0;

    public WorldMapSpot currentLocation;
    public Vector3 locationOffset;

    public GameObject[] moveArrows;

    [HideInInspector] public Controls controls;
    private SpriteRenderer spriteRender;

    //private UI_OverWorld uI_OverWorld;
    private ClientManager clientManager;

    private Pixelplacement.TweenSystem.TweenBase movement;

    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        clientManager = FindObjectOfType<ClientManager>();
        //sceneTransition = FindObjectOfType<UI_SceneTransition>();
        controls = new Controls();

        controls.WorldMap.Select.performed += funny => Select();

        controls.WorldMap.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.WorldMap.Move.canceled += ctx => move = Vector2.zero;

    }

    void Start()
    {
        transform.position = currentLocation.transform.position + locationOffset;
        controls.Enable();

        MoveToSpot(currentLocation);
    }

    private void Update()
    {
        if (move == Vector2.zero)
            return;

        if (movement.Status == Tween.TweenStatus.Running)
            return;

        if (move.y > 0)
            moveDir = 0;

        else if (move.x > 0)
            moveDir = 1;

        else if (move.y < 0)
            moveDir = 2;

        else if (move.x < 0)
            moveDir = 3;

        if (currentLocation.NeighbourSpots[moveDir] != null)
            MoveToSpot(currentLocation.NeighbourSpots[moveDir]);
    }

    void AtSpot()
    {
        if (currentLocation.Level == false)
        {
            MoveToSpot(currentLocation.NeighbourSpots[moveDir]);
            return;
        }

        int spotNum = 0;
        foreach (WorldMapSpot spot in currentLocation.NeighbourSpots)
        {

            if (currentLocation.NeighbourSpots[spotNum] != null)
                moveArrows[spotNum].SetActive(true);

            spotNum++;

            //uI_OverWorld.UpdateDetails();
        }
    }

    void MoveToSpot(WorldMapSpot spot)
    {
        currentLocation = spot;
        if (currentLocation.sceneName != "") //dont if empty 
            clientManager.UpdateMap(currentLocation.sceneName);

        foreach (GameObject arrow in moveArrows)
        {
            arrow.SetActive(false);
        }

        float moveDuration = (Vector2.Distance(transform.position, spot.transform.position + locationOffset) * moveSpeed);
        StartCoroutine(HalfWayToSpot(moveDuration / 2));

        movement = Tween.Position(transform, spot.transform.position + locationOffset, moveDuration, moveDelay, moveCurve, completeCallback: AtSpot);
    }

    void Select()
    {
        if (currentLocation.sceneName == "")
            return;

        if (transform.position == currentLocation.transform.position + locationOffset)
        {
            //controls.Disable();

            //StartCoroutine(sceneTransition.LoadScene(currentLocation.sceneName));
        }
    }

    IEnumerator HalfWayToSpot(float wait)
    {
        yield return new WaitForSeconds(wait);
        //spriteRender.sortingOrder = currentLocation.spriteRender.sortingOrder + 1;
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }
}

