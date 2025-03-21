#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class InitializationCallbacks : AndroidJavaProxy
    {
        private readonly IInitializationListener _listener;

        internal InitializationCallbacks(IInitializationListener listener) : base(AndroidConstants
            .InitializationListener)
        {
            _listener = listener;
        }

        public void onInitializeSuccess()
        {
            _listener.OnInitializeSuccess();
        }
        
        public void onInitializeFailed(AndroidJavaObject error)
        {
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnInitializeFailed(new AdException(description, message, caused));
        }
    }
}

#endif