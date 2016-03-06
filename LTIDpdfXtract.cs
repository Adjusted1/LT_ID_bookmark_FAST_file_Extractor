using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp;


// CLASS DEPENDS ON iTextSharp: http://sourceforge.net/projects/itextsharp/

namespace LT_ID_bookmark_FAST_file_Extractor
{
    public class PdfExtractorUtility
    {
        List<string> files = new List<string>();
        private string querystr = "";
        public void ProcessDirectory(string targetDirectory, List<string> files)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                    if (fileName.Contains(@".pdf"))
                    {
                        files.Add(fileName);
                    }
            }
            // Recurse into subdirectories of this directory. 
            string[] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory, files);
            }

        }
        public void ExtractPages(string sourcePdfPath, string outputPdfPath)
        {
            PdfReader reader = null;
            Document document = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;
            int pageNumber = 0;
            try
            {
                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                reader = new PdfReader(sourcePdfPath);
                /*
                 * 
                 * bookmark section
                 * 
                 */

                PdfReader pdfReader = new PdfReader(sourcePdfPath);

                IList<Dictionary<string, object>> bookmarks = SimpleBookmark.GetBookmark(pdfReader);

                for (int i = 0; i < bookmarks.Count; i++)
                {
                    string bookmark = (bookmarks[i].Values.ToArray().GetValue(0).ToString());
                    if (bookmark.Contains(querystr.ToString()))
                    {
                        pageNumber = i + 1;
                    }

                }
                // Capture the correct size and orientation for the page:
                document = new Document(reader.GetPageSizeWithRotation(pageNumber));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                pdfCopyProvider = new PdfCopy(document,
                    new System.IO.FileStream(outputPdfPath + pageNumber.ToString() + ".pdf", System.IO.FileMode.Create));

                document.Open();

                // Extract the desired page number:
                importedPage = pdfCopyProvider.GetImportedPage(reader, pageNumber);
                pdfCopyProvider.AddPage(importedPage);
                document.Close();
                reader.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}


