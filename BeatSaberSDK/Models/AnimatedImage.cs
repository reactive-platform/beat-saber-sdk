using System;
using System.Collections;
using B83.Image.GIF;
using UnityEngine;

namespace Reactive.BeatSaber.Components
{
    public class AnimatedImage {
        #region Constructors

        public readonly Sprite Sprite;

        public readonly bool IsAnimated;
        private readonly GIFImage _gifImage;
        private readonly RenderTexture _renderTexture;

        private readonly Color32[] _colors;
        private int _currentIndex = 0;
        private float _deltaAccumulated = 0;

        public AnimatedImage(
            GIFImage gifImage
        ) {
            IsAnimated = true;

            _renderTexture = new RenderTexture(gifImage.screen.width, gifImage.screen.height, 0, RenderTextureFormat.Default, 10);
            _renderTexture.Create();

            Sprite = Sprite.Create(_renderTexture.GetTexture2D(), new Rect(0, 0, gifImage.screen.width, gifImage.screen.height), new Vector2(0, 0), 1);
            _colors = Sprite.texture.GetPixels32();
            for (var i = 0; i < _colors.Length; i++) {
                _colors[i] = new Color32(0, 0, 0, 0);
            }

            _gifImage = gifImage;
        }

        public AnimatedImage(Sprite sprite) {
            IsAnimated = false;
            Sprite = sprite;
        }

        #endregion

        #region Playback

        public void OnUpdate(float timeDelta) {
            if (!IsAnimated || _gifImage.imageData.Count == 0) {
                return;
            }

            var _originalTexture = Sprite.texture;
            var frame = _gifImage.imageData[_currentIndex];

            if (_deltaAccumulated == 0) {
                frame.Dispose(_colors, _originalTexture.width, _originalTexture.height);
                try {
                    frame.DrawTo(_colors, _renderTexture.width, _renderTexture.height);
                } catch (Exception) {
                    _originalTexture.SetPixels32(_colors);
                    _originalTexture.Apply();
                    Graphics.Blit(_originalTexture, _renderTexture);
                    return;
                }
                    
                _originalTexture.SetPixels32(_colors);
                _originalTexture.Apply();
                Graphics.Blit(_originalTexture, _renderTexture);
            }

            _deltaAccumulated += timeDelta;
            if (_deltaAccumulated >= frame.graphicControl.fdelay) {
                _deltaAccumulated = 0;
                if (_currentIndex < _gifImage.imageData.Count - 1) {
                    _currentIndex++;
                } else {
                    _currentIndex = 0;
                }
            }
        }

        #endregion
    }
}
