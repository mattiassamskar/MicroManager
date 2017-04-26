using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MicroManager
{
  public static class WindowPlacementHandler
  {
    private const int SwShownormal = 1;
    private const int SwShowminimized = 2;

    public static void LoadWindowPlacement(Window window)
    {
      try
      {
        var wp = Settings.Default.WindowPlacement;
        wp.length = Marshal.SizeOf(typeof(WindowPlacement));
        wp.flags = 0;
        wp.showCmd = (wp.showCmd == SwShowminimized ? SwShownormal : wp.showCmd);
        var hwnd = new WindowInteropHelper(window).Handle;
        SetWindowPlacement(hwnd, ref wp);
      }
      catch
      {
        // ignored
      }
    }

    public static void SaveWindowPlacement(Window window)
    {
      WindowPlacement wp;
      var hwnd = new WindowInteropHelper(window).Handle;
      GetWindowPlacement(hwnd, out wp);
      Settings.Default.WindowPlacement = wp;
      Settings.Default.Save();
    }

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct WindowPlacement
  {
    public int length;
    public int flags;
    public int showCmd;
    public Point minPosition;
    public Point maxPosition;
    public Rect normalPosition;
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct Point
  {
    public int X;
    public int Y;

    public Point(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct Rect
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rect(int left, int top, int right, int bottom)
    {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
    }
  }
}