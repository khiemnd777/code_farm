// UnityInterop.jslib
mergeInto(LibraryManager.library, {
  sendToUnity: function(gameObjectNamePtr, methodNamePtr, argumentPtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var methodName = UTF8ToString(methodNamePtr);
    var argument = UTF8ToString(argumentPtr);

    unityInstance.SendMessage(gameObjectName, methodName, argument);
  },
  SendCoroutineComplete: function(gameObjectNamePtr, coroutineNamePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var coroutineName = UTF8ToString(coroutineNamePtr);
    
    if (window.onCoroutineComplete) {
      window.onCoroutineComplete(gameObjectName, coroutineName);
    }
  },
  SendReturnedStringValueCoroutineComplete: function(gameObjectNamePtr, coroutineNamePtr, stringValuePtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var coroutineName = UTF8ToString(coroutineNamePtr);
    var stringValue = UTF8ToString(stringValuePtr);

    if (window.onCoroutineComplete) {
      window.onCoroutineComplete(gameObjectName, coroutineName, stringValue);
    }
  }
});