using System.IO;
using Neo.IronLua;

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

		public static bool Execute(string arg) {
			if(arg=="/q") {
				return false;
			}else if(arg=="/h") {
			}else {
				System.DateTime dt=System.DateTime.Now;
				string ext=Path.GetExtension(arg).ToLower();
				if(ext==".lua") {
					using (Lua lua=new Lua()) {
						var env=lua.CreateEnvironment();
						try {
							env.DoChunk(File.ReadAllText(arg),"test.lua");
						}catch(System.Exception ex) {
							System.Console.WriteLine(ex.ToString());
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
