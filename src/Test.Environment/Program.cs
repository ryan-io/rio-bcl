using System.Diagnostics;
using simple_plotting;

const int size   = 10000;
var       paths  = new[] { @"C:\Users\stane\Pictures\FWG_3440x1440.jpg" };
using var       parser = new BitmapParser(ref paths);

var sw = Stopwatch.StartNew();

for (var i = 0; i < size; i++) {
	parser.ModifyRgbUnsafe(0, (ref int pxlIndex, ref int red, ref int green, ref int blue) => {
		                          if (pxlIndex % 2 == 0) {
			                          red   -= 25;
			                          green =  10;
			                          blue  += 20;
		                          }
				                           
		                          red   -= 25;
		                          green += 10;
		                          blue  += 20;
	                          });
}

sw.Stop();
Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds/1000 + " ms");

return 0;