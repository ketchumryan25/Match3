using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public string seedString;

    [Header("Game Objects")]
    [SerializeField]public GameObject nodePiece;
    [SerializeField]public GameObject killedPiece;
    [SerializeField] private GameObject highlightPrefab;
    [SerializeField] private GameObject movePopBase;
    [SerializeField] private GameObject movePopSmall;
    [SerializeField] private GameObject movePopMedium;
    [SerializeField] private GameObject movePopBig;
    [SerializeField] private GameObject audioHolder;

    [Header("Hint Stuff")]
    [SerializeField] public List<Color> highlightColors = new List<Color>();
    private List<GameObject> activeHighlights = new List<GameObject>();
    
    [Header("Piece Amounts (100=5, 120=6, 140=7, etc)")]
    [SerializeField]public int pieceLength;
    
    
    [Header("Piece Sizes")]
    [SerializeField]public int pieceBase;
    [SerializeField]public int pieceDouble;
    [SerializeField]public float pieceHalf;
    
    [Header("Match Scores")]
    [SerializeField]public int scoreBase;
    [SerializeField]public float scoreMultiSmall;
    [SerializeField]public float scoreMultiMedium;
    [SerializeField]public float scoreMultiBig;
    [SerializeField]public int scoreCurrent;
    [SerializeField]public bool matchBase;
    [SerializeField]public bool matchMultiSmall;
    [SerializeField]public bool matchMultiMedium;
    [SerializeField]public bool matchMultiBig;


    int width = 9;
    int height = 14;
    int[] fills;
    Node[,] board;
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;

    private List<(Point p1, Point p2)> currentPossibleMoves = new List<(Point, Point)>();
    public int currentMoveCount = 0;
    private bool GameStarted = false;

    System.Random random;

    void Start()
    {
    }

    public void StartGame()
    {
        fills = new int[width];
        int seedHashCode = GetOrCreateSeedHashCode();
        random = new System.Random(seedHashCode);
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();

        IntializeBoard();
        VerifyBoard();
        InstantiateBaord();

        GameStarted = true;
    }

    void IntializeBoard()
    {
        board = new Node[width, height];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                board[x,y] = new Node((boardLayout.rows[y].row[x]) ? - 1 : fillPiece(), new Point(x, y));
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while (isConnected(p, true).Count > 0)
                {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    setValueAtPoint(p, newValue(ref remove));
                }
            }
        }
    }

    void InstantiateBaord()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));

                int val = board[x, y].value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(pieceBase + (pieceDouble * x), -pieceBase - (pieceDouble * y));
                piece.Intialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (getValueAtPoint(one) < 0) return;

        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();
        if (getValueAtPoint(two) > 0)
        {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if(main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
            ResetPiece(pieceOne);
    }
   

    List<Point> isConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = getValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach(Point dir in directions)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for(int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));
                if(getValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
            
        }

        for(int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i+2]) };
            foreach (Point next in check)
            {
                if (getValueAtPoint(next) == val)
                    {
                        line.Add(next);
                        same++;
                    }
            }

            if(same > 1)
                AddPoints(ref connected, line);
        }

        for(int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if(next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };
            foreach (Point pnt in check)
            {
                if (getValueAtPoint(pnt) == val)
                    {
                        square.Add(pnt);
                        same++;
                    }
            }

            if(same > 2)
                AddPoints(ref connected, square);
        }

        if(main)
        {
            for(int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, isConnected(connected[i], false));
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach(Point p in add)
        {
            bool doAdd = true;
            for(int i = 0; i < points.Count; i++)
            {
                if(points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if(doAdd) points.Add(p);
        }
    }

    int fillPiece()
    {
        int val = 1;
        val = (random.Next(0, pieceLength) / (pieceLength / pieces.Length)) + 1;
        return val;
    }

    int getValueAtPoint(Point p)
    {
        if(p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void setValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
            available.Add(i + 1);
        foreach (int i in remove)
            available.Remove(i);
        
        if (available.Count <= 0) return 0;
        return available[random.Next(0, available.Count)];
    }

    void Update()
    {
        if (GameStarted)
        {
            List<NodePiece> finishedUpdating = new List<NodePiece>();
            for (int i = 0; i < update.Count; i++)
            {
                NodePiece piece = update[i];
                if (!piece.UpdatePiece()) finishedUpdating.Add(piece); 
            }
            for (int i = 0; i < finishedUpdating.Count; i++)
            {
                NodePiece piece = finishedUpdating[i];
                FlippedPieces flip = getFlipped(piece);
                NodePiece flippedPiece = null;

                int x = (int)piece.index.x;
                fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

                List<Point> connected = isConnected(piece.index, true);
                bool wasFlipped = (flip != null);

                if (wasFlipped)
                {
                    flippedPiece = flip.getOtherPiece(piece);
                    AddPoints(ref connected, isConnected(flippedPiece.index, true));
                }

                if (connected.Count == 0)
                {
                    if (wasFlipped)
                        FlipPieces(piece.index, flippedPiece.index, false);
                        Debug.LogWarning("There was no match");
                }
                else
                {
                    int matchCount = connected.Count;
                    Debug.LogWarning("There was a match of " + matchCount);
                    SelectMatchPop(matchCount);
                    ClearHighlights();
                    AddScore(matchCount);
                    foreach (Point pnt in connected)
                    {
                        KillPiece(pnt);
                        Node node = getNodeAtPoint(pnt);
                        NodePiece nodePiece = node.GetPiece();
                        if (nodePiece != null)
                        {
                            nodePiece.gameObject.SetActive(false);
                            dead.Add(nodePiece);
                        }
                        node.SetPiece(null);
                    }

                    ApplyGravityToBoard();
                }

                flipped.Remove(flip);
                update.Remove(piece);
            }
        }
    }

    public void SelectMatchPop(int matchCount)
    {
        if (matchCount == 3)
        {
            GameObject pop = Instantiate(movePopBase);
            PlayPop(pop);
        }
        else if (matchCount > 3 && matchCount < 6)
        {
            GameObject pop = Instantiate(movePopSmall);
            PlayPop(pop);
        }
        else if (matchCount >= 6 && matchCount < 9)
        {
            GameObject pop = Instantiate(movePopMedium);
            PlayPop(pop);
        }
        else if (matchCount >= 9)
        {
            GameObject pop = Instantiate(movePopBig);
            PlayPop(pop);
        }
    }

    public void PlayPop(GameObject pop)
    {
        Transform trans = audioHolder.transform;
        AudioSource audio = pop.GetComponent<AudioSource>();
        pop.transform.SetParent(trans);
        audio.Play();
        Destroy(pop, audio.clip.length + 0.1f);
    }

    public void AddScore(int matchCount)
    {
        int scoreStart = matchCount * scoreBase;
        if (matchCount == 3)
        {
            float scoreTemp = scoreCurrent + scoreStart;
            int scoreRounded = Mathf.RoundToInt(scoreTemp); 
            scoreCurrent = scoreRounded;
        }
        else if (matchCount > 3 && matchCount < 6)
        {
            float scoreTemp = scoreCurrent + (scoreStart * scoreMultiSmall);
            int scoreRounded = Mathf.RoundToInt(scoreTemp); 
            scoreCurrent = scoreRounded;
        }
        else if (matchCount >= 6 && matchCount < 9)
        {
            float scoreTemp = scoreCurrent + (scoreStart * scoreMultiMedium);
            int scoreRounded = Mathf.RoundToInt(scoreTemp); 
            scoreCurrent = scoreRounded;
        }
        else if (matchCount >= 9)
        {
            float scoreTemp = scoreCurrent + (scoreStart * scoreMultiBig);
            int scoreRounded = Mathf.RoundToInt(scoreTemp); 
            scoreCurrent = scoreRounded;
        }
    }

    void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = (height-1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val != 0) continue;
                for (int ny = (y-1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1)
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();

                        node.SetPiece(piece);
                        update.Add(piece);

                        got.SetPiece(null);
                    }
                    else
                    {
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPoint = new Point(x, (-1 - fills[x]));
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject. SetActive(true);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }
                            
                        piece.Intialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = getPositionFromPoint(fallPoint);

                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
            if (!killed[i].falling) available.Add(killed[i]);
        
        KilledPiece set = null;
        if (available.Count > 0)
            set = available[0];
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = getValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < pieces.Length)
            set.Intialize(pieces[val], getPositionFromPoint(p));
    }

    public void SetSprites(Sprite[] newSprites)
    {
        pieces = newSprites;
    }

    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        return seed;

    }

    private int GetOrCreateSeedHashCode()
    {
        if (string.IsNullOrEmpty(seedString))
            {
                string seed = getRandomSeed();
                int seedHashCode = seed.GetHashCode();
                seedString = seedHashCode.ToString();
                return seedHashCode;
            }
        else
            {
        if (int.TryParse(seedString, out int existingHash))
            {
                return existingHash;
            }
        else
            {
                string seed = getRandomSeed();
                int seedHashCode = seed.GetHashCode();
                seedString = seedHashCode.ToString();
                return seedHashCode;
            }
        }
    }

