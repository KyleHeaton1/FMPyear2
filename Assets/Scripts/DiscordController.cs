using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.SceneManagement; 

public class DiscordController : MonoBehaviour
{
    public long applicationID = 1098657012050575370;
    [Space]
    public string details = "john";
    [Space]
    public string largeImage = "game_logo";
    public string largeText = "Frogzilla";

    private static bool instanceExists = false;
    public Discord.Discord discord;

    private long time;

    Scene currentScene;

    void Awake() 
    {
        // Transition the GameObject between scenes, destroy any duplicates
        if (!instanceExists)
        {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Log in with the Application ID
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        UpdateStatus();
    }

    void Update()
    {
        // Destroy the GameObject if Discord isn't running
        try
        {
            discord.RunCallbacks();
        }
        catch
        {
            Destroy(gameObject);
        }

        Scene scene = SceneManager.GetActiveScene();
        currentScene = scene;
        if(scene.name == "MainMenu")  details = "On the main menu";
        if(scene.name == "Level1") details = "Playing level 1";
        if(scene.name == "Level2")details = "Playing level 2";
        if(scene.name == "Level3") details = "Playing level 3";
        if(scene.name == "FinalBoss") details = "Fighting John Oil";
    }

    void LateUpdate() 
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
        // Update Status every frame
        try
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = details,
                Assets = 
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                Timestamps =
                {
                    Start = time
                }
            };
            

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
        }
        catch
        {
            // If updating the status fails, Destroy the GameObject
            Destroy(gameObject);
        }
    }
}
