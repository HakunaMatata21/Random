using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class EndGameInterstitial : MonoBehaviour
{
    private  InterstitialAd interstitial;
    public static EndGameInterstitial Instance;
    public string AndroidInterstitialID = "ca-app-pub-3940256099942544/1033173712";//Тест реклама тип interstitial.
    public string MyDeviceID = "XXXX";
    public bool timeForNewAd;
    public bool hasShownAdOneTime;
    

    void Awake() 
    {
        if (Instance == null) //Ако този файл не съществува
        {
            DontDestroyOnLoad(gameObject); //Няма да се унищожава.
            Instance = this; //Това е този файл.
        }
        else if (Instance != this) //Ако това не е този файл.
        {
            Destroy(gameObject); //Унищожи обекта.
        }
    }


    void Start()
    {
        Initialize(); //изпълняване на фаза 1(инициализация на SDK,случва се 1 път в main menu)
        Debug.Log("Initializing SDK");
    }


    public void Initialize()
    {
        Debug.Log("Phase 1");
#if UNITY_ANDROID
        string appId = "XXXX"; //Номер-а на приложението
#else
        string appId = "unexpected_platform";
#endif

        MobileAds.Initialize(appId); //Инициализиране на SDK за рекламите.
    }

    public void RequestAndLoad()
    {
        Debug.Log("Phase 2");
#if UNITY_ANDROID
        string adUnitId = AndroidInterstitialID; //Идентификационен номер на рекламата.
#else
        string adUnitId = "unexpected_platform";
#endif


        if (interstitial != null) //Ако заставката НЕ е изчистена.
        {
            interstitial.Destroy(); //я изчисти.
        }

        interstitial = new InterstitialAd(adUnitId); //Инициализиране на заставката(Interstitial Ad)


        // interstitial.OnAdLoaded += HandleOnAdLoaded;//Този Event ще се изпълни когато рекламата е прикл. с зареждането 
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad; //Изпълнява се когато рекламата неможе да зареди (параметър-а message е за да опише грешката)
        interstitial.OnAdOpening += HandleOnAdOpened;//Ще се изпълни когато рекламата е закрила екрана.
        interstitial.OnAdClosed += HandleOnAdClosed;//Ще се изпълни когато рекламата е затворена със Х бутона или е натиснато копчето "Назад"
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;//Ще се изпълни когато играча кликне на друга апликация(като Google Play Store и остави играта на фонов режим)


        //request = new AdRequest.Builder().Build(); // Създаване на празен request. (Когато рекламите не се тестват се използва това вместо ТЕСТ редовете по-долу)


        //***ТЕСТ***
        AdRequest request = new AdRequest.Builder()
       .AddTestDevice(AdRequest.TestDeviceSimulator)   
       .AddTestDevice(MyDeviceID)  // ID на тестовото устройство(Android Device ID)
       .Build();

        interstitial.LoadAd(request); // Зареждане на самата реклама в празния request.

        hasShownAdOneTime = false; //Рекламата не е показвана
        timeForNewAd = false; //Не е време за ново зареждане на реклама
    }

    public void Preparation()
    {
        Debug.Log("Phase 3");
        if (interstitial.IsLoaded())//Ако рекламата е заредена
        {
            Time.timeScale = 0; //Паузиране на играта
            AudioListener.volume = 0; //mute sound
            if (!hasShownAdOneTime) //Ако рекламата не е показвана
            {
                Invoke("ShowAd", 0.2f); //След 1/5 от секундата премини към последната фаза
            }


        }
    }

    public void ShowAd()
    {
        Debug.Log("Phase 4");
        interstitial.Show(); //Покажи рекламата
        timeForNewAd = true; //Индикатор,че новата реклама не е заредена.
        hasShownAdOneTime = true;//Индикатор,че рекламата е показана веднъж за тази сесия.


    }



    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("Interstitial failed to load: " + args.Message);
        
        
        Time.timeScale = 1; //пускане на играта
        AudioListener.volume = 1; //пускане на звука
        Debug.Log("Ad failed to load");
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");

       
        
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");

        timeForNewAd = true;
        Time.timeScale = 1; //пускане на играта
        AudioListener.volume = 1; //пускане на звука
        
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        print("HandleAdLeavingApplication event received");
     
    }


}
