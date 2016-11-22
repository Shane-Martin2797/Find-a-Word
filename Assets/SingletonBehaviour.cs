using UnityEngine;
using System.Collections;

public abstract class SingletonBehaviour<T> : MonoBehaviour 
where T : MonoBehaviour
{
	public static T Instance { get; private set; }

	public virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			OnSingletonAwake();
		}
		else
		{
			Debug.LogWarning("Singleton " + typeof(T).ToString() + " Already Exists");
			Destroy(this.gameObject);
		}
	}

	public virtual void OnDestroy()
	{
		if (Instance == this)
		{
			OnSingletonDestroy();
			Instance = null;
		}
	}

	public virtual void OnSingletonAwake()
	{
	}

	public virtual void OnSingletonDestroy()
	{
	}
}
