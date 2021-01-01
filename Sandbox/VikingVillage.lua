-- 
clr.System.Reflection.Assembly:Load("ICSharpCode.SharpZipLib");
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local IO=clr.System.IO;
local Zip=clr.ICSharpCode.SharpZipLib.Zip;
local UPT=clr.UnityPackageTool;
local LuaEx=clr.UnityPackageTool.Console.LuaHelper;

local src="C:/Users/Administrator/AppData/Roaming/Unity/Asset Store-5.x/Unity Technologies/Unity EssentialsSample Projects/Viking Village.unitypackage";
local tmp="C:/Users/Administrator/Desktop/UPT";
local dst="C:/Users/Administrator/Desktop/Viking Village.unitypackage";

function ImporterFilter(x)
	return true;
end

function TextureFilter(x)
	return string.find(x,"fx_flame_01_d")==nil and string.find(x,"Cloud_01")==nil and string.find(x,"prop_shield_02_o")==nil;
end

do 
	-- 初始化工具
	local fastZip=Zip.FastZip();
	local pkg=UPT.Package();
	local importer=UPT.Importer();
	pkg.cacheSize=200*1024*1024;
	importer.filter=LuaEx.ToFilter(ImporterFilter);
	-- 导入高质量资源
	importer.Import(src,tmp,pkg);
	fastZip.CreateZip(IO.Path.ChangeExtension(dst,".HiRes.zip"),tmp.."/Assets/",true,"");
	-- 设置纹理后处理器
	local texture=UPT.Postprocessors.TexturePostprocessor(importer);
	texture.maxSize=1024;
	texture.filter=LuaEx.ToFilter(TextureFilter);
	-- 生成低质量资源
	importer.Import(tmp);
	fastZip.CreateZip(IO.Path.ChangeExtension(dst,".LowRes.zip"),tmp.."/Assets/",true,"");
	-- 删除临时文件
	IO.Directory.Delete(tmp,true);
end;