using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpinWheelScript : MonoBehaviour
{
    public float RotatePower;
    public float StopPower;

    private Rigidbody2D rbody;
    int inRotate;
    public bool isSpinning = false;

    // Buff Text
    public GameObject rewardPanel;
    public TextMeshProUGUI rewardText;

    //buff and gold storage
    public int coinAmount;
    public int moveBuffCount;
    public int rainbowBuffCount;
    public int bombBuffCount;



    

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();

        coinAmount = PlayerPrefs.GetInt("Gold Coin");
        moveBuffCount = PlayerPrefs.GetInt("Move Buff");
        rainbowBuffCount = PlayerPrefs.GetInt("Rainbow Buff");
        bombBuffCount = PlayerPrefs.GetInt("Bomb Buff");
    }

    float t;
    private void Update()
    {

        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;

            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += 1 * Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();

                inRotate = 0;
                t = 0;
            }
        }
    }


    public void Rotete()
    {
        if(coinAmount >= 2 && isSpinning == false)
        {
            coinAmount -= 2;
            PlayerPrefs.SetInt("Gold Coin", coinAmount);


            isSpinning = true;
            rewardPanel.SetActive(false);
            if (inRotate == 0)
            {
                rbody.AddTorque(RotatePower);
                inRotate = 1;
            }
        }
        
    }



    public void GetReward()
    {
        float rot = transform.eulerAngles.z;

        if (rot > 0 + 22 && rot <= 45 + 22 )
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 45);
            Win(2);
            isSpinning = false;
        }
        else if (rot > 45 + 22 && rot <= 90 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 90);
            Win(3);
            isSpinning = false;
        }
        else if (rot > 90 + 22 && rot <= 135 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 135);
            Win(4);
            isSpinning = false;
        }
        else if (rot > 135 + 22 && rot <= 180 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 180);
            Win(5);
            isSpinning = false;
        }
        else if (rot > 180 + 22 && rot <= 225 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 225);
            Win(6);
            isSpinning = false;
        }
        else if (rot > 225 + 22 && rot <= 270 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 270);
            Win(7);
            isSpinning = false;
        }
        else if (rot > 270 + 22 && rot <= 315 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 315);
            Win(8);
            isSpinning = false;
        }
        else //if (rot > 315 + 22 && rot <= 360 + 22)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
            Win(1);
            isSpinning = false;
        }

    }


    public void Win(int slotnumber)
    {
        rewardPanel.SetActive(true);

        switch (slotnumber)
        {
            case 1:
                coinAmount += 4;
                PlayerPrefs.SetInt("Gold Coin", coinAmount);
                Debug.Log("Got 4 coins");
                rewardText.text = "YOU GOT 4 GOLD COINS!";
                break;
            case 2:
                bombBuffCount += 1;
                PlayerPrefs.SetInt("Bomb Buff", bombBuffCount);
                Debug.Log("YOU GOT 1 BOMB BUFF");
                rewardText.text = "YOU GOT 1 BOMB BUFF";
                break;
            case 3: case 5: case 7:
                coinAmount += 1;
                PlayerPrefs.SetInt("Gold Coin", coinAmount);
                Debug.Log("Got 1 coins");
                rewardText.text = "YOU GOT 1 GOLD COIN!";
                break;
            case 4: case 8:
                rainbowBuffCount += 1;
                PlayerPrefs.SetInt("Rainbow Buff", rainbowBuffCount);
                Debug.Log("You Got 1 Rainbow Buff");
                rewardText.text = "YOU GOT 1 RAINBOW BUFF!";
                break;
            case 6:
                moveBuffCount += 1;
                PlayerPrefs.SetInt("Move Buff", moveBuffCount);
                Debug.Log("Got 1 Move Buff");
                rewardText.text = "YOU GOT 1 MOVE BUFF!";
                break;

        }

        print("You Got Slot " + slotnumber);
    }


}