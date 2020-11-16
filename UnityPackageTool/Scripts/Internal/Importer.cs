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
			for(int i=0,imax=regexps?.Count??0;i<imax;++i) {
				if(!regexps[i].IsMatch(e.path)) {
					return false;
				}
			}
			return true;
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
			File.WriteAllBytes(fn+".meta",e.meta);
			if(e.isFile) {
				File.WriteAllBytes(fn,e.asset);
			}
			//
			if(onPostImport!=null) {
				onPostImport(fn);
			}
		}

		#endregion Methods
	}
}
