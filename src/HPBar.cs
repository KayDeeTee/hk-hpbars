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

        public static Dictionary<string, int> bosses;

        public override string GetVersion()
        {
            return version;
        }

        public override void Initialize()
        {
            Log("Initializing HPBars");

            bosses = new Dictionary<string, int>();
            bosses.Add( "Head", -1);                                    
            bosses.Add( "False Knight New", -1);
            bosses.Add( "Hornet Boss 1", -1);
            bosses.Add( "Giant Fly", -1);
            bosses.Add( "Mega Jellyfish", -1);
            bosses.Add( "Mawlek Body", -1);
            bosses.Add( "Mawlek Head", -1);
            bosses.Add( "Mantis Lord", -1);
            bosses.Add( "Mantis Lord S1", -1);
            bosses.Add( "Mantis Lord S2", -1);
            bosses.Add( "Mage Lord", -1);
            bosses.Add( "Mage Lord Phase2", -1);
            bosses.Add( "Infected Knight", -1);
            bosses.Add( "Mega Zombie Beam Miner (1)", -1);
            bosses.Add( "Zombie Beam Miner Rematch", -1);
            bosses.Add( "Dung Defender", -1);
            bosses.Add( "Mimic Spider", -1);
            bosses.Add( "Hornet Boss 2", -1);
            bosses.Add( "Fluke Mother", -1);
            bosses.Add( "Mantis Traitor Lord", -1);
            bosses.Add( "Grimm Boss", -1);
            bosses.Add( "Black Knight 1", -1);
            bosses.Add("Black Knight 2", -1);
            bosses.Add("Black Knight 3", -1);
            bosses.Add("Black Knight 4", -1);
            bosses.Add("Black Knight 5", -1);
            bosses.Add("Black Knight 6", -1);
            bosses.Add( "Jar Collector", -1);
            bosses.Add( "Lost Kin", -1);
            bosses.Add( "False Knight Dream", -1);
            bosses.Add( "Nightmare Grimm Boss", -1);
            bosses.Add( "White Defender", -1);
            bosses.Add( "Dream Mage Lord", -1);
            bosses.Add( "Dream Mage Lord Phase2", -1);
            bosses.Add( "Grey Prince", -1);
            bosses.Add( "THK", -1);
            bosses.Add( "Radiance", -1);

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

            if (bosses.ContainsKey(fsm.GameObjectName) || GameManager.instance.sceneName == "Room_Final_Boss_Core")
            {
                bossFSM = fsm.FsmComponent;
                Log(bossFSM.FsmVariables.GetFsmInt("HP").Value);

                if (GameManager.instance.sceneName == "Room_Final_Boss_Core")
                {
                    bossFSM.gameObject.name = "THK";
                }
                if (bosses[bossFSM.Fsm.GameObjectName] == -1)
                    bosses[bossFSM.Fsm.GameObjectName] = bossFSM.FsmVariables.GetFsmInt("HP").Value;

                maxHP = bosses[bossFSM.Fsm.GameObjectName];

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
