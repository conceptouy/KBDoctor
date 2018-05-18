﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.UI.Framework.Services;
using Artech.Architecture.Common.Services;
using System.Collections;
using Artech.Common.Diagnostics;
using Artech.Architecture.Common.Location;
using Artech.Genexus.Common.Objects;

namespace Concepto.Packages.KBDoctorCore.Sources
{
    public static class API
    {
        public static void Main()
        {
            return;
        }
        //
        public static void PrepareCompareNavigations(KnowledgeBase KB, IOutputService output)
        {
            Navigation.PrepareComparerNavigation(KB, output);
        }
        //
        public static bool CompareNavigations(KnowledgeBase KB, IOutputService output)
        {
           Utility.CreateModuleNamesFile(KB);
           Navigation.ReplaceModulesInNVGFiles(KB, output);
           return Navigation.CompareLastNVGDirectories(KB, output);
        }
        //
        public static void GetClassesTypesWithTheSameSignature(IEnumerable<KBObject> objects, out HashSet<int> classes, out Hashtable[] Classes_types)
        {
            Objects.GetClassesTypesWithTheSameSignature(objects, out classes, out Classes_types);
        }
        //
        public static void CleanKBObjectVariables(KBObject obj, IOutputService output)
        {
            CleanKB.CleanKBObjectVariables(obj, output);
        }
        //
        public static void CleanAllKBObjectVariables(KnowledgeBase KB, IOutputService output)
        {
            foreach (KBObject kbo in KB.DesignModel.Objects.GetAll())
                CleanKB.CleanKBObjectVariables(kbo, output);
        }
        //
        public static void RemoveAttributesWithoutTable(KBModel kbmodel, IOutputService output, out List<string[]> lineswriter)
        {
            CleanKB.RemoveAttributesWithoutTable(kbmodel, output, out lineswriter);
        }
        //
        public static void CleanKBObject(KBObject obj, IOutputService output)
        {
            CleanKB.CleanObject(obj, output);
        }
        //
        public static void CleanKBObjects(KnowledgeBase kb, IEnumerable<KBObject> kbojs, IOutputService output)
        {
            CleanKB.CleanObjects(kb,kbojs,output);
        }
        //
        public static void RemoveObjectsNotCalled(KBModel kbmodel, IOutputService output, out List<string[]> lineswriter)
        {
            CleanKB.RemoveObjectsNotCalled(kbmodel, output, out lineswriter);
        }
        //
        public static void SaveObjectsWSDLSource(KnowledgeBase KB, IOutputService output)
        {
            Navigation.SaveObjectsWSDL(KB, output, true);
        }
        //
        public static void SaveObjectsWSDL(KnowledgeBase KB, IOutputService output)
        {
            Navigation.SaveObjectsWSDL(KB, output, false);
        }
        //
        public static void CompareWSDL(KnowledgeBase KB, IOutputService output)
        {
            Navigation.CompareWSDLDirectories(KB, output);
        }
        //
        public static List<KBObject> ObjectsWithoutINOUT(KnowledgeBase KB, IOutputService output)
        {
            return Objects.ParmWOInOut(KB, output);
        }
        //
        public static void PreProcessPendingObjects(KnowledgeBase KB, IOutputService output, List<KBObject> objs)
        {
            output.StartSection("KBDoctor", "Object_review", "Object review");
            foreach (KBObject obj in objs)
            {
                List<KBObject> objlist = new List<KBObject>();
                objlist.Add(obj);
                if (Utility.isRunable(obj)) {
                    
                    //Check objects with parameteres without inout
                    Objects.ParmWOInOut(objlist, output);

                    //Clean variables not used
                    CleanKB.CleanKBObjectVariables(obj, output);

                    //Check complexity metrics
                    //maxNestLevel  6 - ComplexityLevel  30 - MaxCodeBlock  500 - parametersCount  6
                    Objects.CheckComplexityMetrics(objlist, output, 6, 30, 500, 6);

                    //Check commit on exit
                    Objects.CommitOnExit(objlist, output);

                    //Is in module
                    Objects.isInModule(objlist, output);

                    //With variables not based on attributes
                    Objects.ObjectsWithVarNotBasedOnAtt(objlist, output);

                    //Code commented
                    Objects.CodeCommented(objlist, output);

                    /*
                    * Objeto alcanzable?
                    * SDT sin domino o atributo
                    * Tiene todas las referencias?
                    * Mas de un parametro de salida
                    * Nombre "poco claro" / Descripcion "poco clara"
                    */
                }
                if(obj is Artech.Genexus.Common.Objects.Attribute)
                {
                    //Attribute Has Domain
                    Objects.AttributeHasDomain(objlist, output);
                }
                if (obj is SDT)
                {
                    //SDTItems Has Domain
                    Objects.SDTBasedOnAttDomain(objlist, output);
                }
            }
            output.EndSection("KBDoctor", "Object_review", "Object review", true);
        }
    }
}
