using UnityEngine;
using System.Collections;
using UnityEditor;

public class TetrisWindow : EditorWindow
{
    private TetrisBoard board;
    private bool areKeysUp = true;

    private float timeToRepeatPress = 0.2f;
    private float timeOfPress = 0.0f;

    [MenuItem("Totally Legit Work Stuff/Not Tetris")]
    static void Init()
    {
        TetrisWindow window = (TetrisWindow)EditorWindow.GetWindow(typeof(TetrisWindow));
        window.title = "Tetris";
        EditorApplication.update += window.Simulate;
    }

    void Awake()
    {
        if (board == null)
        {
            board = ScriptableObject.CreateInstance<TetrisBoard>();
            board.parentWindow = this;
            Repaint();
        }
    }

    void OnGUI()
    {
        if(board != null)
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.keyDown:
                    if ((e.keyCode == KeyCode.A || e.keyCode == KeyCode.LeftArrow) && areKeysUp)
                    {
                        board.MoveShapeLeft();
                        areKeysUp = false;
                    }
                    else if ((e.keyCode == KeyCode.D || e.keyCode == KeyCode.RightArrow) && areKeysUp)
                    {
                        board.MoveShapeRight();
                        areKeysUp = false;
                    }
                    else if (e.keyCode == KeyCode.J || e.keyCode == KeyCode.Z)
                    {
                        board.RotateShape(TetrisShape.RotateDirection.CounterClockwise);
                    }
                    else if (e.keyCode == KeyCode.L || e.keyCode == KeyCode.C)
                    {
                        board.RotateShape(TetrisShape.RotateDirection.Clockwise);
                    }
                    else if(e.keyCode == KeyCode.Space && areKeysUp)
                    {
                        board.DropShape();
                        areKeysUp = false;
                    }

                    break;
                case EventType.keyUp:
                    areKeysUp = true;
                    break;
            }

            board.Draw();
        }
        else
        {
            if(GUILayout.Button("New Game?"))
            {
                board = ScriptableObject.CreateInstance<TetrisBoard>();
                board.parentWindow = this;
            }
        }
    }

    void Simulate()
    {
        if (!areKeysUp)
        {
            if (Time.realtimeSinceStartup - timeOfPress >= timeToRepeatPress)
            {
                areKeysUp = true;
                timeOfPress = Time.realtimeSinceStartup;
            }
        }
        else
        {
            timeOfPress = Time.realtimeSinceStartup;
        }

        if (board != null)
        {
            board.BoardUpdate(Time.realtimeSinceStartup);

            if(board.IsGameOver())
            {
                board = null;
                Repaint();
            }
        }
    }
}
