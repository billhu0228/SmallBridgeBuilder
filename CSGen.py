#condin=utf-8
import uuid
import os
import shutil
#--------------------------------------------------------

filename="ACADExt"
ProjectList={
	"ACADExtV14":4.0,
	"ACADExtV18":4.6,
}

if os.path.exists(filename):
	shutil.rmtree(filename)

os.mkdir(filename)

for ss in ProjectList.keys():
	Ver=ss[-2:]
	netVer=ProjectList[ss]
	ProjectGuid = str(uuid.uuid4())
	with open(filename+'\\'+ss+'.csproj',"w+") as fid:
		fid.write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n")
		fid.write("<Project ToolsVersion=\"15.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\n")
		fid.write("  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />\n")
		fid.write("  <PropertyGroup>\n")
		fid.write("    <Configuration Condition=\" \'$(Configuration)\' == \'\' \">Debug</Configuration>\n")
		fid.write("    <Platform Condition=\" \'$(Platform)\' == \'\' \">AnyCPU</Platform>\n")		
		fid.write("    <ProjectGuid>%s</ProjectGuid>\n"%ProjectGuid)
		fid.write("    <OutputType>Library</OutputType>\n")
		fid.write("    <AppDesignerFolder>Properties</AppDesignerFolder>\n")
		fid.write("    <RootNamespace>%s</RootNamespace>\n"%filename)
		fid.write("    <AssemblyName>%s</AssemblyName>\n"%ss)
		fid.write("    <TargetFrameworkVersion>v%.1f</TargetFrameworkVersion>\n"%netVer)
		fid.write("    <FileAlignment>512</FileAlignment>\n")
		fid.write("    <TargetFrameworkProfile />\n")
		fid.write("    <DefineConstants>%s</DefineConstants>\n"%ss)
		fid.write("  </PropertyGroup>\n")						
		fid.write("""  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>debug\\</OutputPath>
    <DefineConstants>DEBUG;TRACE;$(DefineConstants)</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath></OutputPath>
    <DefineConstants>TRACE;$(DefineConstants)</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup>
    <RunPostBuildEvent>OnOutputUpdated</RunPostBuildEvent>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="accoremgd">
      <HintPath>D:\\Program Files\\Autodesk\\AutoCAD 20%s\\accoremgd.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="acdbmgd">
      <HintPath>D:\\Program Files\\Autodesk\\AutoCAD 20%s\\acdbmgd.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="acmgd">
      <HintPath>D:\\Program Files\\Autodesk\\AutoCAD 20%s\\acmgd.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Windows.Forms" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Xml" />  
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Class1.cs" />
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\\Microsoft.CSharp.targets" />
  <PropertyGroup>
    <PostBuildEvent>
    </PostBuildEvent>
  </PropertyGroup>
</Project>"""%(Ver,Ver,Ver))

	with open(filename+'\\'+'Class1.cs',"w+") as fid:
		fid.write("""using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

[assembly: CommandClass(typeof(%s.Class1))]
namespace %s
{
    public class Class1
    {
        [CommandMethod("Hello")]
        public void Main()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ed.WriteMessage("\\nHelloKitty");
        }
    }
}
      """%(filename,filename))