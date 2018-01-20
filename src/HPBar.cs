using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace HPBar
{
    public class HPBar : Mod
    {

        private static string version = "0.0.1";

        public static Sprite bg;
        public static Sprite fg;
        public static Sprite ol;

        private static GameObject canvas;
        private static GameObject bg_go;
        private static GameObject fg_go;
        private static GameObject ol_go;

        public static CanvasGroup canvas_group;
        public static Image health_bar;

        public static PlayMakerFSM bossFSM;
        public static int maxHP;


        public override string GetVersion()
        {
            return version;
        }

        public override void Initialize()
        {
            Log("Initializing HPBars");

            bg = CanvasUtil.CreateSprite(ResourceLoader.GetBackgroundImage(), 0, 0, 1, 1);
            fg = CanvasUtil.CreateSprite(ResourceLoader.GetForegroundImage(), 0, 0, 960, 1);
            ol = CanvasUtil.CreateSprite(ResourceLoader.GetOutlineImage(), 0, 0, 966, 27);

            canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1280, 720));
            canvas_group = canvas.GetComponent<CanvasGroup>();

            bg_go = CanvasUtil.CreateImagePanel(canvas, bg, new CanvasUtil.RectData(new Vector2(960, 25), new Vector2(0, 32), new Vector2(0.5f, 0), new Vector2(0.5f, 0)));
            fg_go = CanvasUtil.CreateImagePanel(canvas, fg, new CanvasUtil.RectData(new Vector2(960, 25), new Vector2(0, 32), new Vector2(0.5f, 0), new Vector2(0.5f, 0)));
            ol_go = CanvasUtil.CreateImagePanel(canvas, ol, new CanvasUtil.RectData(new Vector2(966, 27), new Vector2(0, 32), new Vector2(0.5f, 0), new Vector2(0.5f, 0)));

            health_bar = fg_go.GetComponent<Image>();

            health_bar.type = Image.Type.Filled;
            health_bar.fillMethod = Image.FillMethod.Horizontal;
            health_bar.preserveAspect = false;

            bg_go.GetComponent<Image>().preserveAspect = false;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += checkComponent;
            //ModHooks.Instance.SlashHitHook += Instance_SlashHitHook;
            ModHooks.Instance.HeroUpdateHook += Instance_HeroUpdateHook;
            ModHooks.Instance.OnGetEventSenderHook += Instance_OnGetEventSenderHook;
            MonoBehaviour.DontDestroyOnLoad(canvas);

            canvas_group.alpha = 0;

            Log("Initialized HPBars");
        }

        void Instance_HeroUpdateHook()
        {
            if (bossFSM != null)
            {
                if( bossFSM.gameObject.name == "Radiance" )
                    health_bar.fillAmount = (float)(bossFSM.FsmVariables.GetFsmInt("HP").Value-1300) / (float)(maxHP-1300);
                else
                    health_bar.fillAmount = (float)bossFSM.FsmVariables.GetFsmInt("HP").Value / (float)maxHP;
            }
        }

        GameObject Instance_OnGetEventSenderHook(GameObject go, HutongGames.PlayMaker.Fsm fsm)
        {
            Log(fsm.GameObjectName);
            bool update = true;
            if (bossFSM.Fsm.GameObjectName != fsm.GameObjectName) {
                switch (fsm.GameObjectName)
                {
                    case "Head": maxHP = GameManager.instance.sceneName == "Crossroads_10" ? 40 : 40; break;
                    case "False Knight New": maxHP = 65; break;
                    case "Hornet Boss 1": maxHP = 225; break;
                    case "Giant Fly": maxHP = 90; break;
                    case "Mega Jellyfish":
                    case "Mawlek Body":
                    case "Mawlek Head":
                        maxHP = 300; break;
                    case "Mantis Lord": maxHP = 210; break;
                    case "Mantis Lord S1":
                    case "Mantis Lord S2": maxHP = 160; break;
                    case "Mage Lord": maxHP = 275; break;
                    case "Mage Lord Phase2": maxHP = 110; break;
                    case "Infected Knight": maxHP = 525; break;
                    case "Mega Zombie Beam Miner (1)": maxHP = 280; break;
                    case "Zombie Beam Miner Rematch": maxHP = 450; break;
                    case "Dung Defender":
                    case "Mimic Spider":
                    case "Hornet Boss 2": maxHP = 700; break;
                    case "Fluke Mother": maxHP = 350; break;
                    case "Mantis Traitor Lord": maxHP = 360; break;
                    case "Grimm Boss":
                        switch (PlayerData.instance.nailSmithUpgrades)
                        {
                            case 3: maxHP = 930; break;
                            case 4: maxHP = 1000; break;
                            default: maxHP = 800; break;
                        }
                        break;
                    case "Black Knight": maxHP = 220; break;
                    case "Jar Collector": maxHP = 750; break;
                    case "Lost Kin": maxHP = 1200; break;
                    case "False Knight Dream": maxHP = 360; break;
                    case "Nightmare Grimm Boss":
                    case "White Defender": maxHP = 1600; break;
                    case "Dream Mage Lord": maxHP = 900; break;
                    case "Dream Mage Lord Phase2": maxHP = 350; break;
                    case "Grey Prince":
                        int n = PlayerData.instance.greyPrinceDefeats;
                        if (n > 3)
                            n = 3;
                        maxHP = 1200 + (n * 100); break;
                    case "Radiance": maxHP = 3000; break;
                    default:
                        update = false;
                        break;
                }
            }
            

            if (GameManager.instance.sceneName == "Room_Final_Boss_Core")
            {
                maxHP = 1300;
                update = true;
            }

            if (update)
            {
                bossFSM = fsm.FsmComponent;
                Log(bossFSM.FsmVariables.GetFsmInt("HP").Value);

                if (bossFSM.FsmVariables.GetFsmInt("HP").Value > maxHP)
                    maxHP = bossFSM.FsmVariables.GetFsmInt("HP").Value;

                canvas_group.alpha = 1;
                health_bar.fillAmount = (float)bossFSM.FsmVariables.GetFsmInt("HP").Value / (float)maxHP;
                if (bossFSM.FsmVariables.GetFsmInt("HP").Value <= 0 || bossFSM == null)
                    canvas_group.alpha = 0;
            }

            return go;
        }

        public void checkComponent(Scene s, LoadSceneMode lsm)
        {
            bossFSM = null;
            canvas_group.alpha = 0;
        }

    }
}
