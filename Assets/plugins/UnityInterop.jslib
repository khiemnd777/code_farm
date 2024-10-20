// UnityInterop.jslib
mergeInto(LibraryManager.library, {
  sendToUnity: function(gameObjectNamePtr, methodNamePtr, argumentPtr) {
    // Convert pointers to strings
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var methodName = UTF8ToString(methodNamePtr);
    var argument = UTF8ToString(argumentPtr);

    // Use unityInstance.SendMessage to communicate with Unity
    unityInstance.SendMessage(gameObjectName, methodName, argument);
  },
  SendCoroutineComplete: function(gameObjectNamePtr, coroutineNamePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var coroutineName = UTF8ToString(coroutineNamePtr);
    if (window.onCoroutineComplete) {
      window.onCoroutineComplete(gameObjectName, coroutineName);
    }
  }
});