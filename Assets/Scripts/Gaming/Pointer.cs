using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer {
    public int X { get; private set; }
    public int Y { get; private set; }

    public static readonly Pointer p11 = new Pointer(1, 1);
    public static readonly Pointer p12 = new Pointer(1, 2);
    public static readonly Pointer p13 = new Pointer(1, 3);
    public static readonly Pointer p14 = new Pointer(1, 4);
    public static readonly Pointer p21 = new Pointer(2, 1);
    public static readonly Pointer p22 = new Pointer(2, 2);
    public static readonly Pointer p23 = new Pointer(2, 3);
    public static readonly Pointer p24 = new Pointer(2, 4);
    public static readonly Pointer p31 = new Pointer(3, 1);
    public static readonly Pointer p32 = new Pointer(3, 2);
    public static readonly Pointer p33 = new Pointer(3, 3);
    public static readonly Pointer p34 = new Pointer(3, 4);
    public static readonly Pointer p41 = new Pointer(4, 1);
    public static readonly Pointer p42 = new Pointer(4, 2);
    public static readonly Pointer p43 = new Pointer(4, 3);
    public static readonly Pointer p44 = new Pointer(4, 4);

    public Pointer(int x, int y) {
        this.X = x;
        this.Y = y;
    }

    /// <summary>判断两点是否相邻 </summary>
    public bool IsNear(Pointer pointer) {
        return (Mathf.Abs(X - pointer.X) + Mathf.Abs(Y - pointer.Y) == 1);
    }

    /// <summary>右对角线对称 </summary>
    public Pointer GetRightMirror(int matrix) {
        return new Pointer(Y, X);
    }

    /// <summary>左对角线对称 </summary>
    public Pointer GetLeftMirror(int matrix) {
        return new Pointer(matrix + 1 - Y, matrix + 1 - X);
    }

    /// <summary>X对称 </summary>
    public Pointer GetXMirror(int matrix) {
        return new Pointer(matrix + 1 - X, Y);
    }

    /// <summary>Y对称 </summary>
    public Pointer GetYMirror(int matrix) {
        return new Pointer(X, matrix + 1 - Y);
    }

    public override string ToString() {
        return "X:" + X + " Y:" + Y;
    }


}