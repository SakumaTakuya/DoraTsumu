using UnityEngine;

public class ParticleSetting : MonoBehaviour
{

	[SerializeField] private float _lifetime = 0;
	
	// Use this for initialization
	private void Start () 
	{
		Destroy(gameObject,_lifetime);	
	}
}
