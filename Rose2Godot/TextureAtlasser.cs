using Rose2Godot.GodotExporters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Rose2Godot
{

    public struct AtlasLayout
    {
        public int width;
        public int height;
        public List<Bitmap> textures;
        public List<Rect> rects;

        public AtlasLayout(int w, int h)
        {
            width = w;
            height = h;
            textures = new List<Bitmap>();
            rects = new List<Rect>();
        }
    };

    public class TextureAtlasser
    {
		public static Bitmap MakeAtlas(ref List<Bitmap> textures, out Rect[] packedRects, int padding)
		{
			AtlasLayout packResults = PackTextures(textures, 512, 512);
			Bitmap outAtlas = new Bitmap(packResults.width, packResults.height);

			textures = packResults.textures;
			packedRects = packResults.rects.ToArray();

			Bitmap[] readables = new Bitmap[textures.Count];

			for (int i = 0; i < packResults.textures.Count; i++)
			{
				Rect rect = packResults.rects[i];
				Bitmap readableTex = textures[i];

				//load the image uncompressed
				//string fileURL = AssetDatabase.GetAssetPath(packResults.textures[i]);
				//byte[] imgByes = File.ReadAllBytes(fileURL);
				//readableTex = (Bitmap)Image.FromFile(fileURL);
				//readableTex.wrapMode = TextureWrapMode.Clamp;
				//readableTex.LoadImage(imgByes);
				readables[i] = readableTex;

				int localPadding = Math.Min(padding, readableTex.Width / 4);

				//int halfPadding = 0;//padding / 2;
				Rect innerRect = packResults.rects[i];
				innerRect.x += localPadding;
				innerRect.y += localPadding;
				innerRect.Width -= localPadding * 2;
				innerRect.Height -= localPadding * 2;

				for (int x = (int)rect.x; x < (int)rect.x + (int)rect.Width; x++)
				{
					for (int y = (int)rect.y; y < (int)rect.y + (int)rect.Height; y++)
					{
						int xSample = x - (int)innerRect.x;
						int ySample = y - (int)innerRect.y;

						Color pixel = readableTex.GetPixel((int)(xSample / innerRect.Width), (int)(ySample / innerRect.Height));
						outAtlas.SetPixel(x, y, pixel);
					}
				}

				packedRects[i] = innerRect;
			}

			for (int x = 0; x < outAtlas.Width; ++x)
			{
				for (int y = 0; y < outAtlas.Height; ++y)
				{
					float closestDist = float.MaxValue;
					
					Color c = Color.Black;

					for (int r = 0; r < packResults.rects.Count; ++r)
					{
						Rect curRect = packResults.rects[r];
						if (curRect.Contains(new Vector2(x, y)))
						{
							closestDist = -1;
							break;
						}

						float d = DistanceToRect(curRect, x, y);

						if (d < closestDist)
						{
							closestDist = d;
							float uvX = (x - curRect.x) / curRect.Width;
							float uvY = (y - curRect.y) / curRect.Height;
							c = readables[r].GetPixel((int)uvX, (int)uvY);
						}
					}

					if (closestDist > -1)
					{
						outAtlas.SetPixel(x, y, c);
					}
				}
			}

			//outAtlas.wrapMode = TextureWrapMode.Clamp;
			//outAtlas.Apply();
			return outAtlas;
		}

		private static float DistanceToRect(Rect r, int x, int y)
		{
			//float xDist = float.MaxValue;
			//float yDist = float.MaxValue;

			float xDist = Math.Max(Math.Abs(x - r.Center.X) - r.Width / 2, 0);
			float yDist = Math.Max(Math.Abs(y - r.Center.Y) - r.Height / 2, 0);
			return xDist * xDist + yDist * yDist;
		}

		public static AtlasLayout PackTextures(List<Bitmap> textures, int maxWidth, int maxHeight)
		{
			AtlasLayout results = new AtlasLayout(maxWidth, maxHeight);

			List<Rect> freeRects = new List<Rect>();
			List<Bitmap> texturesToPlace = new List<Bitmap>(textures);
			texturesToPlace = texturesToPlace.OrderBy(x => x.Width * x.Height).ToList();

			freeRects.Add(new Rect(0, 0, maxWidth, maxHeight));

			//Walk all textures and find the one that fits the best given 
			//our current freeRect list. Then start again

			while (texturesToPlace.Count > 0)
			{
				int bestShortSideScore = int.MaxValue;
				int bestLongSideScore = int.MaxValue;
				Bitmap bestTex = texturesToPlace[0];
				Rect bestRect = new Rect();

				foreach (Bitmap curTex in texturesToPlace)
				{
					int shortSideScore = int.MaxValue;
					int longSideScore = int.MaxValue;

					Rect target = FindIdealRect(curTex.Width,
						curTex.Height,
						freeRects,
						ref shortSideScore,
						ref longSideScore);

					if (shortSideScore < bestShortSideScore
						|| (shortSideScore == bestShortSideScore && longSideScore < bestLongSideScore))
					{
						bestShortSideScore = shortSideScore;
						bestLongSideScore = longSideScore;
						bestTex = curTex;
						bestRect = target;
					}
				}

				if (bestRect.Width > 0 && bestRect.Height > 0)
				{
					RemoveRectFromFreeList(bestRect, freeRects);
					results.textures.Add(bestTex);
					results.rects.Add(bestRect);
					texturesToPlace.Remove(bestTex);

				}
				else break; //no room left
			}

			return results;
		}

		private static Rect FindIdealRect(int width, int height, List<Rect> freeRects, ref int bestShortSideFit, ref int bestLongSideFit)
		{
			Rect bestNode = new Rect();

			for (int i = 0; i < freeRects.Count; ++i)
			{
				if (freeRects[i].Width >= width && freeRects[i].Height >= height)
				{
					int remainingX = (int)(freeRects[i].Width - width);
					int remainingY = (int)(freeRects[i].Height - height);

					int shortSideFit = Math.Min(remainingX, remainingY);
					int longSideFit = Math.Max(remainingX, remainingY);

					if (shortSideFit < bestShortSideFit ||
						(shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit))
					{
						bestNode = new Rect(freeRects[i].x, freeRects[i].y, width, height);
						bestShortSideFit = shortSideFit;
						bestLongSideFit = longSideFit;
					}
				}
			}

			return bestNode;
		}

		//remove a rect area from the freeRect list
		private static void RemoveRectFromFreeList(Rect rectToRemove, List<Rect> freeRects)
		{
			for (int i = 0; i < freeRects.Count; ++i)
			{
				Rect freeRect = freeRects[i];

				if (freeRect.Overlaps(rectToRemove))
				{
					if (rectToRemove.x < freeRect.x + freeRect.Width && rectToRemove.x + rectToRemove.Width > freeRect.x)
					{
						// New node at the top side of the used node.
						if (rectToRemove.y > freeRect.y && rectToRemove.y < freeRect.y + freeRect.Height)
						{
							Rect newNode = freeRect;
							newNode.Height = rectToRemove.y - newNode.y;
							freeRects.Add(newNode);
						}

						// New node at the bottom side of the used node.
						if (rectToRemove.y + rectToRemove.Height < freeRect.y + freeRect.Height)
						{
							Rect newNode = freeRect;
							newNode.y = rectToRemove.y + rectToRemove.Height;
							newNode.Height = freeRect.y + freeRect.Height - (rectToRemove.y + rectToRemove.Height);
							freeRects.Add(newNode);
						}
					}

					if (rectToRemove.y < freeRect.y + freeRect.Height && rectToRemove.y + rectToRemove.Height > freeRect.y)
					{
						// New node at the left side of the used node.
						if (rectToRemove.x > freeRect.x && rectToRemove.x < freeRect.x + freeRect.Width)
						{
							Rect newNode = freeRect;
							newNode.Width = rectToRemove.x - newNode.x;
							freeRects.Add(newNode);
						}

						// New node at the right side of the used node.
						if (rectToRemove.x + rectToRemove.Width < freeRect.x + freeRect.Width)
						{
							Rect newNode = freeRect;
							newNode.x = rectToRemove.x + rectToRemove.Width;
							newNode.Width = freeRect.x + freeRect.Width - (rectToRemove.x + rectToRemove.Width);
							freeRects.Add(newNode);
						}
					}

					freeRects.RemoveAt(i--);
				}
			}

			//remove free rects that are wholly contained by others

			for (int i = 0; i < freeRects.Count; ++i)
			{
				for (int j = i + 1; j < freeRects.Count; ++j)
				{
					if (freeRects[i].IsContainedIn(freeRects[j]))
					{
						freeRects.RemoveAt(i);
						--i;
						break;
					}

					if (freeRects[j].IsContainedIn(freeRects[i]))
					{
						freeRects.RemoveAt(j);
						--j;
					}
				}
			}
		}
	}
}
