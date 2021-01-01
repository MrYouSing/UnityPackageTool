-- 
clr.System.Reflection.Assembly:Load("ICSharpCode.SharpZipLib");
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local IO=clr.System.IO;
local Zip=clr.ICSharpCode.SharpZipLib.Zip;
local UPT=clr.UnityPackageTool;
local LuaEx=clr.UnityPackageTool.Console.LuaHelper;
local Text=clr.System.Text;

local src="C:/Users/Administrator/AppData/Roaming/Unity/Asset Store-5.x/Quang Phan/3D ModelsCharactersHumanoidsFantasy/BerserkerS2 Mage and Archer.unitypackage";
local tmp="C:/Users/Administrator/Desktop/UPT";
local dst="C:/Users/Administrator/Desktop/BerserkerS2 Mage and Archer.unitypackage";

function ImporterFilter(x)
	return true;
end

function TextureFilter(x)
	return true;
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
	IO.File.WriteAllText(IO.Path.ChangeExtension(dst,".json"),pkg.ToJson(1,true),Text.Encoding.UTF8);
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