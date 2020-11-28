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
	-- 初始化工具
	local fastZip=Zip.FastZip();
	local pkg=UPT.Package();
	local importer=UPT.Importer();
	pkg.cacheSize=200*1024*1024;
	importer.filter=LuaEx.ToFilter(ImporterFilter);
	-- 导入高质量资源
	importer.Import(src,dst,pkg);
	fastZip.CreateZip(IO.Path.ChangeExtension(src,".HiRes.zip"),dst.."/Assets/",true,"");
	-- 设置纹理后处理器
	local texture=UPT.Postprocessors.TexturePostprocessor(importer);
	texture.maxSize=1024;
	texture.OnPostImport(dst.."/Assets/AssetStoreOriginals/APAC_Garden/Art/Textures/Decal/Decal_Damage_D_MaskMap.tif");
	texture.filter=LuaEx.ToFilter(TextureFilter);
	-- 生成低质量资源
	texture.OnPostImport(dst.."/Assets/AssetStoreOriginals/APAC_Garden/Art/Textures/Rock/Rock_C_Albedo.tif");
	texture.OnPostImport(dst.."/Assets/AssetStoreOriginals/APAC_Garden/Art/Textures/Terrian/Ground_Wild/Ground_Wild_MaskMap.tif");
	importer.Import(dst);
	fastZip.CreateZip(IO.Path.ChangeExtension(src,".LowRes.zip"),dst.."/Assets/",true,"");
	-- 删除临时文件
	IO.Directory.Delete(dst,true);
end;