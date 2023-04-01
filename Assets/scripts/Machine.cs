using UnityEngine;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Machine : MonoBehaviour
{
  public string pyExecutedFilePath;
  public string typeName;

  [System.NonSerialized]
  public bool isRunning;

  [System.NonSerialized]
  public bool isReseting;

  ScriptEngine _engine = PythonEngine.instance;
  ScriptSource _source = null;
  ScriptScope _scope = null;

  Dictionary<string, object> _registeredVariables = new Dictionary<string, object>();

  List<string> _yieldFunctions = new List<string>();

  Coroutine _mainStartFnCoroutine;

  Vector3 _currentPosition;

  Quaternion _currentAngle;

  bool _isRotating;

  Map _map;

  // Start is called before the first frame update
  protected virtual void Start()
  {
    RegisterVariableOrFunction("move_forward", new System.Func<IEnumerator>(MoveForward), true);
    RegisterVariableOrFunction("rotate_clockwise", new System.Func<IEnumerator>(RotateClockwise), true);
    RegisterVariableOrFunction("rotate_counterclockwise", new System.Func<IEnumerator>(RotateCounterclockwise), true);
    RegisterVariableOrFunction("move_to", new System.Func<int, int, IEnumerator>(MoveTo), true);
    RegisterVariableOrFunction("get_field", new System.Func<Field>(GetField));
    RegisterVariableOrFunction("log", new System.Action<object>(Log));

    RegisterVariables();

    Init();

    // Init world map
    _map = new Map();
  }

  // Update is called once per frame
  protected virtual void Update()
  {

  }

  public virtual void Run()
  {
    if (isRunning)
    {
      return;
    }

    if (isReseting)
    {
      return;
    }

    isRunning = true;

    Compile();
    Execute();

    _mainStartFnCoroutine = StartCoroutine("StartMainFn");
  }

  public virtual void Stop()
  {
    if (isReseting)
    {
      return;
    }

    isRunning = false;

    if (_mainStartFnCoroutine != null)
    {
      StopCoroutine(_mainStartFnCoroutine);
    }

    isReseting = true;

    StartCoroutine("Reset");
  }

  protected virtual void RegisterVariables()
  {

  }

  protected virtual void Init()
  {
    if (_engine != null)
    {
      _scope = _engine.CreateScope();

      if (_registeredVariables.Any())
      {
        _registeredVariables
          .ToList()
          .ForEach((x) => _scope.SetVariable(x.Key, x.Value));
      }
    }
  }

  void Compile()
  {
    if (_source != null)
    {
      _source = null;
    }

    var script = HandleScript();

    _source = _engine.CreateScriptSourceFromString(script);
  }

  void Execute()
  {
    if (_source != null)
    {
      _source.Execute(_scope);
    }
  }

  string HandleScript()
  {
    var script = File.ReadAllText($@"{CommonConstants.PY_SCRIPT_PATH}{pyExecutedFilePath}");

    script = TranscriptUtils.RemoveBuiltinModule(script, typeName.ToLowerInvariant());
    script = TranscriptUtils.RemoveAsyncKeyword(script);
    script = TranscriptUtils.ReplaceAllYieldFunc(script, _yieldFunctions);
    script = TranscriptUtils.ReplaceAwaitKeywordToYield(script);
    script = script.Trim();

    return script;
  }

  protected void RegisterVariableOrFunction(string name, object value, bool isYieldFunc = false)
  {
    _registeredVariables.Add(name, value);

    if (isYieldFunc)
    {
      _yieldFunctions.Add(name);
    }
  }

  IEnumerator StartMainFn()
  {
    var mainFn = _scope.GetVariable<System.Func<IEnumerator>>("__main");

    yield return StartCoroutine(mainFn.Invoke());

    isRunning = false;
  }

  protected virtual IEnumerator Reset()
  {
    // Reset rotation
    if (_isRotating)
    {
      var startingAngle = transform.rotation;
      var finalAngle = _currentAngle;
      var elapsedTime = 0f;

      while (elapsedTime <= 1f)
      {
        elapsedTime += Time.deltaTime / .35f;
        transform.rotation = Quaternion.Lerp(startingAngle, finalAngle, elapsedTime);
        yield return null;
      }
    }

    // Reset position
    {
      var startingPos = transform.position;
      var finalPos = _currentPosition;

      var elapsedTime = 0f;

      while (elapsedTime <= 1f)
      {
        elapsedTime += Time.deltaTime / .5f;
        transform.position = Vector3.Lerp(startingPos, finalPos, elapsedTime);
        yield return null;
      }
    }

    isReseting = false;

    yield return new WaitForSeconds(.2f);
  }

  protected virtual IEnumerator MoveTo(int x, int y)
  {
    var start = FieldUtils.ToField(transform.position);
    var pathFound = _map.FindPath(start, new Field
    {
      x = x,
      y = y
    });
    var traversePath = _map.TraversePath(pathFound);

    foreach (var path in traversePath)
    {
      print($"{{x: {path.field.x}, y: {path.field.y}, direction: {path.direction}}}");
      var pathDirections = Map.ConvertToPathDirection(path.direction, transform.rotation.eulerAngles.z);
      foreach (var pathDirection in pathDirections)
      {
        switch (pathDirection)
        {
          case PathDirection.Forward:
            {
              yield return StartCoroutine(MoveForward());
            }
            break;
          case PathDirection.TurnLeft:
            {
              yield return StartCoroutine(RotateCounterclockwise());
            }
            break;
          case PathDirection.TurnRight:
            {
              yield return StartCoroutine(RotateClockwise());
            }
            break;
        }
      }
      yield return new WaitForSeconds(.125f);
    }
  }

  protected virtual IEnumerator MoveForward()
  {
    print("Move forward");
    var startingPos = transform.position;

    _currentPosition = startingPos;

    var finalPos = transform.position + transform.right;

    if (FieldUtils.IsBeingInField(finalPos))
    {
      var elapsedTime = 0f;

      while (elapsedTime <= 1f)
      {
        elapsedTime += Time.deltaTime / .35f;
        transform.position = Vector3.Lerp(startingPos, finalPos, elapsedTime);
        yield return null;
      }

      _currentPosition = finalPos;
    }

    yield return new WaitForSeconds(.125f);
  }

  protected virtual IEnumerator RotateClockwise()
  {
    print("Rotate clockwise");
    var startingAngle = transform.rotation;

    _currentAngle = startingAngle;

    var finalAngle = startingAngle * Quaternion.Euler(0f, 0f, -90f);
    var elapsedTime = 0f;

    _isRotating = true;

    while (elapsedTime <= 1f)
    {
      elapsedTime += Time.deltaTime / .35f;
      transform.rotation = Quaternion.Lerp(startingAngle, finalAngle, elapsedTime);
      yield return null;
    }

    _isRotating = false;

    _currentAngle = finalAngle;

    yield return null;
  }

  protected virtual IEnumerator RotateCounterclockwise()
  {
    print("Rotate counterclockwise");
    var startingAngle = transform.rotation;
    _currentAngle = startingAngle;

    var finalAngle = startingAngle * Quaternion.Euler(0f, 0f, 90f);
    var elapsedTime = 0f;

    _isRotating = true;

    while (elapsedTime <= 1f)
    {
      elapsedTime += Time.deltaTime / .35f;
      transform.rotation = Quaternion.Lerp(startingAngle, finalAngle, elapsedTime);
      yield return null;
    }

    _isRotating = false;

    yield return null;
  }

  protected Field GetField()
  {
    return FieldUtils.ToField(_currentPosition);
  }

  void Log(object message)
  {
    print(message);
  }
}
