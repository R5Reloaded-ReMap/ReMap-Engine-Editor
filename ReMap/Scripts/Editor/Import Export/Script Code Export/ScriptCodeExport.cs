using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CodeViewsWindow
{
    public class CodeViewsExport
    {
        // FYI
        // TAB \\
        // 0 = Squirrel Code
        // 1 = DataTable Code
        // 2 = Precache Code
        // 3 = Ent Code ( go to ENT TAB )
        // ENT TAB \\
        // 0 = Script Code
        // 1 = Sound Code
        // 2 = Spawn Code

        private static string code;
        private static string functionName;
        private static int tab;
        private static int tab_temp;
        private static int tabEnt;
        private static int tabEnt_temp;
        private static Vector3 StartingOffset;
        private static bool ShowAdvancedMenu;
        private static bool ShowFunction;
        private static bool ShowEntFunction;
        private static bool ShowEntFunctionTemp;
        internal static bool EnableSelection;
        private static int EntFileID;

        private static bool HelperUseStartingOffset;


        [ MenuItem( "ReMap/Export/Script/Function And Map Offset", false, 25 ) ]
        public static void ExportFunctionAndMapOffset()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = true;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = true;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Function", false, 25 ) ]
        public static void ExportFunction()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = true;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Code And Map Offset", false, 25 ) ]
        public static void ExportCodeAndMapOffset()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = true;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Code", false, 25 ) ]
        public static void ExportCode()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/DataTable", false, 25 ) ]
        public static void ExportDataTable()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 1;
            CodeViewsWindow.tab_temp = 1;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Precache Model", false, 25 ) ]
        public static void Export()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 2;
            CodeViewsWindow.tab_temp = 2;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = true;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.EnableSelection = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Script", false, 25 ) ]
        public static void ExportEntFileScript()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.EnableSelection = false;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Sound", false, 25 ) ]
        public static void ExportEntFileSound()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 1;
            CodeViewsWindow.tabEnt_temp = 1;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.EnableSelection = false;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Spawn", false, 25 ) ]
        public static void ExportEntFileSpawn()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 2;
            CodeViewsWindow.tabEnt_temp = 2;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.EnableSelection = false;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }

        /*
        [ MenuItem( "ReMap/Export/", false, 25 ) ]
        public static void Export()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            //CodeViewsWindow.tab = 
            //CodeViewsWindow.tab_temp = 
            //CodeViewsWindow.tabEnt = 
            //CodeViewsWindow.tabEnt_temp = 
            //CodeViewsWindow.StartingOffset = Vector3.zero;
            //CodeViewsWindow.ShowAdvancedMenu = false;
            //CodeViewsWindow.ShowFunction = 
            //CodeViewsWindow.ShowEntFunction = 
            //CodeViewsWindow.ShowEntFunctionTemp = 
            //CodeViewsWindow.EnableSelection = false;
            //CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = 

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            SetCodeViewsWindowValue();
        }
        */

        internal static void GetCodeViewsWindowValue()
        {
            code = CodeViewsWindow.code;
            functionName = CodeViewsWindow.functionName;
            tab = CodeViewsWindow.tab;
            tab_temp = CodeViewsWindow.tab_temp;
            tabEnt = CodeViewsWindow.tabEnt;
            tabEnt_temp = CodeViewsWindow.tabEnt_temp;
            StartingOffset = CodeViewsWindow.StartingOffset;
            ShowAdvancedMenu = CodeViewsWindow.ShowAdvancedMenu;
            ShowFunction = CodeViewsWindow.ShowFunction;
            ShowEntFunction = CodeViewsWindow.ShowEntFunction;
            EnableSelection = CodeViewsWindow.EnableSelection;
            EntFileID = CodeViewsWindow.EntFileID;
            HelperUseStartingOffset = Helper.UseStartingOffset;
        }

        internal static void SetCodeViewsWindowValue()
        {
            CodeViewsWindow.code = code;
            CodeViewsWindow.functionName = functionName;
            CodeViewsWindow.tab = tab;
            CodeViewsWindow.tab_temp = tab_temp;
            CodeViewsWindow.tabEnt = tabEnt;
            CodeViewsWindow.tabEnt_temp = tabEnt_temp;
            CodeViewsWindow.StartingOffset = StartingOffset;
            CodeViewsWindow.ShowAdvancedMenu = ShowAdvancedMenu;
            CodeViewsWindow.ShowFunction = ShowFunction;
            CodeViewsWindow.ShowEntFunction = ShowEntFunction;
            CodeViewsWindow.EnableSelection = EnableSelection;
            CodeViewsWindow.EntFileID = EntFileID;
            Helper.UseStartingOffset = HelperUseStartingOffset;

            CodeViewsWindow.Refresh();
        }
    }
}