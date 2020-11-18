-- 
clr.System.Reflection.Assembly:Load("ICSharpCode.SharpZipLib");
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local IO=clr.System.IO;
local Zip=clr.ICSharpCode.SharpZipLib.Zip;
local UPT=clr.UnityPackageTool;
local LuaEx=clr.UnityPackageTool.Console.LuaHelper;

local src="H:/Downloads/Snaps Art HD Asian Garden.unitypackage";
local dst="C:/Users/Administrator/Desktop/UPT";

function ImporterFilter(x)
	return true;
end

function TextureFilter(x)
	return string.find(x,"Decal_Damage_C_")==nil and string.find(x,"Decal_Damage_D_")==nil and string.find(x,"Water_Albedo")==nil;
end

do 
	-- 
	local pkg=UPT.Package();
	local importer=UPT.Importer();
	local texture=UPT.Postprocessors.TexturePostprocessor(importer);
	local fastZip=Zip.FastZip();
	-- 
	pkg.cacheSize=200*1024*1024;
	importer.filter=LuaEx.ToFilter(ImporterFilter);
	texture.maxSize=1024;
	texture.filter=LuaEx.ToFilter(TextureFilter);
	-- 
	importer.Import(src,dst,pkg);
	texture.OnPostImport(dst.."/Assets/AssetStoreOriginals/APAC_Garden/Art/Textures/Rock/Rock_C_Albedo.tif");
	texture.OnPostImport(dst.."/Assets/AssetStoreOriginals/APAC_Garden/Art/Textures/Terrian/Ground_Wild/Ground_Wild_MaskMap.tif");
	fastZip.CreateZip(IO.Path.ChangeExtension(src,".zip"),dst.."/Assets/",true,"");
	IO.Directory.Delete(dst,true);
end;