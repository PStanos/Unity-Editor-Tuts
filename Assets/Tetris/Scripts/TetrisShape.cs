using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TetrisShape : ScriptableObject
{
    public class Point : System.Object
    {
        public Point()
        {
            this.x = 0;
            this.y = 0;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Point pt1, Point pt2)
        {
            if (System.Object.ReferenceEquals(pt1, null) && System.Object.ReferenceEquals(pt2, null))
            {
                return true;
            }
            else if (System.Object.ReferenceEquals(pt1, null) || System.Object.ReferenceEquals(pt2, null))
            {
                return false;
            }

            if(pt1.x == pt2.x && pt1.y == pt2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Point pt1, Point pt2)
        {
            if (System.Object.ReferenceEquals(pt1, null) && System.Object.ReferenceEquals(pt2, null))
            {
                return false;
            }
            else if (System.Object.ReferenceEquals(pt1, null) || System.Object.ReferenceEquals(pt2, null))
            {
                return true;
            }

            if (pt1.x == pt2.x && pt1.y == pt2.y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Point operator +(Point pt1, Point pt2)
        {
            return new Point(pt1.x + pt2.x, pt1.y + pt2.y);
        }

        private int x;
        private int y;

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
    };

    public enum Shape
    {
        Line,
        LeftL,
        RightL,
        Square,
        SquiggleL,
        SquiggleR,
        TShape,
        NumShapes
    }

    public enum Orientation
    {
        North,
        East,
        South,
        West,
        NumOrientations
    }

    public enum RotateDirection
    {
        Clockwise,
        CounterClockwise
    }

    public Shape currentShape;

    private Point topLeft;

    public Point TopLeft
    {
        get
        {
            return topLeft;
        }

        set
        {
            topLeft = value;
        }
    }

    private Orientation orientation;

    private Texture tileTexture;

    public Texture TileTexture
    {
        get
        {
            return tileTexture;
        }

        set
        {
            tileTexture = value;
        }
    }

    public bool firstRun = true;

    void Awake()
    {
        if (firstRun)
        {
            System.Random rand = new System.Random();

            currentShape = (Shape)System.Enum.ToObject(typeof(Shape), rand.Next(0, (int)Shape.NumShapes));

            orientation = Orientation.North;

            firstRun = false;
        }
    }

    // Relative to top-most and then left-most tile of shape
    private List<Point> GetShapeFormOffsets(Orientation orientation)
    {
        List<Point> offsets = new List<Point>();

        switch (currentShape)
        {
            case Shape.Line:
                switch (orientation)
                {
                    case Orientation.North:
                    case Orientation.South:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(0, 2));
                        offsets.Add(new Point(0, 3));

                        break;
                    case Orientation.East:
                    case Orientation.West:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(2, 0));
                        offsets.Add(new Point(3, 0));

                        break;
                }

                break;
            case Shape.LeftL:
                switch (orientation)
                {
                    case Orientation.North:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(0, 2));

                        break;
                    case Orientation.South:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(0, 2));
                        offsets.Add(new Point(-1, 2));

                        break;
                    case Orientation.East:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(2, 0));
                        offsets.Add(new Point(2, 1));

                        break;
                    case Orientation.West:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(2, 1));

                        break;
                }

                break;
            case Shape.RightL:
                switch (orientation)
                {
                    case Orientation.North:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(0, 2));
                        offsets.Add(new Point(1, 2));

                        break;
                    case Orientation.South:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(1, 2));

                        break;
                    case Orientation.East:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(2, 0));

                        break;
                    case Orientation.West:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(-1, 1));
                        offsets.Add(new Point(-2, 1));

                        break;
                }

                break;
            case Shape.Square:
                switch (orientation)
                {
                    case Orientation.North:
                    case Orientation.East:
                    case Orientation.South:
                    case Orientation.West:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(0, 1));

                        break;
                }

                break;
            case Shape.SquiggleL:
                switch (orientation)
                {
                    case Orientation.North:
                    case Orientation.South:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(1, 2));

                        break;
                    case Orientation.East:
                    case Orientation.West:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(-1, 1));

                        break;
                }

                break;
            case Shape.SquiggleR:
                switch (orientation)
                {
                    case Orientation.North:
                    case Orientation.South:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(-1, 1));
                        offsets.Add(new Point(-1, 2));

                        break;
                    case Orientation.East:
                    case Orientation.West:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(2, 1));

                        break;
                }

                break;
            case Shape.TShape:
                switch (orientation)
                {
                    case Orientation.North:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(1, 1));
                        offsets.Add(new Point(0, 2));

                        break;
                    case Orientation.South:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(-1, 1));
                        offsets.Add(new Point(0, 2));

                        break;
                    case Orientation.East:
                        offsets.Add(new Point(1, 0));
                        offsets.Add(new Point(2, 0));
                        offsets.Add(new Point(1, 1));

                        break;
                    case Orientation.West:
                        offsets.Add(new Point(0, 1));
                        offsets.Add(new Point(-1, 1));
                        offsets.Add(new Point(1, 1));

                        break;
                }

                break;
        }

        return offsets;
    }

    public List<Point> GetShapeForm(Orientation orientation)
    {
        List<Point> form = new List<Point>();

        form.Add(topLeft);

        List<Point> offsets = GetShapeFormOffsets(orientation);

        foreach (Point offset in offsets)
        {
            form.Add(new Point(topLeft.X + offset.X, topLeft.Y + offset.Y));
        }

        return form;
    }

    public List<Point> GetShapeForm()
    {
        List<Point> form = new List<Point>();

        form.Add(topLeft);

        List<Point> offsets = GetShapeFormOffsets();

        foreach (Point offset in offsets)
        {
            form.Add(new Point(topLeft.X + offset.X, topLeft.Y + offset.Y));
        }

        return form;
    }

    private List<Point> GetShapeFormOffsets()
    {
        return GetShapeFormOffsets(this.orientation);
    }

    public Point FindLowestTile()
    {
        Point lowPoint = null;
        foreach (Point pt in GetShapeForm())
        {
            if(lowPoint == null || pt.Y > lowPoint.Y)
            {
                lowPoint = pt;
            }
        }

        return lowPoint;
    }

    private void ShiftTopLeftPosition(RotateDirection dir, Orientation prevOrientation)
    {
        switch (currentShape)
        {
            case Shape.Line:
                switch(dir)
                {
                    case RotateDirection.Clockwise:

                        switch(prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 2, TopLeft.Y + 2);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 2);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 2, TopLeft.Y - 1);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 2, TopLeft.Y + 1);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 2, TopLeft.Y - 2);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 2);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;
                        }

                        break;
                }

                break;

            case Shape.LeftL:
                switch (dir)
                {
                    case RotateDirection.Clockwise:

                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;
                                
                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;
                                
                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y);
                                break;
                                
                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;
                               
                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y);
                                break;
                        }

                        break;
                }

                break;

            case Shape.RightL:
                switch (dir)
                {
                    case RotateDirection.Clockwise:

                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X + 2, TopLeft.Y);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;
                                
                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X - 2, TopLeft.Y);
                                break;
                        }

                        break;
                }

                break;

            case Shape.SquiggleL:
                switch (dir)
                {
                    case RotateDirection.Clockwise:

                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y + 1);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y - 1);        
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y);
                                break;
                        }

                        break;
                }

                break;

            case Shape.SquiggleR:
                switch (dir)
                {
                    case RotateDirection.Clockwise:

                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 2, TopLeft.Y + 1);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 2, TopLeft.Y);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 2, TopLeft.Y);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 2, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y);
                                break;
                        }

                        break;
                }

                break;

            case Shape.TShape:
                switch (dir)
                {
                    case RotateDirection.Clockwise:

                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;
                        }

                        break;
                    case RotateDirection.CounterClockwise:
                        switch (prevOrientation)
                        {
                            case Orientation.North:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;

                            case Orientation.East:
                                TopLeft = new Point(TopLeft.X + 1, TopLeft.Y - 1);
                                break;

                            case Orientation.South:
                                TopLeft = new Point(TopLeft.X - 1, TopLeft.Y + 1);
                                break;

                            case Orientation.West:
                                TopLeft = new Point(TopLeft.X, TopLeft.Y);
                                break;
                        }

                        break;
                }

                break;
        }
    }

    public bool Rotate(RotateDirection dir)
    {
        ShiftTopLeftPosition(dir, orientation);

        switch (orientation)
        {
            case Orientation.North:
                switch (dir)
                {
                    case RotateDirection.Clockwise:
                        orientation = Orientation.East;

                        break;
                    case RotateDirection.CounterClockwise:
                        orientation = Orientation.West;

                        break;
                }

                break;
            case Orientation.East:
                switch (dir)
                {
                    case RotateDirection.Clockwise:
                        orientation = Orientation.South;

                        break;
                    case RotateDirection.CounterClockwise:
                        orientation = Orientation.North;

                        break;
                }

                break;
            case Orientation.South:
                switch (dir)
                {
                    case RotateDirection.Clockwise:
                        orientation = Orientation.West;

                        break;
                    case RotateDirection.CounterClockwise:
                        orientation = Orientation.East;

                        break;
                }

                break;
            case Orientation.West:
                switch (dir)
                {
                    case RotateDirection.Clockwise:
                        orientation = Orientation.North;

                        break;
                    case RotateDirection.CounterClockwise:
                        orientation = Orientation.South;

                        break;
                }

                break;
        }

        return true;
    }
}
