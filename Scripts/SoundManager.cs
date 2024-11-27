using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioClip[] audioBackground; // 배경음악
    public AudioClip[] audioSfx;        // 효과음
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> _audioSfx = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _audioBackground = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        // 딕셔너리에 효과음 추가
        // 효과음 이름을 오디오클립 이름으로 사용하여 딕셔너리에 추가
        for (int i = 0; i < audioSfx.Length; i++) // 효과음
        {
            
            _audioSfx.Add(audioSfx[i].name, audioSfx[i]);
        }

        for (int i = 0; i < audioBackground.Length; i++)  // 배경음
        {

            _audioBackground.Add(audioBackground[i].name, audioBackground[i]);
        }
    }

    private void Start()
    {
        // 루프 설정 후 배경음악 플레이
        audioSource.loop = true;
        audioSource.clip = _audioBackground["Ground Theme"];
        audioSource.Play();
    }

    public void PlayBackground(string sfxName)
    {
        StartCoroutine(PlayBackgroundCor(sfxName));
    }

    IEnumerator PlayBackgroundCor(string sfxName)  // 배경음악 실행
    {
        audioSource.clip = _audioBackground[sfxName];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length - 0.1f); // 배경음악 길이 만큼 실행
        if (audioSource.clip == _audioBackground["Level Complete"])
        {
            yield break;  // "Level Complete"일 경우 코루틴을 즉시 종료
        }
        audioSource.Stop();
        audioSource.clip = _audioBackground["Ground Theme"];
        audioSource.Play();
    }



    public void PlaySfx(string sfxName)    // 효과음 실행
    {
        audioSource.PlayOneShot(_audioSfx[sfxName]);
    }


}
