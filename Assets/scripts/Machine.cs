using UnityEngine;
//using Microsoft.Scripting.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class Machine : MonoBehaviour, IPointerClickHandler
{
    [DllImport("__Internal")]
    private static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    [SerializeField]
    ObstacleSegmentController _obstacleSegmentController;

    public IDE ide;

    public string pyExecutedFilePath;
    public string typeName;
    [SerializeField]
    PropertiesCanvas _propertiesCanvasPrefab;

    public List<MachineComponent> machineComponentPrefabs;
    List<MachineComponent> _machineComponents = new List<MachineComponent>();

    public List<MachineComponent> machineComponents;

    [SerializeField]
    Transform _machineComponentContainer;

    [System.NonSerialized]
    public bool isRunning;

    [System.NonSerialized]
    public bool isReseting;

    //ScriptEngine _engine = PythonEngine.instance;
    //ScriptSource _source = null;
    //ScriptScope _scope = null;

    Dictionary<string, object> _registeredVariables = new();

    List<string> _yieldFunctions = new();

    Coroutine _mainStartFnCoroutine;

    Vector3 _currentPosition;

    Quaternion _currentAngle;

    bool _isRotating;

    Map _map;

    FieldGrid _fieldGrid;

    float _energy;

    public float energy
    {
        get => _energy; set => _energy = value;
    }

    [SerializeField]
    float _energyForMoving;
    [SerializeField]
    float _energyForRotating;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //AssemblyMachineComponents();

        //RegisterVariableOrFunction("move_forward", new System.Func<IEnumerator>(MoveForward), true);
        //RegisterVariableOrFunction("rotate_clockwise", new System.Func<IEnumerator>(RotateClockwise), true);
        //RegisterVariableOrFunction("rotate_counterclockwise", new System.Func<IEnumerator>(RotateCounterclockwise), true);
        //RegisterVariableOrFunction("move_to", new System.Func<int, int, IEnumerator>(MoveTo), true);
        //RegisterVariableOrFunction("get_field", new System.Func<Field>(GetField));
        //RegisterVariableOrFunction("log", new System.Action<object>(Log));
        //RegisterVariableOrFunction("energy", _energy);

        //RegisterVariables();

        Init();

        // Init energy
        _energy = 100000f;

        _fieldGrid = FindObjectOfType<FieldGrid>();
        // Init world map
        _map = new Map(_fieldGrid.initialWidth, _fieldGrid.initialHeight);
        ide = new IDE();

        SendCoroutineComplete(this.name, "Loaded");
    }

    void AssemblyMachineComponents()
    {
        if (machineComponentPrefabs != null && machineComponentPrefabs.Any())
        {
            for (int i = 0; i < machineComponentPrefabs.Count; i++)
            {
                var componentPrefab = machineComponentPrefabs.ElementAt(i);
                if (componentPrefab)
                {
                    var component = Instantiate<MachineComponent>(componentPrefab, Vector3.zero, Quaternion.identity, _machineComponentContainer);
                    component.machine = this;
                    //component.RegisterVariables();
                    _machineComponents.Add(component);
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //if (PropertiesCanvasUtils.propertiesCanvas != null)
        //{
        //    Destroy(PropertiesCanvasUtils.propertiesCanvas.gameObject);
        //}

        //PropertiesCanvasUtils.propertiesCanvas = Instantiate<PropertiesCanvas>(_propertiesCanvasPrefab);
        //PropertiesCanvasUtils.propertiesCanvas.propertiesPanel.machine = this;
    }

    void OnApplicationQuit()
    {
        if (ide != null)
        {
            ide.Close();
        }
    }

    public virtual void OpenIDE()
    {
        if (ide != null)
        {
            ide.Open(GetExecutedFilePath());
        }
    }

    public virtual void CloseIDE()
    {
        if (ide != null)
        {
            ide.Close();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    //Called when there is an exception
    void LogCallback(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            if (stackTrace.Contains("IronPython"))
            {
                print(message);
                isRunning = false;
            }
        }
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
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

        //Compile();
        //Execute();

        //_mainStartFnCoroutine = StartCoroutine("StartMainFn");
    }

    public virtual void Stop()
    {
        if (isReseting)
        {
            return;
        }

        isReseting = true;
        isRunning = false;

        //if (_mainStartFnCoroutine != null)
        //{
        //    StopCoroutine(_mainStartFnCoroutine);
        //}


        StartCoroutine("Reset");
    }

    protected virtual void RegisterVariables()
    {

    }

    protected virtual void Init()
    {
        //if (_engine != null)
        //{
        //    _scope = _engine.CreateScope();

        //    if (_registeredVariables.Any())
        //    {
        //        _registeredVariables
        //            .ToList()
        //            .ForEach((x) => _scope.SetVariable(x.Key, x.Value));
        //    }
        //}
    }

    void Compile()
    {
        //if (_source != null)
        //{
        //    _source = null;
        //}

        //var script = HandleScript();

        //_source = _engine.CreateScriptSourceFromString(script);
    }

    void Execute()
    {
        //if (_source != null)
        //{
        //    _source.Execute(_scope);
        //}
    }

    string GetExecutedFilePath()
    {
        return $@"{CommonConstants.PY_SCRIPT_PATH}{pyExecutedFilePath}";
    }

    string HandleScript()
    {
        var script = File.ReadAllText(GetExecutedFilePath());

        script = TranscriptUtils.RemoveBuiltinModule(script, typeName.ToLowerInvariant());
        script = TranscriptUtils.RemoveAsyncKeyword(script);
        script = TranscriptUtils.ReplaceAllYieldFunc(script, _yieldFunctions);
        script = TranscriptUtils.ReplaceAwaitKeywordToYield(script);
        script = script.Trim();

        return script;
    }

    public void RegisterVariableOrFunction(string name, object value, bool isYieldFunc = false)
    {
        _registeredVariables.Add(name, value);

        if (isYieldFunc)
        {
            _yieldFunctions.Add(name);
        }
    }

    //IEnumerator StartMainFn()
    //{
    //    var mainFn = _scope.GetVariable<System.Func<IEnumerator>>("__main");

    //    yield return StartCoroutine(mainFn.Invoke());

    //    isRunning = false;
    //}

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

        SendCoroutineComplete(this.name, "Stop");
    }

    public bool ConsumeEnergy(float consuming)
    {
        _energy -= consuming;

        if (_energy < 0f)
        {
            _energy = 0f;
            return false;
        }
        return true;
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
                            var startingPos = transform.position;

                            _currentPosition = startingPos;

                            var finalPos = transform.position + transform.right;

                            if (FieldUtils.IsBeingInField(finalPos, _fieldGrid.initialWidth, _fieldGrid.initialHeight))
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
                            if (!ConsumeEnergy(_energyForMoving))
                            {
                                Stop();
                                yield break;
                            }
                            //yield return new WaitForSeconds(.125f);
                            yield return null;
                        }
                        break;
                    case PathDirection.TurnLeft:
                        {
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

                            _currentAngle = finalAngle;

                            if (!ConsumeEnergy(_energyForRotating))
                            {
                                Stop();
                                yield break;
                            }

                            yield return null;
                        }
                        break;
                    case PathDirection.TurnRight:
                        {
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

                            if (!ConsumeEnergy(_energyForRotating))
                            {
                                Stop();
                                yield break;
                            }

                            yield return null;
                        }
                        break;
                }
            }
        }
        yield return null;
    }

    protected virtual IEnumerator MoveForward()
    {
        if (!isRunning)
        {
            yield break;
        }
        var startingPos = transform.position;

        _currentPosition = startingPos;

        var finalPos = transform.position + transform.right;


        if (FieldUtils.IsBeingInField(finalPos, _fieldGrid.initialWidth, _fieldGrid.initialHeight))
        {
            if (_obstacleSegmentController && !_obstacleSegmentController.HasObstacleAhead(transform))
            {
                var elapsedTime = 0f;
                while (elapsedTime <= 1f)
                {
                    if (!isRunning)
                    {
                        yield return null;
                        break;
                    }
                    elapsedTime += Time.deltaTime / .35f;
                    var newPosition = Vector3.Lerp(startingPos, finalPos, elapsedTime);
                    newPosition.z = -1f;
                    transform.position = newPosition;
                    yield return null;
                }

                finalPos.z = -1f;
                _currentPosition = finalPos;
            }
        }
        else
        {
            yield return null;
        }

        SendCoroutineComplete(this.name, "MoveForward");
    }

    protected virtual IEnumerator RotateClockwise()
    {
        if (!isRunning)
        {
            yield break;
        }
        var startingAngle = transform.rotation;

        _currentAngle = startingAngle;

        var finalAngle = startingAngle * Quaternion.Euler(0f, 0f, -90f);
        var elapsedTime = 0f;

        _isRotating = true;

        while (elapsedTime <= 1f)
        {
            if (!isRunning)
            {
                break;
            }
            elapsedTime += Time.deltaTime / .125f;
            transform.rotation = Quaternion.Lerp(startingAngle, finalAngle, elapsedTime);
            yield return null;
        }

        _isRotating = false;

        _currentAngle = finalAngle;

        yield return null;

        //if (!ConsumeEnergy(_energyForRotating))
        //{
        //    Stop();
        //    SendCoroutineComplete(this.name, "RotateClockwise");
        //    yield break;
        //}
        SendCoroutineComplete(this.name, "RotateClockwise");
    }

    protected virtual IEnumerator RotateCounterclockwise()
    {
        if (!isRunning)
        {
            yield break;
        }
        print("Rotate counterclockwise");
        var startingAngle = transform.rotation;
        _currentAngle = startingAngle;

        var finalAngle = startingAngle * Quaternion.Euler(0f, 0f, 90f);
        var elapsedTime = 0f;

        _isRotating = true;

        while (elapsedTime <= 1f)
        {
            if (!isRunning)
            {
                break;
            }
            elapsedTime += Time.deltaTime / .125f;
            transform.rotation = Quaternion.Lerp(startingAngle, finalAngle, elapsedTime);
            yield return null;
        }

        _isRotating = false;

        _currentAngle = finalAngle;

        yield return null;

        //if (!ConsumeEnergy(_energyForRotating))
        //{
        //    Stop();
        //    SendCoroutineComplete(this.name, "RotateCounterclockwise");
        //    yield break;
        //}
        SendCoroutineComplete(this.name, "RotateCounterclockwise");
    }

    protected virtual IEnumerator Remove()
    {
        var duration = 0.5f; // Duration of fade and scale out
        var elapsedTime = 0f;

        Vector3 originalScale = this.transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;
            this.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        yield return null;
        Destroy(this.gameObject);

        yield return null;
        SendCoroutineComplete(this.name, "Remove");
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
