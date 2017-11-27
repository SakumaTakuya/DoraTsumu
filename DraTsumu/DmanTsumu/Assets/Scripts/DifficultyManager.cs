using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
	[SerializeField] private Text _levelText;
	[SerializeField] private EraserSetting _eraserSetting;
	[SerializeField] private EmitterSetting _emitterSetting;

	private readonly Queue<Setting> _settings = new Queue<Setting>();

	private void Start()
	{
		_settings.Enqueue(new Setting("ふつう",30,5,0.5f));
		_settings.Enqueue(new Setting("かんたん",30,2,0.75f));
		_settings.Enqueue(new Setting("むずかしい",60,10,0.25f));
		ChangeLevel();
	}

	public void ChangeLevel()
	{
		var setting = _settings.Dequeue();
		_levelText.text = setting.Name;
		_emitterSetting.InitialAmount = setting.Amount;
		_emitterSetting.Duration = setting.Duration;
		_eraserSetting.Rate = setting.Rate;
		_settings.Enqueue(setting);
	}
	
	public struct Setting
	{
		public string Name;
		public int Amount;
		public float Duration;
		public float Rate;

		public Setting(string name, int amount, float duration, float rate)
		{
			Name = name;
			Amount = amount;
			Duration = duration;
			Rate = rate;
		}
	}
}
