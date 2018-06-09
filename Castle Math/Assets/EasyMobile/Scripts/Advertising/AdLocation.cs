using System;
using System.Collections;

#if EM_CHARTBOOST
using ChartboostSDK;
#endif

namespace EasyMobile
{
    public sealed class AdLocation
    {
        private readonly string name;
        private static Hashtable map = new Hashtable();

        private AdLocation(string name)
        {
            this.name = name;
            map.Add(name, this);
        }

        /// <summary>
        /// Returns a String that represents the current AdLocation.
        /// </summary>
        /// <returns>A String that represents the current AdLocation</returns>
        public override string ToString()
        {
            return name;
        }

        /// Default location
        public static readonly AdLocation Default = new AdLocation("Default");
        /// initial startup of your app
        public static readonly AdLocation Startup = new AdLocation("Startup");
        /// home screen the player first sees
        public static readonly AdLocation HomeScreen = new AdLocation("Home Screen");
        /// Menu that provides game options
        public static readonly AdLocation MainMenu = new AdLocation("Main Menu");
        /// Menu that provides game options
        public static readonly AdLocation GameScreen = new AdLocation("Game Screen");
        /// Screen with list achievements in the game
        public static readonly AdLocation Achievements = new AdLocation("Achievements");
        /// Quest, missions or goals screen describing things for a player to do
        public static readonly AdLocation Quests = new AdLocation("Quests");
        /// Pause screen
        public static readonly AdLocation Pause = new AdLocation("Pause");
        /// Start of the level
        public static readonly AdLocation LevelStart = new AdLocation("Level Start");
        /// Completion of the level
        public static readonly AdLocation LevelComplete = new AdLocation("Level Complete");
        /// Finishing a turn in a game
        public static readonly AdLocation TurnComplete = new AdLocation("Turn Complete");
        /// The store where the player pays real money for currency or items
        public static readonly AdLocation IAPStore = new AdLocation("IAP Store");
        /// The store where a player buys virtual goods
        public static readonly AdLocation ItemStore = new AdLocation("Item Store");
        /// The game over screen after a player is finished playing
        public static readonly AdLocation GameOver = new AdLocation("Game Over");
        /// List of leaders in the game
        public static readonly AdLocation LeaderBoard = new AdLocation("Leaderboard");
        /// Screen where player can change settings such as sound
        public static readonly AdLocation Settings = new AdLocation("Settings");
        /// Screen display right before the player exists an app
        public static readonly AdLocation Quit = new AdLocation("Quit");

        public static AdLocation LocationFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return AdLocation.Default;
            else if (map[name] != null)
                return map[name] as AdLocation;
            else
                return new AdLocation(name);
        }

        #if EM_CHARTBOOST
        public CBLocation ToChartboostLocation()
        {
            return CBLocation.locationFromName(this.name);
        }
        #endif

        #if UNITY_ADS
        public string ToUnityAdsZoneId()
        {
            return name;
        }
        #endif
    }
}

