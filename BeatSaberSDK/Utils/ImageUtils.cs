using B83.Image.GIF;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Bitmap = System.Drawing.Bitmap;

namespace Reactive.BeatSaber.Components
{
    public static class ImageUtils
    {
        private static Dictionary<string, AnimatedImage> ImageCache { get; } = new();

        public static async Task TryDownload(
            string url,
            Action<AnimatedImage> onSuccessCallback,
            Action<string> onFailCallback,
            CancellationToken token
        )
        {
            if (ImageCache.TryGetValue(url, out var image)) {
                onSuccessCallback.Invoke(image);
                return;
            }

            byte[]? data = null;
            try {
                data = await GetDataAsync(url);
            } catch (Exception e) {
                onFailCallback?.Invoke(e.Message ?? "");
                return;
            }
            if (token.IsCancellationRequested) return;

            GIFImage? gif = await Task.Run(async () =>
            {
                try
                {
                    using (var reader = new BinaryReader(new MemoryStream(data)))
                    {
                        return new GIFLoader().Load(reader);
                    }
                } catch (Exception e) { return null; }
            });

            if (token.IsCancellationRequested) return;

            AnimatedImage animatedImage;
            if (gif != null)
            {
                animatedImage = new AnimatedImage(gif);
                ImageCache[url] = animatedImage;
            } else
            {
                var sprite = await LoadSpriteAsync(data);
                animatedImage = new AnimatedImage(sprite);
                ImageCache[url] = animatedImage;
            }

            if (token.IsCancellationRequested) return;
            onSuccessCallback.Invoke(animatedImage);
        }

        // Majority of the image loading logic is from BSML
        // https://github.com/monkeymanboy/BeatSaberMarkupLanguage/blob/master/BeatSaberMarkupLanguage/Utilities.cs

        /// <summary>
        /// Gets the content of a resource as a string.
        /// </summary>
        /// <param name="assembly">Assembly containing the resource.</param>
        /// <param name="resource">Full path to the resource.</param>
        /// <returns>The contents of the resource as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the resource specified by <paramref name="resource"/> cannot be found in <paramref name="assembly"/>.</exception>
        public static string GetResourceContent(Assembly assembly, string resource)
        {
            using Stream stream = assembly.GetManifestResourceStream(resource) ?? throw new Exception($"ResourceNotFound {assembly} {resource}");
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        public static void AssemblyFromPath(string inputPath, out Assembly assembly, out string path)
        {
            string[] parameters = inputPath.Split(':');
            switch (parameters.Length)
            {
                case 1:
                    path = parameters[0];
                    assembly = Assembly.Load(path.Substring(0, path.IndexOf('.')));
                    break;
                case 2:
                    path = parameters[1];
                    assembly = Assembly.Load(parameters[0]);
                    break;
                default:
                    throw new Exception($"Could not process resource path {inputPath}");
            }
        }

        /// <summary>
        /// Load a texture from an embedded resource in the calling assembly.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static Task<Texture2D> LoadTextureFromAssemblyAsync(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return LoadTextureFromAssemblyAsync(assembly, name);
        }

        /// <summary>
        /// Load a texture from an embedded resource in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to load the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadTextureFromAssemblyAsync(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);

            return stream != null
                ? await LoadImageAsync(stream)
                : throw new FileNotFoundException($"No embedded resource named '{name}' found in assembly '{assembly.FullName}'");
        }

        /// <summary>
        /// Load a sprite from an embedded resource in the calling assembly.
        /// </summary>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static Task<Sprite> LoadSpriteFromAssemblyAsync(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return LoadSpriteFromAssemblyAsync(assembly, name);
        }

        /// <summary>
        /// Load a sprite from an embedded resource in the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to load the embedded resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Sprite> LoadSpriteFromAssemblyAsync(Assembly assembly, string name)
        {
            return LoadSpriteFromTexture(await LoadTextureFromAssemblyAsync(assembly, name));
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(string path, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            using FileStream fileStream = File.OpenRead(path);
            return await LoadImageAsync(fileStream, updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="data">The image data as a byte array.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(byte[] data, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using MemoryStream memoryStream = new(data);
            return await LoadImageAsync(memoryStream, updateMipmaps, makeNoLongerReadable);
        }

        /// <summary>
        /// Similar to <see cref="ImageConversion.LoadImage(Texture2D, byte[], bool)" /> except it uses <see cref="Bitmap" /> to first load the image and convert it on a separate thread, then uploads the raw pixel data directly.
        /// </summary>
        /// <param name="stream">The image data as a stream.</param>
        /// <param name="updateMipmaps">Whether to create mipmaps for the image or not.</param>
        /// <param name="makeNoLongerReadable">Whether the resulting texture should be made read-only or not.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        public static async Task<Texture2D> LoadImageAsync(Stream stream, bool updateMipmaps = true, bool makeNoLongerReadable = true)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            (int width, int height, byte[] data) = await Task.Factory.StartNew(
                () =>
                {
                    using Bitmap bitmap = new(stream);

                    // flip it over since Unity uses OpenGL coordinates - (0, 0) is the bottom left corner instead of the top left
                    bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

                    BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    byte[] data = new byte[bitmapData.Stride * bitmapData.Height];

                    Marshal.Copy(bitmapData.Scan0, data, 0, bitmapData.Stride * bitmapData.Height);

                    bitmap.UnlockBits(bitmapData);

                    return (bitmap.Width, bitmap.Height, data);
                },
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach);

            // basically all processors are little endian these days so pixel format order is reversed
            Texture2D texture = new(width, height, TextureFormat.BGRA32, false);
            texture.LoadRawTextureData(data);
            texture.Apply(updateMipmaps, makeNoLongerReadable);
            return texture;
        }

        public static async Task<Sprite> LoadSpriteAsync(byte[] data, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(await LoadImageAsync(data), pixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            if (spriteTexture == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
            sprite.name = spriteTexture.name;
            return sprite;
        }

        public static byte[] GetResource(Assembly asm, string resourceName)
        {
            using Stream resourceStream = asm.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new(new byte[resourceStream.Length], true);

            resourceStream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        public static async Task<byte[]> GetResourceAsync(Assembly asm, string resourceName)
        {
            using Stream resourceStream = asm.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new(new byte[resourceStream.Length], true);

            await resourceStream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        internal static async Task<byte[]> GetDataAsync(string location)
        {
            if (location.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || location.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return await GetWebDataAsync(location);
            }
            else if (File.Exists(location))
            {
                using (FileStream fileStream = File.OpenRead(location))
                using (MemoryStream memoryStream = new(new byte[fileStream.Length], true))
                {
                    await fileStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            else
            {
                AssemblyFromPath(location, out Assembly asm, out string newPath);
                return await GetResourceAsync(asm, newPath);
            }
        }

        private static Task<byte[]> GetWebDataAsync(string url)
        {
            TaskCompletionSource<byte[]> taskCompletionSource = new();
            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            webRequest.SendWebRequest().completed += (asyncOperation) =>
            {
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    taskCompletionSource.SetException(new Exception($"Failed to get data {webRequest}"));
                }
                else
                {
                    taskCompletionSource.SetResult(webRequest.downloadHandler.data);
                }
            };

            return taskCompletionSource.Task;
        }
    }
}
