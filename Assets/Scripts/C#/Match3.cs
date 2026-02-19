using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public string seedString;

    [Header("Prefabs")]
    [SerializeField]public GameObject nodePiece;
    [SerializeField]public GameObject killedPiece;
    
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

    int width = 9;
    int height = 14;
    int[] fills;
    Node[,] board;
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;

    System.Random random;

    void Start()
    {
    }

    public void StartGame()
    {
        fills = new int[width];
        int seedHashCode = GetOrCreateSeedHashCode();
        // Initialize the random with the seed hash code
        random = new System.Random(seedHashCode);
        //string seed = getRandomSeed();
        //random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();

        IntializeBoard();
        VerifyBoard();
        InstantiateBaord();

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

        foreach(Point dir in directions) // Checking if 2 or more pieces in directions
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

            if (same > 1) // If more than 1 of same piece in direction, then its a match
                AddPoints(ref connected, line); //Add these points to overarching connected list
            
        }

        for(int i = 0; i < 2; i++) // Checking if in middle of two same pieces
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i+2]) };
            foreach (Point next in check) // Check both sides of piece, if same add them to list
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

        for(int i = 0; i < 4; i++) // Check for 2x2
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if(next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };
            foreach (Point pnt in check) // Check all sides of piece, if same add them to list
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

        if(main) // Checks for matches along current match
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

            if (wasFlipped) // If we flipped to make this update
            {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }

            if (connected.Count == 0) // If we didnt make a match
            {
                if (wasFlipped) // If we flipped
                    FlipPieces(piece.index, flippedPiece.index, false); // Flip back
                    Debug.LogWarning("There was no match");
            }
            else  // If we made a match
            {
                int matchCount = connected.Count;
                Debug.LogWarning("There was a match of " + matchCount);
                AddScore(matchCount);
                foreach (Point pnt in connected)  // Remove the node pieces connected
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

            flipped.Remove(flip); // Remove the Flip after update
            update.Remove(piece);
        }
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
                if (val != 0) continue; // If not a hole, do nothing
                for (int ny = (y-1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if (nextVal != -1) // If not hit end but not 0, fill hole
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();

                        // Set hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        // Replace hole
                        got.SetPiece(null);
                    }
                    else // Use dead pieces or create new ones to fill holes
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
    
    // Method to replace the entire sprite array
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

    // Method to get or generate the seed hash code
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
                // If parsing fails, generate a new seed and update
                string seed = getRandomSeed();
                int seedHashCode = seed.GetHashCode();
                seedString = seedHashCode.ToString();
                return seedHashCode;
            }
        }
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
    public int value; //0 = blank, 1 = cube, 2 = sphere, 3 = cylinder, 4 = pyramid, 5 = diamond, -1 = hole
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