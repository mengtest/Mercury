using UnityEngine;

/// <summary>
/// 可挂载单例
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    protected static T _instance;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }

            _instance = FindObjectOfType<T>();
            if (_instance)
            {
                return _instance;
            }

            var obj = new GameObject {name = typeof(T).Name};
            _instance = obj.AddComponent<T>();
            return _instance;
        }
        protected set => _instance = value;
    }

    private void Awake() { OnAwake(); }

    protected virtual void OnAwake()
    {
        if (!_instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() { OnStart(); }

    protected virtual void OnStart() { }

    private void Update() { OnUpdate(); }

    protected virtual void OnUpdate() { }
}