using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LocationDirection
{
  None, Up, Down, Left, Right
}

public enum PathDirection
{
  None, Forward, Backward, TurnRight, TurnLeft
}

public class Location
{
  public Field field;
  public int F;
  public int G;
  public int H;
  public Location previousLocation;
  public LocationDirection direction;
}

public class MapCell
{
  public Field field;
  public bool obstacled;
}

public class Map
{
  public List<MapCell> map = new List<MapCell>();

  public Map(int width, int height)
  {
    for (var y = 0; y < width; y++)
    {
      for (var x = 0; x < height; x++)
      {
        var cell = new MapCell
        {
          field = new Field
          {
            x = x,
            y = y
          }
        };
        map.Add(cell);
      }
    }
  }

  public Location FindPath(Field fieldA, Field fieldB)
  {
    Location current = null;

    var start = new Location { field = fieldA };
    var target = new Location { field = fieldB };
    var openList = new List<Location>();
    var closedList = new List<Location>();

    var g = 0;

    // start by adding the original position to the open list
    openList.Add(start);

    while (openList.Count > 0)
    {
      // get the square with the lowest F score
      var lowest = openList.Min(l => l.F);
      current = openList.First(l => l.F == lowest);

      // add the current square to the closed list
      closedList.Add(current);

      // remove it from the open list
      openList.Remove(current);

      // if we added the destination to the closed list, we've found a path
      if (closedList.FirstOrDefault(l => l.field.x == target.field.x && l.field.y == target.field.y) != null)
        break;

      var adjacentSquares = GetWalkableAdjacentSquares(current.field.x, current.field.y, map, fieldB);
      g++;

      foreach (var adjacentSquare in adjacentSquares)
      {
        // if this adjacent square is already in the closed list, ignore it
        if (closedList.FirstOrDefault(l => l.field.x == adjacentSquare.field.x
                && l.field.y == adjacentSquare.field.y) != null)
          continue;

        // if it's not in the open list...
        if (openList.FirstOrDefault(l => l.field.x == adjacentSquare.field.x
                && l.field.y == adjacentSquare.field.y) == null)
        {
          // compute its score, set the parent
          adjacentSquare.G = g;
          adjacentSquare.H = ComputeHScore(adjacentSquare.field.x, adjacentSquare.field.y, target.field.x, target.field.y);
          adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
          adjacentSquare.previousLocation = current;

          // and add it to the open list
          openList.Insert(0, adjacentSquare);
        }
        else
        {
          // test if using the current G score makes the adjacent square's F score
          // lower, if yes update the parent because it means it's a better path
          if (g + adjacentSquare.H < adjacentSquare.F)
          {
            adjacentSquare.G = g;
            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
            adjacentSquare.previousLocation = current;
          }
        }
      }
    }

    return current;
  }

  public List<Location> TraversePath(Location pathFound)
  {
    var path = new List<Location>();
    var current = pathFound;
    while (current != null)
    {
      path.Add(current);
      var previousLocation = current.previousLocation;

      if (previousLocation != null)
      {
        var currentDirection = current.field.x == previousLocation.field.x
              ? current.field.y == previousLocation.field.y
                  ? LocationDirection.None
                  : current.field.y > previousLocation.field.y ? LocationDirection.Down : LocationDirection.Up
              : current.field.x > previousLocation.field.x ? LocationDirection.Right : LocationDirection.Left;
        previousLocation.direction = currentDirection;
      }

      current = current.previousLocation;
    }

    path.Reverse();

    return path;
  }

  public static List<PathDirection> ConvertToPathDirection(LocationDirection locationDirection, float targetZ)
  {
    var pathDirection = new List<PathDirection>();
    var z = targetZ > 180f ? targetZ - 360f : targetZ;
    switch (locationDirection)
    {
      case LocationDirection.Up:
        {
          if (z == 0f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == -90f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 90f)
          {
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 180f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
        }
        break;
      case LocationDirection.Down:
        {
          if (z == 0f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == -90f)
          {
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 90f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 180f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
        }
        break;
      case LocationDirection.Left:
        {
          if (z == 0f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == -90f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 90f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 180f)
          {
            pathDirection.Add(PathDirection.Forward);
          }
        }
        break;
      case LocationDirection.Right:
        {
          if (z == 0f)
          {
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == -90f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 90f)
          {
            pathDirection.Add(PathDirection.TurnRight);
            pathDirection.Add(PathDirection.Forward);
          }
          else if (z == 180f)
          {
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.TurnLeft);
            pathDirection.Add(PathDirection.Forward);
          }
        }
        break;
    }
    return pathDirection;
  }

  static List<Location> GetWalkableAdjacentSquares(int x, int y, List<MapCell> map, Field target)
  {
    var proposedLocations = new List<Location>
    {
      new Location { field = new Field{ x = x, y = y - 1 } },
      new Location { field = new Field{ x = x, y = y + 1 } },
      new Location { field = new Field{ x = x - 1, y = y } },
      new Location { field = new Field{ x = x + 1, y = y } },
    };

    return proposedLocations.Where(l =>
    {
      var found = map.FirstOrDefault(m => m.field.y == l.field.y && m.field.x == l.field.x);
      if (found == null)
      {
        return false;
      }
      return !found.obstacled || found.field.y == target.y && found.field.x == target.x;
    }).ToList();
  }

  static int ComputeHScore(int x, int y, int targetX, int targetY)
  {
    return Mathf.Abs(targetX - x) + Mathf.Abs(targetY - y);
  }
}
