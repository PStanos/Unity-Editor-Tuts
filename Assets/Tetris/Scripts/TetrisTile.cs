using UnityEngine;
using UnityEditor;
using System.Collections;

public class TetrisTile : ScriptableObject
{
    private Texture colorTexture = null;

    public Texture ColorTexture
    {
        get
        {
            return colorTexture;
        }

        set
        {
            colorTexture = value;
        }
    }

    private readonly float width = 20.0f;
    private readonly float height = 20.0f;

    public bool isFixed = false;

    GUIStyle tileButtonStyle = new GUIStyle
    {
        padding = new RectOffset(0, 0, 0, 0)
    };

    public float Width
    {
        get
        {
            return width;
        }
    }

    public float Height
    {
        get
        {
            return height;
        }
    }

    public bool IsFilled()
    {
        return colorTexture != null;
    }

    public void Draw(float left, float top, Texture text = null)
    {
        if (text != null && colorTexture == null)
        {
            GUI.Button(new Rect(left, top, width, height), text, tileButtonStyle);
        }
        else if (colorTexture != null)
        {
            GUI.Button(new Rect(left, top, width, height), colorTexture, tileButtonStyle);
        }
        else
        {
            GUI.Button(new Rect(left, top, width, height), TetrisBoard.emptyTexture, tileButtonStyle);
        }
    }
}
