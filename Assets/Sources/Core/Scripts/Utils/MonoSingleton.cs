using UnityEngine;
using System.Collections;

/// <summary>
/// Mono singleton.
/// When using it, don't use Awake(), override Init()!!
/// </summary>
/// <see cref="http://wiki.unity3d.com/index.php?title=Singleton#Generic_Based_Singleton_for_MonoBehaviours"/>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T m_instance = null;
	
	public static bool HasInstance { get { return m_instance != null; } }
	
	public static T Instance
	{
		get
		{
			return m_instance;
		}
	}
	
	//'protected' modifier assures that no descendant silently overrides Awake() instead of Init()
	protected void Awake()
	{
		if ( m_instance == null )
		{
			m_instance = this as T;
			m_instance.Init();
		}
	}
 
	// This function is called when the instance is used the first time
	// Put all the initializations you need here, as you would do in Awake
	public virtual void Init(){}
	
	//Can be used to explicitly release the singleton in case of E.g. releasing the level
	private void ReleaseInstance()
	{
		OnReleaseInstance();
		m_instance = null;
	}

	protected virtual void OnReleaseInstance() { }

	protected void OnDestroy()
	{
		ReleaseInstance();
	}
	
	// Make sure the instance isn't referenced anymore when the user quit, just in case.
	private void OnApplicationQuit()
	{
		ReleaseInstance();
	}
	
	//!!! Fix for the script execution order
	//see http://answers.unity3d.com/questions/217941/onenable-awake-start-order.html
	void OnEnable()
	{
	}
}
