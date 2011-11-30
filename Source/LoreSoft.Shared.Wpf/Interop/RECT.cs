﻿using System;
using System.Runtime.InteropServices;

namespace LoreSoft.Shared.Interop
{
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
      this.Left = left;
      this.Top = top;
      this.Right = right;
      this.Bottom = bottom;
    }
  }
}