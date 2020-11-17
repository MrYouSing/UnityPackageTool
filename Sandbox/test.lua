-- 
clr.System.Reflection.Assembly:Load("ICSharpCode.SharpZipLib");
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local IO=clr.System.IO;
local Zip=clr.ICSharpCode.SharpZipLib.Zip;
local UPT=clr.UnityPackageTool;

local src="G:/Downloads/Snaps Art HD Asian Garden.unitypackage";
local dst="C:/Users/Administrator/Desktop/UPT";

do 
	-- 
	local pkg=UPT.Package();
	local importer=UPT.Importer();
	local texture=UPT.Postprocessors.TexturePostprocessor(importer);
	local fastZip=Zip.FastZip();
	-- 
	pkg.cacheSize=200*1024*1024;
	texture.maxSize=1024;
	-- 
	importer.Import(src,dst,pkg);
	fastZip.CreateZip(IO.Path.ChangeExtension(src,".zip"),dst.."/Assets/",true,"");
	IO.Directory.Delete(dst,true);
end;