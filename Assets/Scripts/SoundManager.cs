using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.UI;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {
    /*
    private static SoundManager m_Instance;
    public static SoundManager Instance{
        get{
            if(m_Instance == null){
                //m_Instance = Instantiate();
                GameObject prefab = (GameObject)Resources.Load("Prefabs/SoundManager");
                m_Instance = Instantiate(prefab).GetComponent<SoundManager>();
            }
            return m_Instance;
        }
    }*/

    //[SerializeField]
    //private AudioMixer m_Mixer;

    [SerializeField]
    private AudioSource m_BGM;
    [SerializeField]
    private AudioSource m_SE;
    [SerializeField]
    private AudioSource m_Announce;
    [SerializeField]
    private List<SEVolume> m_SEVolumes = new List<SEVolume>(); 

    private Dictionary<BGM,AudioClip>m_BGMDict = new Dictionary<BGM, AudioClip>();
    private Dictionary<SE, AudioProfile> m_SEDict = new Dictionary<SE, AudioProfile>();
    private Dictionary<Announce,AudioClip>m_AnnDict = new Dictionary<Announce, AudioClip>();

    public Text logText;

    private void Awake()
    {
        foreach (var bgmName in System.Enum.GetNames(typeof(BGM))){
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/BGM/{bgmName}");
            if(clip != null)
            {
                BGM result = BGM.main;
                System.Enum.TryParse(bgmName, out result);
                m_BGMDict.Add(result, clip);

            }
            else{
                Debug.Log($"存在しないクリップ 名前 : {bgmName}");
                logText.text = bgmName;
            }
        }

        foreach (var seName in System.Enum.GetNames(typeof(SE))){
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/SE/{seName}");
            if(clip != null)
            {
                var profile = m_SEVolumes.Find(a=>a.audio.ToString() == seName);
                float volume = 1f;
                if(profile != null){
                    volume = profile.volume;
                }
                SE result = SE.none;
                System.Enum.TryParse(seName, out result);
                m_SEDict.Add(result, new AudioProfile(clip,volume));
            }else{
                //Debug.Log($"存在しないクリック 名前 : {seName}");
            }
        }

        foreach (var anName in System.Enum.GetNames(typeof(Announce))){
            AudioClip clip = Resources.Load<AudioClip>($"Sound/Announce/{anName}");
            if(clip != null)
            {
                Announce result = Announce.none;
                System.Enum.TryParse(anName, out result);
                m_AnnDict.Add(result, clip);
            }else{
                //Debug.Log($"存在しないクリック 名前 : {anName}");
            }
        }
        

    }

    private void Start() {
    }

    public void SetVolume(float volume){
        float vol = Mathf.Clamp(volume,0.0001f,1f);
        float volDB = 20 * Mathf.Log10(vol);
        //m_Mixer.SetFloat("MasterVol", Mathf.Clamp(volDB,-80f,0f));
    }

    private bool m_BGMEnable = true;
    /*
    private bool BGMEnable
    {
        get
        {
            return m_BGMEnable;
        }
        set
        {
            m_BGMEnable = value;
        }
    }*/

    private bool m_SEEnable = true;
    /*
    public bool SEEnable
    {
        get
        {
            return m_SEEnable;
        }
        set
        {
            m_SEEnable = value;
        }
    }*/

//再生されない条件の時でも、クリップは更新されます
    public void PlayBGM(BGM bgm,bool loop = true)
    {
        m_BGM.clip = m_BGMDict[bgm];
        m_BGM.loop = loop;
        if (m_BGMEnable == false) return;        
        m_BGM.Play();
    }

    public void PauseBGM(){
        if (m_BGMEnable == false) return;   
        m_BGM.Pause();
    }

    public void RestartBGM(){
        if (m_BGMEnable == false) return;        
        m_BGM.Play();
    }

    public void PlaySE(SE se)
    {
        if (m_SEEnable == false) return;
        if(m_SEDict.ContainsKey(se)){
            var profile = m_SEDict[se];
            m_SE.volume = profile.volume;
            m_SE.PlayOneShot(profile.clip);
        }else{
            Debug.Log($"SEがありません SE : {se.ToString()}");
        }
        
    }

    public void PlayAnnounce(Announce announce){
        if(m_SEEnable == false)return;
        m_Announce.PlayOneShot(m_AnnDict[announce]);
    }

    public void StopBGM()
    {
        m_BGM.Stop();
    }



    [System.Serializable]
    public class SEVolume{
        public SE audio;
        public float volume = 1f;
    }

    public class AudioProfile{
        public AudioClip clip;
        public float volume = 1f;

        public AudioProfile(AudioClip clip,float vol){
            this.clip = clip;
            this.volume = vol;
        }
    }
}

public enum SE{
    none,
    goukaku,
    open_futo,
    open_futo2,
    ranking
    
}

public enum Announce{
    none,
}

public enum BGM{
    main
}