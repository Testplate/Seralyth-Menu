/*
 * Seralyth Menu  Classes/Mods/VirtualStumpAd.cs
 * A community driven mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Seralyth Software
 * https://github.com/Seralyth/Seralyth-Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GorillaExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using static Seralyth.Menu.Main;

namespace Seralyth.Classes.Mods
{
    public class VirtualStumpAd : MonoBehaviour
    {
        public static VirtualStumpAd Instance { get; private set; }
        public static SpriteRenderer SpriteRenderer { get; private set; }

        private bool hasSetupFeaturedMapVideo;
        private VideoPlayer videoPlayer;

        public static GameObject LoadingText;
        public static GameObject MapInfoText;
        public static GameObject FeaturedMaps;
        public static GameObject DisplayTextObj;

        private Vector3 oldLocalScale = Vector3.zero;
        private string oldText = "";

        private SpriteRendererData cachedSpriteRendererData;

        private Material cachedVideoMaterial;
        private RenderTexture cachedRenderTexture;

        private void Awake() => Instance = this;

        private void Update()
        {
            if (hasSetupFeaturedMapVideo && videoPlayer != null)
            {
                if (videoPlayer.enabled && !videoPlayer.isPlaying)
                {
                    if (!videoPlayer.isLooping)
                        videoPlayer.isLooping = true;

                    videoPlayer.Play();
                }

                return;
            }

            LoadingText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");
            MapInfoText = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");
            FeaturedMaps = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay");
            DisplayTextObj = GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");

            if (DisplayTextObj != null)
            {
                foreach (Transform child in DisplayTextObj.transform)
                {
                    if (child.name.ToLower().EndsWith("tmp"))
                        child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }

            if (MapInfoText == null || FeaturedMaps == null)
                return;

            try
            {
                var featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
                if (featuredMapText != null)
                {
                    oldText = featuredMapText.text;
                    featuredMapText.text = "<b><color=#7C00FA>Seralyth Menu</color></b>";
                    MapInfoText.SetActive(true);
                }

                LoadingText?.SetActive(false);

                GameObject featuredMapImage = FeaturedMaps.transform.Find("FeaturedMapImage")?.gameObject;
                if (featuredMapImage == null)
                    return;

                CacheAndRemoveSpriteRenderer(featuredMapImage);

                MeshFilter mf = featuredMapImage.GetOrAddComponent<MeshFilter>();
                mf.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");

                MeshRenderer mr = featuredMapImage.GetOrAddComponent<MeshRenderer>();

                if (cachedVideoMaterial == null)
                    cachedVideoMaterial = new Material(Shader.Find("Unlit/Texture"));

                mr.material = cachedVideoMaterial;

                videoPlayer = featuredMapImage.GetOrAddComponent<VideoPlayer>();
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.url = $"{PluginInfo.ServerResourcePath}/Videos/vstump-video.mp4";

                if (cachedRenderTexture == null)
                    cachedRenderTexture = new RenderTexture(512, 512, 0);

                videoPlayer.targetTexture = cachedRenderTexture;
                mr.material.mainTexture = cachedRenderTexture;

                if (oldLocalScale == Vector3.zero)
                    oldLocalScale = featuredMapImage.transform.localScale;

                featuredMapImage.transform.localScale = new Vector3(0.845f, 0.445f, 1f);

                videoPlayer.isLooping = true;
                videoPlayer.Play();

                featuredMapImage.SetActive(true);
                hasSetupFeaturedMapVideo = true;
            }
            catch
            {
                // uhm hi....
            }
        }

        private void OnDisable()
        {
            hasSetupFeaturedMapVideo = false;

            if (MapInfoText != null)
            {
                TextMeshPro featuredMapText = MapInfoText.GetComponent<TextMeshPro>();
                if (featuredMapText != null)
                    featuredMapText.text = oldText;

                MapInfoText.SetActive(false);
            }

            LoadingText?.SetActive(true);

            if (DisplayTextObj != null)
            {
                foreach (Transform child in DisplayTextObj.transform)
                {
                    if (child.name.ToLower().EndsWith("tmp"))
                        child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }

            GameObject featuredMapImage = FeaturedMaps?.transform.Find("FeaturedMapImage")?.gameObject;
            if (featuredMapImage == null)
                return;

            Destroy(featuredMapImage.GetComponent<MeshFilter>());
            Destroy(featuredMapImage.GetComponent<MeshRenderer>());

            featuredMapImage.transform.localScale = oldLocalScale;

            Destroy(featuredMapImage.GetComponent<VideoPlayer>());

            ApplySpriteRenderer(featuredMapImage);
        }

        private void CacheAndRemoveSpriteRenderer(GameObject target)
        {
            SpriteRenderer = target.GetComponent<SpriteRenderer>();
            if (SpriteRenderer == null)
                return;

            cachedSpriteRendererData = new SpriteRendererData
            {
                Sprite = SpriteRenderer.sprite,
                Material = SpriteRenderer.material,
                Color = SpriteRenderer.color,
                SortingLayerID = SpriteRenderer.sortingLayerID,
                SortingOrder = SpriteRenderer.sortingOrder,
                FlipX = SpriteRenderer.flipX,
                FlipY = SpriteRenderer.flipY,
                DrawMode = SpriteRenderer.drawMode,
                Size = SpriteRenderer.size
            };

            Destroy(SpriteRenderer);
        }

        private void ApplySpriteRenderer(GameObject target)
        {
            if (cachedSpriteRendererData == null)
                return;

            SpriteRenderer = target.AddComponent<SpriteRenderer>();

            SpriteRenderer.sprite = cachedSpriteRendererData.Sprite;
            SpriteRenderer.material = cachedSpriteRendererData.Material;
            SpriteRenderer.color = cachedSpriteRendererData.Color;
            SpriteRenderer.sortingLayerID = cachedSpriteRendererData.SortingLayerID;
            SpriteRenderer.sortingOrder = cachedSpriteRendererData.SortingOrder;
            SpriteRenderer.flipX = cachedSpriteRendererData.FlipX;
            SpriteRenderer.flipY = cachedSpriteRendererData.FlipY;
            SpriteRenderer.drawMode = cachedSpriteRendererData.DrawMode;
            SpriteRenderer.size = cachedSpriteRendererData.Size;
        }
    }

    public sealed class SpriteRendererData
    {
        public Sprite Sprite;
        public Material Material;
        public Color Color;
        public int SortingLayerID;
        public int SortingOrder;
        public bool FlipX;
        public bool FlipY;
        public SpriteDrawMode DrawMode;
        public Vector2 Size;
    }
}