void ClearHighlights()
{
    foreach (var hl in activeHighlights)
    {
        Destroy(hl);
    }
    activeHighlights.Clear();
}

public List<(Point p1, Point p2)> GetPossibleMoves()
{
    List<(Point, Point)> moves = new List<(Point, Point)>();
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Point p = new Point(x, y);
            // Check Right
            if (x < width - 1 && IsSwapResultingMatch(p, new Point(x + 1, y)))
            {
                NodePiece pieceA = getNodeAtPoint(p)?.GetPiece();
                NodePiece pieceB = getNodeAtPoint(new Point(x + 1, y))?.GetPiece();

                if (pieceA != null && pieceB != null && pieceA.gameObject.activeInHierarchy && pieceB.gameObject.activeInHierarchy)
                {
                    moves.Add((p, new Point(x + 1, y)));
                }
            }
            // Check Down
            if (y < height - 1 && IsSwapResultingMatch(p, new Point(x, y + 1)))
            {
                NodePiece pieceA = getNodeAtPoint(p)?.GetPiece();
                NodePiece pieceB = getNodeAtPoint(new Point(x, y + 1))?.GetPiece();

                if (pieceA != null && pieceB != null && pieceA.gameObject.activeInHierarchy && pieceB.gameObject.activeInHierarchy)
                {
                    moves.Add((p, new Point(x, y + 1)));
                }
            }
        }
    }
    return moves;
}

