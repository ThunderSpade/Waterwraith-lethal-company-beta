using BepInEx;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx.Configuration;
using UnityEngine.AI;
using Waterwraith;

namespace Waterwraith
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {

        public static AssetBundle bundle;

        public static ConfigEntry<float> configSize;
        public static ConfigEntry<float> configSpeed;
        public static ConfigEntry<int> configMaxSnails;
        public static ConfigEntry<int> configRarity;
        public static ConfigEntry<bool> configGoOutside;
        public static ConfigEntry<bool> configEnterShip;
        public static ConfigEntry<bool> configShowTarget;

        private void Awake()
        {
            Logger.LogInfo("Loading a mod by ThunderSpade");
            Logger.LogInfo("awesome!");
               
            // setup for Unity Netcode Patcher
            NetcodePatcher();

            // configuration setup
            configSize = Config.Bind("General", "Scale", 100.0f, "The size of the Waterwraith.");
            configSpeed = Config.Bind("General", "Speed", 0.5f, "The speed of the Waterwraith.");
            configMaxSnails = Config.Bind("General", "Max Waterwraiths", 4, "The maximum number of Waterwraith that can spawn in a round.");
            configRarity = Config.Bind("General", "Rarity", 80, "Honestly not sure exactly how this works, but a higher \"Rarity\" will make the Waterwraith more likely to spawn.");
            configGoOutside = Config.Bind("Pathing", "Can Go Outside", true, "If enabled, allows the Waterwraith to exit the factory and chase players outside.");
            configEnterShip = Config.Bind("Pathing", "Can Enter Ship", true, "If enabled, allows the Waterwraith to target players that are in the ship room.");
            configShowTarget = Config.Bind("General", "Show Target on Scan", true, "If enabled, shows the targeted player when scanned.");

            // check if using LethalConfig
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ainavt.lc.lethalconfig"))
                ConfigManager.setupLethalConfig();

            // loading snail from bundle
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "Waterwraith"));

            if (bundle == null)
            {
                Logger.LogError("Failed to load asset bundle.");
                return;
            }

            EnemyType Waterwraith = bundle.LoadAsset<EnemyType>("Waterwraith.EnemyType");

            if (Waterwraith == null || Waterwraith.enemyPrefab == null)
            {
                Logger.LogError("Waterwraith Failed to load properly.");
                return;
            }


            Waterwraith.enemyPrefab.AddComponent<WaterwraithAI>();
            Waterwraith.enemyPrefab.GetComponent<WaterwraithAI>().enemyType = Waterwraith;
            Waterwraith.enemyPrefab.GetComponentInChildren<EnemyAICollisionDetect>().mainScript = Waterwraith.enemyPrefab.GetComponent<WaterwraithAI>();

            Logger.LogInfo("Applying User-Defined Configuration Settings");
            Waterwraith.enemyPrefab.transform.localScale *= configSize.Value / 100f;
            Waterwraith.enemyPrefab.GetComponent<NavMeshAgent>().speed = configSpeed.Value;
            Waterwraith.MaxCount = configMaxSnails.Value;

            Logger.LogInfo("Registering Waterwraith as Enemy");
            Levels.LevelTypes levelFlags = Levels.LevelTypes.All;
            Enemies.SpawnType spawnType = Enemies.SpawnType.Default;

            TerminalNode WaterwraithNode = ScriptableObject.CreateInstance<TerminalNode>();
            WaterwraithNode.displayText = "The Immortal Snail\n\nDanger level: 50%\n\n" +
                "All that is known about this creature stems from a few sightings deep underground. All reported sightings " +
            "feature the same core set of details: a giant, viscous form with a clear, hazy sheen not unlike hard candy. " +
            "One theory holds that it may be the ectoplasmic incarnation of a kind of psychic phenomenon, " +
            " but as is usually the case with such theories, it is very difficult to prove. " +
            "All witnesses report being suddenly overcome with fear upon sighting the creature, approaching a state of panic and near insanity. " +
            "In fact, every report contains an inordinate amount of extremely vague details, which has led to suspicions that exhaustion and " +
            "fear have caused some simple natural phenomenon to be viewed as a living creature. " +
            "What is this?!? I can see it with my optical receptors, but my sensors cannot detect it!\r\n\r\nCould its physical form be anchored in another dimension? ...Attacking it is futile!\r\n\r\nIf only we could force it to take on physical form... But in its current state... Krzzzt-zrrrk!\r\n\r\nDanger! Danger! Danger! " +
            "\n\n";

            WaterwraithNode.clearPreviousText = true;
            WaterwraithNode.maxCharactersToType = 2000;
            WaterwraithNode.creatureName = "Waterwraith";
            WaterwraithNode.creatureFileID = 1738;

            TerminalKeyword WaterwraithKeyword = TerminalUtils.CreateTerminalKeyword("Waterwraith", specialKeywordResult: WaterwraithNode);

            NetworkPrefabs.RegisterNetworkPrefab(Waterwraith.enemyPrefab);
            Enemies.RegisterEnemy(Waterwraith, configRarity.Value, levelFlags, spawnType, WaterwraithNode, WaterwraithKeyword);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} " + $"{PluginInfo.PLUGIN_VERSION} is loaded!");
        }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}