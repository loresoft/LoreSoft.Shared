using System;
using System.Runtime.InteropServices;

namespace LoreSoft.Shared.Interop
{
  // WINDOWPLACEMENT stores the position, size, and state of a window
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct WINDOWPLACEMENT
  {
    public int length;
    public int flags;
    public int showCmd;
    public POINT minPosition;
    public POINT maxPosition;
    public RECT normalPosition;
  }
}