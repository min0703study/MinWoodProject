
public abstract class BaseData<T> where T : BaseData<T>, new()
{
	private static T instance;

	public static T Instance { 
	get {
		if (instance == null) 
		{
			instance = new T();
			instance.init();
		}
		
		return instance;
	}}
	
	protected static bool isInit = false;

	protected virtual void init()
	{
		if(isInit == true)
			return;
			
		isInit = true;
	}
}
