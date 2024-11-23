using System;

public enum LabyrinthThings
{
  wall = 0,
  floor,
}

public class LabyrinthSettings
{
  public static bool isMazeMode = false;

  public static LabyrinthThings thing = LabyrinthThings.wall;

  public static void SwitchThing(LabyrinthThings thing, Action<LabyrinthThings> postSwitch)
  {
    LabyrinthSettings.thing = thing;
    postSwitch?.Invoke(thing);
  }
}