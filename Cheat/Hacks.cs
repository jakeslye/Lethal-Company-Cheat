using GameNetcodeStuff;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace Cheat
{
    internal class Hacks : MonoBehaviour
    {
        [DllImport("User32.dll")]
        public static extern bool GetAsyncKeyState(int key);

        private String logs = "Logs (Only shows player deaths for now):\n";

        private bool showMenu = false;
        private bool showLogs = true;

        private bool objectEsp = true;
        private bool playerEsp = true;
        private bool enemyEsp = true;

        private bool fastClimb = true;
        private bool godMode = true;
        private bool superJump = true;
        private bool exhaustedHack = true;
        private bool jetpackHack = false;

        private bool healAll = true;
        int spectate = -1;

        private static float TEXT_HEIGHT = 25;
        private static float SELF_CHEATS_X = 0;
        private static float PLAYER_LIST_X = 250;
        private static float SERVER_CHEATS_X = 500;
        private static float BUTTTON_WIDTH = 250;
        private static float BUTTTON_HEIGHT = 30;


        public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
        {
            screen = camera.WorldToViewportPoint(world);
            screen.x *= (float)Screen.width;
            screen.y *= (float)Screen.height;
            screen.y = (float)Screen.height - screen.y;
            return screen.z > 0f;
        }

        private static string boolToStatus(bool b) {
            if (b) return "on";
            return "off";
        }

        public void OnGUI()
        {
            if (showLogs) {
                float fix = 0;
                if (showMenu) fix += BUTTTON_HEIGHT;
                GUI.Label(new Rect(Screen.width - BUTTTON_WIDTH, fix, 200, 800), logs); 
            }

            if (showMenu) {
                GUIStyle title = new GUIStyle();
                title.fontStyle = FontStyle.Bold;
                title.fontSize = 20;
                title.normal.textColor = Color.blue;
                GUI.Label(new Rect(0, 0, 150, TEXT_HEIGHT), "jakethemodder's Lethal Company Menu", title);
                GUI.Box(new Rect(0, 50, 750, 800), "");
                GUI.Label(new Rect(SELF_CHEATS_X, 50, 150, TEXT_HEIGHT), "Self Cheats");
                if (GUI.Button(new Rect(SELF_CHEATS_X, 75, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Object ESP - " + boolToStatus(objectEsp))) objectEsp = !objectEsp;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 105, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Player ESP - " + boolToStatus(playerEsp))) playerEsp = !playerEsp;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 135, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Enemy ESP - " + boolToStatus(enemyEsp))) enemyEsp = !enemyEsp;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 165, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Fast Climb - " + boolToStatus(fastClimb))) fastClimb = !fastClimb;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 195, BUTTTON_WIDTH, BUTTTON_HEIGHT), "God Mode - " + boolToStatus(godMode))) godMode = !godMode;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 225, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Super Jump - " + boolToStatus(superJump))) superJump = !superJump;
                if (GUI.Button(new Rect(SELF_CHEATS_X, 255, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Exhausted Hack - " + boolToStatus(exhaustedHack))) exhaustedHack = !exhaustedHack;
                HUDManager hud = HUDManager.Instance;

                if (hud != null) {
                    GUI.Label(new Rect(SELF_CHEATS_X, 285, 150, TEXT_HEIGHT), "Current XP: " + hud.localPlayerXP);
                    GUI.Label(new Rect(SELF_CHEATS_X, 310, 150, TEXT_HEIGHT), "Current Level: " + hud.localPlayerLevel);
                }
                else
                {
                    GUI.Label(new Rect(SELF_CHEATS_X, 285, 150, TEXT_HEIGHT), "Current XP: ???");
                    GUI.Label(new Rect(SELF_CHEATS_X, 310, 150, TEXT_HEIGHT), "Current Level: ???");
                }

                if (GUI.Button(new Rect(SELF_CHEATS_X, 335, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Add 100 XP")){
                    hud.localPlayerXP += 100;
                }

                GUI.Label(new Rect(PLAYER_LIST_X, 50, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Player List");

                GUI.Label(new Rect(SERVER_CHEATS_X, 50, 150, 25), "Server Cheats");
                if (GUI.Button(new Rect(SERVER_CHEATS_X, 75, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Kill Everyone"))
                {
                    PlayerControllerB[] ps = FindObjectsOfType(typeof(PlayerControllerB)) as PlayerControllerB[];
                    for (int i = 0; i < ps.Length; i++)
                    {
                        PlayerControllerB player = ps[i];
                        player.DamagePlayerFromOtherClientServerRpc(9999, new Vector3(0, 0, 0), 0);
                    }
                }
                if (GUI.Button(new Rect(SERVER_CHEATS_X, 105, BUTTTON_WIDTH, BUTTTON_HEIGHT), "End Game"))
                {
                    StartOfRound sor = FindObjectOfType(typeof(StartOfRound)) as StartOfRound;
                    sor.EndGameServerRpc(0);
                }
                if (GUI.Button(new Rect(SERVER_CHEATS_X, 135, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Heal Everyone - " + boolToStatus(healAll))) healAll = !healAll;
                if (GUI.Button(new Rect(SERVER_CHEATS_X, 165, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Add 300 credits")) {
                    Terminal term = FindObjectOfType(typeof(Terminal)) as Terminal;
                    term.SyncGroupCreditsServerRpc(term.groupCredits += 300, term.numberOfItemsInDropship);
                }




                if (GUI.Button(new Rect(Screen.width - BUTTTON_WIDTH, 0, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Show Logs - " + boolToStatus(showLogs))) showLogs = !showLogs;
            }

            if (objectEsp)
            {
                GrabbableObject[] objs = FindObjectsOfType(typeof(GrabbableObject)) as GrabbableObject[];
                for (int i = 0; i < objs.Length; i++)
                {
                    GrabbableObject obj = objs[i];
                    String name = "Object";
                    if (obj.itemProperties != null)
                    {
                        if (obj.itemProperties.itemName != null)
                        {
                            name = obj.itemProperties.itemName;
                        }
                        if (obj.itemProperties.creditsWorth != null)
                        {
                            name += " (" + obj.itemProperties.creditsWorth + ")";
                        }
                    }
                    Vector3 pos;
                    bool flag = Hacks.WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera, obj.transform.position, out pos);
                    if (flag) GUI.Label(new Rect(pos.x, pos.y, 100, TEXT_HEIGHT), name);
                }
            }


            PlayerControllerB[] players = FindObjectsOfType(typeof(PlayerControllerB)) as PlayerControllerB[];
            for (int i = 0; i < players.Length; i++)
            {
                PlayerControllerB player = players[i];
                String name = player.playerUsername;

                if (playerEsp)
                {
                    Vector3 pos;
                    bool flag = Hacks.WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera, player.playerGlobalHead.transform.position, out pos);
                    GUI.color = Color.blue;
                    if (flag) GUI.Label(new Rect(pos.x, pos.y, 100, 25), name);
                }

                if (showMenu) {
                    GUI.color = Color.white;
                    GUI.Label(new Rect(PLAYER_LIST_X, i*145+75, 150, TEXT_HEIGHT), name);
                    if (GUI.Button(new Rect(PLAYER_LIST_X, i * 145 + 100, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Teleport to " + name)) {
                        PlayerControllerB p = GameNetworkManager.Instance.localPlayerController;
                        p.transform.position = player.transform.position;
                    }
                    if (GUI.Button(new Rect(PLAYER_LIST_X, i * 145 + 130, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Kill " + name)) {
                        player.DamagePlayerFromOtherClientServerRpc(1000, new Vector3(0, 0, 0), 0);
                    }
                    if (GUI.Button(new Rect(PLAYER_LIST_X, i * 145 + 160, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Spectate " + name))
                    {
                        if(spectate == i)
                        {
                            spectate = -2;
                        }
                        else
                        {
                            spectate = i;
                        }
                    }
                    if (GUI.Button(new Rect(PLAYER_LIST_X, i * 145 + 190, BUTTTON_WIDTH, BUTTTON_HEIGHT), "Heal"))
                    {
                        player.DamagePlayerFromOtherClientServerRpc(-100, new Vector3(0, 0, 0), 0);
                    }
                }
            }

            if (enemyEsp)
            {
                EnemyAI[] enemies = FindObjectsOfType(typeof(EnemyAI)) as EnemyAI[];
                for (int i = 0; i < enemies.Length; i++)
                {
                    EnemyAI enemy = enemies[i];
                    String name = enemy.enemyType.enemyName;
                    Vector3 pos;
                    bool flag = Hacks.WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera, enemy.transform.position, out pos);
                    GUI.color = Color.red;
                    if (flag) GUI.Label(new Rect(pos.x, pos.y, 100, 100), name);
                }
            }
        }

        int numberDeadLastRound = 0;
        Vector3 initCamera = new Vector3(float.PositiveInfinity, 0, 0);
        float initJump = -1;
        public void Update()
        {
            if (GetAsyncKeyState(0x2D))
            {
                showMenu = !showMenu;
            }

            if (GameNetworkManager.Instance != null)
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

                if (healAll)
                {
                    PlayerControllerB[] players = FindObjectsOfType(typeof(PlayerControllerB)) as PlayerControllerB[];
                    for (int i = 0; i < players.Length; i++)
                    {
                        PlayerControllerB otherPlayer = players[i];
                        if (otherPlayer.health < 50) otherPlayer.DamagePlayerFromOtherClientServerRpc(-100, new Vector3(0, 0, 0), 0);
                    }
                }

                if (player != null)
                {

                    if (initJump == -1)
                    {
                        initJump = player.jumpForce;
                    }

                    if (spectate >= 0)
                    {
                        if (initCamera.x == float.PositiveInfinity) initCamera = player.gameplayCamera.transform.position;
                        PlayerControllerB[] players = FindObjectsOfType(typeof(PlayerControllerB)) as PlayerControllerB[];
                        if (spectate < players.Length)
                        {
                            PlayerControllerB target = players[spectate];
                            player.gameplayCamera.transform.position = target.playerGlobalHead.transform.position;
                        }
                    }

                    if (spectate == -2)
                    {
                        player.gameplayCamera.transform.position = initCamera;
                        spectate = -1;
                        initCamera.x = float.PositiveInfinity;
                    }


                    if (fastClimb)
                    {
                        player.climbSpeed = 12f;
                    }
                    else
                    {
                        player.climbSpeed = 4f;
                    }

                    if (godMode)
                    {
                        player.health = 100;
                    }

                    if (superJump)
                    {
                        player.jumpForce = 50;
                    }
                    else
                    {
                        player.jumpForce = initJump;
                    }

                    if (exhaustedHack)
                    {
                        player.sprintMeter = 1f;
                    }

                    if (jetpackHack) {
                        player.jetpackControls = true;
                    }
                    else
                    {
                        player.jetpackControls = false;
                    }
                }

                DeadBodyInfo[] deadBodies = FindObjectsOfType(typeof(DeadBodyInfo)) as DeadBodyInfo[];
                int newNumOfDead = deadBodies.Length;
                if (newNumOfDead > numberDeadLastRound)
                {
                    logs += deadBodies[newNumOfDead - 1].playerScript.playerUsername + " has died!!!\n";
                    numberDeadLastRound++;
                }
            }
        }
    }
}
