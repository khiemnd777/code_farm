using UnityEngine;

public class FieldUtils
{
  private static Vector3 baseVector
  {
    get => CommonConstants.FIELD_BASE_VECTOR;
  }

  private static int maxRange = CommonConstants.FIELD_MAX_RANGE;

  public static Field ToField(Vector3 position)
  {
    var basePos = baseVector;
    var x = position.x - basePos.x;
    var y = -position.y - basePos.y;

    return new Field
    {
      x = Mathf.RoundToInt(x),
      y = Mathf.RoundToInt(y)
    };
  }

  public static Vector3 ToVector(Field field)
  {
    var basePos = baseVector;
    var x = field.x + basePos.x;
    var y = -field.y - basePos.y;

    return new Vector3(x, y, 0f);
  }

  public static bool IsBeingInField(Vector3 position, int maxWidth, int maxHeight)
  {
    var field = ToField(position);
    return field.x >= 0 && field.x <= maxWidth && field.y >= 0 && field.y <= maxHeight;
  }
}
