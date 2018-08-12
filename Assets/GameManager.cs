using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int FishCollected { get; private set; }
	public int MinionFishCounter { get; private set; }
	public int PlatformFishCounter { get; private set; }

	[SerializeField]
	private GameObject fishPrefab;
	[SerializeField]
	private GameObject minionPrefab;
	[SerializeField]
	private GameObject badMinionPrefab;
	[SerializeField]
	private GameObject projectorPrefab;
	[SerializeField]
	private Platform platform;
	[SerializeField]
	private Transform fishesParent;
	[SerializeField]
	private King king;
	[SerializeField]
	private FollowTarget cameraTarget;
	[SerializeField]
	private string[] penguinFacts;
	[SerializeField]
	private TMPro.TMP_Text collectedDisplay;
	[SerializeField]
	private TMPro.TMP_Text highScoreDisplay;
	[SerializeField]
	private TMPro.TMP_Text factsDisplay;
	[SerializeField]
	private List<Transform> platforms = new List<Transform>();
	[SerializeField]
	private List<Transform> outPlatforms = new List<Transform>();
	[SerializeField]
	private Animator panelAnimator;
	[SerializeField]
	private AudioSource mainSong;
	[SerializeField]
	private Image sliderBGImage;
	[SerializeField]
	private List<Transform> innerPlatforms = new List<Transform>();

	private bool restartShown;
	private Color originalSliderBGImage;
	private Transform platformTransform;
	private float fireNextFish;
	private int highScore = 0;
	private int nextMinionSpawn = 5;
	private int nextPlatformRemove = 10;
	private static GameManager _instance;

	private void Awake()
	{
		_instance = this;
	}

	// Use this for initialization
	private void Start()
	{
#if UNITY_EDITOR
		QualitySettings.vSyncCount = 0;
#endif
		platformTransform = platform.transform;
		highScore = PlayerPrefs.GetInt("HighScore", 0);
		highScoreDisplay.text = "High Score " + highScore;
		Time.timeScale = 0;
		originalSliderBGImage = sliderBGImage.color;
	}

	// Update is called once per frame
	private void Update()
	{
		if (Time.time > fireNextFish)
		{
			FireFish();
		}
		if (MinionFishCounter >= nextMinionSpawn)
		{
			MinionFishCounter -= nextMinionSpawn;
			Vector3 position = platforms[Random.Range(0, platforms.Count)].position + new Vector3(0, 2, 0);
			Instantiate(minionPrefab, position, Quaternion.identity);
			AudioManager.PlayMinionSpawned();
			nextMinionSpawn = 5 + platform.Effectors;
			if (platform.Effectors % 2 == 0)
			{
				position = platforms[Random.Range(0, platforms.Count)].position + new Vector3(0, 2, 0);
				Instantiate(badMinionPrefab, position, Quaternion.identity);
				AudioManager.PlayMinionSpawned();
			}
		}
		if (PlatformFishCounter >= nextPlatformRemove && (innerPlatforms.Count > 0 || outPlatforms.Count > 0))
		{
			PlatformFishCounter = 0;
			Transform platform = king.CurrentPlatform;
			if (outPlatforms.Count > 0)
			{
				platform = outPlatforms[Random.Range(0, outPlatforms.Count)];
			}
			else
			{
				platform = innerPlatforms[Random.Range(0, innerPlatforms.Count)];
			}
			StartCoroutine(Dissolve(platform));
		}
		if (platform.RunTime > 45f)
		{
			mainSong.pitch = 1.05f;
			sliderBGImage.color = Color.red;
		}
		else
		{
			mainSong.pitch = 1f;
			sliderBGImage.color = originalSliderBGImage;
		}
	}

	private IEnumerator Dissolve(Transform platform)
	{
		cameraTarget.Target = platform;
		float delay = Time.unscaledTime + 0.5f;
		while (Time.unscaledTime < delay)
		{
			yield return new WaitForEndOfFrame();
		}
		float end = Time.unscaledTime + 1f;
		Time.timeScale = 0;
		Material mat = platform.GetComponent<MeshRenderer>().material;
		float strength = 0;
		AudioManager.PlayDissolved();
		while (Time.unscaledTime < end)
		{
			strength += Time.unscaledDeltaTime;
			mat.SetFloat("_DissolveIntensity", strength);
			yield return new WaitForEndOfFrame();
		}
		platforms.Remove(platform);
		innerPlatforms.Remove(platform);
		outPlatforms.Remove(platform);
		platform.GetComponent<MeshRenderer>().enabled = false;
		cameraTarget.Target = king.transform;
		Destroy(platform.gameObject);
		Time.timeScale = 1;
	}

	private void FireFish()
	{
		fireNextFish = Time.time + Mathf.Clamp(2f / platform.Effectors, 0.5f, 2);
		Vector2 dir = Random.insideUnitCircle.normalized * 40;
		Vector3 heading = new Vector3(dir.x, -5f, dir.y);
		Vector3 origin = platformTransform.position + heading;
		GameObject go = Instantiate(fishPrefab, origin, Quaternion.Euler(Quaternion.LookRotation(origin, Vector3.up).eulerAngles + new Vector3(0, 90, 0)), fishesParent);

		Vector3 target = new Vector3(
			Random.Range(-8f, 8f),
			2,
			Random.Range(-8f, 8f)
		);

		float t = Random.Range(3f, 6f);
		float vx = (target.x - origin.x) / t;
		float vz = (target.z - origin.z) / t;
		float vy = ((target.y - origin.y) - 0.5f * Physics.gravity.y * t * t) / t;

		go.GetComponent<Rigidbody>().velocity = new Vector3(vx, vy, vz);
		Fish fish = go.GetComponent<Fish>();
		fish.GameManager = this;
		float yRotation = Quaternion.LookRotation(target - origin, Vector3.up).eulerAngles.y - 90;
		fish.Marker = Instantiate(projectorPrefab, target + Vector3.up * 2, Quaternion.Euler(90, yRotation, 0));
	}

	public void AddFish()
	{
		platform.RunTime -= 2f;
		if (platform.RunTime < 0)
		{
			platform.RunTime = 0;
		}
		FishCollected++;
		MinionFishCounter++;
		PlatformFishCounter++;
		if (FishCollected > highScore)
		{
			highScore = FishCollected;
			highScoreDisplay.text = "High Score " + highScore;
		}

		collectedDisplay.text = "Score " + FishCollected.ToString();
	}

	public static void GameOver()
	{
		if (_instance.restartShown) return;
		if (_instance.highScore == _instance.FishCollected)
			PlayerPrefs.SetInt("HighScore", _instance.FishCollected);
		_instance.restartShown = true;
		Time.timeScale = 0;
		_instance.factsDisplay.text = _instance.penguinFacts[Random.Range(0, _instance.penguinFacts.Length)];
		_instance.panelAnimator.SetBool("ShowRestart", true);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(0);
	}

	public void StartGame()
	{
		Time.timeScale = 1;
		panelAnimator.SetBool("ShowStart", false);
		panelAnimator.SetBool("ShowRestart", false);
	}
}