public void SpawnHighlights(List<(Point p1, Point p2)> moves)
{
    ClearHighlights();

    foreach (var move in moves)
    {
        Vector2 pos1 = getPositionFromPoint(move.p1);
        Vector2 pos2 = getPositionFromPoint(move.p2);

        GameObject hl1 = Instantiate(highlightPrefab, gameBoard);
        hl1.GetComponent<RectTransform>().anchoredPosition = pos1;
        GameObject hl2 = Instantiate(highlightPrefab, gameBoard);
        hl2.GetComponent<RectTransform>().anchoredPosition = pos2;

        Vector2 direction1 = pos2 - pos1;
        float angle1 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
        hl1.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle1);
        Vector2 direction2 = pos1 - pos2;
        float angle2 = Mathf.Atan2(direction2.y, direction2.x) * Mathf.Rad2Deg;
        hl2.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle2);
        
        if (highlightColors.Count > 0)
            {
                Color randomColor = highlightColors[Random.Range(0, highlightColors.Count)];
                randomColor.a = 1f;
                var image1 = hl1.GetComponent<UnityEngine.UI.Image>();
                var image2 = hl2.GetComponent<UnityEngine.UI.Image>();
                if (image1 != null && image2 != null)
                {
                    image1.color = randomColor;
                    image2.color = randomColor;
                }
            }        

        hl1.name = getNodeAtPoint(move.p1)?.GetPiece()?.gameObject.name ?? "Highlight_" + move.p1.x + "_" + move.p1.y;
        activeHighlights.Add(hl1);
        
        hl2.name = getNodeAtPoint(move.p2)?.GetPiece()?.gameObject.name ?? "Highlight_" + move.p2.x + "_" + move.p2.y;
        activeHighlights.Add(hl2);
    }
}

