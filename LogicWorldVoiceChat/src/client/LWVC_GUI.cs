using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FoxFireFive_LogicWorldVoiceChat.Client
{
    public class LWVC_GUI
    {
        private static string dll_path;
        private static string base_dir;
        private static string assets_path;
        private static string muted_path;
        private static string unmuted_path;

        public static bool hidden = false;

        public static GameObject muted_icon;
        public static GameObject unmuted_icon;

        public static void Setup()
        {
            dll_path = Assembly.GetExecutingAssembly().Location;
            base_dir = Directory.GetParent(dll_path).Parent.Parent.FullName;
            assets_path = Path.Combine(base_dir, "GameData", "LogicWorldVoiceChat", "assets");
            muted_path = Path.Combine(assets_path, "Muted.png");
            unmuted_path = Path.Combine(assets_path, "Unmuted.png");

            Texture2D muted_tex = LoadTexture(muted_path);
            Texture2D unmuted_tex = LoadTexture(unmuted_path);

            Sprite muted_sprite = TextureToSprite(muted_tex);
            Sprite unmuted_sprite = TextureToSprite(unmuted_tex);

            Canvas canvas = FindBuildingCanvas();
            if (canvas != null)
            {
                muted_icon = CreateIconTopRight(canvas, muted_sprite, "LWVC_MutedIcon", new Vector2(-20, -20));
                unmuted_icon = CreateIconTopRight(canvas, unmuted_sprite, "LWVC_UnmutedIcon", new Vector2(-20, -20));
            }
        }

        private static Texture2D LoadTexture(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Missing file: {path}");
                return null;
            }

            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
            tex.LoadImage(data);
            return tex;
        }

        private static Sprite TextureToSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private static Canvas FindBuildingCanvas()
        {
            Scene scene = SceneManager.GetSceneByName("UI_Gameplay");

            if (!scene.isLoaded)
            {
                Debug.LogError("UI_Gameplay scene is not loaded??");
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                if (root.name == "Building Canvas")
                {
                    return root.GetComponent<Canvas>();
                }
            }

            Debug.LogError("Building Canvas not found in UI_Gameplay??");
            return null;
        }

        private static GameObject CreateIconTopRight(Canvas canvas, Sprite sprite, string name, Vector2 offset)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(canvas.transform, false);
            obj.AddComponent<Image>().sprite = sprite;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(96, 96);
            rect.anchoredPosition = offset;

            return obj;
        }

        public static void HideIcons()
        {
            if (hidden) return;

            muted_icon.SetActive(false);
            unmuted_icon.SetActive(false);

            hidden = true;
        }
    }
}
