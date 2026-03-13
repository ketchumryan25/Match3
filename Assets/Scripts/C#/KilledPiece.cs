using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KilledPiece : MonoBehaviour
{    
    [Header("Piece Sizes")]
    [SerializeField]public float pieceDouble;
    [SerializeField]public float pieceMultiplier;
    public bool falling;   

    [Header("Piece Options")]
    [SerializeField] float speed = 16f;
    [SerializeField] float gravity = 32f;

    Vector2 moveDir;
    RectTransform rect;
    Image img;

    public void Intialize(Sprite piece, Vector2 start)
    {
        falling = true;

        moveDir = Vector2.up;
        moveDir.x = Random.Range(-1.0f, 1.0f);
        moveDir *= speed / 2;

        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        img.sprite = piece;
        rect.anchoredPosition = start;
    }

    // Update is called once per frame
    void Update()
    {
        if (!falling) return;
        moveDir.y -= Time.deltaTime * gravity;
        moveDir.x = Mathf.Lerp(moveDir.x, 0, Time.deltaTime);
        rect.anchoredPosition += moveDir * Time.deltaTime * speed;
        if (rect.position.x < (-pieceDouble * pieceMultiplier) || rect.position.x > Screen.width + (pieceDouble * pieceMultiplier) || rect.position.y < (-pieceDouble * pieceMultiplier) || rect.position.y > Screen.height + (pieceDouble * pieceMultiplier))
            falling = false;
    }
}
