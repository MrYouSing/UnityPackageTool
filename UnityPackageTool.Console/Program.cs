using System.IO;
using Neo.IronLua;
using ICSharpCode.SharpZipLib.Zip;
using UnityPackageTool.Postprocessors;

namespace UnityPackageTool.Console
{
	class Program
	{
		static void Main(string[] args) {
			string arg;
			for(int i=0,imax=args?.Length??0;i<imax;++i) {
				arg=args[i];
				if(!Execute(arg)) {
					return;
				}
			}
			while(true) {
				arg=System.Console.ReadLine();
				if(!Execute(arg)) {
					return;
				}
			}
		}

		public static void ConvertTexture(string arg) {
			var importer=new Importer();
			var texture=new TexturePostprocessor(importer);
			texture.maxSize=1024;if(arg.StartsWith("-max ")) {
				int j="-max ".Length,i=arg.IndexOf(' ',j);
				int.TryParse(arg.Substring(j,i-j),out texture.maxSize);
				arg=arg.Substring(i+1);
			}
			importer.Import(arg);
		}

		public static bool Execute(string arg) {
			// Trim
			if(arg.StartsWith("\"")&&arg.EndsWith("\"")) {
				arg=arg.Substring(1,arg.Length-2);
			}
			//
			if(arg=="/q") {
				return false;
			}else if(arg=="/h") {
			}else if(arg.StartsWith("texture ",System.StringComparison.OrdinalIgnoreCase)) {
				ConvertTexture(arg.Substring("texture ".Length));
				System.Console.WriteLine("Conversion is completed.");
			}else if(arg.StartsWith("zip ",System.StringComparison.OrdinalIgnoreCase)&&arg.EndsWith(".unitypackage")) {
				arg=arg.Substring("zip ".Length);
				string dir=Path.ChangeExtension(arg,null)+"_UPT";
				new Importer().Import(arg,dir);
				FastZip zip=new FastZip();
				zip.CreateZip(Path.ChangeExtension(arg,".zip"),dir+"/Assets/",true,"");
				Directory.Delete(dir,true);
				System.Console.WriteLine("Conversion is completed.");
			}else if(arg.EndsWith(".unitypackage")) {
				new Importer().Import(arg,Path.ChangeExtension(arg,null));
				System.Console.WriteLine("Extraction is completed.");
			}else {
				System.DateTime dt=System.DateTime.Now;
				string ext=Path.GetExtension(arg).ToLower();
				if(ext==".lua") {
					using (Lua lua=new Lua()) {
						var env=lua.CreateEnvironment();
						try {
							env.DoChunk(File.ReadAllText(arg),"test.lua");
						}catch(System.Exception ex) {
							if(ex!=null) {
								System.Console.WriteLine(ex.ToString());
							}
						}
					}
				} else if(ext==".js"||ext==".ts") {
				}
				System.Console.WriteLine("Execute("+arg+") costs "+(System.DateTime.Now-dt).TotalSeconds+"s.");
			}
			return true;
		}
	}
}
