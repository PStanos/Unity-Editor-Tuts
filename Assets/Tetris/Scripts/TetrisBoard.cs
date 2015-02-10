using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TetrisBoard : ScriptableObject
{
    private TetrisShape shape;

    private readonly int rowCount = 22;
    private readonly int colCount = 10;

    private readonly float borderSize = 15.0f;

    private List<List<TetrisTile>> tiles = new List<List<TetrisTile>>();

    public static List<Texture> tileTextures = new List<Texture>();
    public static Texture emptyTexture;

    private float dropDelay = 0.2f;
    private float dropStartTime = -1.0f;

    private int numRowsCleared = 0;

    public EditorWindow parentWindow;

    public bool workMode = true;
    private string workModeString = "";

    void Awake()
    {
        if (tiles.Count == 0 && rowCount != 0)
        {
            for (int row = 0; row < rowCount; row++)
            {
                List<TetrisTile> newRow = new List<TetrisTile>();
                for (int col = 0; col < colCount; col++)
                {
                    newRow.Add(ScriptableObject.CreateInstance<TetrisTile>());
                }

                tiles.Add(newRow);
            }
        }

        if (tileTextures.Count == 0)
        {
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/blue.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/cyan.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/green.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/orange.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/purple.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/red.png"));
            tileTextures.Add(Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/yellow.png"));

            emptyTexture = Resources.LoadAssetAtPath<Texture>("Assets/Tetris/Textures/white.png");
        }
        
        if (shape == null)
        {
            SpawnShape();
        }
    }

    public bool IsGameOver()
    {
        return tiles[GetSpawnPosition().Y][GetSpawnPosition().X].isFixed;
    }

    private void SpawnShape()
    {
        if(IsGameOver())
        {
            return;
        }

        System.Random rand = new System.Random();
        shape = ScriptableObject.CreateInstance<TetrisShape>();
        shape.TopLeft = GetSpawnPosition();
        shape.TileTexture = tileTextures[rand.Next(0, tileTextures.Count)];

        if (parentWindow != null)
        {
            parentWindow.Repaint();
        }
    }

    public void Draw()
    {
        if(workMode)
        {
            EditorGUILayout.FloatField("Flux Radiance", 1.0f);
            EditorGUILayout.ColorField("Flux Appearance", Color.blue);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.IntField("Shrink Factor", 3);
            EditorGUILayout.IntField("Growth Factor", 25);

            EditorGUILayout.EndHorizontal();

            workModeString = EditorGUILayout.TextField("Config String", workModeString);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.IntField("Flux Capacitance", 1000);
            EditorGUILayout.IntField("Voltage Reactivity", 555);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.IntField("Uncontrolled Factorization Level", 1);
            EditorGUILayout.IntField("Evil Incarnate", 666);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.TextField("Summary Matrix", "Failed to summarize: time space anomaly!");

            if (workModeString.ToLower() == "tetris")
            {
                workMode = false;
            }

            return;
        }

        EditorGUI.LabelField(new Rect(borderSize * 2.0f + tiles[0].Count * tiles[0][0].Width, 0.0f, 100.0f, 100.0f), "Rows cleared: " + numRowsCleared.ToString());

        int rowNum = 0;
        foreach (List<TetrisTile> row in tiles)
        {
            // Top two rows are hidden
            if (rowNum >= 0)
            {
                GUILayout.Space(borderSize);

                int colNum = 0;
                foreach (TetrisTile tile in row)
                {
                    bool isPartOfShape = false;

                    foreach (TetrisShape.Point pt in shape.GetShapeForm())
                    {
                        if (pt == new TetrisShape.Point(colNum, rowNum))
                        {
                            isPartOfShape = true;
                            break;
                        }
                    }

                    tile.Draw(borderSize + colNum * tile.Width, rowNum * tile.Height, isPartOfShape ? shape.TileTexture : emptyTexture);
                    colNum++;
                }

                GUILayout.Space(borderSize);
            }
            rowNum++;
        }
    }

    private TetrisShape.Point GetSpawnPosition()
    {
        return new TetrisShape.Point(colCount / 2, 0);
    }

    public void BoardUpdate(float time)
    {
        if (workMode)
            return;

        if (dropStartTime < 0.0f)
        {
            dropStartTime = time;
        }

        if (time - dropStartTime >= dropDelay)
        {
            if(shape != null)
            {
                if (CanShapeMoveDown())
                {
                    shape.TopLeft = new TetrisShape.Point(shape.TopLeft.X, shape.TopLeft.Y + 1);
                    parentWindow.Repaint();
                }
                else
                {
                    foreach (TetrisShape.Point pt in shape.GetShapeForm())
                    {
                        tiles[pt.Y][pt.X].ColorTexture = shape.TileTexture;
                        tiles[pt.Y][pt.X].isFixed = true;
                    }

                    SpawnShape();
                    ClearLines();
                }
            }

            dropStartTime = time;
        }
    }

    private bool CanShapeMoveRight()
    {
        foreach (TetrisShape.Point pt in shape.GetShapeForm())
        {
            if (pt.X + 1 >= colCount || tiles[pt.Y][pt.X + 1].IsFilled())
            {
                return false;
            }
        }

        return true;
    }
     
    private bool CanShapeMoveDown()
    {
        foreach (TetrisShape.Point pt in shape.GetShapeForm())
        {
            if (pt.Y + 1 >= rowCount || tiles[pt.Y + 1][pt.X].IsFilled())
            {
                return false;
            }
        }

        return true;
    }

    private bool CanShapeMoveLeft()
    {
        foreach (TetrisShape.Point pt in shape.GetShapeForm())
        {
            if (pt.X - 1 < 0 || tiles[pt.Y][pt.X - 1].IsFilled())
            {
                return false;
            }
        }

        return true;
    }

    public void MoveShapeLeft()
    {
        if (CanShapeMoveLeft())
        {
            shape.TopLeft = new TetrisShape.Point(shape.TopLeft.X - 1, shape.TopLeft.Y);
            parentWindow.Repaint();
        }
    }

    public void MoveShapeRight()
    {
        if (CanShapeMoveRight())
        {
            shape.TopLeft = new TetrisShape.Point(shape.TopLeft.X + 1, shape.TopLeft.Y);
            parentWindow.Repaint();
        }
    }

    public void ClearLines()
    {
        bool lineCleared = true;

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                if (!tiles[row][col].IsFilled())
                {
                    lineCleared = false;
                    break;
                }
            }

            if (lineCleared)
            {
                for (int moveRow = row - 1; moveRow >= 0; moveRow--)
                {
                    tiles[moveRow + 1] = tiles[moveRow];
                }

                numRowsCleared++;
                dropDelay -= dropDelay / 10.0f;
            }
            else
            {
                lineCleared = true;
            }
        }
    }

    public void RotateShape(TetrisShape.RotateDirection dir)
    {
        do
        {
            shape.Rotate(dir);
        }
        while (!ShapeCanFit());

        parentWindow.Repaint();
    }

    private bool ShapeCanFit()
    {
        List<TetrisShape.Point> points = shape.GetShapeForm();

        foreach (TetrisShape.Point point in points)
        {
            if (point.X < 0 || point.X >= tiles[0].Count || point.Y < 0 || point.Y >= tiles.Count)
            {
                return false;
            }
        }

        int x = 0;
        int y = 0;
        foreach (List<TetrisTile> row in tiles)
        {
            foreach (TetrisTile tile in row)
            {
                if (tile.isFixed)
                {
                    foreach(TetrisShape.Point point in points)
                    {
                        if(new TetrisShape.Point(x, y) == point)
                        {
                            return false;
                        }
                    }
                }

                x++;
            }

            y++;
            x = 0;
        }

        return true;
    }

    public void DropShape()
    {
        while(CanShapeMoveDown())
        {
            shape.TopLeft = new TetrisShape.Point(shape.TopLeft.X, shape.TopLeft.Y + 1);
        }

        foreach (TetrisShape.Point pt in shape.GetShapeForm())
        {
            tiles[pt.Y][pt.X].ColorTexture = shape.TileTexture;
            tiles[pt.Y][pt.X].isFixed = true;
        }

        SpawnShape();
        ClearLines();

        parentWindow.Repaint();
    }
}
