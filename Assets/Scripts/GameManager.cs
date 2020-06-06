using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public SkinnedMeshRenderer water;
    public Material currentInWire;
    public Material wireMat;
    public GameObject[] wires;
    public float blendChangeVelocity = 20f;
    public Animator magnetAnim;
    public Animator floaterAnim;
    private bool startWave = false;
    private bool[] increasing;
    public GameObject charge;
    private Mesh skinnedMesh;

    private int shapesCount;
    private bool wireCurrentOn = false;
    private bool passToNext = false;
    // Start is called before the first frame update
    void Start()
    {
        shapesCount = water.sharedMesh.blendShapeCount;
        increasing = new bool[shapesCount];
        for (int i = 0; i < shapesCount; i++)
            increasing[i] = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startWave)
        {
            startWave = false;
            water.SetBlendShapeWeight(0, 1f);
        }
        for(int i = 0; i < shapesCount; i++)
        {
           
            float currentVal = water.GetBlendShapeWeight(i);
            if (passToNext)
            {
                currentVal = 1f;
                passToNext = false;
            }
            if (currentVal > 0)
            {
                if (increasing[i])
                {
                    currentVal += blendChangeVelocity * Time.deltaTime;
                }
                else { 
                    currentVal -= blendChangeVelocity * Time.deltaTime; 
                }
            }
            if (currentVal < 0)
            {
                currentVal = 0;
                increasing[i] = true;
            }
            if (currentVal >= 100)
            {
                passToNext = true;
                increasing[i] = false;
                currentVal = 100;
            }
        
            water.SetBlendShapeWeight(i,currentVal);
        }
        passToNext = false;
        float checkVal = water.GetBlendShapeWeight(4);

        if (!magnetAnim.GetCurrentAnimatorStateInfo(0).IsName("MagnetOscilate") && checkVal < 50 && checkVal > 40 && increasing[4])
        {
            magnetAnim.Play("MagnetOscilate");
            floaterAnim.Play("FloaterAnim");
            Vector3 curScale = charge.transform.localScale;
            if (curScale.y >= 1)
                curScale.y = 1;
            else
                curScale.y += 0.1f;
            charge.transform.localScale = curScale;
            if(!wireCurrentOn && curScale.y > 0.15f){
                ChangeWireMat();
                wireCurrentOn = true;
            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tap DEtected");
            startWave = true;
        }


    }

    private void ChangeWireMat()
    {
        for (int i= 0; i < wires.Length; i++){
            wires[i].GetComponent<Renderer>().material = currentInWire; 
        }
    }
}
