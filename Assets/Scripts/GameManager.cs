﻿using Genetics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public DayNight DayNight { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        DayNight = GetComponentInChildren<DayNight>();

    }
}