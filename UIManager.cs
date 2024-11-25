using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text timer;
    float time = 400f;

    public Text coinNum;
    public int coin = 0;

    public Text scoreNum;
    public int score = 0;

    void Start()
    {

    }

    void Update()
    {
        Timer();
        Coin();
        Score();
    }

    void Timer()
    {
        time -= Time.deltaTime * 2;
        if (time >= 0)
            timer.text = ((int)time).ToString();
    }

    void Coin()
    {
        coinNum.text = coin.ToString("D2");
    }

    void Score()
    {
        scoreNum.text = score.ToString("D7");
    }
}
