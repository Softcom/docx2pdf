/*
Copyright 2015 Softcom

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Diagnostics;

namespace Docx2PDF
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(@args[0]);

            if (file.Extension.ToLower() == ".doc" || file.Extension.ToLower() == ".xml" || file.Extension.ToLower() == ".wml" || file.Extension.ToLower() == ".docx")
            {

                Console.WriteLine("Starting document conversion...");

                //Set the Word Application Window Title
                string wordAppId = "" + DateTime.Now.Ticks;

                Word.Application word = new Word.Application();
                word.Application.Caption = wordAppId;
                word.Application.Visible = true;
                int processId = GetProcessIdByWindowTitle(wordAppId);
                word.Application.Visible = false;

                try
                {



                    object filename = file.FullName;
                    string newfilename = Path.ChangeExtension(file.FullName, ".pdf");

                    Word._Document document = word.Documents.OpenNoRepairDialog(filename,
                        ConfirmConversions: false,
                        ReadOnly: true,
                        AddToRecentFiles: false,
                        PasswordDocument: Type.Missing,
                        PasswordTemplate: Type.Missing,
                        Revert: false,
                        WritePasswordDocument: Type.Missing,
                        WritePasswordTemplate: Type.Missing,
                        Format: Type.Missing,
                        Encoding: Type.Missing,
                        Visible: false,
                        OpenAndRepair: false,
                        DocumentDirection: Type.Missing,
                        NoEncodingDialog: true,
                        XMLTransform: Type.Missing);
                    Console.WriteLine("Converting document '{0}' to PDF.", file);

                    document.ExportAsFixedFormat(
                        newfilename,
                        Word.WdExportFormat.wdExportFormatPDF,
                        OpenAfterExport: false,
                        OptimizeFor: Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen, 
                        Range: Word.WdExportRange.wdExportAllDocument,
                        Item: Word.WdExportItem.wdExportDocumentContent,
                        IncludeDocProps: true,
                        KeepIRM: false,
                        CreateBookmarks: Word.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, 
                        DocStructureTags: true,
                        BitmapMissingFonts: false,
                        UseISO19005_1: false,
                        FixedFormatExtClassPtr: Type.Missing);
                    document.Close();
                    document = null;

                    word.Quit();
                    word = null;
                    Console.WriteLine("Success, quitting Word.");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error ocurred: {0}", e);
                }
                finally
                {

                    // Terminate Winword instance by PID.
                    Console.WriteLine("Terminating Winword process with the Windowtitle '{0}' and the Application ID: '{1}'.", wordAppId, processId);
                    Process process = Process.GetProcessById(processId);
                    process.Kill();

                }
            }
            else
            {
                Console.WriteLine("Only DOC / WML / XML files possible.");
            }

        }

        public static int GetProcessIdByWindowTitle(string paramWordAppId)
        {
            Process[] P_CESSES = Process.GetProcessesByName("WINWORD");
            for (int p_count = 0; p_count < P_CESSES.Length; p_count++)
            {
                if (P_CESSES[p_count].MainWindowTitle.Equals(paramWordAppId))
                {
                    return P_CESSES[p_count].Id;
                }
            }
            return Int32.MaxValue;
        }

    }
}
