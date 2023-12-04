using System.Drawing;
using System.Drawing.Imaging;

namespace BCL.Parsing {
	public class BitmapParser : IDisposable {
		/// <summary>
		/// A delegate type for manipulating the RGB values of a particular pixel in a bitmap.
		/// </summary>
		public delegate void BitmapProcessDelegate(ref int red, ref int green, ref int blue);

		/// <summary>
		/// Returns a reference to the internal array of Bitmap objects.
		/// </summary>
		public ref Bitmap[] GetAllBitmaps() => ref _bitmaps;

		/// <summary>
		/// Returns a read-only reference to the array of the paths to all images.
		/// </summary>
		public ref readonly string[] GetAllPaths() => ref _paths;

		/// <summary>
		/// Retrieves a specific Bitmap object from the BitmapParser's array, using a provided index.
		/// </summary>
		/// <param name="bitmapIndex">The index of the Bitmap object to be retrieved.</param>
		/// <returns>A reference to the Bitmap object at the specified index.</returns>
		/// <exception cref="NullReferenceException">Thrown when the internal Bitmap array has not been initialized.</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown when the provided index is outside the bounds of the Bitmap array.</exception>
		public ref Bitmap GetBitmap(int bitmapIndex) {
			if (_bitmaps == null)
				throw new NullReferenceException();

			if (bitmapIndex > _bitmaps.Length - 1)
				throw new IndexOutOfRangeException(
					"Index was greater than or equal to the length of the bitmap collection.");

			if (bitmapIndex < 0)
				throw new IndexOutOfRangeException("Index was less than zero.");

			return ref _bitmaps[bitmapIndex];
		}

		/// <summary>
		/// Retrieves the path of a specific bitmap image.
		/// </summary>
		/// <param name="index">Index position of the image path in the list of paths.</param>
		/// <returns>Returns a reference to the path string of the image.</returns>
		/// <exception cref="NullReferenceException">Thrown when the list of paths is null.</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown when index is out of the range of the list of paths.</exception>
		public ref string GetPath(int index) {
			if (_paths == null)
				throw new NullReferenceException();

			if (index > _paths.Length - 1)
				throw new IndexOutOfRangeException(
					"Index was greater than or equal to the length of the bitmap collection.");

			if (index < 0)
				throw new IndexOutOfRangeException("Index was less than zero.");

			return ref _paths[index];
		}

		/// <summary>
		/// Releases the resources used by the BitmapParser class.
		/// </summary>
		public void Dispose() {
			if (_isDisposed)
				return;

			//TODO: research the need for GC.SuppressFinalize(this);
			// GC.SuppressFinalize(this);

			foreach (var bitmap in _bitmaps)
				bitmap.Dispose();

			_bitmaps    = default!;
			_isDisposed = true;
		}

		/// <summary>
		/// This code is base on: https://csharpexamples.com/fast-image-processing-c/
		///		from author Turgay
		/// Modifies the RGB values of a Bitmap at a specific index in an unsafe and concurrent manner.
		/// </summary>
		/// <param name="bitmapIndex">The index of the Bitmap in the internal array.</param>
		/// <param name="functor">A delegate function that performs the desired modifications on the pixel data.</param>
		/// <returns>Returns reference to the internal array of Bitmap objects.</returns>
		public unsafe ref Bitmap[] ModifyRgbUnsafe(int bitmapIndex, BitmapProcessDelegate functor) {
			ref var bmp = ref GetBitmap(bitmapIndex);

			// Lock the bitmap bits. This will allow us to modify the bitmap data.
			// Data is modified by traversing bitmap data (created in this method) and invoking functor.

			var bitmapData = bmp.LockBits(
				GetNewRect(ref bmp),
				ImageLockMode.ReadWrite,
				bmp.PixelFormat);

			// this gives us size in bits... divide by 8 to get size in bytes
			var   bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
			var   height        = bitmapData.Height;
			var   width         = bitmapData.Width * bytesPerPixel;
			byte* pxlPtr        = (byte*)bitmapData.Scan0;

			// note documentation for Parallel.For
			//		from INCLUSIVE ::: to EXCLUSIVE
			//		okay to pass 0, height
			Parallel.For(0, height,
				integer => {
					byte* row = pxlPtr + integer * bitmapData.Stride;

					for (var i = 0; i < width; i += bytesPerPixel) {
						// the current pixel to work on; passes rgb values to a delegate
						int red   = row[i + 2];
						int green = row[i + 1];
						int blue  = row[i];

						functor.Invoke(ref red, ref green, ref blue);
						GuardRgbValue(ref red);
						GuardRgbValue(ref green);
						GuardRgbValue(ref blue);

						row[i + 2] = (byte)red;
						row[i + 1] = (byte)green;
						row[i]     = (byte)blue;
					}
				});

			bmp.UnlockBits(bitmapData);

			return ref _bitmaps;
		}

		/// <summary>
		/// Asynchronously saves Bitmap objects to the files at a specified path.
		/// </summary>
		/// <param name="path">The directory path where the Bitmaps should be saved.</param>
		/// <param name="disposeOnSuccess">Optional parameter determining whether to dispose the BitmapParser on successful save. Default is false.</param>
		/// <returns>Returns a Task that represents the asynchronous operation. The task result contains void.</returns>
		/// <exception cref="DirectoryNotFoundException">Thrown when the provided path is null, empty, or consists only of white-space characters.</exception>
		public async Task SaveBitmapsAsync(string path, bool disposeOnSuccess = false) {
			if (string.IsNullOrWhiteSpace(path))
				throw new DirectoryNotFoundException("Could not find directory at path " + path);

			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}

			var count = _bitmaps.Length;
			var tasks = new Task[count];

			for (var i = 0; i < count; i++) {
				tasks[i] = SaveBitmapTask(_bitmaps[i], GetSanitizedPath(path));
			}

			await Task.WhenAll(tasks);

			if (disposeOnSuccess)
				Dispose();
		}

		string GetSanitizedPath(string path) {
			var newPath = Path.Combine(path, Path.GetFileNameWithoutExtension(GetPath(0)) + "_bmpParsed.png");
			return newPath;
		}

		static void GuardRgbValue(ref int value) {
			if (value < 0)
				value = 0;

			if (value > 255)
				value = 255;
		}

		static Task SaveBitmapTask(Bitmap bmp, string path) => Task.Run(() => bmp.Save(path, ImageFormat.Png));

		static Rectangle GetNewRect(ref Bitmap bmp) => new(0, 0, bmp.Width, bmp.Height);

#region PLUMBING

		public BitmapParser(ref string[] imgPaths) {
			_paths   = imgPaths;
			_bitmaps = new Bitmap[imgPaths.Length];

			for (var i = 0; i < imgPaths.Length; i++) {
				if (!File.Exists(imgPaths[i]))
					throw new FileNotFoundException("Could not find file at path " + imgPaths[i]);

				var bmp = new Bitmap(imgPaths[i]);
				_bitmaps[i] = bmp;
			}
		}

		Bitmap[]          _bitmaps;
		bool              _isDisposed;
		readonly string[] _paths;

#endregion
	}
}