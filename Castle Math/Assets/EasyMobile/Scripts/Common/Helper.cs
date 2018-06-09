using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyMobile.Internal
{
    /// <summary>
    /// Helper class dedicated for internal use by EM only.
    /// Note that we initialize this class singleton in the Awake function of <see cref="EM_PrefabManager"/>.
    /// </summary>
    [AddComponentMenu("")]
    internal class Helper : MonoBehaviour
    {
        public static readonly DateTime UnixEpoch =
            DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

        /// <summary>
        /// The singleton instance of this class. Upon the first access, a new game object
        /// attached with this class instance will be created if none exists. Therefore the first
        /// access to this property should always be made from the main thread. In general, we should
        /// always access this instace from the main thread, unless we're certain that a game object
        /// has been created before (e.g. by using the <see cref="IsInitialize"/> method).  
        /// </summary>
        /// <value>The instance.</value>
        public static Helper Instance
        { 
            get
            {
                if (mInstance == null)
                {
                    Init();
                }

                return mInstance;
            }
        }

        // The singleton instance.
        private static Helper mInstance;

        // List of actions to run on the game thread
        private static List<Action> mToMainThreadQueue = new List<Action>();

        // Member variable used to copy actions from mToMainThreadQueue and
        // execute them on the game thread.
        List<Action> localToMainThreadQueue = new List<Action>();

        // Flag indicating whether there's any action queued to be run on game thread.
        private volatile static bool mIsToMainThreadQueueEmpty = true;

        // List of actions to be invoked upon application pause event.
        private static List<Action<bool>> mPauseCallbackQueue =
            new List<Action<bool>>();

        // List of actions to be invoked upon application focus event.
        private static List<Action<bool>> mFocusCallbackQueue =
            new List<Action<bool>>();

        // Flag indicating whether this is a dummy instance.
        private static bool mIsDummy = false;

        #region Public API

        /// <summary>
        /// Creates the singleton instance of this class and a game object that carries it.
        /// This must be called once from the main thread.
        /// You can call it before accessing the <see cref="Instance"/> singleton,
        /// though <see cref="Instance"/> automatically calls this method if needed, so you can bypass this
        /// and access that property directly, provided that you're on the main thread.
        /// Also note that this method does nothing if initialization has been done before,
        /// so it's safe to call it multiple times.
        /// </summary>
        public static void Init()
        {
            if (mInstance != null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                var go = new GameObject("EM_Helper");
                go.hideFlags = HideFlags.HideAndDontSave;
                mInstance = go.AddComponent<Helper>();
                DontDestroyOnLoad(go);
            }
            else
            {
                mInstance = new Helper();
                mIsDummy = true;
            }
        }

        /// <summary>
        /// Internally calls <see cref="Init"/>. Basically we're just giving
        /// the method another name to make things clearer. 
        /// </summary>
        public static void InitIfNeeded()
        {
            Init();
        }

        /// <summary>
        /// Determines if a game object attached with this class singleton instance exists.
        /// 
        /// </summary>
        /// <returns><c>true</c> if is initialized; otherwise, <c>false</c>.</returns>
        public static bool IsInitialized()
        {
            return mInstance != null;
        }

        /// <summary>
        /// Gets the app installation timestamp (in local timezone). This requires the EasyMobile prefab instance
        /// to be added to the first scene of the game. If such instance is not found, the
        /// Epoch time (01/01/1970) will be returned instead.
        /// </summary>
        /// <returns>The app installation time in local timezone.</returns>
        public static DateTime GetAppInstallationTime()
        {
            if (EM_Manager.Instance != null)
            {
                return EM_Manager.Instance.GetAppInstallationTimestamp();
            }
            else
            {
                return Helper.UnixEpoch.ToLocalTime();
            }
        }

        /// <summary>
        /// Determines if the current build is a development build.
        /// </summary>
        /// <returns><c>true</c> if is development build; otherwise, <c>false</c>.</returns>
        public static bool IsUnityDevelopmentBuild()
        {
            #if DEBUG || DEVELOPMENT_BUILD
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Starts a coroutine from non-MonoBehavior objects.
        /// </summary>
        /// <param name="routine">Routine.</param>
        public static void RunCoroutine(IEnumerator routine)
        {
            if (routine != null)
                Instance.StartCoroutine(routine);
        }

        /// <summary>
        /// Stops a coroutine from non-MonoBehavior objects.
        /// </summary>
        /// <param name="routine">Routine.</param>
        public static void EndCoroutine(IEnumerator routine)
        {
            if (routine != null)
                Instance.StopCoroutine(routine);
        }

        /// <summary>
        /// Converts the specified action to one that runs on the main thread.
        /// The converted action will be invoked upon the next Unity Update event.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <returns>The main thread.</returns>
        /// <param name="act">Act.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Action<T> ToMainThread<T>(Action<T> act)
        {
            if (act == null)
            {
                return delegate
                {
                };
            }

            return (arg) => RunOnMainThread(() => act(arg));
        }

        /// <summary>
        /// Converts the specified action to one that runs on the main thread.
        /// The converted action will be invoked upon the next Unity Update event.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <returns>The main thread.</returns>
        /// <param name="act">Act.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        public static Action<T1, T2> ToMainThread<T1, T2>(Action<T1, T2> act)
        {
            if (act == null)
            {
                return delegate
                {
                };
            }

            return (arg1, arg2) => RunOnMainThread(() => act(arg1, arg2));
        }

        /// <summary>
        /// Converts the specified action to one that runs on the main thread.
        /// The converted action will be invoked upon the next Unity Update event.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <returns>The main thread.</returns>
        /// <param name="act">Act.</param>
        /// <typeparam name="T1">The 1st type parameter.</typeparam>
        /// <typeparam name="T2">The 2nd type parameter.</typeparam>
        /// <typeparam name="T3">The 3rd type parameter.</typeparam>
        public static Action<T1, T2, T3> ToMainThread<T1, T2, T3>(Action<T1, T2, T3> act)
        {
            if (act == null)
            {
                return delegate
                {
                };
            }

            return (arg1, arg2, arg3) => RunOnMainThread(() => act(arg1, arg2, arg3));
        }

        /// <summary>
        /// Schedules the specifies action to be run on the main thread (game thread).
        /// The action will be invoked upon the next Unity Update event.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <param name="action">Action.</param>
        public static void RunOnMainThread(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (mIsDummy)
            {
                return;
            }

            // Note that this requires the singleton game object to be created first (for Update() to run).
            if (!IsInitialized())
            {
                Debug.LogError("Using RunOnMainThread without initializing Helper.");
                return;
            }

            lock (mToMainThreadQueue)
            {
                mToMainThreadQueue.Add(action);
                mIsToMainThreadQueueEmpty = false;
            }
        }

        /// <summary>
        /// Adds a callback that is invoked upon the Unity event OnApplicationFocus.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <see cref="OnApplicationFocus"/>
        /// <param name="callback">Callback.</param>
        public static void AddFocusCallback(Action<bool> callback)
        {
            if (!mFocusCallbackQueue.Contains(callback))
            {
                mFocusCallbackQueue.Add(callback);
            }
        }

        /// <summary>
        /// Removes the callback from the list to call upon OnApplicationFocus event.
        /// is called.
        /// </summary>
        /// <returns><c>true</c>, if focus callback was removed, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        public static bool RemoveFocusCallback(Action<bool> callback)
        {
            return mFocusCallbackQueue.Remove(callback);
        }

        /// <summary>
        /// Adds a callback that is invoked upon the Unity event OnApplicationPause.
        /// Only works if initilization has done (<see cref="Init"/>).
        /// </summary>
        /// <see cref="OnApplicationPause"/>
        /// <param name="callback">Callback.</param>
        public static void AddPauseCallback(Action<bool> callback)
        {
            if (!mPauseCallbackQueue.Contains(callback))
            {
                mPauseCallbackQueue.Add(callback);
            }
        }

        /// <summary>
        /// Removes the callback from the list to invoke upon OnApplicationPause event.
        /// is called.
        /// </summary>
        /// <returns><c>true</c>, if focus callback was removed, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        public static bool RemovePauseCallback(Action<bool> callback)
        {
            return mPauseCallbackQueue.Remove(callback);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given value is null.
        /// </summary>
        /// <returns>The input value.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T NullArgumentTest<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
        
            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the indicated parameter name if the given value is null.
        /// </summary>
        /// <returns>The argument test.</returns>
        /// <param name="value">Value.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T NullArgumentTest<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }

        /// <summary>
        /// Constructs a UTC <see cref="DateTime"/> from the milliseconds since Unix Epoch.
        /// </summary>
        /// <returns>The DateTime value in UTC.</returns>
        /// <param name="millisSinceEpoch">Milliseconds since Epoch.</param>
        public static DateTime FromMillisSinceUnixEpoch(long millisSinceEpoch)
        {
            return UnixEpoch.Add(TimeSpan.FromMilliseconds(millisSinceEpoch));
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to miliseconds.
        /// </summary>
        /// <returns>The milliseconds.</returns>
        /// <param name="span">Time span.</param>
        public static long ToMilliseconds(TimeSpan span)
        {
            double millis = span.TotalMilliseconds;

            if (millis > long.MaxValue)
            {
                return long.MaxValue;
            }

            if (millis < long.MinValue)
            {
                return long.MinValue;
            }

            return Convert.ToInt64(millis);
        }

        /// <summary>
        /// Stores a <see cref="DateTime"/> as string to <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="time">Time.</param>
        /// <param name="ppkey">PlayerPrefs key to store the value.</param>
        public static void StoreTime(string ppkey, DateTime time)
        {
            PlayerPrefs.SetString(ppkey, time.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the stored string in the <see cref="PlayerPrefs"/>, converts it to a <see cref="DateTime"/> and returns.
        /// If no value was stored previously, the given default time is returned.
        /// </summary>
        /// <returns>The time.</returns>
        /// <param name="ppkey">PlayPrefs key to retrieve the value.</param>
        public static DateTime GetTime(string ppkey, DateTime defaultTime)
        {
            string storedTime = PlayerPrefs.GetString(ppkey, string.Empty);

            if (!string.IsNullOrEmpty(storedTime))
                return DateTime.FromBinary(Convert.ToInt64(storedTime));
            else
                return defaultTime;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <returns>The method name.</returns>
        /// <param name="func">Func.</param>
        public static string GetMethodName(Delegate method)
        {
            return method.Method.Name;
        }

        /// <summary>
        /// Creates an object of the output type and copies values of all public 
        /// properties and fields of the input object to the matching public
        /// properties and fields of the output object.
        /// This method is intended to be a convenient replacement for manual copying codes
        /// when there is a need for converting objects of two similar classes.
        /// </summary>
        /// <returns>The object data.</returns>
        /// <param name="inObj">In object.</param>
        /// <typeparam name="TIn">The 1st type parameter.</typeparam>
        /// <typeparam name="TOut">The 2nd type parameter.</typeparam>
        public static TOut CopyObjectData<TIn, TOut>(TIn inObj) where TOut : new()
        {
            if (inObj is Enum)
                throw new NotImplementedException("Input must be class not enum!");

            if (inObj == null)
                return default(TOut);

            TOut outObj = new TOut();

            Type fromType = inObj.GetType();
            Type toType = outObj.GetType();

            // Copy properties
            PropertyInfo[] inProps = fromType.GetProperties();

            foreach (PropertyInfo inProp in inProps)
            {
                PropertyInfo outProp = toType.GetProperty(inProp.Name);

                if (outProp != null && outProp.CanWrite)
                {
                    object value = inProp.GetValue(inObj, null);
                    outProp.SetValue(outObj, value, null);
                }
            }

            // Copy fields
            FieldInfo[] inFields = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo inField in inFields)
            {
                FieldInfo outField = toType.GetField(inField.Name);

                if (outField != null && outField.IsPublic)
                {
                    object value = inField.GetValue(inObj);
                    outField.SetValue(outObj, value);
                }
            }

            return outObj;
        }

        /// <summary>
        /// Gets the key associated with the specified value in the given dictionary.
        /// </summary>
        /// <returns>The key for value.</returns>
        /// <param name="dict">Dict.</param>
        /// <param name="val">Value.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TVal">The 2nd type parameter.</typeparam>
        public static TKey GetKeyForValue<TKey, TVal>(IDictionary<TKey, TVal> dict, TVal val)
        {
            foreach (KeyValuePair<TKey, TVal> entry in dict)
            {
                if (entry.Value.Equals(val))
                {
                    return entry.Key;
                }
            }

            return default(TKey);
        }

        /// <summary>
        /// Removes all leading and trailing white-spaces from the input and returns.
        /// </summary>
        /// <returns>The trimmed identifier.</returns>
        /// <param name="id">Identifier.</param>
        public static string AutoTrimId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return id.Trim();
        }

        #endregion // Public API

        #region Internal Stuff

        // Destroys the proxy game object that carries the instance of this class if one exists.
        static void DestroyProxy()
        {
            if (mInstance == null)
                return;

            if (!mIsToMainThreadQueueEmpty || mPauseCallbackQueue.Count > 0 || mFocusCallbackQueue.Count > 0)
                return;

            if (!mIsDummy)
                Destroy(mInstance.gameObject);

            mInstance = null;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnDisable()
        {
            if (mInstance == this)
            {
                mInstance = null;
            }
        }

        void Update()
        {
            if (mIsDummy || mIsToMainThreadQueueEmpty)
            {
                return;
            }

            // Copy the shared queue into a local queue while
            // preventing other threads to modify it.
            localToMainThreadQueue.Clear();
            lock (mToMainThreadQueue)
            {
                localToMainThreadQueue.AddRange(mToMainThreadQueue);
                mToMainThreadQueue.Clear();
                mIsToMainThreadQueueEmpty = true;
            }

            // Execute queued actions (from local queue).
            for (int i = 0; i < localToMainThreadQueue.Count; i++)
            {
                localToMainThreadQueue[i].Invoke();
            }
        }

        void OnApplicationFocus(bool focused)
        {
            for (int i = 0; i < mFocusCallbackQueue.Count; i++)
            {
                var act = mFocusCallbackQueue[i];
                try
                {
                    act(focused);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception executing action in OnApplicationFocus:" +
                        e.Message + "\n" + e.StackTrace);
                }
            }
        }

        void OnApplicationPause(bool paused)
        {
            for (int i = 0; i < mPauseCallbackQueue.Count; i++)
            {
                var act = mPauseCallbackQueue[i];
                try
                {
                    act(paused);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception executing action in OnApplicationPause:" +
                        e.Message + "\n" + e.StackTrace);
                }
            }
        }

        #endregion // Internal Stuff
    }
}