using System.Collections.Generic;
using Neo.IronLua;

namespace UnityPackageTool.Console {
	public class LuaHelper {
		#region Fields

		public static List<object> s_TmpList=new List<object>();

		#endregion Fields

		#region Methods

		public static System.Func<string,bool> ToFilter(System.Func<object,LuaResult> func) {
			s_TmpList.Add(func);
			return (x)=>{
				var result=func(x);
				return result.ToBoolean();
			};
		}

		#endregion Methods
	}
}
