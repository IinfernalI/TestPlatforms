﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    [SerializeField] private int battery;
    [SerializeField] private Text energyText;
    [SerializeField] private int energyTotal = 2000;
    [SerializeField] private int energyApply = 5;
    
    [SerializeField] private float rotSpeed = 100f;
    [SerializeField] private float flySpeed = 15f;
    
    [SerializeField] private AudioClip flySound;
    [SerializeField] private AudioClip boomSound;
    [SerializeField] private AudioClip finishSound;
    
    [SerializeField] private ParticleSystem leftFlameParticles;
    [SerializeField] private ParticleSystem rightFlameParticles;
    [SerializeField] private ParticleSystem DeathParticles;
    [SerializeField] private ParticleSystem FinishParticles;

    private bool colissionOff = false;
    private Rigidbody rigidBody;
    private AudioSource audioSource;

    enum State 
    {
        Playing,
        Dead,
        NextLevel
    };
    private State state = State.Playing;
    
    // Start is called before the first frame update
    void Start()
    {
        energyText.text = energyTotal.ToString();
        state = State.Playing;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Playing && energyTotal > 5)
        {
            Launch();
            Rotation();
        }

        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
        
    }

    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        else if (Input.GetKeyDown(KeyCode.S))
        {
            colissionOff = !colissionOff;
        }
    }

    void OnCollisionEnter(Collision collision) //Метод который берет колизию обьекста к которому прикреплен скрипт
    {
        if (state == State.Dead || state == State.NextLevel || colissionOff)
        {
            return;
        }
        
        switch (collision.gameObject.tag)
        {
            case "Friendly": 
                print("ok");
                break;
            case "Finish":
                Finish();
                break;
            case "Battery":
                PlusEnergy(battery, collision.gameObject);
                break;
            default:
                Lose();
                break;
        }
    }

    void PlusEnergy(int energyToAdd, GameObject batteryObj)
    {
        batteryObj.GetComponent<BoxCollider>().enabled = false;
        energyTotal += energyToAdd;
        energyText.text = energyTotal.ToString();
        Destroy(batteryObj);
    }
    
    void Lose()
    {
        state = State.Dead;
        audioSource.Stop();
        audioSource.PlayOneShot(boomSound);
        DeathParticles.Play();
        Invoke("LoadFirstLevel",2f);
        print("BOOM"); 
    }
    
    void Finish()
    {
        state = State.NextLevel;
        audioSource.Stop();
        audioSource.PlayOneShot(finishSound);
        FinishParticles.Play();
        Invoke("LoadNextLevel",2f);
    }

    void LoadNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;

        if (nextLevelIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 1;
        }
        SceneManager.LoadScene(nextLevelIndex);
        //SceneManager.LoadScene("Level 2"); //Загрузка новой сцены
        //SceneManager.LoadScene(1); можно так же так указывать как и индекс
    }
    
    void LoadFirstLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Launch()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            energyTotal -= Convert.ToInt32(energyApply * Time.deltaTime);
            energyText.text = energyTotal.ToString();
            rigidBody.AddRelativeForce(Vector3.up * flySpeed * Time.deltaTime); 
            if (!audioSource.isPlaying) 
            { 
                audioSource.PlayOneShot(flySound);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                leftFlameParticles.Play();
                rightFlameParticles.Play();
            }
        }
        else
        {
            audioSource.Pause();
            leftFlameParticles.Stop();
            rightFlameParticles.Stop();
        }
    }

    void Rotation()
    {
        float rotationSpeed = rotSpeed * Time.deltaTime;
        rigidBody.freezeRotation = true;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
            //transform.Rotate(new Vector3(0,0,2)); можно так же пользоваться таким результатом
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
            //transform.Rotate(new Vector3(0,0,-2));
            //transform.Rotate(-Vector3.forward); или таким
        }
        rigidBody.freezeRotation = false;
    }
}
