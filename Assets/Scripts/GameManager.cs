using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool m_GameStarted;
    public Vector2 m_NewPosition;
    public List<int> m_SheepOnHand = new List<int>();
    public List<int> m_SheepSaved = new List<int>();
    public List<Quest> m_AllQuests = new List<Quest>();
    public List<Quest> m_CompletedQuests = new List<Quest>();
    public List<Item> m_Items = new List<Item>();
    public int m_FishAmount;
    public int m_PausePage;
    public bool m_IsDelivering;
    public bool m_CaughtRareFish;
    public bool m_BigMode;
    private bool m_isFillingHeart;

    [Header("Components")]
    [SerializeField] private Animator m_Anim;
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private AudioSource m_BGMPlayer;
    [SerializeField] private AudioSource m_SFXPlayer;
    [SerializeField] private GameObject m_LoadingIcon;

    [Header("Music Parameters")]
    private float m_MusicTime = 64;     // Time between each song.
    private float m_NextSong;           // Time until next song.

    [Header("System Audio")]
    [SerializeField] private AudioClip m_TransferSFX;

    [Header("Effects Prefabs")]
    [SerializeField] private GameObject m_SparklesPrefabs;

    [Header("Level Up Components")]
    [SerializeField] private CanvasGroup m_GroupHolder;
    [SerializeField] private CanvasGroup m_LevelUpTint;
    [SerializeField] private CanvasGroup m_HeartHolder;
    [SerializeField] private Image m_HeartFill;
    [SerializeField] private Image m_HeartGlow;
    [SerializeField] private RectTransform m_GlowTransform;
    [SerializeField] private AudioClip m_FadeSFX;
    [SerializeField] private AudioClip m_IncreaseSFX;
    public int m_HappyLevel;
    private int m_MaxHappiness = 10;

    [Header("Flags - Maple")]
    public bool m_FlagCleanedMapleRoom;
    [Header("Flags - Farmer")]
    public bool m_FlagMetFarmer;
    public bool m_FlagSavedSheep;
    [Header("Flags - Fisher")]
    public bool m_FlagMetFisher;
    public bool m_FlagThankFisher;
    [Header("Flags - Cat")]
    public bool m_FlagFedCat;
    [Header("Flags - Old Lady")]
    public bool m_FlagMetOldLady;
    public bool m_FlagHaveGlasses;
    public bool m_FlagLadyHelped;
    [Header("Flags - Cafe")]
    public bool m_FlagMetCafe;
    public int m_DeliveryStage;
    public bool m_FlagHaveTea;
    public bool m_FlagDrankTea;
    [Header("Flags - Old Ghost")]
    public bool m_FlagMetGhost;
    public bool m_FlagHaveMemento;
    public bool m_FlagGhostThanks;
    [Header("Flags - Postman")]
    public bool m_FlagMetPostman;
    public bool m_FlagThanksPostman;
    [Header("Flags - Mycen")]
    public bool m_FlagCheckedMycen;
    public bool m_FlagMycenDoorOpen;
    public bool m_FlagAskedDIY;
    public bool m_FlagHaveToolbox;
    public bool m_FlagReturnedToolbox;
    [Header("Flags - Helena")]
    public bool m_FlagMetHelena;
    public bool m_FlagCleanedHelenaRoom;
    public bool m_FlagHelenaBook;
    [Header("Flags - NPCs")]
    public bool m_FlagMetOswald;
    public bool m_FlagMetHilda;
    public bool m_FlagMetJan;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            m_Anim = GetComponentInChildren<Animator>();
            m_LoadingIcon.SetActive(false);

            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.F4) || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return)))
        {
            SetFullScreen(!Screen.fullScreen);
        }
    }*/

    public void OnFullscreenTrigger(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        SetFullScreen(!Screen.fullScreen);
    }

    public void ResetGameStats()
    {
        m_GameStarted = false;
        m_SheepSaved.Clear();
        m_AllQuests.Clear();
        m_CompletedQuests.Clear();
        m_Items.Clear();

        m_FishAmount = 0;
        m_HappyLevel = 0;
        m_DeliveryStage = 0;
        m_PausePage = 0;
        m_BGMPlayer.volume = 1.0f;

        m_FlagCleanedMapleRoom = false;
        m_FlagMetFarmer = false;
        m_FlagSavedSheep = false;
        m_FlagMetFisher = false;
        m_FlagThankFisher = false;
        m_FlagFedCat = false;
        m_FlagMetOldLady = false;
        m_FlagHaveGlasses = false;
        m_FlagLadyHelped = false;
        m_FlagMetCafe = false;
        m_FlagHaveTea = false;
        m_FlagDrankTea = false;
        m_FlagMetGhost = false;
        m_FlagHaveMemento = false;
        m_FlagGhostThanks = false;
        m_FlagMetPostman = false;
        m_FlagThanksPostman = false;
        m_FlagCheckedMycen = false;
        m_FlagMycenDoorOpen = false;
        m_FlagAskedDIY = false;
        m_FlagHaveToolbox = false;
        m_FlagReturnedToolbox = false;
        m_FlagMetHelena = false;
        m_FlagCleanedHelenaRoom = false;
        m_FlagHelenaBook = false;
        m_FlagMetOswald = false;
        m_FlagMetHilda = false;
        m_FlagMetJan = false;
        m_BigMode = false;
    }

    public void UnlockMedal(int medal_id)
    {
        StartCoroutine(NGIO.UnlockMedal(medal_id));
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetCamFollower(GameObject followObj)
    {
        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        var vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        vcam.m_Follow = followObj.transform;
    }

    public void PlayBGM(AudioClip song)
    {
        if((m_BGMPlayer.isPlaying && m_BGMPlayer.loop == false)) return;

        if(isOutside())
        {
            if(Time.time > m_NextSong)
            {
                // When cooldown has passed, add new cooldown.
                m_NextSong = Time.time + song.length + m_MusicTime;
            }
            else 
            {
                // If it's still under cooldown, then don't play anything.
                m_BGMPlayer.Stop();
                return;
            }
        }
        else
        {
            if(m_BGMPlayer.isPlaying)
            {
                // Indoor music will not cutout outdoor music.
                song = m_BGMPlayer.clip;
            }
            else 
            {
                // Only play the indoor music sometimes.
                int r = Random.Range(0, 4);
                if(r == 0)
                {
                    m_BGMPlayer.Stop();
                    return;
                }
            }
        }
        
        m_BGMPlayer.loop = !isOutside();
        m_BGMPlayer.clip = song;
        m_BGMPlayer.Play();
    }

    public void PlaySFX(AudioClip sound)
    {
        m_SFXPlayer.clip = sound;
        m_SFXPlayer.Play();
    }

    public void FadeOutBGM()
    {
        if(m_BGMPlayer.isPlaying == false) return;
        StartCoroutine(FadeMusicOut());
    }

    public void StopBGM()
    {
        m_BGMPlayer.Stop();
    }

    public void SetMasterVolume(float value)
    {
        m_AudioMixer.SetFloat("master", value);
    }

    public void SetBGMVolume(float value)
    {
        m_AudioMixer.SetFloat("bgm", value);
    }

    public void SetBGSVolume(float value)
    {
        m_AudioMixer.SetFloat("bgs", value);
    }

    public void SetSFXVolume(float value)
    {
        m_AudioMixer.SetFloat("sfx", value);
    }

    public float GetMasterVolume()
    {
        float value;
        m_AudioMixer.GetFloat("master", out value);
        return value;
    }

    public float GetBGMVolume()
    {
        float value;
        m_AudioMixer.GetFloat("bgm", out value);
        return value;
    }

    public float GetBGSVolume()
    {
        float value;
        m_AudioMixer.GetFloat("bgs", out value);
        return value;
    }

    public float GetSFXVolume()
    {
        float value;
        m_AudioMixer.GetFloat("sfx", out value);
        return value;
    }

    public void AddQuest(Quest quest)
    {
        if(m_AllQuests.Contains(quest)) return;
        m_AllQuests.Add(quest);
    }

    public void CompleteQuest(Quest quest)
    {
        if(m_CompletedQuests.Contains(quest)) return;
        if(!m_AllQuests.Contains(quest)) AddQuest(quest);
        m_CompletedQuests.Add(quest);
    }

    public void SpawnSparkles(Vector3 pos)
    {
        GameObject go = Instantiate(m_SparklesPrefabs, pos, Quaternion.identity) as GameObject;
    }

    public void AddItem(Item item)
    {
        if(m_Items.Contains(item)) return;
        m_Items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if(!m_Items.Contains(item)) return;
        m_Items.Remove(item);
    }

    public void AddSheep(int sheepID)
    {
        if(m_SheepOnHand.Contains(sheepID)) return;
        m_SheepOnHand.Add(sheepID);
    }

    public void RemoveSheep(int sheepID)
    {
        if(!m_SheepOnHand.Contains(sheepID)) return;
        m_SheepOnHand.Remove(sheepID);
    }

    public void SaveSheep(int sheepID)
    {
        if(m_SheepSaved.Contains(sheepID)) return;
        m_SheepSaved.Add(sheepID);
    }

    public void IncreaseHappiness()
    {
        if(isFillingHeart()) return;
        StartCoroutine(LevelUp());
    }

    public void LoadNewScene(int sceneIndex)
    {
        StartCoroutine(TransferSequence(sceneIndex, Vector2.zero, false));
    }

    public void TransferPlayer(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        StartCoroutine(TransferSequence(sceneIndex, newPos, zoomTransfer));
    }

    public void StartDelivery(bool state)
    {
        GameManager.instance.m_IsDelivering = state;
        
        Player p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if(state == true)
        {
            p.CarryPizza(m_DeliveryStage);
        }
        else 
        {
            p.CarryPizza(-1);
        }
    }

    public void HeartFill()
    {
        m_HeartFill.fillAmount = Mathf.Clamp((float)m_HappyLevel / (float)m_MaxHappiness, 0.0f, 1.0f);
    }

    public bool isFillingHeart()
    {
        return m_isFillingHeart;
    }

    public bool isTalking()
    {
        return FindObjectOfType<DialogueManager>().isTalking();
    }

    public bool isPaused()
    {
        return FindObjectOfType<PauseManager>().isPaused();
    }

    public bool isOutside()
    {
        bool isOutside = false;
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            isOutside = true;
        }
        return isOutside;
    }

    public int getSceneID()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    IEnumerator FadeMusicOut()
    {
        float t = 0.0f;
        float fadeDur = 0.8f;
        while(t < 1.0f)
        {
            t += Time.unscaledDeltaTime / fadeDur;
            m_BGMPlayer.volume = 1.0f - t;
            yield return null;
        }
        m_BGMPlayer.Stop();
    }

    IEnumerator TransferSequence(int sceneIndex, Vector2 newPos, bool zoomTransfer)
    {
        if(zoomTransfer)
        {
            var camera = Camera.main;
            var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
            var vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;

            if(vcam != null)
            {
                float farSize = vcam.m_Lens.OrthographicSize;
                float zoomSize = vcam.m_Lens.OrthographicSize * 0.75f;
                float zoomDur = 0.75f;
                float t = 0.0f;
                while(t < 1.0f)
                {
                    t += Time.deltaTime / zoomDur;
                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(farSize, zoomSize, t);
                    yield return null;
                }
            }
        }
        
        m_NewPosition = newPos;

        if(!zoomTransfer) PlaySFX(m_TransferSFX);
        m_Anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);

        //SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        m_LoadingIcon.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while(!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        m_LoadingIcon.SetActive(false);
        m_Anim.SetTrigger("FadeIn");
    }

    IEnumerator LevelUp()
    {
        m_isFillingHeart = true;
        // Initialize Everything
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        bool frozePlayer = false;
        if(player.canMove())
        {
            player.StopMovement(true);
            frozePlayer = true;
        }
        m_GroupHolder.alpha = 1.0f;
        m_HeartHolder.alpha = 0.0f;
        Color heartColor = new Color(0.859f, 0.267f, 0.49f);
        m_HeartFill.color = heartColor;
        m_HeartGlow.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        HeartFill();
        
        // Tint screen black
        float blackDur = 1.0f;
        float t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / blackDur;
            m_LevelUpTint.alpha = t;
            yield return null;
        }

        // Grow Heart on screen (with current fill)
        float heartDur = 1.0f;
        t = 0.0f;
        PlaySFX(m_FadeSFX);
        while(t < 1.0f)
        {
            t += Time.deltaTime / heartDur;
            m_HeartHolder.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.32f);

        // Make fill bigger + white to pink fade
        // Glow effect
        m_HappyLevel++;
        float fillDur = 1.0f;
        t = 0.0f;
        Color startColor = Color.white;
        Color endColor = heartColor;
        Color glowStart = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Color glowEnd = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Vector2 startSize = new Vector2(1.1f, 1.1f);
        Vector2 endSize = new Vector2(1.6f, 1.6f);
        PlaySFX(m_IncreaseSFX);
        HeartFill();
        while(t < 1.0f)
        {
            t += Time.deltaTime / fillDur;
            m_HeartFill.color = Color.Lerp(startColor, endColor, t);
            m_GlowTransform.localScale = Vector2.Lerp(startSize, endSize, t);
            m_HeartGlow.color = Color.Lerp(glowStart, glowEnd, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        player.PlayLevelUp();

        // Make heart shrink again
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / heartDur;
            m_HeartHolder.alpha = 1.0f - t;
            yield return null;
        }

        // Get rid of tint
        t = 0.0f;
        while(t < 1.0f)
        {
            t += Time.deltaTime / blackDur;
            m_LevelUpTint.alpha = 1.0f - t;
            yield return null;
        }

        if(frozePlayer)
        {
            player.StopMovement(false);
        }
        m_isFillingHeart = false;
    }
}
