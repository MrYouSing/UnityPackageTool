-- 
clr.System.Reflection.Assembly:Load("ICSharpCode.SharpZipLib");
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local IO=clr.System.IO;
local Zip=clr.ICSharpCode.SharpZipLib.Zip;
local UPT=clr.UnityPackageTool;
local LuaEx=clr.UnityPackageTool.Console.LuaHelper;

local src="C:/Users/Administrator/AppData/Roaming/Unity/Asset Store-5.x/Cafofo";
local tmp="C:/Users/Administrator/Desktop/UPT";
local dst="E:/Downloads/Unity/Cafofo.unitypackage";
local whitelist={
 "Fantasy Sounds Bundle"
,"Dynamic Cave Ambience"
,"Dynamic Village Ambience"
,"Fantasy Interface Sounds"
,"Magic Spells Sound Effects"
,"Medieval Combat Sounds"
,"Monster Sounds Pack"
,"RPG Dungeon Sounds"
};

function table_indexOf(t,x) 
	for i,v in ipairs(t) do
		if v==x then
			return i;
		end
	end
	return -1;
end

function ImporterFilter(x)
	return true;
end

do 
	-- 初始化工具
	local fastZip=Zip.FastZip();
	local pkg=UPT.Package();
	local importer=UPT.Importer();
	-- 导入资源
	local list=IO.Directory.GetFiles(src,"*.unitypackage",1);
	for i=0,list.Length-1,1 do
		if table_indexOf(whitelist,IO.Path.GetFileNameWithoutExtension(list[i]))>=0 then
			clr.System.Console.WriteLine(list[i]);
			importer.Import(list[i],tmp,pkg);
			n=n+1;
		end
	end
	fastZip.CreateZip(IO.Path.ChangeExtension(dst,".zip"),tmp.."/Assets/",true,"");
	-- 删除临时文件
	IO.Directory.Delete(tmp,true);
	clr.System.Console.WriteLine("Import "..n.." package(s).");
end;