public bool IsSwapResultingMatch(Point p1, Point p2)
{
    int val1 = getValueAtPoint(p1);
    int val2 = getValueAtPoint(p2);

    if (val1 <= 0 || val2 <= 0) return false;

    setValueAtPoint(p1, val2);
    setValueAtPoint(p2, val1);

    bool matchResult = IsPartOfMatch(p1) || IsPartOfMatch(p2);

    bool squareResult = HasSquareMatch(p1) || HasSquareMatch(p2);

    setValueAtPoint(p1, val1);
    setValueAtPoint(p2, val2);

    return matchResult || squareResult;
}

private bool HasSquareMatch(Point p)
{
    int val = getValueAtPoint(p);
    if (val <= 0) return false;

    // Check all 4 possible squares around p
    List<Point> origins = new List<Point>
    {
        new Point(p.x - 1, p.y - 1),
        new Point(p.x, p.y - 1),
        new Point(p.x - 1, p.y),
        new Point(p.x, p.y)
    };

    foreach (var origin in origins)
    {
        // Check bounds
        if (origin.x >= 0 && origin.y >= 0 && origin.x < width - 1 && origin.y < height - 1)
        {
            if (getValueAtPoint(origin) == val &&
                getValueAtPoint(new Point(origin.x + 1, origin.y)) == val &&
                getValueAtPoint(new Point(origin.x, origin.y + 1)) == val &&
                getValueAtPoint(new Point(origin.x + 1, origin.y + 1)) == val)
            {
                return true;
            }
        }
    }
    return false;
}

public bool IsPartOfMatch(Point p)
{
    int val = getValueAtPoint(p);
    if (val <= 0) return false;

    int countLeft = 0;
    int countRight = 0;
    for (int x = p.x - 1; x >= 0; x--)
        if (getValueAtPoint(new Point(x, p.y)) == val) countLeft++;
        else break;
    for (int x = p.x + 1; x < width; x++)
        if (getValueAtPoint(new Point(x, p.y)) == val) countRight++;
        else break;
    if (countLeft + countRight + 1 >= 3) return true;

    int countUp = 0;
    int countDown = 0;
    for (int y = p.y - 1; y >= 0; y--)
        if (getValueAtPoint(new Point(p.x, y)) == val) countUp++;
        else break;
    for (int y = p.y + 1; y < height; y++)
        if (getValueAtPoint(new Point(p.x, y)) == val) countDown++;
        else break;
    if (countUp + countDown + 1 >= 3) return true;

    return false;
}

public void ShowHintsAndCount()
{
    ClearHighlights();
    currentPossibleMoves = GetPossibleMoves();
    SpawnHighlights(currentPossibleMoves);
    currentMoveCount = currentPossibleMoves.Count;
    Debug.Log("Total possible moves: " + currentMoveCount);
}

public void PossibleMoveCount()
    {
        currentPossibleMoves = GetPossibleMoves();
        currentMoveCount = currentPossibleMoves.Count;
    }

    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(pieceBase + (pieceDouble * p.x), -pieceBase - (pieceDouble * p.y));
    }
}

[System.Serializable]
public class Node
{
    public int value; //0 = blank, -1 = hole
    public Point index;
    NodePiece piece;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public NodePiece GetPiece()
    {
        return piece;
    }
}


[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o; two = t;
    }

    public NodePiece getOtherPiece(NodePiece p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }
}