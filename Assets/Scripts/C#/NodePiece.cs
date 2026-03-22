using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int value; //0 = blank, 1 = apple, 2 = banana, 3 = blueberry, 4 = grape, 5 = orange, 6 = pear, 7 = strawberry, -1 = hole
    public Point index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;
    [HideInInspector]
    public GameObject match3;
    [HideInInspector]
    public Match3 m3Script;    

    [Header("Piece Sizes")]
    [SerializeField]public int pieceBase;
    [SerializeField]public int pieceDouble;
    [SerializeField]public float pieceHalf;

    bool updating;
    Image img;

    public void Intialize(int v, Point p, Sprite piece)
    {
        match3 = GameObject.Find("_Match3");
        if (match3 != null)
        {
            m3Script = match3.GetComponent<Match3>();
        }
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        value = v;
        SetIndex(p);
        img.sprite = piece;
    }

    public void SetIndex(Point p)
    {
        index = p;
        ResetPosition();
        UpdateName();
    }
    public void ResetPosition()
    {
        pos = new Vector2(pieceBase + (pieceDouble * index.x), -pieceBase - (pieceDouble * index.y));
    }

    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

    public bool UpdatePiece()
    {
        if (Vector3.Distance(rect.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            updating = false;
            return false;
        }
    }

    public void MovePosition(Vector2 move)
    {
        rect.anchoredPosition += move * Time.deltaTime * pieceHalf;
    }
    
    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * pieceHalf);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (updating) return;
        MovePieces.instance.MovePiece(this);
        Debug.Log("Grab " + transform.name);
        if (m3Script != null)
        {
            m3Script.currentPiece = transform.gameObject;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        MovePieces.instance.DropPiece();
        Debug.Log("Let go of " + transform.name);
        if (m3Script != null)
        {
            m3Script.currentPiece = null;
        }
    }
}
