using System;
using System.Runtime.InteropServices;

namespace LoreSoft.Shared.Interop
{
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct POINT
  {
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }
  }
}