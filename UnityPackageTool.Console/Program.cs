using UnityPackageTool;
using UnityPackageTool.Postprocessors;

namespace UnityPackageTool.Console
{
	class Program
	{
		static void Main(string[] args) {
			var pkg=new Package("F:/BaiduNetdiskDownload/AVPro Video 1.10.0(u5.6.4).unitypackage");
			var importer=new Importer();
			var texture=new TexturePostprocessor(importer);
			//
			importer.destination="C:/Users/Administrator/Desktop/Sandbox/UPT";
			texture.maxSize=16;
			//
			importer.Import(pkg);
			System.Console.ReadLine();
		}
	}
}
