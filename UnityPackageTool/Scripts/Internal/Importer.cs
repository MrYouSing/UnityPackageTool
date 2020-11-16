using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityPackageTool {
	public class Importer {
		#region Fields

		public string destination;
		public List<Regex> regexps;
		public System.Action<string> onPostImport=null;

		#endregion Fields

		#region Methods

		public virtual bool CanImport(Package.Entry e) {
			if(e==null||!e.enabled) {return false;}
			int i=0,imax=regexps?.Count??0;
			for(;i<imax;++i) {
				if(regexps[i].IsMatch(e.path)) {
					break;
				}
			}
			return imax==0||i<imax;
		}

		public virtual string GetPath(string path) {
			return Path.Combine(destination,path);
		}

		public virtual void Import(Package package) {
			Package.Entry e;
			foreach(var it in package.entries) {
				e=it.Value;
				if(CanImport(e)) {
					Import(e);
				}
			}
		}

		public virtual void Import(Package.Entry e) {
			//
			string fn=GetPath(e.path);
			string dn=Path.GetDirectoryName(fn);
			if(!Directory.Exists(dn)) {
				Directory.CreateDirectory(dn);
			}
			//
			if(e.meta!=null) {
				File.WriteAllBytes(fn+".meta",e.meta);
			}
			if(e.isFile) {
				File.WriteAllBytes(fn,e.asset);
			}
			e.asset=null;
			e.meta=null;
			//
			OnPostImport(fn);
		}

		public virtual void OnPostImport(string path) {
			if(onPostImport!=null) {
				onPostImport(path);
			}
			System.Console.WriteLine("Import:"+path);
		}

		public virtual void Import(string src,string dst,ref Package package) {
			if(!string.IsNullOrEmpty(dst)) {
				destination=dst;
			}
			if(package==null) {package=new Package();}
			//
			package.onEntryCompleted+=Import;
			package.Init(src);
			package.onEntryCompleted-=Import;
			// TODO: Directories????
			Package.Entry e;
			foreach(var it in package.entries) {
				e=it.Value;
				if(CanImport(e)) {
				if(e.meta!=null) {
					Import(e);
				}}
			}
		}

		public virtual void Import(string src,string dst) {
			Package package=null;
			Import(src,dst,ref package);
		}

		public virtual void Import(string path) {
			OnPostImport(path);
			//
			if(Directory.Exists(path)) {
				foreach(string fn in Directory.GetFiles(path)) {
					OnPostImport(fn);
				}
				foreach(string dn in Directory.GetDirectories(path)) {
					Import(dn);
				}
			}
		}

		#endregion Methods
	}
}
