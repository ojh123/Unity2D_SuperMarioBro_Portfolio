using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioClip[] audioBackground; // �������
    public AudioClip[] audioSfx;        // ȿ����
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> _audioSfx = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _audioBackground = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
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

        // ��ųʸ��� ȿ���� �߰�
        // ȿ���� �̸��� �����Ŭ�� �̸����� ����Ͽ� ��ųʸ��� �߰�
        for (int i = 0; i < audioSfx.Length; i++) // ȿ����
        {
            
            _audioSfx.Add(audioSfx[i].name, audioSfx[i]);
        }

        for (int i = 0; i < audioBackground.Length; i++)  // �����
        {

            _audioBackground.Add(audioBackground[i].name, audioBackground[i]);
        }
    }

    private void Start()
    {
        // ���� ���� �� ������� �÷���
        audioSource.loop = true;
        audioSource.clip = _audioBackground["Ground Theme"];
        audioSource.Play();
    }

    public void PlayBackground(string sfxName)
    {
        StartCoroutine(PlayBackgroundCor(sfxName));
    }

    IEnumerator PlayBackgroundCor(string sfxName)  // ������� ����
    {
        audioSource.clip = _audioBackground[sfxName];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length - 0.1f); // ������� ���� ��ŭ ����
        if (audioSource.clip == _audioBackground["Level Complete"])
        {
            yield break;  // "Level Complete"�� ��� �ڷ�ƾ�� ��� ����
        }
        audioSource.Stop();
        audioSource.clip = _audioBackground["Ground Theme"];
        audioSource.Play();
    }



    public void PlaySfx(string sfxName)    // ȿ���� ����
    {
        audioSource.PlayOneShot(_audioSfx[sfxName]);
    }


}
