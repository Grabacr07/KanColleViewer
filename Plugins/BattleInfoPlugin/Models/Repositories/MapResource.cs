using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DDW.Swf;

namespace BattleInfoPlugin.Models.Repositories
{
    public class MapResource
    {
		public static BitmapSource GetMapImage(MapInfo map)
        {
            return ExistsAssembly ? MapResourcePrivate.GetMapImage(map) : null;
        }

        public static IDictionary<int, Point> GetMapCellPoints(MapInfo map)
        {
            return ExistsAssembly ? MapResourcePrivate.GetMapCellPoints(map) : new Dictionary<int, Point>();
        }

        public static bool HasMapSwf(MapInfo map)
        {
            return ExistsAssembly && MapResourcePrivate.HasMapSwf(map);
        }

        private static bool? _ExistsAssembly;

        public static bool ExistsAssembly
        {
            get
            {
                if (_ExistsAssembly.HasValue) return _ExistsAssembly.Value;

                try
				{
					string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
					
                    System.Reflection.Assembly.UnsafeLoadFrom(Path.Combine(MainFolder,"lib","SwfFormat.dll"));
					System.Reflection.Assembly.UnsafeLoadFrom(Path.Combine(MainFolder, "lib", "ICSharpCode.SharpZipLib.dll"));
                    _ExistsAssembly = true;
                }
                catch (FileNotFoundException)
                {
                    _ExistsAssembly = false;
                }
                return _ExistsAssembly.Value;
            }
        }

        private static class MapResourcePrivate
        {
            public static BitmapSource GetMapImage(MapInfo map)
            {
                var swf = map.ToSwf();
                if (swf == null) return null;

                return swf.Tags
                    .SkipWhile(x => x.TagType != TagType.ShowFrame) //1フレーム飛ばす
                    .OfType<DefineBitsTag>()
                    .FirstOrDefault(x => x.TagType == TagType.DefineBitsJPEG3)
                    .ToBitmapFrame();
            }

            public static IDictionary<int, Point> GetMapCellPoints(MapInfo map)
            {
                var swf = map.ToSwf();
                if (swf == null) return new Dictionary<int, Point>();

                var places = swf.Tags
                    .OfType<DefineSpriteTag>()
                    .SelectMany(sprite => sprite.ControlTags)
                    .OfType<PlaceObject2Tag>()
                    .Where(place => place.Name != null)
                    .Where(place => place.Name.StartsWith("line"))
                    .ToArray();
                var cellNumbers = Master.Current.MapCells
                    .Where(c => c.Value.MapInfoId == map.Id)
                    //.Where(c => new[] { 0, 1, 2, 3, 8 }.All(x => x != c.Value.ColorNo)) //敵じゃないセルは除外(これでは気のせいは見分け付かない)
                    .Select(c => c.Value.IdInEachMapInfo)
                    .ToArray();
                return cellNumbers
                    .OrderBy(n => n)
                    .Select(n => new KeyValuePair<int, Point>(n, places.FindPoint(n)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public static bool HasMapSwf(MapInfo map)
            {
                return map.HasMapSwf();
            }
        }
    }

    static class MapResourceExtensions
    {
        private static readonly string mapDir = Properties.Settings.Default.CacheDirPath + "\\kcs\\resources\\swf\\map\\";

        public static bool HasMapSwf(this MapInfo map)
        {
            var filePath = mapDir
                + map.MapAreaId.ToString("00") + "_" + map.IdInEachMapArea.ToString("00") + ".swf";
            return File.Exists(filePath);

        }

        public static SwfCompilationUnit ToSwf(this MapInfo map)
        {
            var filePath = mapDir
                + map.MapAreaId.ToString("00") + "_" + map.IdInEachMapArea.ToString("00") + ".swf";
            if (!File.Exists(filePath)) return null;
            var reader = new SwfReader(File.ReadAllBytes(filePath));
            return new SwfCompilationUnit(reader);
        }

        public static Point FindPoint(this IEnumerable<PlaceObject2Tag> places, int num)
        {
            return places
                .Single(p => p.Name == "line" + num)
                .ToPoint();
        }

        public static Point ToPoint(this PlaceObject2Tag tag)
        {
            return new Point(tag.Matrix.TranslateX / 20, tag.Matrix.TranslateY / 20);
        }

        public static BitmapFrame ToBitmapFrame(this DefineBitsTag tag)
        {
            if (tag == null) return null;

            BitmapFrame frame;
            try
            {
                using (var stream = new MemoryStream(RepairJpegData(tag.JpegData)))
                {
                    frame = BitmapFrame.Create(stream,
                        BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                }
            }
            catch (NotSupportedException)
            {
                return null;
            }

            var pixelCount = (uint)(frame.Width * frame.Height);
            var alphaData = tag.HasAlphaData
                ? SwfReader.Decompress(tag.CompressedAlphaData, pixelCount)
                : CreateNonTransparentAlphaData(pixelCount).ToArray();
            var bmp = AlphaBlending(frame, alphaData);
            bmp.Freeze();
            var blendedFrame = BitmapFrame.Create(bmp, null, null, frame.ColorContexts);
            blendedFrame.Freeze();
            return blendedFrame;
        }

        private static BitmapSource AlphaBlending(BitmapSource bitmap, byte[] alphaData)
        {
            var pixelCount = (uint)(bitmap.Width * bitmap.Height);
            var bmp = new WriteableBitmap(new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0));
            var bytes = new byte[pixelCount * 4];
            var rect = new Int32Rect(0, 0, (int)bmp.Width, (int)bmp.Height);
            bmp.CopyPixels(rect, bytes, bmp.BackBufferStride, 0);
            for (var i = 0; i < alphaData.Length; i++)
            {
                bytes[i * 4 + 3] = alphaData[i];
            }
            bmp.WritePixels(rect, bytes, bmp.BackBufferStride, 0);
            return bmp;
        }

        private static IEnumerable<byte> CreateNonTransparentAlphaData(uint length)
        {
            for (var i = 0; i < length; i++)
            {
                yield return 0xFF;
            }
        }

        private static byte[] RepairJpegData(byte[] data)
        {
            //なぜか先頭2バイトが消えてることがある
            if (data.LongLength < 2) return data;
            if (data[0] == 0xFF && data[1] == 0xE0)
                return new byte[] { 0xFF, 0xD8 }.Concat(data).ToArray();
            return data;
        }
    }
}
